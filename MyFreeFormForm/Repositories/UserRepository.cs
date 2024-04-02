using MyFreeFormForm.Data;
using MyFreeFormForm.Core.Repositories;


namespace MyFreeFormForm.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<MyIdentityUsers> GetUsers()
        {
            return _context.Users.ToList();
        }

        public MyIdentityUsers GetUser(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public MyIdentityUsers UpdateUser(MyIdentityUsers user)
        {
            _context.Update(user);
            _context.SaveChanges();

            return user;
        }
    }
}
