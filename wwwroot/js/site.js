// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Display confirmation before deleting a book or user
function confirmDeletion(event) {
    if (!confirm("Are you sure you want to delete this?")) {
        event.preventDefault();
    }
}
 
// Attach event listeners when the page loads
document.addEventListener("DOMContentLoaded", function() {
    let deleteButtons = document.querySelectorAll(".delete-btn");
    deleteButtons.forEach(button => {
        button.addEventListener("click", confirmDeletion);
    });
 
    // Auto-hide success messages after 3 seconds
    let successMessages = document.querySelectorAll(".success-message");
    successMessages.forEach(msg => {
        setTimeout(() => {
msg.style.display = "none";
        }, 3000);
    });
});