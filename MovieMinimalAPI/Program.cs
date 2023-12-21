using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;


namespace MovieMinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Activer CORS
            builder.Services.AddCors();

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

            // Activer CORS (autoriser toutes les origines, m�thodes et en-t�tes pour simplifier ; ajustez selon vos besoins)
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


            app.UseHttpsRedirection();

            app.MapGet("/movies/{id}", (int id) => GetOneMovie(id));

            app.MapGet("/movies", GetAllMovie);

            app.MapPost("/movies", (Movie newMovie) =>
            {
                Movie created = AddMovie(newMovie);
                return Results.Created($"/movies/{created.Id}", created);
            });

            app.MapPost("/actors", (Actor newActor) =>
            {
                Actor created = AddActor(newActor);
                return Results.Created($"/actors/{created.Id}", created);
            });

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


        private static Movie GetOneMovie(int id)
        {

            Movie OneMovie = new();


            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandSelect = new MySqlCommand("SELECT * FROM movie WHERE Id_movie = @Id;", connection);
            commandSelect.Parameters.AddWithValue("@Id", id);

            using var reader = commandSelect.ExecuteReader();

            if (reader.Read())
            {
                OneMovie = new Movie
                {
                    Id = (int)reader["Id_movie"],
                    Title = reader["title"].ToString(),
                    ReleaseYear = (DateTime)reader["release_year"],
                    CreateDate = (DateTime)reader["creation_date_movie"],
                    Duration = (int)reader["duration"]
                };
            }

            return OneMovie;
        }

        private static Movie AddMovie([FromBody] Movie newMovie)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandInsert = new MySqlCommand("INSERT INTO movie (title, release_year) VALUES (@titre, @dateSortie); SELECT LAST_INSERT_ID();", connection);
            commandInsert.Parameters.AddWithValue("@titre", newMovie.Title);
            commandInsert.Parameters.AddWithValue("@dateSortie", newMovie.ReleaseYear);

            newMovie.Id = Convert.ToInt32(commandInsert.ExecuteScalar());

            return newMovie;
        }

        private static Actor AddActor([FromBody] Actor newActor)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandInsert = new MySqlCommand("INSERT INTO actor (firstname_actor, lastname_actor, birthdate_actor) VALUES (@prenom, @nom, @age);SELECT LAST_INSERT_ID();", connection);
            commandInsert.Parameters.AddWithValue("@prenom", newActor.FirstName);
            commandInsert.Parameters.AddWithValue("@nom", newActor.LastName);
            commandInsert.Parameters.AddWithValue("@age", newActor.Birthdate);

            newActor.Id = Convert.ToInt32(commandInsert.ExecuteScalar());

            return newActor;

        }



    }


}
