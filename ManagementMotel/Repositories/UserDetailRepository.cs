using ManagementMotel.Interfaces;
using ManagementMotel.Models;

namespace ManagementMotel.Repositories
{
    public class UserDetailRepository : GenericRepository<UserDetail>, IUserDetailRepository
    {
        public UserDetailRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
