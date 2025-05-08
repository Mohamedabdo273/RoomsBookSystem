using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RoomBooking
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public RoomType? RoomType { get; set; } 
        public int AdultsCount { get; set; }
        public int ChildrenCount { get; set; }
        public Booking? Booking { get; set; }
    }
    public enum RoomType
    {
        Single,
        Double,
        Suite
    }


}
