document.addEventListener('DOMContentLoaded', function() {


    // Handle form submission
    document.getElementById('accountCreationForm').addEventListener('submit', function(e) {
        e.preventDefault();
        submitAccountCreationForm();
    });


});



function submitAccountCreationForm() {
    var username = document.getElementById('username').value;

    console.log("Submitting username:", username); // Debugging

    fetch('http://localhost:5041/api/registration/verifyAccount', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: username }), // Make sure to match the DTO structure
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        console.log('Response data:', data); // Debugging
        alert('Account verified successfully!');
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error verifying account. ' + error.message);
    });
}



