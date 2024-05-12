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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const SOurl = appConfig.api.SystemObservability;
    // Construct the URL with the provided time span
    const url = `${SOurl}/api/v1/SystemObservability/UsedFeatureInsertion?FeatureName=${featureName}`;

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


