using BulkyBook.Models;
using BulkyBook.Models.ViewModels;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
		void Update(ApplicationUser applicationUser);
    }
}
