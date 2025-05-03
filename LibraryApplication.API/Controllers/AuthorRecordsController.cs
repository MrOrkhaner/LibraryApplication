using LibraryApplication.Api.Data;
using LibraryApplication.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Api.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorRecordsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthorRecordsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorWithBooksDto>>> GetAll()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .Select(a => new AuthorWithBooksDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Nationality = a.Nationality,
                    Books = a.Books.Select(b => new BookDtoWithoutAuthor
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Genre = b.Genre,
                        PublishedOn = b.PublishedOn,
                        AuthorId = b.AuthorId
                    }).ToList()
                })
                .ToListAsync();

            return authors;
        }


        // GET: api/authors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorWithBooksDto>> Get(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .Select(a => new AuthorWithBooksDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Nationality = a.Nationality,
                    Books = a.Books.Select(b => new BookDtoWithoutAuthor
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Genre = b.Genre,
                        PublishedOn = b.PublishedOn,
                        AuthorId = b.AuthorId
                    }).ToList()
                })
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return NotFound($"Author with ID {id} does not exist.");

            return author;
        }



        // POST: api/authors
        [HttpPost]
        public async Task<ActionResult<object>> Create(AuthorEntity author)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(author.FullName) || string.IsNullOrWhiteSpace(author.Nationality))
                return BadRequest("FullName and Nationality are required.");

            // Check for existing author with the same FullName and Nationality
            bool exists = await _context.Authors.AnyAsync(a =>
                a.FullName == author.FullName && a.Nationality == author.Nationality);

            if (exists)
                return Conflict("An author with the same FullName and Nationality already exists.");

            // Save if valid and unique
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var result = new
            {
                Id = author.Id,
                FullName = author.FullName,
                Nationality = author.Nationality
            };

            return CreatedAtAction(nameof(Get), new { id = author.Id }, result);
        }




        // PUT: api/authors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AuthorEntity updatedAuthor)
        {
            if (id != updatedAuthor.Id)
                return BadRequest("ID in URL does not match ID in body.");

            var existingAuthor = await _context.Authors.FindAsync(id);
            if (existingAuthor == null)
                return NotFound($"Author with ID {id} does not exist.");

            // Check for duplicates (excluding this author)
            bool duplicateExists = await _context.Authors.AnyAsync(a =>
                a.Id != id &&
                a.FullName.ToLower() == updatedAuthor.FullName.ToLower() &&
                a.Nationality.ToLower() == updatedAuthor.Nationality.ToLower());

            if (duplicateExists)
                return BadRequest("An author with the same name and nationality already exists.");

            existingAuthor.FullName = updatedAuthor.FullName;
            existingAuthor.Nationality = updatedAuthor.Nationality;

            await _context.SaveChangesAsync();
            return NoContent();
        }



        // DELETE: api/authors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound($"Author with ID {id} does not exist.");

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
    public class AuthorWithBooksDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public List<BookDtoWithoutAuthor> Books { get; set; }
    }

    public class BookDtoWithoutAuthor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime PublishedOn { get; set; }
        public int AuthorId { get; set; }
    }

}
