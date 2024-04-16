import { HubConnectionBuilder } from "@microsoft/signalr";
import Chart from 'chart.js/auto';
import 'chartjs-adapter-date-fns'; // This imports the adapter

document.addEventListener('DOMContentLoaded', function () {
    let chart; // Global reference to the chart instance
    let connection = null;
    let userId = document.getElementById('loggedInUser').value;
    let chartTypeDropdown = document.getElementById('chartType');
    let chartTypes = ['bar', 'line', 'pie', 'doughnut', 'radar', 'polarArea', 'bubble', 'scatter'];
    let currentData = null;

    function initializeChart() {
        const ctx = document.getElementById('myChart').getContext('2d');
        chart = new Chart(ctx, {
            type: 'bar', // Default chart type
            data: {
                labels: [],
                datasets: [{
                    label: 'Real-time Data',
                    data: [],
                    borderColor: 'rgb(75, 192, 192)',
                    tension: 0.1
                }]
            }
        });
    }

    async function startSignalRConnection() {
        connection = new HubConnectionBuilder().withUrl("/dataHub").build();

        connection.on("ReceiveData", function (data) {
            console.log("Received data:", data);
            currentData = data;
            redrawChart(currentData);
        });

        connection.on("UpdateChart", function (data) {
            console.log("Updating chart with data:", data);
            currentData = data;
            redrawChart(currentData);
        });

        connection.on("UpdateValue", function (data) {
            console.log(`Count of ${data.FieldName}: ${data.Count}`);
            currentData = data;
            redrawChart(currentData);
            // Update the UI or alert the user
        });
        connection.on("UpdateExpiringEntries", function (data) {
            console.log("Updating chart with expiring entries:", data);
            currentData = data;
            redrawChart(currentData);
        });

        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error(err);
            setTimeout(startSignalRConnection, 5000); // Reconnect on failure
        }
    }

    document.getElementById('requestData').addEventListener('click', function (e) {
        e.preventDefault();
        const analysisType = document.getElementById('dataSelection').value;
        // Example for date fields; you'd add UI elements to capture these dates
        const fieldName = fieldNameDropdown.options[fieldNameDropdown.selectedIndex].value; // This needs to be set based on user input or selection
        const startDate = document.getElementById('startDate').value;
        const endDate = document.getElementById('endDate').value;

        if (connection) {
            console.log("Requesting data...", analysisType, fieldName, startDate, endDate, connection);
            connection.invoke("RequestData", analysisType, fieldName, startDate, endDate, userId)
                .then(() => {
                    console.log("Request data sent successfully");
                    // update the Chart.js UI
                })
                .catch(err => {
                    console.error("Error invoking 'RequestData':", err);
                    if (err instanceof Error) {
                        alert(`Failed to load data: ${err.message}`);
                    } else {
                        alert(`Failed to load data: ${err}`);
                    }
                });
        }
    });
    function redrawChart(data) {
        const chartType = chartTypeDropdown.options[chartTypeDropdown.selectedIndex].value; // Get the current selected value from dropdown
        const ctx = document.getElementById('myChart').getContext('2d');

        if (chart) {
            chart.destroy(); // Destroy the existing chart instance
        }

        let axistype = GetdataConfiguration(data);

        let config = {
            type: chartType, // Dynamic chart type
            data: {
                labels: data.labels,
                datasets: [{
                    label: data.title,
                    data: data.data,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(255, 159, 64, 0.2)',
                        'rgba(255, 205, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(201, 203, 207, 0.2)'
                    ],
                    borderColor: [
                        'rgb(255, 99, 132)',
                        'rgb(255, 159, 64)',
                        'rgb(255, 205, 86)',
                        'rgb(75, 192, 192)',
                        'rgb(54, 162, 235)',
                        'rgb(153, 102, 255)',
                        'rgb(201, 203, 207)'
                    ],
                    borderWidth: 1, // Adjusted for visibility in all chart types
                    tension: 0.1
                }]
            },
            options: {
                scales: {}
            }
        };

        // Conditional configuration based on chart type
        switch (chartType) {
            case 'line':
            case 'bar':
                config.options.scales = {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Duration (Days)'
                        },
                        type: axistype, // This is important to show the data correctly'
                        time: {
                            unit: 'day',
                            tooltipFormat: 'MMM dd, yyyy',
                        },
                        min: data.startDate,
                        max: data.endDate,

                    }

                };
                break;
            case 'pie':
            case 'doughnut':
            case 'polarArea':
                // For these charts, adjust the data directly since they use a different dataset structure
                config.data = {
                    datasets: [{
                        data: data.data, /*[10, 20, 30],*/
                        backgroundColor: config.data.datasets[0].backgroundColor.slice(0, 3),
                        borderColor: config.data.datasets[0].borderColor.slice(0, 3),
                        borderWidth: 2,
                    }],
                    labels: ['Red', 'Yellow', 'Blue']
                };
                break;
            case 'radar':
                config.options.elements = {
                    line: {
                        borderWidth: 3,
                        fill: false
                    }
                };
                break;
            case 'bubble':
                config.data.datasets[0].data = data.data.map(value => ({
                    x: value.x,
                    y: value.y,
                    r: value.size // Assuming `size` is provided as part of `value`
                }));
                config.options.scales = {
                    y: {
                        beginAtZero: true
                    }
                };
                break;
            case 'scatter':
                config.data.datasets[0].data = data.data.map(value => ({
                    x: value.x,
                    y: value.y
                }));
                config.options.scales = {
                    y: {
                        beginAtZero: true
                    }
                };
                break;
        }

        // Create a new chart with dynamic configuration
        chart = new Chart(ctx, config);
    }

    function GetdataConfiguration(data, config) {
        let axisType = 'linear'; // Default axis type

        if (data.counts == undefined && data.startDate && data.endDate) {
            config.options.scales.x.type = 'time';
        }
        else if (data.data.some(value => typeof value === 'number')) {
            config.options.scales.y.type = 'linear';
        }
        else if (data.data.some(value => typeof value === 'string')) {
            config.options.scales.y.type = 'category';
        }
        else if (data.data.some(value => typeof value === 'object')) {
            config.options.scales.y.type = 'logarithmic';
        }
        else if (data.data.some(value => typeof value === 'boolean')) {
            config.options.scales.y.type = 'category';
        }
        else {
            config.options.scales.y.type = 'category';
        }
        return axisType;
    }

    chartTypes.forEach(type => {
        const option = document.createElement('option');
        option.value = type;
        option.text = type;
        chartTypeDropdown.appendChild(option);
    });

    document.getElementById('updateChartType').addEventListener('click', function (e) {

        redrawChart(currentData);
    });

    // Initialize chart and SignalR connection
    initializeChart();
    startSignalRConnection();
});