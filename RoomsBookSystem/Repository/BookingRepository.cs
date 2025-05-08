using infrastructure.Data;
using Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BookingRepository : Repository<Booking>, IBooking
    {
        public BookingRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
