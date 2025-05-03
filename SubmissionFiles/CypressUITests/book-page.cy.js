describe('Book Page Tests', () => {
    beforeEach(() => {
        cy.visit('https://localhost:44330/books.html');
        cy.contains('Book List').should('be.visible');
        cy.get('#booksTable').should('exist');
    });

    it('Rejects book with empty title', () => {
        cy.get('#title').should('exist').and('be.visible').clear();
        cy.get('#genre').clear().type('Test Genre');
        cy.get('#publishedOn').clear().type('2025-05-03');
        cy.get('#authorId').select(1);

        cy.get('form').submit();
        cy.on('window:alert', (msg) => {
            expect(msg).to.include('Title is required');
        });
    });

    it('Updates an existing book', () => {
        // Save the first row's ID
        cy.get('#booksTable tbody tr').first().find('td').first().invoke('text').then(bookId => {
            const trimmedId = bookId.trim();
            cy.wrap(trimmedId).as('bookId');
        });

        // Update the first book row
        cy.get('#booksTable tbody tr').first().within(() => {
            cy.get('input[data-field="title"]').clear().type('Updated Book');
            cy.get('input[data-field="genre"]').clear().type('Updated Genre');
            cy.get('input[data-field="publishedOn"]').clear().type('2025-05-04');
            cy.contains('Update').click();
        });

        // Wait and reload the page
        cy.wait(1500);
        cy.visit('https://localhost:44330/books.html');

        // Find the row again by ID and verify updated values
        cy.get('@bookId').then(bookId => {
            cy.get('#booksTable tbody tr').each(($row) => {
                const idCell = $row.children[0];
                if (idCell && idCell.textContent.trim() === bookId) {
                    cy.wrap($row).within(() => {
                        cy.get('input[data-field="title"]').should('have.value', 'Updated Book');
                        cy.get('input[data-field="genre"]').should('have.value', 'Updated Genre');
                    });
                }
            });
        });
    });

    it('Deletes a book', () => {
        cy.get('#title').clear().type('Book To Delete');
        cy.get('#genre').clear().type('Drama');
        cy.get('#publishedOn').clear().type('2025-05-01');
        cy.get('#authorId').select(1);
        cy.get('form').submit();

        cy.on('window:alert', () => {});
        cy.wait(1000);

        cy.get('#booksTable tbody tr').last().within(() => {
            cy.contains('Delete').click();
        });

        cy.on('window:confirm', () => true);
        cy.wait(500);
        cy.reload();
        cy.get('#booksTable').should('not.contain', 'Book To Delete');
    });

    it('Fails if Title input is hidden or disabled', () => {
        cy.get('#title')
            .should('exist')
            .and('be.visible')
            .and('not.be.disabled');
    });

    it('Navigates to Authors page via button', () => {
        cy.contains('Go to Authors Page').should('exist').click();
        cy.url().should('include', '/authors.html');
        cy.contains('Author List').should('be.visible');
    });
});
