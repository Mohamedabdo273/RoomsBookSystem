using infrastructure.Services.IService;
using Models;
using Repository.IRepository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace infrastructure.Services
{
    public class HotelBranchService : IHotelBranchService
    {
        private readonly IHotelBranch _hotelBranchRepo;
        private readonly string _imageFolderPath;

        public HotelBranchService(IHotelBranch hotelBranchRepo, IWebHostEnvironment webHostEnvironment)
        {
            _hotelBranchRepo = hotelBranchRepo;
            _imageFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "HotelBranchImages");
            EnsureImageDirectoryExists();
        }

        public async Task<IEnumerable<HotelBranch>> GetAllAsync()
        {
            return await _hotelBranchRepo.GetAsync();
        }

        public async Task<HotelBranch?> GetByIdAsync(int id)
        {
            return await _hotelBranchRepo.GetOneAsync(null, x => x.Id == id);
        }

        public async Task<HotelBranch> CreateAsync(HotelBranch hotelBranch, IFormFile? branchImage)
        {
            hotelBranch.Img = await SaveImageAsync(branchImage);
            await _hotelBranchRepo.CreateAsync(hotelBranch);
            await _hotelBranchRepo.CommitAsync();
            return hotelBranch;
        }

        public async Task<HotelBranch?> UpdateAsync(int id, HotelBranch hotelBranch, IFormFile? branchImage)
        {
            var existingBranch = await GetByIdAsync(id);
            if (existingBranch == null) return null;

            if (branchImage != null && branchImage.Length > 0)
            {
                await DeleteImage(existingBranch.Img);
                existingBranch.Img = await SaveImageAsync(branchImage);
            }

            // Update other properties
            existingBranch.Name = hotelBranch.Name;
            existingBranch.Location = hotelBranch.Location;
            
            _hotelBranchRepo.Edit(existingBranch);
            await _hotelBranchRepo.CommitAsync();

            return existingBranch;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var hotel = await GetByIdAsync(id);
            if (hotel is not null)
            {
                if (!string.IsNullOrEmpty(hotel.Img))
                {
                    await DeleteImage(hotel.Img);
                }
                _hotelBranchRepo.Delete(hotel);
                await _hotelBranchRepo.CommitAsync();
                return true;
            }
            return false;
        }

        private async Task<string> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return string.Empty;

            EnsureImageDirectoryExists();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(_imageFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        private async Task DeleteImage(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var filePath = Path.Combine(_imageFolderPath, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        private void EnsureImageDirectoryExists()
        {
            if (!Directory.Exists(_imageFolderPath))
            {
                Directory.CreateDirectory(_imageFolderPath);
            }
        }
    }
}
