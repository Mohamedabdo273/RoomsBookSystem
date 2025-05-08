using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using RoomsBookSystem.Models;

namespace RoomsBookSystem.Services
{
    public interface IHotelBranchService
    {
        Task<IEnumerable<HotelBranch>> GetAllAsync();
        Task<HotelBranch> GetByIdAsync(int id);
        Task<HotelBranch> CreateAsync(HotelBranch hotelBranch);
        Task<HotelBranch> UpdateAsync(HotelBranch hotelBranch);
        Task DeleteAsync(int id);
        Task<int> GetTotalCountAsync(string searchString);
        Task<IEnumerable<HotelBranch>> GetPaginatedAsync(int pageNumber, int pageSize, string searchString, string sortOrder);
    }
} 