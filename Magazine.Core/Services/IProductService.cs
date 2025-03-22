using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magazine.Core.Models;

namespace Magazine.Core.Services
{
    public interface IProductService
    {
        // Метод для добавления нового элемента
        Product Add(Product product);

        // Метод для удаления элемента
        Product Remove(Guid id);

        // Метод для изменения элемента
        Product Edit(Product product);

        // Метод для поиска элемента по параметрам
        Product Search(Guid id);
    }
}
