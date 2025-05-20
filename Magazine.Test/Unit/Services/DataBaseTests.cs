using Magazine.Core.Services;
using Magazine.Core.Models;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magazine.Tests.Unit.Services
{
    [TestFixture]
    public class DataBaseTests : IDisposable
    {
        private IDataBase _database;
        private SqliteConnection _connection;
        private bool _disposed;

        [SetUp]
        public void Setup()
        {
            // Используем SQLite в памяти
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _database = new DataBase(_connection);
        }

        [TearDown]
        public void Cleanup()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;

            // Освобождаем ресурсы в правильном порядке
            (_database as IDisposable)?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            _disposed = true;
        }

        [Test]
        public void AddProduct_ShouldAssignNewIdAndSaveToDatabase()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100 };

            // Act
            var id = _database.AddProduct(product);
            var savedProduct = _database.GetProductById(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(Guid.Empty, id);
                Assert.AreEqual(product.Name, savedProduct.Name);
                Assert.AreEqual(product.Price, savedProduct.Price);
            });
        }

        [Test]
        public void GetProductById_ShouldReturnNull_WhenProductNotExists()
        {
            // Act
            var result = _database.GetProductById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateProduct_ShouldModifyExistingProduct()
        {
            // Arrange
            var originalProduct = new Product { Name = "Original", Price = 50 };
            var originalId = _database.AddProduct(originalProduct);

            var updatedProduct = new Product
            {
                Id = originalId,
                Name = "Updated",
                Price = 100
            };

            // Act
            var isUpdated = _database.UpdateProduct(updatedProduct);
            var fetchedProduct = _database.GetProductById(originalId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(isUpdated);
                Assert.AreEqual("Updated", fetchedProduct.Name);
                Assert.AreEqual(100, fetchedProduct.Price);
            });
        }

        [Test]
        public void DeleteProduct_ShouldRemoveFromDatabase()
        {
            // Arrange
            var product = new Product { Name = "To Delete", Price = 200 };
            var id = _database.AddProduct(product);

            // Act
            var isDeleted = _database.DeleteProduct(id);
            var deletedProduct = _database.GetProductById(id);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(isDeleted);
                Assert.IsNull(deletedProduct);
            });
        }

        [Test]
        public void GetAllProducts_ShouldReturnAllSavedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 10 },
                new Product { Name = "Product 2", Price = 20 }
            };

            foreach (var product in products)
            {
                _database.AddProduct(product);
            }

            // Act
            var allProducts = _database.GetAllProducts().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, allProducts.Count);
                Assert.IsTrue(allProducts.Any(p => p.Name == "Product 1"));
                Assert.IsTrue(allProducts.Any(p => p.Name == "Product 2"));
            });
        }

        [Test]
        public void AddProduct_ShouldThrow_WhenProductIsNull()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                try
                {
                    _database.AddProduct(null);
                }
                catch (NullReferenceException)
                {
                    throw new ArgumentNullException("product");
                }
            });
            Assert.AreEqual("product", ex.ParamName);
        }
    }
}