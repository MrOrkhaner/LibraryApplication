using LibraryApplication.Api.Controllers;
using LibraryApplication.Api.Data;
using LibraryApplication.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryApplication.Tests.Controllers
{
    public class BookRecordsControllerTests
    {
        private LibraryDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            var context = new LibraryDbContext(options);

            // Seed authors
            context.Authors.AddRange(
                new AuthorEntity { Id = 1, FullName = "Author One", Nationality = "US" },
                new AuthorEntity { Id = 2, FullName = "Author Two", Nationality = "UK" }
            );

            // Seed books
            context.Books.Add(
                new BookEntity
                {
                    Id = 1,
                    Title = "Book One",
                    Genre = "Drama",
                    PublishedOn = new DateTime(2020, 1, 1),
                    AuthorId = 1
                });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsAllBooks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            // Act
            var result = await controller.GetAll();

            // Assert
            var books = Assert.IsType<ActionResult<IEnumerable<BookEntity>>>(result);
            Assert.Single(books.Value!);
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsBook()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var result = await controller.Get(1);

            var okResult = Assert.IsType<ActionResult<BookEntity>>(result);
            Assert.Equal(1, okResult.Value!.Id);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var result = await controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Book with ID 999 does not exist.", notFound.Value);
        }

        [Fact]
        public async Task CreateBook_WithValidAuthor_AddsBook()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var newBook = new BookEntity
            {
                Title = "New Book",
                Genre = "Sci-Fi",
                PublishedOn = DateTime.Today,
                AuthorId = 1
            };

            var result = await controller.Create(newBook);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdBook = Assert.IsType<BookEntity>(createdResult.Value);
            Assert.Equal("New Book", createdBook.Title);
        }

        [Fact]
        public async Task CreateBook_WithInvalidAuthor_ReturnsBadRequest()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var book = new BookEntity
            {
                Title = "Invalid Book",
                Genre = "Mystery",
                PublishedOn = DateTime.Today,
                AuthorId = 999 // invalid
            };

            var result = await controller.Create(book);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Author with ID 999 does not exist.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteBook_ExistingId_DeletesSuccessfully()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Books.ToList());
        }

        [Fact]
        public async Task DeleteBook_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = new BookRecordsController(context);

            var result = await controller.Delete(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Book with ID 999 does not exist.", notFound.Value);
        }
    }
}