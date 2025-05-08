using Microsoft.AspNetCore.Http;
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
        Task<HotelBranch> CreateAsync(HotelBranch hotelBranch, IFormFile? branchImage);
        Task<HotelBranch?> UpdateAsync(int id, HotelBranch hotelBranch, IFormFile? branchImage);
        Task<bool> DeleteAsync(int id);
    }
}
