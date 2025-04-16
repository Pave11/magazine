using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.WebApi.Services;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Magazine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Magazine.Tests
{
    [TestFixture]
    public class ProductControllerIntegrationTests
    {
        private ProductController _controller;
        private ProductService _service;
        private string _testFilePath = "integration_test_products.txt";

        [SetUp]
        public void Setup()
        {
            // Настройка конфигурации
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[] {
                    new KeyValuePair<string, string>("DataBaseFilePath", _testFilePath)
                })
                .Build();

            // Инициализация сервиса и контроллера с реальными зависимостями
            _service = new ProductService(configuration);
            _controller = new ProductController(_service);

            // Очистить тестовый файл перед каждым тестом
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            // Удалить тестовый файл после каждого теста
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Test]
        public void AddAndGet_IntegrationTest()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "Integration Test Product",
                Definition = "Test Desc",
                Price = 500,
                Image = "test.jpg"
            };

            // Act - Добавление продукта
            var addActionResult = _controller.Add(newProduct);

            // Assert - Проверка типа результата
            Assert.IsInstanceOf<ActionResult<Product>>(addActionResult);

            // Получаем результат как CreatedAtActionResult
            var createdAtActionResult = addActionResult.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult, "Результат должен быть типа CreatedAtActionResult");

            var addedProduct = createdAtActionResult.Value as Product;
            Assert.IsNotNull(addedProduct, "Значение результата должно быть типа Product");
            Assert.IsNotNull(addedProduct.Id, "ID продукта не должен быть null");

            // Act - Получение продукта
            var getActionResult = _controller.GetById(addedProduct.Id);

            // Assert - Проверка типа результата
            Assert.IsInstanceOf<ActionResult<Product>>(getActionResult);

            // Получаем результат как OkObjectResult
            var okObjectResult = getActionResult.Result as OkObjectResult;
            Assert.IsNotNull(okObjectResult, "Результат должен быть типа OkObjectResult");

            var retrievedProduct = okObjectResult.Value as Product;
            Assert.IsNotNull(retrievedProduct, "Полученный продукт не должен быть null");

            // Assert - Проверка данных
            Assert.AreEqual(addedProduct.Id, retrievedProduct.Id);
            Assert.AreEqual(newProduct.Name, retrievedProduct.Name);
            Assert.AreEqual(newProduct.Definition, retrievedProduct.Definition);
            Assert.AreEqual(newProduct.Price, retrievedProduct.Price);
            Assert.AreEqual(newProduct.Image, retrievedProduct.Image);
        }

        [Test]
        public void AddEditGet_IntegrationTest()
        {
            // Arrange
            var initialProduct = new Product
            {
                Name = "Initial Product",
                Definition = "Initial Desc",
                Price = 100,
                Image = "initial.jpg"
            };

            // Act - Добавление продукта
            var addResult = _controller.Add(initialProduct);
            var addedProduct = (addResult.Result as CreatedAtActionResult)?.Value as Product;

            // Act - Редактирование продукта
            var updatedProduct = new Product
            {
                Id = addedProduct.Id,
                Name = "Updated Product",
                Definition = "Updated Desc",
                Price = 200,
                Image = "updated.jpg"
            };

            var editResult = _controller.Edit(updatedProduct);
            var editedProduct = (editResult.Result as OkObjectResult)?.Value as Product;

            // Act - Получение продукта
            var getResult = _controller.GetById(addedProduct.Id);
            var retrievedProduct = (getResult.Result as OkObjectResult)?.Value as Product;

            // Assert
            Assert.AreEqual(updatedProduct.Name, retrievedProduct.Name);
            Assert.AreEqual(updatedProduct.Definition, retrievedProduct.Definition);
            Assert.AreEqual(updatedProduct.Price, retrievedProduct.Price);
            Assert.AreEqual(updatedProduct.Image, retrievedProduct.Image);
        }
    }
}