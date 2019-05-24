using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieTicketBooking.Models
{
    public class Contact
    {
        public string YourEmail { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Comments { get; set; }
    }
}