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

            app.MapPost("/movies", (PostMovie newMovie) =>
            {
                PostMovie created = AddMovie(newMovie);
                return Results.Created($"/movies/{created.Id}", created);
            });

            app.MapPost("/actors", (Actor newActor) =>
            {
                Actor created = AddActor(newActor);
                return Results.Created($"/actors/{created.Id}", created);
            });

            app.MapDelete("/actors/{id}", (int id) =>
            {
                bool delete = DeleteActor(id);
                if (delete)
                {
                    return Results.Ok($"Acteur {id} supprimé");
                }
                else
                {
                    return Results.NotFound($"Acteur avec l'ID {id} non trouvé.");
                }
            });

            app.MapPut("/movies/{id}", (int id, Movie updatedMovie) =>
            {
                Movie modifiedMovie = UpdateMovie(id, updatedMovie);
                if (modifiedMovie != null)
                {
                    return Results.Ok(modifiedMovie);
                }
                else
                {
                    return Results.NotFound($"Film avec l'ID {id} non trouvé.");
                }
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
                        ReleaseYear = (int)reader["release_year"],
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
                    ReleaseYear = (int)reader["release_year"],
                    CreateDate = (DateTime)reader["creation_date_movie"],
                    Duration = (int)reader["duration"]
                };
            }

            return OneMovie;
        }

        private static PostMovie AddMovie([FromBody] PostMovie newMovie)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandInsert = new MySqlCommand("INSERT INTO movie (title, release_year) VALUES (@titre, @dateSortie); SELECT LAST_INSERT_ID();", connection);
            commandInsert.Parameters.AddWithValue("@titre", newMovie.titre);
            commandInsert.Parameters.AddWithValue("@dateSortie", newMovie.dateSortie);

            newMovie.Id = Convert.ToInt32(commandInsert.ExecuteScalar());

            return newMovie;
        }

        private static Actor AddActor([FromBody] Actor newActor)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandInsert = new MySqlCommand("INSERT INTO actor (firstname_actor, lastname_actor, birthdate_actor) VALUES (@prenom, @nom, @age);SELECT LAST_INSERT_ID();", connection);
            commandInsert.Parameters.AddWithValue("@prenom", newActor.prenom);
            commandInsert.Parameters.AddWithValue("@nom", newActor.nom);
            commandInsert.Parameters.AddWithValue("@age", newActor.age);

            newActor.Id = Convert.ToInt32(commandInsert.ExecuteScalar());

            return newActor;

        }

        private static bool DeleteActor(int id)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            using var commandDelete = new MySqlCommand("DELETE FROM actor WHERE Id_actor = @Id;", connection);
            commandDelete.Parameters.AddWithValue("@Id", id);

            int lignesAffectees = commandDelete.ExecuteNonQuery();

            return lignesAffectees > 0;

        }

        private static Movie UpdateMovie(int id, Movie updatedMovie)
        {
            var connectionMySqlString = "Server=127.0.0.1;Port=3306;User ID=root;Password=;Database=streaming;";

            using MySqlConnection connection = new MySqlConnection(connectionMySqlString);
            connection.Open();

            // Vérifier si le film existe
            using var commandSelect = new MySqlCommand("SELECT * FROM movie WHERE Id_movie = @Id;", connection);
            commandSelect.Parameters.AddWithValue("@Id", id);

            using var reader = commandSelect.ExecuteReader();

            if (!reader.Read())
            {
                return null; // Film non trouvé
            }

            reader.Close();

            // Mettre à jour le film
            using var commandUpdate = new MySqlCommand("UPDATE movie SET title = @Title, release_year = @ReleaseYear, duration = @Duration WHERE Id_movie = @Id;", connection);
            commandUpdate.Parameters.AddWithValue("@Id", id);
            commandUpdate.Parameters.AddWithValue("@Title", updatedMovie.Title);
            commandUpdate.Parameters.AddWithValue("@ReleaseYear", updatedMovie.ReleaseYear);
            commandUpdate.Parameters.AddWithValue("@Duration", updatedMovie.Duration);

            int rowsAffected = commandUpdate.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                // Récupérer le film mis à jour
                using var commandSelectUpdated = new MySqlCommand("SELECT * FROM movie WHERE Id_movie = @Id;", connection);
                commandSelectUpdated.Parameters.AddWithValue("@Id", id);

                using var readerUpdated = commandSelectUpdated.ExecuteReader();

                if (readerUpdated.Read())
                {
                    return new Movie
                    {
                        Id = (int)readerUpdated["Id_movie"],
                        Title = readerUpdated["title"].ToString(),
                        ReleaseYear = (int)readerUpdated["release_year"],
                        CreateDate = (DateTime)readerUpdated["creation_date_movie"],
                        Duration = (int)readerUpdated["duration"]
                    };
                }
            }

            return null; // Échec de la mise à jour
        }





    }


}
