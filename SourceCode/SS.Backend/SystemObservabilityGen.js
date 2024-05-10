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