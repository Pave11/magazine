using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Magazine.Tests.Integration
{
    [TestFixture]
    public class DataBaseIntegrationTests
    {
        private IDataBase _database;
        private SqliteConnection _connection;
        private string _testDbPath;

        [SetUp]
        public void Setup()
        {
            // Используем уникальное имя файла для каждого теста
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_db_{Guid.NewGuid()}.db");

            // Создаем и открываем соединение
            _connection = new SqliteConnection($"Data Source={_testDbPath};Pooling=false");
            _connection.Open();

            // Инициализация БД
            _database = new DataBase(_connection);
        }

        [TearDown]
        public void Cleanup()
        {
            // Освобождаем ресурсы в правильном порядке
            (_database as IDisposable)?.Dispose();

            // Явно закрываем и освобождаем соединение
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;

            // Удаляем тестовую БД с несколькими попытками
            if (File.Exists(_testDbPath))
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        File.Delete(_testDbPath);
                        break;
                    }
                    catch (IOException) when (i < 4)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }

        [Test]
        public void AddProduct_ShouldPersistProductInDatabase()
        {
            var product = new Product
            {
                Name = "Test Product",
                Price = 99.99m,
                Definition = "Test Description",
                Image = "test.jpg"
            };

            var productId = _database.AddProduct(product);
            var retrievedProduct = _database.GetProductById(productId);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(retrievedProduct);
                Assert.AreEqual(product.Name, retrievedProduct.Name);
                Assert.AreEqual(product.Price, retrievedProduct.Price);
                Assert.AreEqual(product.Definition, retrievedProduct.Definition);
                Assert.AreEqual(product.Image, retrievedProduct.Image);
            });
        }


        [Test]
        public void GetProductById_ShouldReturnNull_ForNonExistentId()
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
            var productId = _database.AddProduct(originalProduct);

            var updatedProduct = new Product
            {
                Id = productId,
                Name = "Updated",
                Price = 100,
                Definition = "Updated Description",
                Image = "updated.jpg"
            };

            // Act
            var updateResult = _database.UpdateProduct(updatedProduct);
            var retrievedProduct = _database.GetProductById(productId);

            // Assert
            Assert.IsTrue(updateResult);
            Assert.IsNotNull(retrievedProduct);
            Assert.AreEqual("Updated", retrievedProduct.Name);
            Assert.AreEqual(100, retrievedProduct.Price);
            Assert.AreEqual("Updated Description", retrievedProduct.Definition);
            Assert.AreEqual("updated.jpg", retrievedProduct.Image);
        }

        [Test]
        public void DeleteProduct_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var product = new Product { Name = "To Delete", Price = 200 };
            var productId = _database.AddProduct(product);

            // Act
            var deleteResult = _database.DeleteProduct(productId);
            var retrievedProduct = _database.GetProductById(productId);

            // Assert
            Assert.IsTrue(deleteResult);
            Assert.IsNull(retrievedProduct);
        }

        [Test]
        public void GetAllProducts_ShouldReturnAllAddedProducts()
        {
            // Arrange
            var product1 = new Product { Name = "Product 1", Price = 10 };
            var product2 = new Product { Name = "Product 2", Price = 20 };

            _database.AddProduct(product1);
            _database.AddProduct(product2);

            // Act
            var allProducts = _database.GetAllProducts().ToList();

            // Assert
            Assert.AreEqual(2, allProducts.Count);
            Assert.IsTrue(allProducts.Any(p => p.Name == "Product 1"));
            Assert.IsTrue(allProducts.Any(p => p.Name == "Product 2"));
        }

        [Test]
        public void AddProduct_ShouldAutoGenerateId_WhenNotProvided()
        {
            // Arrange
            var product = new Product { Name = "No ID", Price = 30 };

            // Act
            var productId = _database.AddProduct(product);

            // Assert
            Assert.AreNotEqual(Guid.Empty, productId);
            Assert.AreEqual(productId, product.Id);
        }

        [Test]
        public void UpdateProduct_ShouldReturnFalse_ForNonExistentProduct()
        {
            // Arrange
            var nonExistentProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Non-existent",
                Price = 100
            };

            // Act
            var result = _database.UpdateProduct(nonExistentProduct);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteProduct_ShouldReturnFalse_ForNonExistentProduct()
        {
            // Act
            var result = _database.DeleteProduct(Guid.NewGuid());

            // Assert
            Assert.IsFalse(result);
        }
    }
}