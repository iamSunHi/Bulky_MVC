using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		public ICategoryRepository CategoryRepository { get; private set; }
		public ICoverTypeRepository CoverTypeRepository { get; private set; }
		public IProductRepository ProductRepository { get; private set; }
		public ICompanyRepository CompanyRepository { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			CategoryRepository = new CategoryRepository(context);
			CoverTypeRepository = new CoverTypeRepository(context);
			ProductRepository = new ProductRepository(context);
			CompanyRepository = new CompanyRepository(context);
		}

		public void Save()
		{
			_context.SaveChanges();
		}
	}
}
