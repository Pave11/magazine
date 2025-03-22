using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Models
{
    public class Product
    {
        // Уникальный идентификатор
        public Guid Id { get; set; }

        // Текстовое описание товара
        public string Definition { get; set; }

        // Название товара
        public string Name { get; set; }

        // Стоимость товара
        public decimal Price { get; set; }

        // Изображение товара (можно использовать строку для хранения пути к изображению)
        public string Image { get; set; }

        // Конструктор по умолчанию
        public Product()
        {
            Id = Guid.NewGuid(); // Генерация нового уникального идентификатора при создании объекта
        }
    }
}
