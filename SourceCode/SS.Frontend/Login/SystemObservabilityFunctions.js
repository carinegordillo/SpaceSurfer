
// Function to fetch analysis data
async function fetchAnalysis(timeSpan) {
    // Retrieve tokens from sessionStorage
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    // Check if tokens are present
    if (!idToken) {
        console.error('ID token not found.');
        return;
    }

    if (!username) {
        console.error('Username not found in token.');
        return;
    }

    if (!accessToken) {
        console.error('Access token not found.');
        return;
    }

    // Construct the URL with the provided time span
    const url = `http://localhost:5295/api/v1/SystemObservability/Information?timeSpan=${timeSpan}`;

    try {
        // Fetch data from the API
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json"
            }
        });

        // Check if the response is OK
        if (!response.ok) {
            throw new Error(`Error fetching analysis: ${response.status}`);
        }

        // Parse and return the response data
        const data = await response.json();
        return data;
    }
    catch (error) {
        // Catch and log any errors that occur during the process
        throw new Error(`Error fetching analysis due to error: ${error.message}`);
    }
}

// Function to insert the view duration insertion data for each view
async function fetchInsertViewDuration(viewName, duration) {
    // Retrieve tokens from sessionStorage
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    // Check if tokens are present
    if (!idToken) {
        console.error('ID token not found.');
        return;
    }

    if (!username) {
        console.error('Username not found in token.');
        return;
    }

    if (!accessToken) {
        console.error('Access token not found.');
        return;
    }

    // Construct the URL with the provided view name and duration
    const url = `http://localhost:5295/api/v1/SystemObservability/ViewDurationInsertion?viewName=${viewName}&durationInSeconds=${duration}`;

    try {
        // Fetch data from the API
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        // Parse and return the response data
        const data = await response.json();

        // Check if there's any error in the response
        if (data.hasError) {
            throw new Error(`Error inserting the view duration: ${response.status}`);
        }

        return data;
    }
    catch (error) {
        // Catch and log any errors that occur during the process
        throw new Error(`Caught an error inserting the view duration: ${error.message}`);
    }
}

// Function to insert the used feature after every use
async function fetchInsertUsedFeature(featureName) {
    // Retrieve tokens from sessionStorage
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    // Check if tokens are present
    if (!idToken) {
        console.error('ID token not found.');
        return;
    }

    if (!username) {
        console.error('Username not found in token.');
        return;
    }

    if (!accessToken) {
        console.error('Access token not found.');
        return;
    }

    // Construct the URL with the provided feature name
    const url = `http://localhost:5295/api/v1/SystemObservability/UsedFeatureInsertion?FeatureName=${featureName}`;

    try {
        // Fetch data from the API
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        // Parse and return the response data
        const data = await response.json();

        // Check if there's any error in the response
        if (data.hasError) {
            throw new Error(`Error inserting the feature use: ${response.status}`);
        }

        return data;
    }
    catch (error) {
        // Catch and log any errors that occur during the process
        throw new Error(`Caught an error inserting the feature use: ${error.message}`);
    }
}


async function updateTimeSpan() {
    var newTimeSpan = document.getElementById('timeSpan').value;
    const acceptedValues = ["6 months", "12 months", "24 months"];

    var interval = sessionStorage.getItem('intervalId')

    if (interval)
    {
        clearInterval(parseInt(interval));
        sessionStorage.removeItem('intervalId');
    }

    if (acceptedValues.includes(newTimeSpan))
    {
        timeSpan = newTimeSpan;

    // Call getAnalysisInformation every 60 seconds
    var intervalId = setInterval(async () => {

        analysis = await fetchAnalysis(timeSpan);

        // Login
        var loginsData = analysis.loginsCount;
        var loginLabels = loginsData.map(item => `${item.month}, ${item.year}`);
        var successfulLogins = loginsData.map(item => item.successfulLogins);
        var failedLogins = loginsData.map(item => item.failedLogins);

        // Registration
        var registrationData = analysis.registrationCount;
        var registrationLabels = registrationData.map(item => `${item.month}, ${item.year}`);
        var successfulRegistrations = loginsData.map(item => item.successfulRegistrations);
        var failedRegistrations = loginsData.map(item => item.failedRegistrations);

        // Update logins trend chart
        updateLoginsTrendChart(loginLabels, successfulLogins, failedLogins);
        updateRegistrationTrendChart(registrationLabels, successfulRegistrations, failedRegistrations);

    }, 60000);

    sessionStorage.setItem('intervalId', intervalId);





        if (intervalId) {
            // If interval is already running, clear it and start a new one
            clearInterval(intervalId);
            intervalId = null;
        }

        analysis = await fetchAnalysis(timeSpan);

        // Login
        var loginsData = analysis.loginsCount;
        var loginLabels = loginsData.map(item => `${item.month}, ${item.year}`);
        var successfulLogins = loginsData.map(item => item.successfulLogins);
        var failedLogins = loginsData.map(item => item.failedLogins);

        // Registration
        var registrationData = analysis.registrationCount;
        var registrationLabels = registrationData.map(item => `${item.month}, ${item.year}`);
        var successfulRegistrations = registrationData.map(item => item.successfulRegistrations);
        var failedRegistrations = registrationData.map(item => item.failedRegistrations);

        // Update logins trend chart
        updateLoginsTrendChart(loginLabels, successfulLogins, failedLogins);
        updateRegistrationTrendChart(registrationLabels, successfulRegistrations, failedRegistrations);



    }
    else {
        console.error("Invalid time span value. Accepted values are: 6 months, 12 months, 24 months");
    }
}

function updateLoginsTrendChart(labels, successfulLogins, failedLogins) {
    var ctx = document.getElementById('loginsTrendChart').getContext('2d');

    if (window.loginsChart) {
        window.loginsChart.destroy(); // Destroy the existing chart
    }

    window.loginsChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Successful Logins',
                data: successfulLogins,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            },
            {
                label: 'Failed Logins',
                data: failedLogins,
                fill: false,
                borderColor: 'rgb(255, 99, 132)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true, // Set maintainAspectRatio to true
            aspectRatio: 1, // Set aspect ratio to 1:1
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Month, Year'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Number of Logins'
                    }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom'
                }
            },
            layout: {
                padding: {
                    top: 10,
                    right: 10,
                    bottom: 10,
                    left: 10
                }
            }
        }
    });
}

function updateRegistrationTrendChart(labels, successfulRegistrations, failedRegistrations) {
    var ctx = document.getElementById('registrationsTrendChart').getContext('2d');

    if (window.registrationsChart) {
        window.registrationsChart.destroy(); // Destroy the existing chart
    }

    window.registrationsChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Successful Registrations',
                data: successfulRegistrations,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            },
            {
                label: 'Failed Registrations',
                data: failedRegistrations,
                fill: false,
                borderColor: 'rgb(255, 99, 132)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true, // Set maintainAspectRatio to true
            aspectRatio: 1, // Set aspect ratio to 1:1
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Month, Year'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Number of Registrations'
                    }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom'
                }
            },
            layout: {
                padding: {
                    top: 10,
                    right: 10,
                    bottom: 10,
                    left: 10
                }
            }
        }
    });
}
