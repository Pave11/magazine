using Magazine.Core.Models;
using Magazine.Core.Services;
using System;
using System.Threading.Tasks;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IDataBase _database;

        public ProductService(IDataBase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public Product Add(Product product)
        {
            // 1. Проверка входных данных
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            // 2. Валидация обязательных полей
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be empty", nameof(product.Name));

            if (product.Price <= 0)
                throw new ArgumentException("Price must be greater than zero", nameof(product.Price));

            // 3. Генерация нового ID если:
            // - ID не указан (Guid.Empty)
            // - или ID указан, но уже существует в БД
            if (product.Id == Guid.Empty || _database.GetProductById(product.Id) != null)
            {
                product.Id = Guid.NewGuid();
            }

            // 4. Добавление в базу данных
            _database.AddProduct(product);

            return product;
        }

        public Product Remove(Guid id)
        {
            var product = _database.GetProductById(id);
            if (product == null)
                return null;

            _database.DeleteProduct(id);
            return product;
        }

        public Product Edit(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var existingProduct = _database.GetProductById(product.Id);
            if (existingProduct == null)
                return null;

            _database.UpdateProduct(product);
            return product;
        }

        public Product GetById(Guid id)
        {
            return _database.GetProductById(id);
        }
    }
}