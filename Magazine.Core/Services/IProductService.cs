using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magazine.Core.Models;

namespace Magazine.Core.Services
{
    namespace Magazine.Core.Services
    {
        public interface IProductService
        {
            Product Add(Product product);
            Product Remove(Guid id);
            Product Edit(Product product);
            Product Search(string name, decimal? price);
            Product GetById(Guid id);
        }
    }
}
