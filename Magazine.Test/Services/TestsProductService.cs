using NUnit.Framework;
using Moq;
using Magazine.WebApi.Services;
using Magazine.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magazine.Tests
{
    [TestFixture]
    public class TestsProductService
    {
        private ProductService _productService;
        private Mock<IConfiguration> _mockConfiguration;
        private List<Product> _testProducts;
        private string _testFilePath = "test_products.txt";

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["DataBaseFilePath"]).Returns(_testFilePath);

            _testProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Definition = "Desc 1", Price = 100, Image = "img1.jpg" },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Definition = "Desc 2", Price = 200, Image = "img2.jpg" }
            };

            // Очистить тестовый файл перед каждым тестом
            if (System.IO.File.Exists(_testFilePath))
            {
                System.IO.File.Delete(_testFilePath);
            }
        }

        [TearDown]
        public void Cleanup()
        {
            // Удалить тестовый файл после каждого теста
            if (System.IO.File.Exists(_testFilePath))
            {
                System.IO.File.Delete(_testFilePath);
            }
        }

        [Test]
        public void Add_Product_AddsToCollectionAndFile()
        {
            // Arrange
            _productService = new ProductService(_mockConfiguration.Object);
            var newProduct = new Product { Name = "New Product", Definition = "New Desc", Price = 300, Image = "new.jpg" };

            // Act
            var result = _productService.Add(newProduct);

            // Assert
            Assert.IsNotNull(result.Id);
            Assert.AreEqual(newProduct.Name, result.Name);
            Assert.AreEqual(1, System.IO.File.ReadAllLines(_testFilePath).Length);
        }

        [Test]
        public void Remove_ExistingId_RemovesFromCollectionAndFile()
        {
            // Arrange
            System.IO.File.WriteAllLines(_testFilePath, _testProducts.Select(p => $"{p.Id};{p.Name};{p.Definition};{p.Price};{p.Image}"));
            _productService = new ProductService(_mockConfiguration.Object);
            var productToRemove = _testProducts[0];

            // Act
            var result = _productService.Remove(productToRemove.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productToRemove.Id, result.Id);
            Assert.AreEqual(_testProducts.Count - 1, System.IO.File.ReadAllLines(_testFilePath).Length);
        }

        [Test]
        public void Remove_NonExistingId_ReturnsNull()
        {
            // Arrange
            _productService = new ProductService(_mockConfiguration.Object);

            // Act
            var result = _productService.Remove(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Edit_ExistingProduct_UpdatesInCollectionAndFile()
        {
            // Arrange
            System.IO.File.WriteAllLines(_testFilePath, _testProducts.Select(p => $"{p.Id};{p.Name};{p.Definition};{p.Price};{p.Image}"));
            _productService = new ProductService(_mockConfiguration.Object);
            var productToEdit = _testProducts[0];
            var updatedProduct = new Product
            {
                Id = productToEdit.Id,
                Name = "Updated Name",
                Definition = "Updated Desc",
                Price = 999,
                Image = "updated.jpg"
            };

            // Act
            var result = _productService.Edit(updatedProduct);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedProduct.Name, result.Name);
            Assert.AreEqual(updatedProduct.Price, result.Price);

            var fileLines = System.IO.File.ReadAllLines(_testFilePath);
            Assert.IsTrue(fileLines.Any(line => line.Contains(updatedProduct.Name)));
        }

        [Test]
        public void Edit_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            _productService = new ProductService(_mockConfiguration.Object);
            var nonExistingProduct = new Product { Id = Guid.NewGuid(), Name = "Non Existing" };

            // Act
            var result = _productService.Edit(nonExistingProduct);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetById_ExistingId_ReturnsProduct()
        {
            // Arrange
            System.IO.File.WriteAllLines(_testFilePath, _testProducts.Select(p => $"{p.Id};{p.Name};{p.Definition};{p.Price};{p.Image}"));
            _productService = new ProductService(_mockConfiguration.Object);
            var expectedProduct = _testProducts[0];

            // Act
            var result = _productService.GetById(expectedProduct.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedProduct.Id, result.Id);
            Assert.AreEqual(expectedProduct.Name, result.Name);
        }

        [Test]
        public void GetById_NonExistingId_ReturnsNull()
        {
            // Arrange
            _productService = new ProductService(_mockConfiguration.Object);

            // Act
            var result = _productService.GetById(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }
    }
}