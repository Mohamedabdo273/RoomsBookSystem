
using infrastructure.Data;
using Models;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Repository
{
    public class HotelBranchRepository : Repository<HotelBranch>, IHotelBranch
    {
        public HotelBranchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
