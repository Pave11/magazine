using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Magazine.Tests.Unit.Controllers
{
    [TestFixture]
    public class ProductControllerStubTests
    {
        // Ручная реализация Stub для IProductService
        private class StubProductService : IProductService
        {
            public Product Add(Product product)
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));
                return product; // Просто возвращаем тот же объект
            }

            public Product Remove(Guid id)
            {
                if (id == Guid.Empty)
                    return null;
                return new Product { Id = id }; // Возвращаем продукт с переданным ID
            }

            public Product Edit(Product product)
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product));
                return product; // Имитируем успешное обновление
            }

            public Product GetById(Guid id)
            {
                if (id == Guid.Empty)
                    return null;
                return new Product { Id = id }; // Всегда возвращаем продукт с запрошенным ID
            }
        }

        private ProductController _controller;
        private StubProductService _stubService;

        [SetUp]
        public void Setup()
        {
            _stubService = new StubProductService();
            _controller = new ProductController(_stubService); // Внедряем Stub
        }

        // Тест успешного добавления
        [Test]
        public void Add_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            var testProduct = new Product { Name = "Test" };

            // Act
            var result = _controller.Add(testProduct);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.AreEqual(testProduct.Name, ((Product)createdResult.Value).Name);
        }

        // Тест добавления null
        [Test]
        public void Add_NullProduct_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Add(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        // Тест получения существующего продукта
        [Test]
        public void GetById_ValidId_ReturnsProduct()
        {
            // Arrange
            var testId = Guid.NewGuid();

            // Act
            var result = _controller.GetById(testId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(testId, ((Product)okResult.Value).Id);
        }

        // Тест попытки удаления
        [Test]
        public void Remove_ExistingId_ReturnsOk()
        {
            // Act
            var result = _controller.Remove(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        // Тест редактирования
        [Test]
        public void Edit_ValidProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var testProduct = new Product { Id = Guid.NewGuid(), Name = "Updated" };

            // Act
            var result = _controller.Edit(testProduct);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(testProduct.Name, ((Product)okResult.Value).Name);
        }
    }
}