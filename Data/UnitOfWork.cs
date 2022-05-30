using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ebill.Data.Repository;
using ebill.Data.Repository.Interface;
using ebill.Data.Models;
using ebill.Contracts;
using Microsoft.Extensions.Options;

namespace ebill.Data
{
    public class UnitOfWork : IUnitOfWork
    {

        readonly DataContext _context;
        readonly ILogger<dynamic> _log;
        readonly Connections _connections;

        public IProductRepository _ProductRepository;
        public IItemRepository _ItemRepository;
        public ISettingsRepository _SettingsRepository;
        public IOracleRepository  _OracleRepository;


        public UnitOfWork(DataContext context, ILogger<dynamic> log,IOptionsMonitor<Connections> connectionsMonitor)
        {
            _context = context;
            _log = log;
            _connections = connectionsMonitor.CurrentValue;
        }


        public IOracleRepository Oracle
        {
            get
            {
                if (_OracleRepository == null)
                    _OracleRepository = new OracleRepository(_connections, _log);

                return _OracleRepository;
            }
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

        public IItemRepository Item
        {
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
