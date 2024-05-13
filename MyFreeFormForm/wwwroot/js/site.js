// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// add event listener to the button deleteUser and call the function deleteUser in the backend

document.getElementById("deleteUser")?.addEventListener("click", function () {
    var userId = this.getAttribute("data-userid");
    if (confirm("Are you sure you want to delete this user? User ID: " + userId)) {
        deleteUser(userId);
    }


    function deleteUser(userId) {
        fetch('/user/DeleteUsers/' + userId, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => {
            if (response.ok) {
                alert("User deleted successfully.");
                window.location.href = "/user";
            } else {
                alert("Failed to delete user.");
            }
        })
        .catch((error) => {
            console.error('Error:', error);
            alert("Error deleting user.");
        });
    }

});