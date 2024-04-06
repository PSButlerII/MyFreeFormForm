document.addEventListener('DOMContentLoaded', function () {

    console.log('FormSearch.js loaded');

    let searchButton = document.getElementById('searchBtn');

    searchButton?.addEventListener('click', function (e) {
        e.preventDefault();
        let searchTerm = document.getElementById('searchTerm').value;

        fetch(`/api/SearchFormsAsync?searchTerm=${encodeURIComponent(searchTerm)}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            .then(response => {
                if (!response.ok) {
                    console.log(response);
                    logMessage("INFO",response);
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log(data);
                let searchResults = document.getElementById('searchResults');
                searchResults.innerHTML = '';
                data.forEach(result => {
                    let resultElement = document.createElement('div');
                    resultElement.innerHTML = `<h3>${result.title}</h3><p>${result.content}</p>`; // Ensure these properties exist in your result objects
                    searchResults.appendChild(resultElement);
                });
            })
            .catch(error => {
                console.error('There has been a problem with your fetch operation:', error);
            });
    });

});