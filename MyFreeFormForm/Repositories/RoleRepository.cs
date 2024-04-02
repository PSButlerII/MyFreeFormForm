using Microsoft.AspNetCore.Identity;
using MyFreeFormForm.Core.Repositories;
using MyFreeFormForm.Data;

namespace MyFreeFormForm.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<IdentityRole> GetRoles()
        {
            return _context.Roles.ToList();
        }
    }
}
