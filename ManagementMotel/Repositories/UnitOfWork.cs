using ManagementMotel.Interfaces;
using ManagementMotel.Models;
using Microsoft.AspNetCore.Identity;

namespace ManagementMotel.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //public IEquipmentTypeRepository EquipmentTypes { get; }
        //public IEquipmentRepository Equipments { get; }
        //public IAssignmentEquipmentRepository AssignmentEquipments { get; }


        public UserManager<ApplicationUser> UserManager => _userManager;
        public RoleManager<IdentityRole> RoleManager => _roleManager;

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
            //this.EquipmentTypes = equipmentType;
            //this.Equipments = equipment;
            //this.AssignmentEquipments = assignmentEquipment;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
