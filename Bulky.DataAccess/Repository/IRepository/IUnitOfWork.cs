﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		IApplicationUserRepository ApplicationUserRepository { get; }
		ICategoryRepository CategoryRepository { get; }
		ICoverTypeRepository CoverTypeRepository { get; }
		IProductRepository ProductRepository { get; }
		ICompanyRepository CompanyRepository { get; }
		IShoppingCartRepository ShoppingCartRepository { get; }
		IOrderHeaderRepository OrderHeaderRepository { get; }
		IOrderDetailRepository OrderDetailRepository { get; }
		void Save();
	}
}
