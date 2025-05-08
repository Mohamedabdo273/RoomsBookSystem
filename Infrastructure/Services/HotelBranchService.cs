
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
    public class HotelBranchService : IHotelBranchService
    {
        private readonly IHotelBranch _hotelBranchRepo;

        public HotelBranchService(IHotelBranch hotelBranchRepo)
        {
            _hotelBranchRepo = hotelBranchRepo;
        }

        public async Task<IEnumerable<HotelBranch>> GetAllAsync()
        {
            return await _hotelBranchRepo.GetAsync();
        }

        public async Task<HotelBranch?> GetByIdAsync(int id)
        {
            return await _hotelBranchRepo.GetOneAsync(null, x => x.Id == id);
        }

        public async Task CreateAsync(HotelBranch hotelBranch)
        {
            await _hotelBranchRepo.CreateAsync(hotelBranch);
            await _hotelBranchRepo.CommitAsync();
        }

        public async Task UpdateAsync(HotelBranch hotelBranch)
        {
            _hotelBranchRepo.Edit(hotelBranch);
            await _hotelBranchRepo.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hotel = await GetByIdAsync(id);
            if (hotel is not null)
            {
                _hotelBranchRepo.Delete(hotel);
                await _hotelBranchRepo.CommitAsync();
            }
        }
    }
}
