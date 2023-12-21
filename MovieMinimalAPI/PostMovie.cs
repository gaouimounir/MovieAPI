namespace MovieMinimalAPI;

public class PostMovie
{
    public int Id { get; set; }
    public string? titre { get; set; }
    public int Duration { get; set; }
    public int dateSortie { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModifDate { get; set; }
}

