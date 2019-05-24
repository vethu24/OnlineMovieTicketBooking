using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MovieTicketBooking.Models
{
    [Table("MovieDetailsViewModels")]
    public class MovieDetailsViewModels
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfTime { get; set; }
        public int price { get; set; }
        public string MoviePicture { get; set; }
    }
}