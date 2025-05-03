using LibraryApplication.Api.Controllers;
using LibraryApplication.Api.Data;
using LibraryApplication.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryApplication.Tests.Controllers
{
    public class AuthorRecordsControllerTests
    {
        private LibraryDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            var context = new LibraryDbContext(options);

            // Seed authors
            context.Authors.AddRange(
                new AuthorEntity { Id = 1, FullName = "Author One", Nationality = "USA" },
                new AuthorEntity { Id = 2, FullName = "Author Two", Nationality = "UK" }
            );

            // Seed book for author 1
            context.Books.Add(new BookEntity
            {
                Id = 1,
                Title = "Book by Author One",
                Genre = "Drama",
                PublishedOn = new DateTime(2022, 1, 1),
                AuthorId = 1
            });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsAllAuthors()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<ActionResult<IEnumerable<AuthorEntity>>>(result);
            Assert.Equal(2, okResult.Value!.Count());
        }

        [Fact]
        public async Task GetById_ExistingId_ReturnsAuthor()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var result = await controller.Get(1);

            var okResult = Assert.IsType<ActionResult<AuthorEntity>>(result);
            Assert.Equal("Author One", okResult.Value!.FullName);
        }

        [Fact]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var result = await controller.Get(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Author with ID 999 does not exist.", notFound.Value);
        }

        [Fact]
        public async Task CreateAuthor_AddsSuccessfully()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var newAuthor = new AuthorEntity
            {
                FullName = "Author Three",
                Nationality = "Canada"
            };

            var result = await controller.Create(newAuthor);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var author = Assert.IsType<AuthorEntity>(created.Value);
            Assert.Equal("Author Three", author.FullName);
        }

        [Fact]
        public async Task UpdateAuthor_ValidId_UpdatesSuccessfully()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var updated = new AuthorEntity
            {
                Id = 1,
                FullName = "Updated Name",
                Nationality = "Germany"
            };

            var result = await controller.Update(1, updated);

            Assert.IsType<NoContentResult>(result);
            var author = await context.Authors.FindAsync(1);
            Assert.Equal("Updated Name", author!.FullName);
        }

        [Fact]
        public async Task UpdateAuthor_IdMismatch_ReturnsBadRequest()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var updated = new AuthorEntity { Id = 999, FullName = "Mismatch", Nationality = "X" };

            var result = await controller.Update(1, updated);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID in URL does not match ID in body.", badRequest.Value);
        }

        [Fact]
        public async Task DeleteAuthor_WithBooks_DeletesBooksToo()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Books.Where(b => b.AuthorId == 1));
            Assert.Null(await context.Authors.FindAsync(1));
        }

        [Fact]
        public async Task DeleteAuthor_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthorRecordsController(context);

            var result = await controller.Delete(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Author with ID 999 does not exist.", notFound.Value);
        }
    }
}
