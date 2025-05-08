using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using infrastructure.Services.IService;
using System.Security.Claims;
using RoomsBookSystem.Models.ViewModels;

namespace RoomsBookSystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomBookingService _roomBookingService;
        private readonly IHotelBranchService _hotelBranchService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(
            IBookingService bookingService,
            IRoomBookingService roomBookingService,
            IHotelBranchService hotelBranchService,
            UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _roomBookingService = roomBookingService;
            _hotelBranchService = hotelBranchService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Bookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = await _bookingService.GetAllAsync();
            var userBookings = bookings.Where(b => b.CustomerId == userId).ToList();

            return View(userBookings);
        }

        public async Task<IActionResult> BookingDetails(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(User);
            if (booking.CustomerId != userId)
            {
                return Forbid();
            }

            return View(booking);
        }

        public async Task<IActionResult> CreateBooking()
        {
            var hotelBranches = await _hotelBranchService.GetAllAsync();
            ViewBag.HotelBranches = hotelBranches;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(Booking model)
        {
            if (!ModelState.IsValid)
            {
                var branches = await _hotelBranchService.GetAllAsync();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            decimal totalPrice = 0;
            var roomBookings = new List<RoomBooking>();

            // Calculate base price without discount
            foreach (var room in model.RoomBookings)
            {
                var roomBooking = new RoomBooking
                {
                    RoomType = room.RoomType.Value,
                    AdultsCount = room.AdultsCount,
                    ChildrenCount = room.ChildrenCount
                };

                decimal roomPrice = CalculateRoomPrice(room.RoomType.Value, room.AdultsCount, room.ChildrenCount);
                totalPrice += roomPrice;
                roomBookings.Add(roomBooking);
            }

            var hasPreviousBookings = await _bookingService.GetAllAsync();
            hasPreviousBookings = hasPreviousBookings.Where(b => b.CustomerId == user.Id).ToList();

            double discount = 0;
            if (!hasPreviousBookings.Any())
            {
                user.IsRepeated = true;
                await _userManager.UpdateAsync(user);
            }
            else if (user.IsRepeated == true)
            {
                discount = 5; // 5% discount
                totalPrice = totalPrice * 0.95m; // Apply 5% discount
            }

            // Create the booking
            var booking = new Booking
            {
                CustomerId = user.Id,
                HotelBranchId = model.HotelBranchId,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                TotalPrice = totalPrice,
                Discount = discount,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalRooms = model.RoomBookings.Count
            };

            await _bookingService.CreateAsync(booking);

            foreach (var roomBooking in roomBookings)
            {
                roomBooking.BookingId = booking.Id;
                await _roomBookingService.CreateAsync(roomBooking);
            }

            return RedirectToAction(nameof(Bookings));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (booking.CustomerId != userId)
            {
                return Forbid();
            }
            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Confirmed)
            {
                return BadRequest("Cannot cancel a booking that is not pending or confirmed.");
            }

            booking.Status = BookingStatus.Cancelled;
            await _bookingService.UpdateAsync(booking);

            return RedirectToAction(nameof(BookingDetails), new { id = booking.Id });
        }

        private decimal CalculateRoomPrice(RoomType roomType, int adults, int children)
        {

            decimal basePrice = roomType switch
            {
                RoomType.Single => 100,
                RoomType.Double => 150,
                RoomType.Suite => 250,
                _ => 100     
            };
            decimal additionalAdultsPrice = (adults - 1) * 50;
            decimal childrenPrice = children * 25;
            decimal totalRoomPrice = basePrice + additionalAdultsPrice + childrenPrice;

            return totalRoomPrice;
        }
    }
}
