using LibraryApplication.Api.Data;
using LibraryApplication.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Api.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BookRecordsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BookRecordsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookWithAuthorDto>>> GetAll()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Select(b => new BookWithAuthorDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Genre = b.Genre,
                    PublishedOn = b.PublishedOn,
                    AuthorId = b.AuthorId,
                    Author = new AuthorDto
                    {
                        Id = b.Author.Id,
                        FullName = b.Author.FullName,
                        Nationality = b.Author.Nationality
                    }
                })
                .ToListAsync();

            return Ok(books);
        }



        // GET: api/books/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookWithAuthorDto>> Get(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Id == id)
                .Select(b => new BookWithAuthorDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Genre = b.Genre,
                    PublishedOn = b.PublishedOn,
                    AuthorId = b.AuthorId,
                    Author = new AuthorDto
                    {
                        Id = b.Author.Id,
                        FullName = b.Author.FullName,
                        Nationality = b.Author.Nationality
                    }
                })
                .FirstOrDefaultAsync();

            if (book == null)
                return NotFound($"Book with ID {id} does not exist.");

            return Ok(book);
        }


        [HttpPost]
        public async Task<ActionResult<BookWithAuthorDto>> Create(BookEntity book)
        {
            // Check if the author exists
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == book.AuthorId);
            if (author == null)
            {
                return BadRequest($"Author with ID {book.AuthorId} does not exist.");
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Build DTO to avoid including books array with null
            var result = new BookWithAuthorDto
            {
                Id = book.Id,
                Title = book.Title,
                Genre = book.Genre,
                PublishedOn = book.PublishedOn,
                AuthorId = book.AuthorId,
                Author = new AuthorDto
                {
                    Id = author.Id,
                    FullName = author.FullName,
                    Nationality = author.Nationality
                }
            };

            return CreatedAtAction(nameof(Get), new { id = book.Id }, result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BookEntity updatedBook)
        {
            if (id != updatedBook.Id)
                return BadRequest("ID in URL does not match ID in body.");

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return NotFound($"Book with ID {id} does not exist.");

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == updatedBook.AuthorId);
            if (!authorExists)
                return BadRequest($"Author with ID {updatedBook.AuthorId} does not exist.");

            existingBook.Title = updatedBook.Title;
            existingBook.Genre = updatedBook.Genre;
            existingBook.PublishedOn = updatedBook.PublishedOn;
            existingBook.AuthorId = updatedBook.AuthorId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound($"Book with ID {id} does not exist.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    public class BookWithAuthorDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime? PublishedOn { get; set; }
        public int AuthorId { get; set; }
        public AuthorDto Author { get; set; }
    }

    public class AuthorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
    }

}
