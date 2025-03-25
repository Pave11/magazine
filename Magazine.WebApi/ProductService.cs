using Magazine.Core.Models;
using Magazine.Core.Services.Magazine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magazine.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new List<Product>();

        public Product Add(Product product)
        {
            try
            {
                _products.Add(product);
                return product;
            }
            catch (Exception ex)
            {
                // Обработка исключения
                throw new Exception("Ошибка при добавлении продукта", ex);
            }
        }

        public Product Remove(Guid id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    _products.Remove(product);
                    return product;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Обработка исключения
                throw new Exception("Ошибка при удалении продукта", ex);
            }
        }

        public Product Edit(Product product)
        {
            try
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    existingProduct.Definition = product.Definition;
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Image = product.Image;
                    return existingProduct;
                }
                return null;
            }
            catch (Exception ex)
            {
                // Обработка исключения
                throw new Exception("Ошибка при редактировании продукта", ex);
            }
        }

        public Product Search(string name, decimal? price)
        {
            try
            {
                return _products.FirstOrDefault(p =>
                    (string.IsNullOrEmpty(name) || p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                    (!price.HasValue || p.Price == price.Value));
            }
            catch (Exception ex)
            {
                // Обработка исключения
                throw new Exception("Ошибка при поиске продукта", ex);
            }
        }

        public Product GetById(Guid id) // Реализация метода GetById
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }
}