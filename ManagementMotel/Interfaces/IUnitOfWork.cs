using ManagementMotel.Models;
using Microsoft.AspNetCore.Identity;

namespace ManagementMotel.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        UserManager<ApplicationUser> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        IUserDetailRepository UserDetail { get; }

        int Complete();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
