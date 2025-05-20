using Magazine.Core.Models;
using Magazine.Core.Services;
using Magazine.WebApi.Controllers;
using Magazine.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using System;
using System.IO;

namespace Magazine.Tests.Integration
{
    [TestFixture]
    public class ProductControllerIntegrationTests : IDisposable
    {
        private ProductController _controller;
        private SqliteConnection _connection;
        private readonly string _testDbPath;
        private bool _disposed;

        public ProductControllerIntegrationTests()
        {
            // Уникальное имя файла для каждого запуска тестов
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_controller_{Guid.NewGuid()}.db");
        }

        [SetUp]
        public void Setup()
        {
            // Убедимся, что файла нет (на случай предыдущих неудачных запусков)
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }

            // Создаем соединение с отключенным пулингом
            _connection = new SqliteConnection($"Data Source={_testDbPath};Pooling=false");
            _connection.Open();

            // Инициализируем цепочку зависимостей
            var db = new DataBase(_connection);
            var service = new ProductService(db);
            _controller = new ProductController(service);
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
            _controller = null;

            // Явно закрываем соединение
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;

            // Пытаемся удалить файл БД
            TryDeleteDatabaseFile();

            _disposed = true;
        }

        private void TryDeleteDatabaseFile()
        {
            if (!File.Exists(_testDbPath)) return;

            // Делаем несколько попыток удаления
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

        [Test]
        public void AddAndGet_ShouldPersistProductInDatabase()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Price = 100.50m };

            // Act
            var addResult = _controller.Add(product).Result as CreatedAtActionResult;
            var addedProduct = addResult?.Value as Product;

            var getResult = _controller.GetById(addedProduct?.Id ?? Guid.Empty).Result as OkObjectResult;
            var retrievedProduct = getResult?.Value as Product;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(addedProduct, "Product should be added");
                Assert.IsNotNull(retrievedProduct, "Product should be retrievable");
                Assert.AreEqual(product.Name, retrievedProduct.Name, "Names should match");
                Assert.AreEqual(product.Price, retrievedProduct.Price, "Prices should match");
            });
        }
    }
}