using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string? CustomerId { get; set; } 

        public int HotelBranchId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalRooms { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser? Customer { get; set; }
        public HotelBranch? HotelBranch { get; set; }
        public ICollection<RoomBooking>? RoomBookings { get; set; }
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
