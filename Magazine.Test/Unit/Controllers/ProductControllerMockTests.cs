using NUnit.Framework;
using Moq;
using Magazine.WebApi.Controllers;
using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Magazine.Tests.Unit.Controllers
{
    [TestFixture]
    public class ProductControllerMockTests
    {
        private ProductController _controller;
        private Mock<IProductService> _mockProductService;
        private Product _testProduct;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);

            _testProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Definition = "Test Description",
                Price = 100,
                Image = "test.jpg"
            };
        }

        [Test]
        public void Add_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            _mockProductService.Setup(s => s.Add(It.IsAny<Product>())).Returns(_testProduct);

            // Act
            var result = _controller.Add(_testProduct);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            _mockProductService.Verify(s => s.Add(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public void Add_NullProduct_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Add(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public void Remove_ExistingId_ReturnsOkResult()
        {
            // Arrange
            _mockProductService.Setup(s => s.Remove(_testProduct.Id)).Returns(_testProduct);

            // Act
            var result = _controller.Remove(_testProduct.Id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            _mockProductService.Verify(s => s.Remove(_testProduct.Id), Times.Once);
        }

        [Test]
        public void Remove_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.Remove(It.IsAny<Guid>())).Returns((Product)null);

            // Act
            var result = _controller.Remove(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }

        [Test]
        public void Edit_ValidProduct_ReturnsOkResult()
        {
            // Arrange
            _mockProductService.Setup(s => s.Edit(_testProduct)).Returns(_testProduct);

            // Act
            var result = _controller.Edit(_testProduct);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            _mockProductService.Verify(s => s.Edit(_testProduct), Times.Once);
        }

        [Test]
        public void Edit_NullProduct_ReturnsBadRequest()
        {
            // Act
            var result = _controller.Edit(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public void GetById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetById(_testProduct.Id)).Returns(_testProduct);

            // Act
            var result = _controller.GetById(_testProduct.Id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            _mockProductService.Verify(s => s.GetById(_testProduct.Id), Times.Once);
        }

        [Test]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Product)null);

            // Act
            var result = _controller.GetById(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        }
    }
}