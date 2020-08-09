using AuthAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PertixCore.Helpers
{
    public class AuthDataContext : IdentityDbContext<User, Role, Guid>
    {
        public AuthDataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
