document.addEventListener('DOMContentLoaded', function () {

    window.logMessage=function(level, message) {
        // Prepare the log entry
        const logEntry = {
            timestamp: new Date().toISOString(),
            level: level,
            message: message,
        };

        // Convert log entry to a string or format as needed
        const logString = JSON.stringify(logEntry);

        // Send the logString to a server
        fetch('/log', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: logString,
        })
        .catch(error => console.error('Failed to log message to server:', error));
    }

});