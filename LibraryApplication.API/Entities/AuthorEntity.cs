using System.ComponentModel.DataAnnotations;

namespace LibraryApplication.Api.Entities
{
    public class AuthorEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        [MinLength(1, ErrorMessage = "FullName must not be empty.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Nationality is required.")]
        [MinLength(1, ErrorMessage = "Nationality must not be empty.")]
        public string Nationality { get; set; } = null!;

        public ICollection<BookEntity> Books { get; set; } = new List<BookEntity>();
    }
}