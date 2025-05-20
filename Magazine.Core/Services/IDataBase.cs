using Magazine.Core.Models;
using System;
using System.Collections.Generic;

namespace Magazine.Core.Services
{
    /// <summary>
    /// Интерфейс для работы с базой данных.
    /// Все методы соответствуют CRUD-операциям для Product.
    /// </summary>
    public interface IDataBase
    {
        /// <summary>
        /// Инициализирует базу данных (создает таблицу, если её нет).
        /// </summary>
        void InitializeDatabase();

        /// <summary>
        /// Добавляет новый продукт в базу.
        /// </summary>
        Guid AddProduct(Product product);

        /// <summary>
        /// Удаляет продукт по ID.
        /// </summary>
        bool DeleteProduct(Guid id);

        /// <summary>
        /// Обновляет данные продукта.
        /// </summary>
        bool UpdateProduct(Product product);

        /// <summary>
        /// Получает продукт по ID.
        /// </summary>
        Product GetProductById(Guid id);

        /// <summary>
        /// Получает все продукты из базы.
        /// </summary>
        IEnumerable<Product> GetAllProducts();
    }
}