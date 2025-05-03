document.addEventListener("DOMContentLoaded", () => {
    fetchAuthors();

    document.getElementById("authorForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const fullName = document.getElementById("fullName").value.trim();
        const nationality = document.getElementById("nationality").value.trim();

        fetch("https://localhost:44330/api/authors", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ fullName, nationality })
        })
            .then(res => {
                if (!res.ok) {
                    return res.text().then(msg => { throw new Error(msg); });
                }
                return res.json();
            })
            .then(() => location.reload())
            .catch(err => alert("Error: " + err.message));
    });
});

// ✅ Fetch and render authors
function fetchAuthors() {
    fetch("https://localhost:44330/api/authors")
        .then(response => {
            if (!response.ok) throw new Error("Failed to fetch authors.");
            return response.json();
        })
        .then(authors => {
            const tbody = document.querySelector("#authorsTable tbody");
            tbody.innerHTML = "";

            authors.forEach(author => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${author.id}</td>
                    <td><input type="text" value="${author.fullName}" data-id="${author.id}" data-field="fullName" /></td>
                    <td><input type="text" value="${author.nationality}" data-id="${author.id}" data-field="nationality" /></td>
                    <td>
                        <button onclick="updateAuthor(${author.id})">Update</button>
                        <button onclick="deleteAuthor(${author.id})">Delete</button>
                    </td>
                `;
                tbody.appendChild(row);
            });
        })
        .catch(error => {
            console.error("Error loading authors:", error);
        });
}

// ✅ Update author with specific error messages
function updateAuthor(id) {
    const fullName = document.querySelector(`input[data-id="${id}"][data-field="fullName"]`).value.trim();
    const nationality = document.querySelector(`input[data-id="${id}"][data-field="nationality"]`).value.trim();

    fetch(`https://localhost:44330/api/authors/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id, fullName, nationality })
    })
        .then(res => {
            if (!res.ok) {
                return res.text().then(msg => { throw new Error(msg); });
            }
            location.reload();
        })
        .catch(err => alert("Error: " + err.message));
}

// ✅ Delete author after checking books
function deleteAuthor(id) {
    fetch(`https://localhost:44330/api/books`)
        .then(res => res.json())
        .then(books => {
            const authorBooks = books.filter(book => book.authorId === id);
            const count = authorBooks.length;

            const message = count > 0
                ? `⚠️ This author has ${count} book(s). Deleting them will also remove those books.\nAre you sure?`
                : "Are you sure you want to delete this author?";

            if (!confirm(message)) return;

            fetch(`https://localhost:44330/api/authors/${id}`, {
                method: "DELETE"
            })
                .then(res => {
                    if (!res.ok) throw new Error("Delete failed");
                    location.reload();
                })
                .catch(err => alert("Error: " + err.message));
        });
}
