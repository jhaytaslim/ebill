using ebill.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebill.Data
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }

        IItemRepository Item { get; }

        ISettingsRepository Settings { get; }
    }
}
