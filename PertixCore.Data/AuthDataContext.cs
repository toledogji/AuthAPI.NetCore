using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using PertixCore.Core.Models;

namespace PertixCore.Helpers
{
    public class AuthDataContext : IdentityDbContext<User, Role, Guid>
    {
        public AuthDataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
