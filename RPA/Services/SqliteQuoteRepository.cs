using Microsoft.Data.Sqlite;
using Shared.Interfaces;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Services
{
    public class SqliteQuoteRepository : IQuoteRepository
    {
        private readonly string _connectionString;

        public SqliteQuoteRepository(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Quotes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Text TEXT NOT NULL,
                Author TEXT NOT NULL,
                Tags TEXT,
                CreatedAt TEXT NOT NULL
            )";
            command.ExecuteNonQuery();
        }

        public async Task<IEnumerable<Quote>> GetAllAsync()
        {
            var quotes = new List<Quote>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Text, Author, Tags, CreatedAt FROM Quotes";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                quotes.Add(new Quote
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    Author = reader.GetString(2),
                    Tags = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }
            return quotes;
        }

        public async Task<Quote?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Text, Author, Tags, CreatedAt FROM Quotes WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Quote
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    Author = reader.GetString(2),
                    Tags = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };
            }
            return null;
        }

        public async Task<IEnumerable<Quote>> GetByAuthorAsync(string author)
        {
            var quotes = new List<Quote>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Text, Author, Tags, CreatedAt FROM Quotes WHERE Author LIKE @author";
            command.Parameters.AddWithValue("@author", $"%{author}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                quotes.Add(new Quote
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    Author = reader.GetString(2),
                    Tags = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }
            return quotes;
        }

        public async Task AddAsync(Quote quote)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
            INSERT INTO Quotes (Text, Author, Tags, CreatedAt)
            VALUES (@text, @author, @tags, @createdAt)";
            command.Parameters.AddWithValue("@text", quote.Text);
            command.Parameters.AddWithValue("@author", quote.Author);
            command.Parameters.AddWithValue("@tags", quote.Tags ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@createdAt", quote.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Quote quote)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
            UPDATE Quotes SET Text = @text, Author = @author, Tags = @tags, CreatedAt = @createdAt
            WHERE Id = @id";
            command.Parameters.AddWithValue("@text", quote.Text);
            command.Parameters.AddWithValue("@author", quote.Author);
            command.Parameters.AddWithValue("@tags", quote.Tags ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@createdAt", quote.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@id", quote.Id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Quotes WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
