namespace FrontApp.Models
{
    public class Article
    {
       
        public required string Title { get; set; }
        public string? Subtitle { get; set; }
        public required string Author { get;set; }

        public required DateTime Published { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Subtitle))
            {
                return $"{Title} by {Author} ({Published.ToShortDateString()})";
            }
            return $"{Title}: {Subtitle} by {Author} ({Published.ToShortDateString()})";
        }


    }
}
