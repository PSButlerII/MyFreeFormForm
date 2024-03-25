document.addEventListener('DOMContentLoaded', function () {
    console.time('DynamicFormJS');
    const form = document.querySelector('form');
    // const forms = document.querySelectorAll('.carousel-item form, #staticForm');
    const carousel = document.getElementById('dataCarousel');
    var carouselInstance = bootstrap.Carousel.getOrCreateInstance(carousel); // Bootstrap 5 way to reinitialize or get instance
    carouselInstance.cycle();
    const uploadForm = document.getElementById('uploadForm');
    const carouselInner = document.getElementById('carouselInner');
    let currentIndex = 0; // Track the current index of the carousel
    let totalItems = 0;
    let noteIndex = 1; // Start with 1 because 0 is already in the HTML

    function showSpinnerModal() {
        var spinnerModal = new bootstrap.Modal(document.getElementById('spinnerModal'), {
            backdrop: 'static', // Optional: Include a backdrop that doesn't close the modal when clicked.
            keyboard: false // Optional: Prevent closing the modal with the keyboard.
        });
        console.log('Showing Spinner')
        spinnerModal.show();
    }

    function hideSpinnerModal() {
        var spinnerModalEl = document.getElementById('spinnerModal');
        var spinnerModal = bootstrap.Modal.getInstance(spinnerModalEl);
        if (spinnerModal) {
            setTimeout(spinnerModal.hide(), 3, console.log('Hiding Spinner'));

        }
    }      

   /* document.getElementById('addNoteBtn').addEventListener('click', function () {
        let noteSection = document.getElementById('notesSection');

        // Check if noteSection exists, create it if not
        if (!noteSection) {
            noteSection = document.createElement('div');
            noteSection.id = 'notesSection';
            // Append the newly created section where it needs to go in your form
            const dynamicForm = document.getElementById('dynamicForm'); // Assuming this is your form's ID
            dynamicForm.appendChild(noteSection);
        }

        const newNoteInput = `<div class="input-group mb-3" id="noteInput${noteIndex}">
        <input type="text" class="form-control" name="FormNotes[${noteIndex}]" placeholder="Add a note" aria-label="Add a note">
        <button class="btn btn-outline-secondary removeNoteBtn" type="button">Remove</button>
        </div>`;
        noteSection.insertAdjacentHTML('beforeend', newNoteInput);
        noteIndex++;
    });


    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('removeNoteBtn')) {
            const noteGroup = e.target.closest('.input-group');
            if (noteGroup) {
                noteGroup.remove(); // Remove this note field
            }
        }
    });*/


    $('#dataCarousel').carousel('pause');

    // Listen for the slid event
    carousel.addEventListener('slid.bs.carousel', function (event) {
        currentIndex = event.to;
        console.log('Current index: ' + (currentIndex + 1) + ' of ' + totalItems);
        // Update the index display
        updateIndexDisplay();
    });

    /// Update the index display
    const updateIndexDisplay = () => {
        const totalItems = carousel.querySelectorAll('.carousel-item').length;
        document.getElementById('currentSlide').textContent = currentIndex + 1; // Adjust for human-friendly indexing
        document.getElementById('totalSlides').textContent = totalItems;
    };

    // Add A field to the Form
    // document.getElementById('addFieldBtn').addEventListener('click', addField);
    document.getElementById('addFieldBtn').addEventListener('click', function () {
        addFieldBasedOnVisibility();
    });

    function addFieldBasedOnVisibility() {
        // Check if the carousel is currently visible
        const carousel = document.getElementById('dataCarousel');
        const isCarouselVisible = carousel.style.display !== 'none';

        if (isCarouselVisible) {
            // If the carousel is visible, assume context is 'carouselForm'
            addField('carouselForm');
        } else {
            // If the carousel is not visible, assume context is 'staticForm'
            addField('staticForm');
        }
    }

    document.querySelector('#customNextBtn').addEventListener('click', handleCarouselControlClick);

    document.querySelector('#customPrevBtn').addEventListener('click', handleCarouselControlClick);

    // Initialize or update carousel after adding items dynamically
    bootstrap.Carousel.getOrCreateInstance(carousel);
    updateIndexDisplay();

    // Delegate event for dynamically added remove buttons
    carouselInner.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('remove-field-btn')) {
            console.log('Remove button clicked');
            const fieldGroup = e.target.closest('.dynamic-field');
            if (fieldGroup) {
                fieldGroup.remove();
                updateFieldNames();
            }
        }
    });

    // Delegate event for dynamically added remove buttons
    form.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-field-btn')) {
            const fieldGroup = e.target.closest('.dynamic-field');
            console.log('Remove button clicked for form')
            if (fieldGroup) {
                fieldGroup.remove();
                updateFieldNames();
            }
        }
    });

    // If you need to programmatically go to a specific slide
    function goToSlide(index) {
        const carouselInstance = bootstrap.Carousel.getOrCreateInstance(carousel);
        carouselInstance.to(index); // Go to a specific index
    }

    function handleCarouselControlClick(event) {
        const totalItems = carousel.querySelectorAll('.carousel-item').length;
        let newIndex = currentIndex;

        if (event.target.matches('#customNextBtn') || event.target.closest('.carousel-control-next')) {
            newIndex = (currentIndex + 1) % totalItems; // Wrap to 0 if it's the last item
        } else if (event.target.matches('#customPrevBtn') || event.target.closest('.carousel-control-prev')) {
            newIndex = (currentIndex - 1 + totalItems) % totalItems; // Wrap to the last item if it's the first
        }

        goToSlide(newIndex);
        updateFieldNames();
    }

    function addField(context = 'staticForm') {
        // Determine the target form based on the context
        const targetForm = context === 'carouselForm'
            ? document.querySelector('.carousel-item.active form') || document.querySelector('.carousel-item.active')
            : document.querySelector('#staticForm') || document.querySelector('form');

        // Use the current timestamp as a unique identifier for each field group
        let uniqueId = Date.now();

        // Calculate the index for the new field based on existing dynamic fields
        const index = targetForm.querySelectorAll('.dynamic-field').length;

        // Create a container div for the new field
        const fieldGroup = document.createElement('div');
        fieldGroup.className = 'form-group dynamic-field';
        fieldGroup.id = `field-group-${uniqueId}`;

        // Field Name
        const fieldNameInput = createInputField(`Fields[${index}].FieldName`, 'Field Name', 'text');
        fieldGroup.appendChild(createLabel('Field Name'));
        fieldGroup.appendChild(fieldNameInput);

        // Field Type
        const fieldTypeSelect = createFieldTypeSelect(index);
        fieldGroup.appendChild(createLabel('Field Type'));
        fieldGroup.appendChild(fieldTypeSelect);

        // Field Value
        //const fieldValueInput = createInputField(`Fields[${index}].Value`, 'Value', 'text');
        const fieldValueInput = createInputField(`Fields[${index}].FieldValue`, 'Value', 'text');
        //add an id to the field value input
        fieldValueInput.id = 'Field-Value';
        fieldGroup.appendChild(createLabel('Value'));
        fieldGroup.appendChild(fieldValueInput);

        // Remove Button
        const removeBtn = document.createElement('button');
        removeBtn.type = 'button';
        removeBtn.className = 'remove-field-btn btn btn-danger';
        removeBtn.textContent = 'Remove';
        removeBtn.onclick = function () { fieldGroup.remove(); };
        fieldGroup.appendChild(removeBtn);

        // Append the constructed field group to the target form
        targetForm.appendChild(fieldGroup);

        // Event listener to change the input field type based on the selected option
        fieldTypeSelect.addEventListener('change', function (e) {
            fieldValueInput.type = e.target.value;
        });
    }

    // Helper function to create input fields
    function createInputField(name, placeholder, type) {
        const input = document.createElement('input');
        input.type = type;
        input.className = 'form-control';
        input.name = name;
        input.placeholder = placeholder;
        return input;
    }

    // Helper function to create labels
    function createLabel(text) {
        const label = document.createElement('label');
        label.textContent = text;
        return label;
    }

    // Helper function to create a select element for field types
    function createFieldTypeSelect(index) {
        const select = document.createElement('select');
        select.className = 'form-control';
        select.name = `Fields[${index}].FieldType`;
        // TODO: These fields are actually in an enum, so you can get the options from the server

     /*   ['text', 'email', 'number', 'date'].forEach(type => {
            const option = document.createElement('option');
            option.value = type;
            option.textContent = type.charAt(0).toUpperCase() + type.slice(1);
            select.appendChild(option);
        });*/

        //get the field types from the server
        // Fetch the field types from the server
      fetch('FieldTypes')
            .then(response => response.json())
            .then(data => {
                console.log('Field Types:', data);
                data.fieldTypes.forEach(type => {
                    const option = document.createElement('option');
                    option.value = type;
                    option.textContent = type.charAt(0).toUpperCase() + type.slice(1);
                    select.appendChild(option);
                });
            });

        return select;
    }

    function createFieldGroup(fieldName, input) {

        const group = document.createElement('div');
        group.className = 'form-group';

        const label = document.createElement('label');
        label.textContent = fieldName;
        label.htmlFor = input.name; // Associate the label with the input using the input's name
        // Add the remove button
        const removeBtn = document.createElement('button');
        removeBtn.type = "button";
        removeBtn.className = "remove-field-btn";
        removeBtn.textContent = "Remove";
        // Delegate the remove functionality to the newly added button
        removeBtn.addEventListener('click', function () {
            group.remove(); // Remove the dynamic field container
            updateFieldNames(); // Call this function to re-index the field names if necessary
        });
        group.appendChild(label);
        group.appendChild(input);
        group.appendChild(removeBtn);

        return group;
    }

    //Need to create another function to handle the form submission of all of the forms that are in the carousel or the static form
    // This function will be called when the user clicks the submit button
    // This function will loop through all of the forms and submit them

    // Add event listener to the submit button
    //document.getElementById('submitData').addEventListener('click', submitAllForms);

    async function setupFormSubmission(form, rowIndex) {
        form.addEventListener('submit', async function (e) {
            // TODO: Get the value for the field name associated with the form.  current it is not being added to the form data.

            e.preventDefault();
            console.time('setupFormSubmission')
            console.log('Form data from parameters', form)

            const formData = new FormData(this);
            formData.append('FormName', document.getElementById('FormName').value);
            formData.append('Description', document.getElementById('Description').value);

            console.log('Form submitted', formData)

            for (let [key, value] of formData.entries()) {
                console.log(`${key}: ${value}`);
                //set the fields and values to the form data
                //formData.set(key, value);

            }

            try {
                const response = await fetch(this.action, {
                    method: 'POST',
                    body: formData,
                    contentType: 'multipart/form-data'
                })
                if (!response.ok) throw new Error('Network response was not ok. ', response);

                const data = await response.json();

                console.timeEnd('setupFormSubmission')
                //loadForms(data.ids);
                console.log(data);

            } catch (error) {
                console.error('Error:', error);
            }
            //hideSpinnerModal();
            console.log(`Form ${rowIndex + 1} submitted`);
            // On successful submission:
            // Show success message
            // Mark the form as submitted in the UI
            // Optionally, move to the next carousel item
        });
    }

    document.getElementById('submitAllData').addEventListener('click', function (e) {
        e.preventDefault();
        submitAllForms();
    });

    async function submitAllForms() {
        const forms = document.querySelectorAll('.carousel-item form, #staticForm');
        if (forms.length === 0) {
            console.error('No forms found.');
            return;
        }
        for (const form of forms) {
            const formData = new FormData(form);
            formData.append('FormName', document.getElementById('FormName').value);
            formData.append('Description', document.getElementById('Description').value);
            try {
                console.time('FormSubmission');
                const response = await fetch('dynamic', {
                    method: 'POST',
                    body: formData // No need to set Content-Type; it will be set automatically with boundary
                });
                //const data = await response.json();
                /*if (!data.success) {
                    console.error('Submission failed:', data.message);
                    data.errors.forEach((error, index) => {
                        console.error(`Error ${index + 1}:`, error);
                    });
                } else {
                    console.log('Submission success:', data.message);
                }*/
                if (!response.ok) throw new Error('Network response was not ok.');
                console.log('Submission success:', await response.json());
            } catch (error) {
                console.error('Error during form submission:', error);
            } finally {
                console.timeEnd('FormSubmission');
            }
        }
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

    if (uploadForm) {
        console.time('uploadForm')
        uploadForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const formData = new FormData(this);
            console.log('Show Spinner');
            showSpinnerModal();
            try {
                const response = await fetch(this.action, {
                    method: 'POST',
                    body: formData,
                    headers: { 'Accept': 'application/json', }
                });
                if (!response.ok) throw new Error('Network response was not ok.');
                const data = await response.json();

                console.timeEnd('uploadForm')

                console.time('handleUploadResponse')
                handleUploadResponse(data);              

                console.timeEnd('handleUploadResponse')
            } catch (error) {
                console.error('Error:', error);
            }
            //hideSpinnerModal();
            
        });
    }

    async function handleUploadResponse(data) {
        if (data.success && data.fields) {
            const clearFields = confirm('Do you want to clear existing fields before adding new ones?');
            if (clearFields) {
                resetCarousel();
                resetForm();
            }

            dataCarousel.style.display = '';

            //submitData.style.display = '';
            $('#uploadModal').modal('hide');
            const carouselInner = document.getElementById('carouselInner');
            data.fields.forEach((row, rowIndex) => {
                const form = document.createElement('form');
                form.action = 'dynamic'; // Adjust if your application's route is different
                form.method = "POST";    
                form.className = "form-group";
                form.id = `form-${rowIndex}`;

                const carouselItem = document.createElement('div');
                carouselItem.className = "carousel-item" + (rowIndex === 0 ? " active" : "");
                carouselItem.classList.add('carousel-item');
                if (rowIndex === 0) carouselItem.classList.add('active');

                addFormSection(row, rowIndex, form); // Now passing form instead of carouselItem

                const submitBtn = document.createElement('button');
                submitBtn.type = "submit";
                submitBtn.className = "btn btn-primary";
                submitBtn.textContent = "Submit Form";

                // Append button to form, then form to carouselItem
                form.appendChild(submitBtn);
                //form.appendChild(removeBtn);
                carouselItem.appendChild(form);
                carouselInner.appendChild(carouselItem);

                // Count the number of forms
                const formCount = document.querySelectorAll('.carousel-item').length;
                totalItems = formCount;
                updateIndexDisplay();
                // Setup form submission
                setupFormSubmission(form, rowIndex);
                hideSpinnerModal();
            });
            // Update counter   

        }
        else {
            console.error('Upload failed', data.message);
        }
        $('#staticSubmitBtn').hide();
    }

    function resetCarousel() {
        carouselInner.innerHTML = '';
        currentIndex = 0; // Reset index
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

    function addFormSection(form, rowIndex, container) {     
        
        //showSpinnerModal();
        const fields = form.Fields || [];
        const fieldsArray = form.Fields ? JSON.parse(form.Fields) : [];
        const totalFields = fieldsArray.length;
        
        if (fieldsArray.length>0) {
            // Now `fieldsArray` is an array, you can iterate over it
            //const fieldsArray = JSON.parse(form.Fields);
            // get a count of the number of forms in the array
            //console.log('Field Count from fieldArray:', fieldCount)
            fieldsArray.forEach((field, index) => {
                const { FieldName, FieldType, FieldValue } = field;

                // Create inputs for FieldName, FieldType, and FieldValue
                const fieldNameInput = createInputField(`Fields[${rowIndex}].FieldName`, FieldName, 'hidden');
                fieldNameInput.value = FieldName; // Set the value for hidden input

                const fieldTypeInput = createInputField(`Fields[${rowIndex}].FieldType`, FieldType, 'hidden');
                fieldTypeInput.value = FieldType; // This might need adjustment based on how you define FieldType

                const fieldValueInput = createInputField(`Fields[${rowIndex}].FieldValue`, FieldValue, FieldType);
                fieldValueInput.value = FieldValue; // Assign the field value

                // Append the field name and type as hidden inputs, and field value as its appropriate type
                container.appendChild(fieldNameInput);
                container.appendChild(fieldTypeInput);
                container.appendChild(createFieldGroup(FieldName, fieldValueInput)); // Only display the value input
            });

        }
        else {            
            // get a count of the number of forms in the object
            // console.log('Field Count from Object:', fieldCount);
            // showSpinnerModal();
           // const totalFields = Object.keys(form).length;
            Object.entries(form).forEach(([fieldName, fieldValue],index) => {
                // Assuming all fields are to be treated as DynamicFields

                const inputType = getInputType(fieldName, fieldValue);

                // Creating inputs for FieldName, FieldType, and FieldValue
                const fieldNameInput = document.createElement('input');
                fieldNameInput.type = 'hidden'; // Hidden because we don't need to display it
                fieldNameInput.name = `Fields[${rowIndex}].FieldName`;
                fieldNameInput.value = fieldName;

                const fieldTypeInput = document.createElement('input');
                fieldTypeInput.type = 'hidden'; // Hidden, assuming you have logic to determine this
                fieldTypeInput.name = `Fields[${rowIndex}].FieldType`;
                fieldTypeInput.value = inputType; // This might need adjustment based on how you define FieldType

                const fieldValueInput = document.createElement('input');
                fieldValueInput.type = inputType;
                fieldValueInput.name = `Fields[${rowIndex}].FieldValue`;
                fieldValueInput.className = "form-control dynamic-field";
                fieldValueInput.value = fieldValue;

                // Append these inputs to the container
                container.appendChild(createFieldGroup(fieldName, fieldValueInput)); // Only display the value input
                container.appendChild(fieldNameInput); // Add to the form, but it's hidden
                container.appendChild(fieldTypeInput); // Add to the form, but it's hidden
                
            });            
        }
    }
    
    function updateFieldNames() {
        document.querySelectorAll('.dynamic-field').forEach((fieldGroup, index) => {
            const input = fieldGroup.querySelector('input[type="text"]');
            const select = fieldGroup.querySelector('select');

            if (input) input.name = `Fields[${index}].FieldName`;
            if (select) select.name = `Fields[${index}].FieldType`;
        });
    }
    
    function resetForm() {
        document.querySelectorAll('.dynamic-field').forEach(fieldGroup => fieldGroup.remove());
    }

    document.getElementById('goToFormBtn').addEventListener('click', function () {
        // Retrieve the user input from the form number input field
        const formNumber = parseInt(document.getElementById('formNumberInput').value, 10);

        // Validate the input to ensure it is within the range of existing forms
        if (!isNaN(formNumber) && formNumber >= 1 && formNumber <= totalItems) {
            // Convert the user-friendly form number to a 0-based index for the carousel
            const formIndex = formNumber - 1;

            // Use the goToSlide function to navigate to the selected form
            goToSlide(formIndex);
            updateFieldNames();
        } else {
            alert("Please enter a valid form number.");
        }
    });

    //hideSpinner();
    async function formLoader(formIds) {
        // Construct the URL for the LoadForms action
        // If the url needs to call an endpoint from the FormsController, the url will be different than the one below. I will look like this: /Forms/LoadForms?ids=1,2,3
        console.time('loadForm')
        const url = `/Forms/LoadForms?ids=${formIds}`;
        console.log(url);
        // Fetch the forms from the server

        try {
            const response = await fetch(url, {
                method: 'GET',
                //headers: { 'Accept': 'application/json', }
            });
            if (!response.ok) throw new Error('Network response was not ok.');
            const data = await response.json();
            console.timeEnd('loadForm')

            console.time('loadResponse')
            console.log('Forms loaded:', data);
            // Process the loaded forms
            if (data.success && data.forms.length) {
                const clearFields = confirm('Do you want to clear existing fields before adding new ones?');
                if (clearFields) {
                    resetCarousel();
                    resetForm();
                }
                const firstForm = data.forms[0];

                // Update Form Name and Description fields
                document.getElementById('FormName').value = firstForm.FormName || '';
                document.getElementById('Description').value = firstForm.Description || '';

                // because the data is coming back as response.json, the data is already an object. No need to parse it, just use it to add the forms
                dataCarousel.style.display = '';
                //submitData.style.display = '';
                //$('#uploadModal').modal('hide');
                const carouselInner = document.getElementById('carouselInner');
                data.forms.forEach((row, rowIndex) => {
                    console.log(`My Form ${rowIndex}:`, row);

                    const form = document.createElement('form');
                    form.action = 'dynamic'; // Adjust if your application's route is different
                    form.method = "POST";
                    form.className = "form-group";
                    form.id = `form-${rowIndex}`;

                    const carouselItem = document.createElement('div');
                    carouselItem.className = "carousel-item" + (rowIndex === 0 ? " active" : "");
                    carouselItem.classList.add('carousel-item');
                    if (rowIndex === 0) carouselItem.classList.add('active');

                    addFormSection(row, rowIndex, form); // Now passing form instead of carouselItem

                    const submitBtn = document.createElement('button');
                    submitBtn.type = "submit";
                    submitBtn.className = "btn btn-primary";
                    submitBtn.textContent = "Submit Form";

                    // Append button to form, then form to carouselItem
                    form.appendChild(submitBtn);
                    //form.appendChild(removeBtn);
                    carouselItem.appendChild(form);
                    carouselInner.appendChild(carouselItem);

                    // Count the number of forms
                    const formCount = document.querySelectorAll('.carousel-item').length;
                    totalItems = formCount;
                    updateIndexDisplay();
                    // Setup form submission
                    setupFormSubmission(form, rowIndex);
                });

            }
            console.timeEnd('loadResponse')

        } catch (error) {
            console.error('Error:', error);
        }
    }

    window.loadForms = async function(formIds) {
        // Construct the URL for the LoadForms action
        // If the url needs to call an endpoint from the FormsController, the url will be different than the one below. I will look like this: /Forms/LoadForms?ids=1,2,3
        console.time('loadForm')
        const url = `/Forms/LoadForms?ids=${formIds}`;
        console.log(url);
        // Fetch the forms from the server

        try {
            const response = await fetch(url, {
                method: 'GET',
                //headers: { 'Accept': 'application/json', }
            });
            if (!response.ok) throw new Error('Network response was not ok.');
            const data = await response.json();
            console.timeEnd('loadForm')

            console.time('loadResponse')
            console.log('Forms loaded:', data);
            // Process the loaded forms
            if (data.success && data.forms.length) {
                const clearFields = confirm('Do you want to clear existing fields before adding new ones?');
                if (clearFields) {
                    resetCarousel();
                    resetForm();
                }
                const firstForm = data.forms[0];

                // Update Form Name and Description fields
                document.getElementById('FormName').value = firstForm.FormName || '';
                document.getElementById('Description').value = firstForm.Description || '';

                // because the data is coming back as response.json, the data is already an object. No need to parse it, just use it to add the forms
                dataCarousel.style.display = '';

                //staticSubmitBtn.style.display = 'hide'
                //submitData.style.display = '';
                //$('#uploadModal').modal('hide');
                const carouselInner = document.getElementById('carouselInner');
                data.forms.forEach((row, rowIndex) => {
                    console.log(`My Form ${rowIndex}:`, row);

                    const form = document.createElement('form');
                    form.action = 'dynamic'; // Adjust if your application's route is different
                    form.method = "POST";
                    form.className = "form-group";
                    form.id = `form-${rowIndex}`;

                    const carouselItem = document.createElement('div');
                    carouselItem.className = "carousel-item" + (rowIndex === 0 ? " active" : "");
                    carouselItem.classList.add('carousel-item');
                    if (rowIndex === 0) carouselItem.classList.add('active');

                    addFormSection(row, rowIndex, form); // Now passing form instead of carouselItem

                    const submitBtn = document.createElement('button');
                    submitBtn.type = "submit";
                    submitBtn.className = "btn btn-primary";
                    submitBtn.textContent = "Submit Form";

                    // Append button to form, then form to carouselItem
                    form.appendChild(submitBtn);
                    //form.appendChild(removeBtn);
                    carouselItem.appendChild(form);
                    carouselInner.appendChild(carouselItem);

                    // Count the number of forms
                    const formCount = document.querySelectorAll('.carousel-item').length;
                    totalItems = formCount;
                    updateIndexDisplay();
                    // Setup form submission
                    setupFormSubmission(form, rowIndex);
                });
                $('#staticSubmitBtn').hide();
            }
            console.timeEnd('loadResponse')

        } catch (error) {
            console.error('Error:', error);
        }
    }

    console.timeEnd('DynamicFormJS');
});
