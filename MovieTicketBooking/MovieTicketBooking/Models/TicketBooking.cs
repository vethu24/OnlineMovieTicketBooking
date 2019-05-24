using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieTicketBooking.Models
{
    public class TicketBooking
    {
        [Key]
        public int Id { get; set; }
        public string Movie_Name { get; set; }
        public string UserId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int MovieId { get; set; }
        public int Price { get; set; }

        public string SeatNumber { get; set; }
    }
}