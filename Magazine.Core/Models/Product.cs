using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Models
{
       public class Product
        {
            public Guid Id { get; set; } = Guid.NewGuid(); // Генерация нового ID по умолчанию
            public string Definition { get; set; } // Текстовое описание товара
            public string Name { get; set; } // Название товара
            public decimal Price { get; set; } // Стоимость товара
            public string Image { get; set; } // Изображение товара
        }
}
