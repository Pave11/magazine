using NUnit.Framework;
using Moq;
using Magazine.Core.Services;
using Magazine.Core.Models;
using System;
using System.Collections.Generic;
using Magazine.WebApi.Services;

namespace Magazine.Tests.Unit.Services
{
    [TestFixture]
    public class TestsProductService
    {
        private ProductService _productService;
        private Mock<IDataBase> _mockDatabase;
        private List<Product> _testProducts;

        [SetUp]
        public void Setup()
        {
            _mockDatabase = new Mock<IDataBase>();
            _productService = new ProductService(_mockDatabase.Object);

            _testProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 100 },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 200 }
            };
        }

        [Test]
        public void Add_Product_GeneratesNewIdAndCallsDatabase()
        {
            // Arrange
            var newProduct = new Product { Name = "New Product", Price = 300 };
            _mockDatabase.Setup(db => db.AddProduct(It.IsAny<Product>())).Returns(newProduct.Id);

            // Act
            var result = _productService.Add(newProduct);

            // Assert
            Assert.IsNotNull(result.Id);
            Assert.AreNotEqual(Guid.Empty, result.Id);
            _mockDatabase.Verify(db => db.AddProduct(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public void Remove_ExistingId_DeletesFromDatabase()
        {
            // Arrange
            var productId = _testProducts[0].Id;
            _mockDatabase.Setup(db => db.GetProductById(productId)).Returns(_testProducts[0]);
            _mockDatabase.Setup(db => db.DeleteProduct(productId)).Returns(true);

            // Act
            var result = _productService.Remove(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.Id);
            _mockDatabase.Verify(db => db.DeleteProduct(productId), Times.Once);
        }

        [Test]
        public void Edit_ExistingProduct_UpdatesInDatabase()
        {
            // Arrange
            var updatedProduct = new Product
            {
                Id = _testProducts[0].Id,
                Name = "Updated",
                Price = 999
            };
            _mockDatabase.Setup(db => db.GetProductById(updatedProduct.Id)).Returns(_testProducts[0]);
            _mockDatabase.Setup(db => db.UpdateProduct(updatedProduct)).Returns(true);

            // Act
            var result = _productService.Edit(updatedProduct);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", result.Name);
            _mockDatabase.Verify(db => db.UpdateProduct(updatedProduct), Times.Once);
        }

        [Test]
        public void GetById_ExistingId_ReturnsProduct()
        {
            // Arrange
            var productId = _testProducts[0].Id;
            _mockDatabase.Setup(db => db.GetProductById(productId)).Returns(_testProducts[0]);

            // Act
            var result = _productService.GetById(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.Id);
        }
    }
}