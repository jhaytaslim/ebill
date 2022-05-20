using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ebill.Data.Repository.Interface;
using ebill.Data.Models;
using ebill.Data;

namespace ebill.Data.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context, ILogger<dynamic> log) : base(context, log)
        {
        }

        private DataContext _appContext => (DataContext)_context;

        private ILogger<dynamic> logger => (ILogger<dynamic>)_log;

    }
}
