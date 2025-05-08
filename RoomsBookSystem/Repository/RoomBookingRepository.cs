
using infrastructure.Data;
using Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RoomBookingRepository : Repository<RoomBooking>, IRoomBooking
    {
        public RoomBookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
