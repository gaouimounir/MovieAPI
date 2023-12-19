using System.Data.Common;
using MySql.Data.MySqlClient;
using Npgsql;

namespace MovieMinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapGet("/movies/{1}", async () =>
            {

            });
            app.MapGet("/movies", GetAllMovie);

            app.Run();
        }

        private static Movie[] GetAllMovie()
        {
            List<Movie> movies = new();

            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";


            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandSelect = new MySqlCommand("SELECT * FROM movie;", connection);
            using var reader = commandSelect.ExecuteReader();
            {
                while (reader.Read())
                {
                    Movie movieToAdd = new()
                    {
                        Id = (int)reader["Id_movie"],
                        Title = reader["title"].ToString(),
                        ReleaseYear = (DateTime)reader["release_year"],
                        CreateDate = (DateTime)reader["creation_date_movie"],
                        Duration = (int)reader["duration"]
                    };
                    movies.Add(movieToAdd);
                }
            }

            return movies.ToArray();
        }
    }


}
