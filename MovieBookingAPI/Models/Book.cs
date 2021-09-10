using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingAPI.Models
{
    public class Book
    {
        [Key]
        public int BookingId { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }

        public string screen { get; set; }
        public string MovieName { get; set; }

        public int NumberofTickets { get; set; }
        public int NumberofSeats { get; set; }

    }
}
