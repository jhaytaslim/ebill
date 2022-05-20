using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ebill.Data.Repository;
using ebill.Data.Repository.Interface;
using ebill.Data.Models;

namespace ebill.Data
{
    public class UnitOfWork : IUnitOfWork
    {

        readonly DataContext _context;
        readonly ILogger<dynamic> _log;

        public IProductRepository _ProductRepository;
        public IItemRepository _ItemRepository;
        public ISettingsRepository _SettingsRepository;


        public UnitOfWork(DataContext context, ILogger<dynamic> log)
        {
            _context = context;
            _log = log;
        }

        public IProductRepository Product
        {
            get
            {
                if (_ProductRepository == null)
                    _ProductRepository = new ProductRepository(_context, _log);

                return _ProductRepository;
            }
        }

        public IItemRepository Item {
            get
            {
                if (_ItemRepository == null)
                    _ItemRepository = new ItemRepository(_context, _log);

                return _ItemRepository;
            }
        }

        public ISettingsRepository Settings
        {
            get
            {
                if (_SettingsRepository == null)
                    _SettingsRepository = new SettingsRepository(_context, _log);

                return _SettingsRepository;
            }
        }

    }
}
