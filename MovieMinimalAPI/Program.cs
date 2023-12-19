using System.Data.Common;

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

            var connectionPgSqlString = "Host=localhost;Port=5432;Username=postgres;Password=admin;Database=Streaming;Pooling=true;";
            //string connectionMySqlString = "TODO";
            //string connectionMsSqlString = "TODO";

            using var dataSource = NpgsqlDataSource.Create(connectionPgSqlString);
            using var cmd = dataSource.CreateCommand("SELECT * FROM nomTablemovie");
            using var reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    Movie movieToAdd = new()
                    {
                        Id = (int)reader["nomColonneId"],
                        Title = reader["nomColonneTitle"].ToString(),
                        ReleaseYear = (int)reader["nomColonneReleaseYear"],
                        CreateDate = (DateTime)reader["nomColonneCreateDate"],
                    };
                    movies.Add(movieToAdd);
                }
            }

            return movies.ToArray();
        }
    }

   
}
