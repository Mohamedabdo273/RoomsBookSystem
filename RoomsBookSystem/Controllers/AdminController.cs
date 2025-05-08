using infrastructure.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Http;

namespace RoomsBookSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private const int PageSize = 10;
        private readonly IHotelBranchService _hotelBranchService;
        private readonly IBookingService _bookingService;
        private readonly IRoomBookingService _roomBookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            IHotelBranchService hotelBranchService,
            IBookingService bookingService,
            IRoomBookingService roomBookingService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _hotelBranchService = hotelBranchService;
            _bookingService = bookingService;
            _roomBookingService = roomBookingService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // =================== User Management ===================
        public async Task<IActionResult> Users(int page = 1, string? searchString = null)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.FullName.Contains(searchString) ||
                    u.NationalId.Contains(searchString));
            }

            int totalItems = users.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var pagedUsers = users
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Get roles for each user
            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in pagedUsers)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            var viewModel = new
            {
                Users = pagedUsers,
                UserRoles = userRoles,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchString = searchString
            };

            return View(viewModel);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var bookings = await _bookingService.GetAllAsync();
            var userBookings = bookings.Where(b => b.CustomerId == id).ToList();

            var viewModel = new
            {
                User = user,
                Roles = roles,
                Bookings = userBookings
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string role, bool isInRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (isInRole)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToAction(nameof(UserDetails), new { id = userId });
        }

        // =================== Hotel Branches ===================
        public async Task<IActionResult> HotelBranches(int page = 1, string? searchString = null)
        {
            var branches = await _hotelBranchService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                branches = branches.Where(b =>
                    b.Name.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase) ||
                    b.Location.ToLower().Contains(searchString.ToLower(), StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalItems = branches.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var pagedBranches = branches
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(pagedBranches);
        }

        public IActionResult CreateHotelBranch() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHotelBranch(HotelBranch hotelBranch, IFormFile? branchImage)
        {
            if (ModelState.IsValid)
            {
                await _hotelBranchService.CreateAsync(hotelBranch, branchImage);
                TempData["Success"] = "Hotel branch created successfully!";
                return RedirectToAction(nameof(HotelBranches));
            }
            return View(hotelBranch);
        }

        public async Task<IActionResult> EditHotelBranch(int id)
        {
            var hotelBranch = await _hotelBranchService.GetByIdAsync(id);
            if (hotelBranch == null)
            {
                return NotFound();
            }
            return View(hotelBranch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHotelBranch(int id, HotelBranch hotelBranch, IFormFile? branchImage)
        {
            if (id != hotelBranch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _hotelBranchService.UpdateAsync(id, hotelBranch, branchImage);
                if (result == null)
                {
                    return NotFound();
                }
                TempData["Success"] = "Hotel branch updated successfully!";
                return RedirectToAction(nameof(HotelBranches));
            }
            return View(hotelBranch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHotelBranch(int id)
        {
            var result = await _hotelBranchService.DeleteAsync(id);
            if (result)
            {
                TempData["Success"] = "Hotel branch deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete hotel branch.";
            }
            return RedirectToAction(nameof(HotelBranches));
        }

        // =================== Bookings ===================
        public async Task<IActionResult> Bookings(int page = 1, string? searchString = null)
        {
            var bookings = await _bookingService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                    (b.Customer != null && b.Customer.FullName != null &&
                     b.Customer.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (b.Customer != null && b.Customer.Email != null &&
                     b.Customer.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (b.HotelBranch != null && b.HotelBranch.Name != null &&
                     b.HotelBranch.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            int totalItems = bookings.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var pagedBookings = bookings
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new
            {
                Bookings = pagedBookings,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchString = searchString
            };

            return View(viewModel);
        }

        public async Task<IActionResult> BookingDetails(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        public async Task<IActionResult> DeleteBooking(int id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction(nameof(Bookings));
        }

        // =================== Room Bookings ===================
        public async Task<IActionResult> RoomBookings(int page = 1, string? searchString = null)
        {
            var roomBookings = await _roomBookingService.GetAllAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                roomBookings = roomBookings.Where(r =>
                    (r.Booking != null && r.Booking.Customer != null &&
                     r.Booking.Customer.FullName != null &&
                     r.Booking.Customer.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    r.RoomType.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (r.Booking != null && r.Booking.HotelBranch != null &&
                     r.Booking.HotelBranch.Name != null &&
                     r.Booking.HotelBranch.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Calculate pagination
            int totalItems = roomBookings.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var pagedRoomBookings = roomBookings
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var viewModel = new
            {
                RoomBookings = pagedRoomBookings,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchString = searchString
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DeleteRoomBooking(int id)
        {
            await _roomBookingService.DeleteAsync(id);
            return RedirectToAction(nameof(RoomBookings));
        }

        // =================== Booking Management ===================
        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, BookingStatus status)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Status = status;
            await _bookingService.UpdateAsync(booking);
            return RedirectToAction(nameof(BookingDetails), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingDiscount(int id, double discount)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Discount = discount;
            booking.TotalPrice = CalculateTotalPrice(booking);
            await _bookingService.UpdateAsync(booking);
            return RedirectToAction(nameof(BookingDetails), new { id });
        }

        private decimal CalculateTotalPrice(Booking booking)
        {
            if (booking.RoomBookings == null) return 0;

            decimal totalPrice = booking.RoomBookings.Sum(rb =>
            {
                decimal roomPrice = rb.RoomType switch
                {
                    RoomType.Single => 100,
                    RoomType.Double => 150,
                    RoomType.Suite => 250,
                    _ => 0
                };


                roomPrice += (rb.AdultsCount - 1) * 50;
                roomPrice += rb.ChildrenCount * 25;

                return roomPrice;
            });

            // Apply discount if any
            if (booking.Discount > 0)
            {
                totalPrice -= (totalPrice * (decimal)booking.Discount / 100);
            }

            return totalPrice;
        }
    }

}
