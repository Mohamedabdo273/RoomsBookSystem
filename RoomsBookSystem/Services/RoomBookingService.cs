using infrastructure.Services.IService;
using Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class RoomBookingService : IRoomBookingService
    {
        private readonly IRoomBooking _roomBookingRepo;

        public RoomBookingService(IRoomBooking roomBookingRepo)
        {
            _roomBookingRepo = roomBookingRepo;
        }

        public async Task<IEnumerable<RoomBooking>> GetAllAsync()
        {
            return await _roomBookingRepo.GetAsync([e=>e.Booking.Customer, e => e.Booking.HotelBranch], tracked: false);
        }

        public async Task<RoomBooking?> GetByIdAsync(int id)
        {
            return await _roomBookingRepo.GetOneAsync([e => e.Booking], x => x.Id == id);
        }

        public async Task CreateAsync(RoomBooking roomBooking)
        {
            await _roomBookingRepo.CreateAsync(roomBooking);
            await _roomBookingRepo.CommitAsync();
        }

        public async Task UpdateAsync(RoomBooking roomBooking)
        {
            _roomBookingRepo.Edit(roomBooking);
            await _roomBookingRepo.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var room = await GetByIdAsync(id);
            if (room is not null)
            {
                _roomBookingRepo.Delete(room);
                await _roomBookingRepo.CommitAsync();
            }
        }
    }
}

