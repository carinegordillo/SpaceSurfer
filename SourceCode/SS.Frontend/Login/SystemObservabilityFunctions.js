let intervalId = null;

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

async function updateTimeSpan() {
    var newTimeSpan = document.getElementById('timeSpan').value;
    const acceptedValues = ["6 months", "12 months", "24 months"];

    if (intervalId) {
        clearInterval(intervalId);
        intervalId = null;
    }

    if (acceptedValues.includes(newTimeSpan)) {
        timeSpan = newTimeSpan;

        // Call getAllData every 60 seconds
        intervalId = setInterval(async () => {
            await getAllData(timeSpan);
        }, 60000);

        // Initial fetch
        await getAllData(timeSpan);
    } else {
        console.error("Invalid time span value. Accepted values are: 6 months, 12 months, 24 months");
    }
}


async function getAllData(timesSpan) {
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

    createTopListData(analysis);

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

function createTopListData(analysis) {
    const topItems = {
        viewsDuration: analysis.viewsDurationCount,
        usedFeatures: analysis.usedFeatureCount,
        companyReservations: analysis.topCompanyReservationCount,
        companySpaces: analysis.topCompanySpaceCount
    };

    for (let itemType in topItems) {
        if (topItems.hasOwnProperty(itemType)) {
            const topItemsList = topItems[itemType];
            const listContainer = document.getElementById(`${itemType}List`);

            // Clear existing list
            while (listContainer.firstChild) {
                listContainer.removeChild(listContainer.firstChild);
            }

            // Create and append new numbered list items
            for (let i = 0; i < Math.min(topItemsList.length, 3); i++) {
                const item = topItemsList[i];
                const ul = document.createElement('ul');
                const span = document.createElement('span');
                span.textContent = `${item.companyName || item.viewName || item.featureName}: ${item.durationInSeconds || item.usageCount || item.reservationCount || item.spaceCount}`;
                ul.appendChild(span);
                listContainer.appendChild(ul);

                // Highlight the first item
                if (i === 0) {
                    ul.style.fontWeight = 'bold';
                }

                // Add number to list item
                const number = document.createElement('span');
                number.textContent = i + 1;
                number.style.marginRight = '5px';
                ul.insertBefore(number, span);
            }
        }
    }
}

function stopTimeSpan()
{
    clearInterval(intervalId);
    intervalId = null;
}