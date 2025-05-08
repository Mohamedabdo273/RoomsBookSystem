using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models
{
   public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        
        [StringLength(14)]
        public string? NationalId { get; set; }

        public bool? IsRepeated { get; set; } = false;
        public ICollection<Booking>? Bookings { get; set; }
    }
}
