describe('Author Page Tests', () => {
    beforeEach(() => {
        cy.visit('https://localhost:44330/authors.html');
        cy.contains('Author List').should('be.visible');
        cy.get('#authorsTable').should('exist');
    });

    it('Rejects duplicate author on submit', () => {
        cy.get('#fullName').should('exist').and('be.visible').clear().type('Efe Kilic');
        cy.get('#nationality').should('exist').and('be.visible').clear().type('Turkish');
        cy.get('form').should('exist').submit();

        cy.on('window:alert', (msg) => {
            expect(msg).to.include('already exists');
        });
    });

    it('Updates an existing author', () => {
        cy.get('#authorsTable tbody tr').its('length').should('be.gte', 1);
        cy.get('#authorsTable tbody tr').eq(0).within(() => {
            cy.get('input[data-field="fullName"]').should('be.visible').clear().type('Updated Name');
            cy.get('input[data-field="nationality"]').should('be.visible').clear().type('Updated Country');
            cy.contains('Update').click();
        });

        cy.reload();
        cy.get('#authorsTable tbody tr').eq(0).within(() => {
            cy.get('input[data-field="fullName"]').should('have.value', 'Updated Name');
            cy.get('input[data-field="nationality"]').should('have.value', 'Updated Country');
        });
    });

    it('Deletes an author with no books', () => {
        cy.get('#fullName').should('exist').clear().type('Delete Me');
        cy.get('#nationality').should('exist').clear().type('Nowhere');
        cy.get('form').submit();

        cy.on('window:alert', () => {}); // ignore if already exists
        cy.wait(1000); // wait for insertion

        cy.get('#authorsTable tbody tr').last().within(() => {
            cy.contains('Delete').click();
        });

        cy.on('window:confirm', () => true);
        cy.wait(500);
        cy.reload();
        cy.get('#authorsTable').should('not.contain', 'Delete Me');
    });

    it('Fails if Full Name input is hidden or disabled', () => {
        cy.get('#fullName')
            .should('exist')
            .and('be.visible')
            .and('not.be.disabled');
    });

    it('Navigates to Books page via button', () => {
        cy.contains('Go to Books Page').should('exist').click();
        cy.url().should('include', '/books.html');
        cy.contains('Book List').should('be.visible');
    });
});
