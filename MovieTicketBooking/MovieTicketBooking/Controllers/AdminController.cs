
using MovieTicketBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MovieTicketBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index(string movieGenre, string searchString)
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

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           MovieDetailsViewModels  p = db.MovieDetailsViewModels.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }
            return View(p);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovieDetailsViewModels movie, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                string path = Path.Combine(Server.MapPath("~/Images"),
                                       Path.GetFileName(file.FileName));
                file.SaveAs(path);
                db.MovieDetailsViewModels.Add(new MovieDetailsViewModels { ID = movie.ID, Name = movie.Name, Genre = movie.Genre, DateOfTime = movie.DateOfTime, price = movie.price, MoviePicture = "~/Images/" + file.FileName });
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieDetailsViewModels movie = db.MovieDetailsViewModels.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Genre,DateOfTime,price,MoviePicture")] MovieDetailsViewModels movie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovieDetailsViewModels c = db.MovieDetailsViewModels.Find(id);
            if (c == null)
            {
                return HttpNotFound();
            }
            return View(c);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovieDetailsViewModels c = db.MovieDetailsViewModels.Find(id);
            db.MovieDetailsViewModels.Remove(c);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}