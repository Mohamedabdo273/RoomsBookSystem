using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services.IService
{
    public interface IHotelBranchService
    {
        Task<IEnumerable<HotelBranch>> GetAllAsync();
        Task<HotelBranch?> GetByIdAsync(int id);
        Task CreateAsync(HotelBranch hotelBranch);
        Task UpdateAsync(HotelBranch hotelBranch);
        Task DeleteAsync(int id);
    }
}
