document.addEventListener('DOMContentLoaded', function () {
    const form = document.querySelector('form');

    // Delegate event for dynamically added remove buttons
    form.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-field-btn')) {
            const fieldGroup = e.target.closest('.dynamic-field');
            if (fieldGroup) {
                fieldGroup.remove();
                updateFieldNames();
            }
        }
    });

    function addField() {
        const uniqueId = Date.now(); // Using the current timestamp as a unique identifier
        const index = document.querySelectorAll('.dynamic-field').length;

        // Create a container div to hold the dynamic field
        const container = document.createElement('div');
        container.className = "form-group dynamic-field";
        container.id = `field-group-${uniqueId}`;

        // Create and append the input for the field name
        const fieldNameInput = document.createElement('input');
        fieldNameInput.type = "text";
        fieldNameInput.name = `Fields[${index}].FieldName`;
        fieldNameInput.className = "form-control";
        fieldNameInput.placeholder = "Field Name";
        container.appendChild(fieldNameInput);

        // Create and append the select dropdown for field type
        const fieldTypeSelect = document.createElement('select');
        fieldTypeSelect.name = `Fields[${index}].FieldType`;
        fieldTypeSelect.className = "form-control field-type-select";
        ['text', 'email', 'number', 'date'].forEach(value => {
            const option = document.createElement('option');
            option.value = value;
            option.textContent = value.charAt(0).toUpperCase() + value.slice(1); // Capitalize first letter
            fieldTypeSelect.appendChild(option);
        });
        container.appendChild(fieldTypeSelect);

        // Create and append the input for the field value
        const fieldValueInput = document.createElement('input');
        fieldValueInput.type = "text"; // Default type
        fieldValueInput.name = `Fields[${index}].Value`;
        fieldValueInput.className = "form-control value-input";
        fieldValueInput.placeholder = "Value";
        container.appendChild(fieldValueInput);

        // Create and append the remove button
        const removeBtn = document.createElement('button');
        removeBtn.type = "button";
        removeBtn.className = "remove-field-btn";
        removeBtn.textContent = "Remove";
        container.appendChild(removeBtn);

        // Append the constructed div to the form
        document.querySelector('form').appendChild(container);

        // Event listener to change input field type
        fieldTypeSelect.addEventListener('change', function (e) {
            fieldValueInput.type = e.target.value; // Change the type of the input field based on the selected option
        });

        // Delegate the remove functionality to the newly added button
        removeBtn.addEventListener('click', function () {
            container.remove(); // Remove the dynamic field container
            updateFieldNames(); // Call this function to re-index the field names if necessary
        });
    }

    // Ensure updateFieldNames function exists and is updated to handle the new structure
    function updateFieldNames() {
        document.querySelectorAll('.dynamic-field').forEach((fieldGroup, index) => {
            const input = fieldGroup.querySelector('input[type="text"]');
            const select = fieldGroup.querySelector('select');

            if (input) input.name = `Fields[${index}].FieldName`;
            if (select) select.name = `Fields[${index}].FieldType`;
        });
    }


    document.getElementById('addFieldBtn').addEventListener('click', addField);

    const uploadForm = document.getElementById('uploadForm');
    if (uploadForm) {
        uploadForm.addEventListener('submit', function (e) {
            e.preventDefault(); // Prevent the default form submission behavior
            const formData = new FormData(this);
            fetch(this.action, {
                method: 'POST',
                body: formData,
                headers: { 'Accept': 'application/json', }
            })
            .then(response => response.json())
            .then(handleUploadResponse)
            .catch(error => console.error('Error:', error));
        });
    }

/*    function handleUploadResponse(data) {
        if (data.success && data.fields) {
            console.log('Upload successful', data);

            const clearFields = confirm('Do you want to clear existing fields before adding new ones?');
            if (clearFields) {
                resetForm();
            }

            // Iterate over fields data to create new form fields
            data.fields.forEach((field) => {
                // Here, adapt this part to match the structure of your data.fields
                // Assuming field has { name: "FieldName", type: "FieldType", value: "FieldValue" }
                addDynamicField(field);
            });
        } else {
            console.error('Upload failed', data.message);
        }
    }
    function addDynamicField(field) {
        const container = document.createElement('div');
        container.className = "dynamic-field";

        // Dynamically create and append elements based on the field object
        Object.entries(field).forEach(([key, value]) => {
            const label = document.createElement('label');
            label.textContent = key + ": ";
            container.appendChild(label);

            const input = document.createElement('input');
            input.type = "text"; // You can adjust this based on the actual data types
            input.value = value;
            input.className = "form-control";
            input.readOnly = true; // Make it read-only if you're just displaying data
            container.appendChild(input);

            // Add a line break for better formatting
            container.appendChild(document.createElement('br'));
        });

        const removeBtn = document.createElement('button');
        removeBtn.textContent = "Remove";
        removeBtn.className = "remove-field-btn btn btn-danger btn-sm";
        removeBtn.onclick = () => container.remove();
        container.appendChild(removeBtn);

        document.querySelector('form').appendChild(container);
    }*/

    function handleUploadResponse(data) {
        if (data.success && data.fields) {
            const clearFields = confirm('Do you want to clear existing fields before adding new ones?');
            if (clearFields) {
                resetForm();
            }

            // Iterate over rows of data to create a form section for each
            data.fields.forEach((row, rowIndex) => {
                addFormSection(row, rowIndex);
            });
        } else {
            console.error('Upload failed', data.message);
        }
    }

    function getInputType(fieldName, fieldValue) {
        //TODO: You can add more complex checks based on your data. Since the data is coming from a excel spreadsheet, there are more data types to consider
        // Simple checks to determine input type based on field name or value
        if (fieldName.toLowerCase().includes('date')) {
            return 'date';
        } else if (fieldName.toLowerCase().includes('email')) {
            return 'email';
        } else if (!isNaN(parseFloat(fieldValue)) && isFinite(fieldValue)) {
            // Check if the value is a number
            return 'number';
        }
        // Default to text
        return 'text';
    }

    function addFormSection(row, rowIndex) {
        const section = document.createElement('div');
        section.className = "form-section";
        section.id = `section-${rowIndex}`;

        Object.entries(row).forEach(([fieldName, fieldValue]) => {
            const fieldContainer = document.createElement('div');
            fieldContainer.className = "form-group";

            const label = document.createElement('label');
            label.textContent = fieldName;
            fieldContainer.appendChild(label);

            const input = document.createElement('input');
            input.className = "form-control";
            input.name = `Rows[${rowIndex}][${fieldName}]`;
            input.value = fieldValue;

            // Dynamically set the input type
            input.type = getInputType(fieldName, fieldValue);

            // Special handling for dates to convert into 'YYYY-MM-DD' format if not already
            if (input.type === 'date' && !/^\d{4}-\d{2}-\d{2}$/.test(fieldValue)) {
                input.value = convertToDateInputValue(fieldValue);
            }

            fieldContainer.appendChild(input);
            section.appendChild(fieldContainer);
        });

        document.querySelector('form').appendChild(section);
    }

    function convertToDateInputValue(value) {
        // Example conversion from 'MM/DD/YYYY' to 'YYYY-MM-DD'
        //TODO: You'll need to adjust this based on the actual date format in your data
        const dateParts = value.split('/');
        if (dateParts.length === 3) {
            return `${dateParts[2]}-${dateParts[0].padStart(2, '0')}-${dateParts[1].padStart(2, '0')}`;
        }
        // Return original value if conversion isn't possible
        return value;
    }



    function resetForm() {
        // Remove existing form sections
        document.querySelectorAll('.form-section').forEach(section => section.remove());
    }


    function resetForm() {
        document.querySelectorAll('.dynamic-field').forEach(fieldGroup => fieldGroup.remove());
    }


    function resetForm() {
        document.querySelectorAll('.dynamic-field').forEach(fieldGroup => fieldGroup.remove());
    }

    function updateFieldNames() {
        document.querySelectorAll('.dynamic-field').forEach((fieldGroup, index) => {
            const input = fieldGroup.querySelector('input[type="text"]');
            const select = fieldGroup.querySelector('select');

            if (input) input.name = `Fields[${index}].FieldName`;
            if (select) select.name = `Fields[${index}].FieldType`;
        });
    }
});
