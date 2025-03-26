using Magazine.Core.Models;
using Magazine.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new List<Product>();
        private readonly string _dataBaseFilePath;
        private readonly Mutex _mutex; // Объявление мьютекса

        public ProductService(IConfiguration configuration)
        {
            _dataBaseFilePath = configuration["DataBaseFilePath"];
            LoadProducts();
            _mutex = new Mutex(); // Инициализация мьютекса
        }

        public Product Add(Product product)
        {
            _mutex.WaitOne();           
           product.Id = Guid.NewGuid();
            _products.Add(product); 
            WriteToFile();
            _mutex.ReleaseMutex();
            return product;
        }

        public Product Remove(Guid id)
        {
            _mutex.WaitOne(); // Добавлено ожидание мьютекса
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product != null)
                {
                    _products.Remove(product);
                    WriteToFile();
                    return product;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удалении продукта", ex);
            }
            finally
            {
                _mutex.ReleaseMutex(); // Обязательно освобождаем мьютекс
            }
        }

        public Product Edit(Product product)
        {
            _mutex.WaitOne(); // Добавлено ожидание мьютекса
            try
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct != null)
                {
                    existingProduct.Definition = product.Definition;
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Image = product.Image;
                    WriteToFile();
                    return existingProduct;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при редактировании продукта", ex);
            }
            finally
            {
                _mutex.ReleaseMutex(); // Обязательно освобождаем мьютекс
            }
        }

        public Product GetById(Guid id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        private void LoadProducts()
        {
            if (File.Exists(_dataBaseFilePath))
            {
                var lines = File.ReadAllLines(_dataBaseFilePath);
                foreach (var line in lines)
                {
                    var product = DeserializeProduct(line);
                    if (product != null)
                    {
                        _products.Add(product);
                    }
                }
            }
        }

        private void WriteToFile()
        {
            var lines = _products.Select(SerializeProduct).ToArray();
            File.WriteAllLines(_dataBaseFilePath, lines);
        }

        private string SerializeProduct(Product product)
        {
            return $"{product.Id};{product.Name};{product.Definition};{product.Price};{product.Image}";
        }

        private Product DeserializeProduct(string line)
        {
            var parts = line.Split(';');
            if (parts.Length == 5 && Guid.TryParse(parts[0], out var id))
            {
                return new Product
                {
                    Id = id,
                    Name = parts[1],
                    Definition = parts[2],
                    Price = decimal.TryParse(parts[3], out var price) ? price : 0,
                    Image = parts[4]
                };
            }
            return null;
        }
    }
}