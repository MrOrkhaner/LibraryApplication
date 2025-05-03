# LibraryApplication 

This is a full-stack web application for managing books and authors, developed as part of a university assignment. The project includes API development, database integration, UI implementation, and complete testing (unit, API, UI, and performance).

## 🛠 Technologies Used

- **Backend:** ASP.NET Core (.NET 8)
- **Database:** MySQL (MySQL Workbench)
- **Frontend:** HTML, CSS, JS
- **API Testing:** Postman
- **UI Testing:** Cypress
- **Performance Testing:** Apache JMeter

---

## How to Run the Project

### Prerequisites

- Visual Studio or JetBrains Rider
- .NET SDK 8.0+
- MySQL Server & phpMyAdmin
- Node.js and npm (for Cypress)
- JMeter (for performance testing)

### 1. Clone the Repository

```bash
git clone https://github.com/MrOrkhaner/LibraryApplication
```

### 2. Setup the Database

1. Create a MySQL database named `libraryAPIdb`.
2. Import the provided schema `ApplicationDBSQL.sql` (LibraryApplication/SubmissionFiles/ApplicationDBSQL.sql) into the database using MySQL Workbench or the command line.
3. Default database credentials:
   - **User:** root
   - **Password:** 1234


### 3. Configure `appsettings.json`
- In the `LibraryAPI` project folder, locate the `appsettings.json` file.
- Update the connection string to match your local MySQL setup:

```json
"ConnectionStrings": {
"DefaultConnection": "server=localhost;Database=libraryAPIdb;user=root;password=YOUR_PASSWORD"
}
```
Replace `YOUR_PASSWORD` with your MySQL root password.

---

### 4. Run the Application

```bash
cd LibraryApplication.API
dotnet run
```

Access the app at `https://localhost:44330`

---

## Project Structure

```plaintext
LibraryApplication/
│
├── LibraryApplication.API/         # .NET Core backend
│   ├── Controllers/
│   ├── Models/
│   ├── Repositories/
│   └── ...  
│
├── wwwroot/                        # Frontend files
│   ├── books.html
│   ├── authors.html
│   └── css/, js/ etc.
│
├── cypress/                        # Cypress UI tests
│   └── e2e/
│       ├── book-page.cy.js
│       └── author-page.cy.js
│
├── SubmissionFiles/               
│   ├── PostmanCollectionAndTestResults/
│   ├── CypressUITests/
│   └── PerformanceTestResults/
│
└── README.md
```

---

## ✅ Testing Summary

### Unit Tests
- Implemented using `xUnit` and `EF Core InMemory`.
- Located in `LibraryApplication.Tests` project.
- The results are in 'SubmissionFiles/UnitTestResults'

### API Tests
- Postman tests located in:
  `SubmissionFiles/PostmanCollectionAndTestResults/`

### UI Tests
- Cypress scripts and result reports available in:
  `SubmissionFiles/CypressUITests/`

### Performance Tests
- JMeter tests simulate 10, 50, and 100 concurrent users.
- CSV reports saved in:
  `SubmissionFiles/PerformanceTestResults/`

---

## Author

- Orkhan - [MrOrkhaner](https://github.com/MrOrkhaner)