namespace LibraryApplication.Api.Entities
{
    public class BookEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime PublishedOn { get; set; }

        public int AuthorId { get; set; }
        public AuthorEntity? Author { get; set; }  

    }
}