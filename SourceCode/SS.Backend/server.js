const express = require('express');
const port = 8080;
const app = express();

app.use(express.json())


// Enable CORS
app.use(function (req, res, next) {
    res.setHeader('Access-Control-Allow-Origin', 'http://localhost:3000');
    res.setHeader('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, PATCH, DELETE');
    res.setHeader('Access-Control-Allow-Headers', '*');

    // Handle preflight requests
    if (req.method === 'OPTIONS') {
        res.sendStatus(200); // Respond to preflight request
    } else {
        next();
    }
});

app.post('/api/AccountDeletion/Delete', (req, res) => {
    // Extract data from the request body
    const { username } = req.body;

    // Implement logic to delete the account based on the username
    // For example, you can use a database query or call a service to delete the account
    // This is just a placeholder logic, replace it with your actual logic
    const isAccountDeleted = DeletionService.DeleteAccount(username);

    if (isAccountDeleted) {
        // If the account is deleted successfully, send a success response
        res.status(200).json({ message: 'Account deleted successfully' });
    } else {
        // If there was an error deleting the account, send an error response
        res.status(500).json({ error: 'Failed to delete the account' });
    }
});



// Forward POST requests to ASP.NET Core controller

app.post('/', (request, response) =>
{
    response.setHeader('Content-Type', 'text/html');

    // Example of returning a "view" from the server to the client
    response.send("<h2 style=\"color: green\"><i>POST SUCCESSFUL</i></h2>");
})

app.post('/error', (request, response) => {

    // Example of a server-side error with useful error message
    response.status(500).send('Database is full');
})

app.options('/*', (request, response) => {
    response.sendStatus(200);
})


// Start the server
app.listen(port, () => {
    console.log(`Server is running on http://localhost:${port}`);
});
