using Magazine.Core.Models;
using Magazine.Core.Services;
using Microsoft.Data.Sqlite;
using System;

namespace Magazine.Tests.Shared
{
    public class DatabaseFixture : IDisposable
    {
        public IDataBase Database { get; }
        public string TestDbPath { get; } = "test_fixture.db";

        public DatabaseFixture()
        {
            // Удаляем старую тестовую БД, если существует
            if (System.IO.File.Exists(TestDbPath))
            {
                System.IO.File.Delete(TestDbPath);
            }

            // Создаем новую БД и инициализируем
            Database = new DataBase($"Data Source={TestDbPath}");
        }

        public void Dispose()
        {
            // Очищаем ресурсы
            (Database as IDisposable)?.Dispose();

            // Удаляем тестовую БД
            try
            {
                if (System.IO.File.Exists(TestDbPath))
                {
                    System.IO.File.Delete(TestDbPath);
                }
            }
            catch
            {
                // Игнорируем ошибки удаления файла
            }
        }

        public Product CreateTestProductInDatabase(string name = "Test Product", decimal price = 100m)
        {
            var product = new Product
            {
                Name = name,
                Price = price,
                Definition = "Test Description",
                Image = "test.jpg"
            };

            product.Id = Database.AddProduct(product);
            return product;
        }

        public void ExecuteNonQuery(string sql)
        {
            using var connection = new SqliteConnection($"Data Source={TestDbPath}");
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}