
using MyFreeFormForm.Data;

namespace MyFreeFormForm.Core.Repositories
{
    public interface IUserRepository
    {
        ICollection<MyIdentityUsers> GetUsers();

        MyIdentityUsers GetUser(string id);

        MyIdentityUsers UpdateUser(MyIdentityUsers user);
    }
}
