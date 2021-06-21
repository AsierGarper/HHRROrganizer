using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HHRROrganizer.Models
{
    public class AppUser: IdentityUser
    {
        [Key]
        public int OrganizationNumber { get; set; }
    }
}
