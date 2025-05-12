using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Users
{
    public class ApplicationUser : IdentityUser<int> // Se cambia string por int
    {

        public string? UserType { get; set; }
    }
}
