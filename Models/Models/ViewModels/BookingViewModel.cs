using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Models;

namespace RoomsBookSystem.Models.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public int HotelBranchId { get; set; }

        [Required]
        [Display(Name = "Check In Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check Out Date")]
        public DateTime CheckOutDate { get; set; }

        public List<RoomBookingViewModel> Rooms { get; set; } = new List<RoomBookingViewModel>();
        public IEnumerable<HotelBranch>? HotelBranches { get; set; }
    }

    public class RoomBookingViewModel
    {
        [Required]
        [Display(Name = "Room Type")]
        public RoomType RoomType { get; set; }

        [Required]
        [Range(1, 4)]
        [Display(Name = "Number of Adults")]
        public int NumberOfAdults { get; set; }

        [Required]
        [Range(0, 4)]
        [Display(Name = "Number of Children")]
        public int NumberOfChildren { get; set; }
    }
} 