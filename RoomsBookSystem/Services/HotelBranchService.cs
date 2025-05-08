using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Models;
using RoomsBookSystem.Models;

namespace RoomsBookSystem.Services
{
    public class HotelBranchService : IHotelBranchService
    {
        private readonly ApplicationDbContext _context;

        public HotelBranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelBranch>> GetAllAsync()
        {
            return await _context.HotelBranches.ToListAsync();
        }

        public async Task<HotelBranch> GetByIdAsync(int id)
        {
            return await _context.HotelBranches.FindAsync(id);
        }

        public async Task<HotelBranch> CreateAsync(HotelBranch hotelBranch)
        {
            _context.HotelBranches.Add(hotelBranch);
            await _context.SaveChangesAsync();
            return hotelBranch;
        }

        public async Task<HotelBranch> UpdateAsync(HotelBranch hotelBranch)
        {
            _context.Entry(hotelBranch).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return hotelBranch;
        }

        public async Task DeleteAsync(int id)
        {
            var hotelBranch = await _context.HotelBranches.FindAsync(id);
            if (hotelBranch != null)
            {
                _context.HotelBranches.Remove(hotelBranch);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCountAsync(string searchString)
        {
            var query = _context.HotelBranches.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(b => 
                    b.Name.ToLower().Contains(searchString) || 
                    b.Location.ToLower().Contains(searchString)
                    
                );
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<HotelBranch>> GetPaginatedAsync(int pageNumber, int pageSize, string searchString, string sortOrder)
        {
            var query = _context.HotelBranches.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower();
                query = query.Where(b => 
                    b.Name.ToLower().Contains(searchString) || 
                    b.Location.ToLower().Contains(searchString)
                  
                );
            }

            // Apply sorting
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(b => b.Name),
                "location_asc" => query.OrderBy(b => b.Location),
                "location_desc" => query.OrderByDescending(b => b.Location),
                _ => query.OrderBy(b => b.Name) // Default: name_asc
            };

            // Apply pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
} 