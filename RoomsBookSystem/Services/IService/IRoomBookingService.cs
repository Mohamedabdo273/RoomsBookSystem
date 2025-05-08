using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services.IService
{ 
       public interface IRoomBookingService
        {
            Task<IEnumerable<RoomBooking>> GetAllAsync();
            Task<RoomBooking?> GetByIdAsync(int id);
            Task CreateAsync(RoomBooking roomBooking);
            Task UpdateAsync(RoomBooking roomBooking);
            Task DeleteAsync(int id);
        }
    
}
