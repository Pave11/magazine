using Magazine.Core.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Magazine.Core.Services
{
    public class DataBase : IDataBase, IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly bool _ownsConnection;

        // SQL-запросы
        private const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS Products (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Definition TEXT,
                Price REAL NOT NULL,
                Image TEXT
            )";

        private const string CreateIndexQuery = @"
            CREATE INDEX IF NOT EXISTS IX_Products_Id ON Products(Id)";

        private const string InsertProductQuery = @"
            INSERT INTO Products (Id, Name, Definition, Price, Image)
            VALUES (@Id, @Name, @Definition, @Price, @Image)";

        private const string SelectProductByIdQuery = @"
            SELECT Id, Name, Definition, Price, Image 
            FROM Products 
            WHERE Id = @Id";

        private const string UpdateProductQuery = @"
            UPDATE Products 
            SET Name = @Name, Definition = @Definition, 
                Price = @Price, Image = @Image 
            WHERE Id = @Id";

        private const string DeleteProductQuery = @"
            DELETE FROM Products 
            WHERE Id = @Id";

        private const string SelectAllProductsQuery = @"
            SELECT Id, Name, Definition, Price, Image 
            FROM Products";

        // Основной конструктор
        public DataBase(SqliteConnection connection, bool ownsConnection = true)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _ownsConnection = ownsConnection;

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            InitializeDatabase();
        }

        // Альтернативный конструктор для строки подключения
        public DataBase(string connectionString)
            : this(new SqliteConnection(connectionString), true)
        {
        }

        public void InitializeDatabase()
        {
            ExecuteNonQuery(CreateTableQuery);
            ExecuteNonQuery(CreateIndexQuery);
        }

        private void ExecuteNonQuery(string query)
        {
            using var command = new SqliteCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public Guid AddProduct(Product product)
        {
            using var command = new SqliteCommand(InsertProductQuery, _connection);
            command.Parameters.AddWithValue("@Id", product.Id.ToString());
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Definition", product.Definition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
            return product.Id;
        }

        public Product GetProductById(Guid id)
        {
            using var command = new SqliteCommand(SelectProductByIdQuery, _connection);
            command.Parameters.AddWithValue("@Id", id.ToString());

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Name = reader["Name"].ToString(),
                    Definition = reader["Definition"]?.ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    Image = reader["Image"]?.ToString()
                };
            }
            return null;
        }

        public bool UpdateProduct(Product product)
        {
            using var command = new SqliteCommand(UpdateProductQuery, _connection);
            command.Parameters.AddWithValue("@Id", product.Id.ToString());
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Definition", product.Definition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@Image", product.Image ?? (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteProduct(Guid id)
        {
            using var command = new SqliteCommand(DeleteProductQuery, _connection);
            command.Parameters.AddWithValue("@Id", id.ToString());
            return command.ExecuteNonQuery() > 0;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var products = new List<Product>();

            using var command = new SqliteCommand(SelectAllProductsQuery, _connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                products.Add(new Product
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Name = reader["Name"].ToString(),
                    Definition = reader["Definition"]?.ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    Image = reader["Image"]?.ToString()
                });
            }

            return products;
        }

        public void Dispose()
        {
            if (_ownsConnection)
            {
                _connection?.Close();
                _connection?.Dispose();
            }
        }
    }
}