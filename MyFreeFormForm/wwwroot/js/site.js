// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*document.addEventListener('DOMContentLoaded', function () {

    document?.querySelector('#printBtn')?.addEventListener('click', printFormAsDocument);

    function printFormAsDocument() {
        const form = document.querySelector('form'); // Adjust the selector to target the correct form if necessary
        let printContent = '<div style="font-family: Arial, sans-serif;">';

        // Title
        const formName = document.getElementById('FormName') ? document.getElementById('FormName').value : '';
        const description = document.getElementById('Description') ? document.getElementById('Description').value : '';
        const formNotes = ''; // Assume formNotes is captured similarly

        // Add the static fields (if not empty)
        if (formName) printContent += `<strong>Form Name:</strong> ${formName}<br>`;
        if (description) printContent += `<strong>Description:</strong> ${description}<br>`;
        if (formNotes) printContent += `<strong>Form Notes:</strong> ${formNotes}<br>`;

        // Dynamically add the dynamic fields
        document.querySelectorAll('.form-group').forEach(group => {
            const label = group.querySelector('label') ? group.querySelector('label').textContent : 'No Label';
            const input = group.querySelector('input, select, textarea');
            const value = input ? input.value : '';

            printContent += `<strong>${label}</strong>: ${value}<br>`;
        });

        printContent += '</div>';

        // Open a new window or tab for printing
        const printWindow = window.open('', '_blank');
        printWindow.document.write(printContent);
        printWindow.document.close();
        printWindow.focus();
        printWindow.print();
        printWindow.close();
    }
});*/