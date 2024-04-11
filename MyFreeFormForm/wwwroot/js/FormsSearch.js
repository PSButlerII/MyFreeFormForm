document.addEventListener('DOMContentLoaded', function () {
    let userId = document.getElementById('loggedInUser').value;
    let fieldNameDropdown = document.getElementById('fieldNameDropdown');
    let dateFieldDropdown = document.getElementById('dateGroup');
    let searchButton = document.getElementById('searchBtn');
    let resetButton = document.getElementById('resetBtn');
    //TODO: Add signalR hub for data transfer

    console.log('FormsSearch.js loaded');

    function GetFieldNames() {
        fetch(`/search/GetFieldNames?userId=${userId}`) // Include userId in the request
            .then(response => response.json())
            .then(data => {
                const dropdown = document.getElementById('fieldNameDropdown');
                data.forEach(name => {
                    const option = new Option(name, name); // new Option(text, value)
                    dropdown?.add(option);
                });
            })
            .catch(error => console.error('Error fetching field names:', error));
    }
    GetFieldNames();

    function GetDateFields() {
        fetch(`/search/GetDateFields?userId=${userId}`) // Include userId in the request
            .then(response => response.json())
            .then(data => {
                const dropdown = document.getElementById('dateGroup');
                data.forEach(name => {
                    const option = new Option(name, name); // new Option(text, value)
                    dropdown?.add(option);
                });
            })
            .catch(error => console.error('Error fetching Date fields :', error));
    }
    GetDateFields();

    searchButton?.addEventListener('click', function (e) {
        e.preventDefault();
        // Gather all the input values
        let searchTerm = document.getElementById('searchTerm').value;
        let dateField = dateFieldDropdown.options[dateFieldDropdown.selectedIndex].value;
        let startDate = document.querySelector('input[name="startDate"]').value;
        let endDate = document.querySelector('input[name="endDate"]').value;
        let fieldName = fieldNameDropdown.options[fieldNameDropdown.selectedIndex].value;
        let minValue = document.querySelector('input[name="minValue"]').value;
        let maxValue = document.querySelector('input[name="maxValue"]').value;
        //TODO: we will need to determine if the serachTerm will be used for the formName or the formNotes
        let queryParams = new URLSearchParams({ userId, searchTerm });

        // Construct the URL with query parameters. Only add parameters that have values.
        if(dateField) queryParams.append("dateField", dateField);
        if (startDate) queryParams.append("startDate", startDate);
        if (endDate) queryParams.append("endDate", endDate);
        if (fieldName) queryParams.append("fieldName", fieldName);
        if (minValue) queryParams.append("minValue", minValue);
        if (maxValue) queryParams.append("maxValue", maxValue);  

        let searchUrl = `/search/SearchFormsAsync?${queryParams.toString()}`;

        fetch(searchUrl)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                // Check if the response is JSON
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json(); // It's JSON, parse it and proceed
                } else {
                    throw new Error('Received non-JSON response from server');
                }
            })
            .then(data => handleSearchResponse(data)) // Now 'data' is guaranteed to be JSON
            .catch(error => {
                console.error('Fetch error:', error.message);
                // Optionally, handle specific error responses here
            });


    });

    function handleSearchResponse(data) {
        let searchResults = document.getElementById('searchResults');
        searchResults.innerHTML = ''; // Clear existing results

        if (!data.success || data.data.length === 0) {
            searchResults.innerHTML = '<p>No results found.</p>';
            return;
        }

        data.data.forEach(result => {
            const card = document.createElement('div');
            card.className = 'form-card';

            // Create a header or title for the card
            const title = document.createElement('h3');
            title.textContent = result.formName;
            card.appendChild(title);

            // List for form details
            const detailsList = document.createElement('ul');

            // Function to add details to the list
            const addDetailToList = (label, value) => {
                const listItem = document.createElement('li');
                listItem.textContent = `${label}: ${value || 'N/A'}`; // Use 'N/A' for empty values
                detailsList.appendChild(listItem);
            };

            // Adding main form details
            addDetailToList('Description', result.description);
            addDetailToList('Created Date', result.createdDate);
            addDetailToList('Updated Date', result.updatedDate);
            addDetailToList('Form Notes', result.formNotes);

            // Iterate over formFields to add to the list
            result.formFields.forEach(field => {
                addDetailToList(field.fieldName, field.fieldValue);
            });

            card.appendChild(detailsList);
            searchResults.appendChild(card);
        });
    }

    // Reset form fields and UI elements
    function resetForm() {
        // Clear input fields
        document.getElementById('searchTerm').value = '';
        document.querySelector('input[name="startDate"]').value = '';
        document.querySelector('input[name="endDate"]').value = '';
        document.querySelector('input[name="minValue"]').value = '';
        document.querySelector('input[name="maxValue"]').value = '';

        // Reset dropdowns to their initial state
        fieldNameDropdown.selectedIndex = 0;
        dateFieldDropdown.selectedIndex = 0;

        // Clear search results
        let searchResults = document.getElementById('searchResults');
        searchResults.innerHTML = '';

        // Optionally, re-fetch initial data if your dropdowns are dynamically populated
/*        GetFieldNames();
        GetDateFields();*/
    }

    // Event listener for the reset button
    resetButton?.addEventListener('click', function () {
        resetForm();
    });

});