namespace MovieMinimalAPI;

public class Movie
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime ReleaseYear { get; set; }
    public DateTime CreateDate { get; set; }

    public int Duration { get; set; }
}