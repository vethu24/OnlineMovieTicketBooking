using Microsoft.AspNet.Identity.Owin;
using MovieTicketBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nexmo.Api;
using System.Net.Mail;

namespace MovieTicketBooking.Controllers
{
    public class HomeController : Controller
    {
        private Models.ApplicationDbContext db = new ApplicationDbContext();
        int count = 1;
       
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
           

            return View();
        }

        [HttpPost]
        public ActionResult Contact(Contact model)
        {
            MailMessage mm = new MailMessage("24thusshan@gmail.com", model.YourEmail);
            mm.Subject = model.Subject;
            mm.Body = model.Comments;
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("24thusshan@gmail.com", "your pwd");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
            ViewBag.Message = "Mail Send Successfully";

            return View();
        }
        public ActionResult MovieIndex(string movieGenre, string searchString)
        {
            var GenreLst = new List<string>();

            var GenreQry = from d in db.MovieDetailsViewModels
                           orderby d.Genre
                           select d.Genre;
            GenreLst.AddRange(GenreQry.Distinct());
            ViewBag.movieGenre = new SelectList(GenreLst);

            var movies = from m in db.MovieDetailsViewModels
                         select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }
            return View(movies.ToList());
        }


        [HttpGet]
        [Authorize]
        public ActionResult BookNow(int Id)
        {
            TicketBooking c = new TicketBooking();
            var item = db.MovieDetailsViewModels.FirstOrDefault(a => a.ID == Id);
            c.Movie_Name = item.Name;
            c.Date = new DateTime(item.DateOfTime.Year, item.DateOfTime.Month, item.DateOfTime.Day, item.DateOfTime.Hour, item.DateOfTime.Minute, item.DateOfTime.Second);

            c.Price = item.price;
            c.MovieId = item.ID;
            return View(c);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult BookNow(TicketBooking c, int Id, LoginViewModel model)
        {

            List<TicketBooking> cart = new List<TicketBooking>();
          

            var cc = db.MovieDetailsViewModels.FirstOrDefault(a => a.ID == Id);

            count = c.SeatNumber.Split(',').Length;

            if (!checkseat(c.SeatNumber, c.MovieId))
            {
                foreach (var item in c.SeatNumber.Split(','))
                {
                    cart.Add(new TicketBooking { Id = c.Id, Movie_Name = cc.Name, UserId = User.Identity.Name, Date = new DateTime(cc.DateOfTime.Year, cc.DateOfTime.Month, cc.DateOfTime.Day, cc.DateOfTime.Hour, cc.DateOfTime.Minute, cc.DateOfTime.Second), MovieId = cc.ID, Price = cc.price, SeatNumber = item });

                }

                foreach (var item in cart)
                {
                    db.TicketBooking.Add(item);
                    db.SaveChanges();
                }

                TempData["Sucess"] = "seat number Booked,Check your cart";
            }
            else
            {
                TempData["Seatenomsg"] = "please change your seatno";
            }
            return RedirectToAction("BookNow");
        }

        [Authorize]
        private bool checkseat(string SeatNumber, int id)
        {
            bool flag = true;

            string seats = SeatNumber;
            string[] seatreserve = SeatNumber.Split(',');
            var seatnolist = db.TicketBooking.Where(a => a.MovieId == id).ToList();
            foreach (var item in seatnolist)
            {
                string alreadybook = item.SeatNumber;
                foreach (var iteam1 in seatreserve)
                {
                    if (iteam1 == alreadybook)
                    {
                        flag = false;
                        break;
                    }
                }
            }

            if (flag == false)
                return true;
            else
                return false;
        }
        [Authorize]
        public ActionResult Checkout()
        {
            return View(db.TicketBooking.Where(x => x.UserId == User.Identity.Name));
            //return View(db.Cart.ToList());
        }
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketBooking product = db.TicketBooking.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketBooking product = db.TicketBooking.Find(id);
            db.TicketBooking.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public ActionResult PaymentDetails(int Id)
        {
            PaymentDetails vm = new PaymentDetails();
            var item = db.TicketBooking.FirstOrDefault(a => a.Id == Id);
            vm.Total = item.Price;
            vm.MovieName = item.Movie_Name;
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentDetails(PaymentDetails p, int Id)
        {
            if (ModelState.IsValid)
            {
                PaymentDetails vm = new PaymentDetails();
                var item = db.TicketBooking.FirstOrDefault(a => a.Id == Id);
             
                vm.MovieName = item.Movie_Name;

                var results = SMS.Send(new SMS.SMSRequest
                {
                    from = Nexmo.Api.Configuration.Instance.Settings["appsettings:NEXMO_FROM_NUMBER"],
                    to = p.Phone,
                    text = vm.MovieName
                });



                MailMessage mm = new MailMessage("24thusshan@gmail.com", p.Email);
                mm.Subject = vm.MovieName;
                mm.Body = p.Name;
                mm.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential("24thusshan@gmail.com", "your pwd");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = nc;
                smtp.Send(mm);



             
                db.PaymentDetails.Add(new PaymentDetails { Id = p.Id, Name = p.Name, Phone = p.Phone, Email = p.Email, CardType = p.CardType, CreditCardNo = p.CreditCardNo, CVV = p.CVV, ExpireDate = p.ExpireDate, Total = item.Price, MovieName = vm.MovieName });
                db.SaveChanges();
                //TempData["Sucess"] = "Thank You For Your Booking";
                //ViewBag.Message = "Mail And SMS Send Successfully!!! Check Your Email And SMS";
                return RedirectToAction("Detail");
            }

            return View(p);
        }
        [Authorize]
        public ActionResult Detail()
        {
            return View(db.PaymentDetails.Where(x => x.Email == User.Identity.Name));
        }

       
    }
}