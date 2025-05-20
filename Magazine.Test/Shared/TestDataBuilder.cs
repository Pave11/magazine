using Magazine.Core.Models;
using System;
using System.Collections.Generic;

namespace Magazine.Tests.Shared
{
    public static class TestDataBuilder
    {
        public static Product CreateProduct(
            Guid? id = null,
            string name = "Test Product",
            decimal price = 100m,
            string definition = "Test Description",
            string image = "test.jpg")
        {
            return new Product
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Price = price,
                Definition = definition,
                Image = image
            };
        }

        public static List<Product> CreateProductList(int count)
        {
            var products = new List<Product>();
            for (int i = 1; i <= count; i++)
            {
                products.Add(CreateProduct(
                    name: $"Product {i}",
                    price: 10m * i
                ));
            }
            return products;
        }

        public static Product CreateInvalidProduct()
        {
            return new Product
            {
                Id = Guid.Empty,
                Name = "",
                Price = -10m,
                Definition = null,
                Image = null
            };
        }

        public static Product CreateProductWithLongName(int length = 100)
        {
            return CreateProduct(
                name: new string('X', length)
            );
        }

        public static Product CreateProductWithMaxValues()
        {
            return CreateProduct(
                price: decimal.MaxValue,
                definition: new string('D', 1000),
                image: $"http://test.com/{new string('I', 200)}.jpg"
            );
        }
    }
}