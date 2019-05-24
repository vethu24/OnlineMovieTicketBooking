using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MovieTicketBooking.Models
{
    public class PaymentDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
   
        public string CardType { get; set; }
        
        public int CreditCardNo { get; set; }

        public int CVV { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpireDate { get; set; }
        public int Total { get; set; }
        public string MovieName { get; set; }
    }
}