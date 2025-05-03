document.addEventListener("DOMContentLoaded", () => {
    fetchBooks();
    loadAuthorsDropdown();

    document.getElementById("bookForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const title = document.getElementById("title").value.trim();
        const genre = document.getElementById("genre").value.trim();
        const publishedOn = document.getElementById("publishedOn").value;
        const authorId = parseInt(document.getElementById("authorId").value);

        fetch("https://localhost:44330/api/books", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ title, genre, publishedOn, authorId })
        })
            .then(async res => {
                if (!res.ok) {
                    const contentType = res.headers.get("content-type") || "";
                    if (contentType.includes("application/json")) {
                        const errorData = await res.json();
                        if (errorData?.errors?.Author || errorData?.error?.includes("does not exist")) {
                            throw new Error("There is no author with the typed ID.");
                        }
                        throw new Error("Failed to add book.");
                    } else {
                        const text = await res.text();
                        if (text.includes("Author with ID") && text.includes("does not exist")) {
                            throw new Error("There is no author with the typed ID.");
                        }
                        throw new Error("Failed to add book.");
                    }
                }
                return res.json();
            })
            .then(() => location.reload())
            .catch(err => alert("Error: " + err.message));
    });
});

function fetchBooks() {
    fetch("https://localhost:44330/api/books")
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch books.");
            return response.json();
        })
        .then(books => {
            const tbody = document.querySelector("#booksTable tbody");
            tbody.innerHTML = "";

            books.forEach(book => {
                const publishedDate = book.publishedOn
                    ? new Date(book.publishedOn).toISOString().split("T")[0]
                    : "";

                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${book.id}</td>
                    <td><input type="text" value="${book.title}" data-id="${book.id}" data-field="title" /></td>
                    <td><input type="text" value="${book.genre}" data-id="${book.id}" data-field="genre" /></td>
                    <td><input type="date" value="${publishedDate}" data-id="${book.id}" data-field="publishedOn" /></td>
                    <td>${book.author?.fullName || "Unknown"}</td>
                    <td>
                        <button onclick="updateBook(${book.id})">Update</button>
                        <button onclick="deleteBook(${book.id})">Delete</button>
                    </td>
                `;
                tbody.appendChild(row);
            });
        })
        .catch(error => {
            console.error("Error loading books:", error);
        });
}

function loadAuthorsDropdown() {
    fetch("https://localhost:44330/api/authors")
        .then(res => res.json())
        .then(data => {
            const authors = data.$values || data;
            const select = document.getElementById("authorId");

            authors.forEach(author => {
                const option = document.createElement("option");
                option.value = author.id;
                option.textContent = author.fullName;
                select.appendChild(option);
            });
        })
        .catch(err => console.error("Error loading authors for dropdown:", err));
}

function updateBook(id) {
    const titleInput = document.querySelector(`input[data-id="${id}"][data-field="title"]`);
    const genreInput = document.querySelector(`input[data-id="${id}"][data-field="genre"]`);
    const publishedOnInput = document.querySelector(`input[data-id="${id}"][data-field="publishedOn"]`);
    const authorNameCell = titleInput.closest('tr').querySelector('td:nth-child(5)');
    const title = titleInput.value.trim();
    const genre = genreInput.value.trim();
    const publishedOn = publishedOnInput.value;
    const authorName = authorNameCell.textContent.trim();

    if (!title || !genre || !publishedOn || !authorName) {
        alert("All fields must be filled before updating.");
        return;
    }

    fetch("https://localhost:44330/api/authors")
        .then(res => res.json())
        .then(authors => {
            const match = authors.find(a => a.fullName === authorName);
            if (!match) {
                alert("Author not found.");
                return;
            }

            const authorId = match.id;

            return fetch(`https://localhost:44330/api/books/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ id, title, genre, publishedOn, authorId })
            });
        })
        .then(async res => {
            if (!res) return; // safeguard
            if (!res.ok) {
                const contentType = res.headers.get("content-type") || "";
                if (contentType.includes("application/json")) {
                    const errorData = await res.json();
                    if (errorData?.errors?.Author || errorData?.error?.includes("does not exist")) {
                        throw new Error("There is no author with the typed ID.");
                    }
                    throw new Error("Update failed.");
                } else {
                    const text = await res.text();
                    if (text.includes("Author with ID") && text.includes("does not exist")) {
                        throw new Error("There is no author with the typed ID.");
                    }
                    throw new Error("Update failed.");
                }
            }

            // ✅ Set flag for Cypress before reload
            window.lastUpdatedBookId = id;

            // Optional: small delay for Cypress to detect the DOM update
            setTimeout(() => location.reload(), 300);
        })
        .catch(err => alert("Error: " + err.message));
}



function deleteBook(id) {
    if (!confirm("Are you sure you want to delete this book?")) return;

    fetch(`https://localhost:44330/api/books/${id}`, {
        method: "DELETE"
    })
        .then(res => {
            if (!res.ok) throw new Error("Delete failed");
            location.reload();
        })
        .catch(err => alert("Error: " + err.message));
}
