
using infrastructure.Services.IService;
using Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBooking _bookingRepo;

        public BookingService(IBooking bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _bookingRepo.GetAsync(
                [
                    b => b.Customer,
                    b => b.HotelBranch,
                    b => b.RoomBookings
                ]);
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _bookingRepo.GetOneAsync([
                    b => b.Customer,
                    b => b.HotelBranch,
                    b => b.RoomBookings
                ],expression: b => b.Id == id);
        }

        public async Task CreateAsync(Booking booking)
        {
            await _bookingRepo.CreateAsync(booking);
            await _bookingRepo.CommitAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _bookingRepo.Edit(booking);
            await _bookingRepo.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _bookingRepo.GetOneAsync(expression: b => b.Id == id);
            if (booking != null)
            {
                _bookingRepo.Delete(booking);
                await _bookingRepo.CommitAsync();
            }
        }
    }
}
