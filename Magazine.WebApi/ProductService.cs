using System;
using System.Collections.Generic;
using Magazine.Core.Models;
using Magazine.Core.Services;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        // Здесь вы можете использовать коллекцию для хранения продуктов
        // В реальном приложении вы бы использовали контекст базы данных
        private readonly List<Product> _products = new List<Product>();

        public Product Add(Product product)
        {
            try
            {
                // Добавление нового продукта
                _products.Add(product);
                return product; // Возвращаем созданный продукт
            }
            catch (Exception ex)
            {
                // Обработка исключений должна происходить в вызывающем методе
                throw new Exception("Ошибка при добавлении продукта", ex);
            }
        }

        public Product Remove(Guid id)
        {
            try
            {
                // Поиск продукта по идентификатору
                var product = _products.Find(p => p.Id == id);
                if (product == null)
                {
                    return null; // Если продукт не найден, возвращаем null
                }

                // Удаление продукта
                _products.Remove(product);
                return product; // Возвращаем удалённый продукт
            }
            catch (Exception ex)
            {
                // Обработка исключений должна происходить в вызывающем методе
                throw new Exception("Ошибка при удалении продукта", ex);
            }
        }

        public Product Edit(Product product)
        {
            try
            {
                // Поиск продукта по идентификатору
                var existingProduct = _products.Find(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    return null; // Если продукт не найден, возвращаем null
                }

                // Обновление данных продукта
                existingProduct.Name = product.Name;
                existingProduct.Definition = product.Definition;
                existingProduct.Price = product.Price;
                existingProduct.Image = product.Image;

                return existingProduct; // Возвращаем изменённый продукт
            }
            catch (Exception ex)
            {
                // Обработка исключений должна происходить в вызывающем методе
                throw new Exception("Ошибка при редактировании продукта", ex);
            }
        }

        public Product Search(Guid id)
        {
            try
            {
                // Поиск продукта по идентификатору
                return _products.Find(p => p.Id == id); // Возвращаем найденный продукт или null
            }
            catch (Exception ex)
            {
                // Обработка исключений должна происходить в вызывающем методе
                throw new Exception("Ошибка при поиске продукта", ex);
            }
        }
    }
}