using Microsoft.AspNetCore.Identity;

namespace MyFreeFormForm.Core.Repositories
{
    public interface IRoleRepository
    {
        ICollection<IdentityRole> GetRoles();
    }
}
