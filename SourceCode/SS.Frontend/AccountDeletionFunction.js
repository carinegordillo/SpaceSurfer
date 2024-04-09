// AccountDeletionFunction.js

// Show and hide password
function showPassword() {
    var visibility = document.getElementById("OTP");
    if (visibility.type === "password") {
        visibility.type = "text";
    }
    else {
        visibility.type = "password";
    }
}

// Change the view for being a SPA
function changeView(viewId) {
    // Get all the view elements
    var views = document.getElementsByClassName("center-box");

    // Hide all the views
    for (var i = 0; i < views.length; i++) {
        views[i].style.display = "none";
    }

    // Show the requested view if it exists
    var requestedView = document.getElementById(viewId);
    if (requestedView) {
        requestedView.style.display = "block";
    } else {
        console.error("View with ID '" + viewId + "' does not exist");
    }
}



// Checks to see if all fields are filled in
function validateCredentials()
{
    var email = document.getElementById("email").value;
    var otp = document.getElementById("OTP").value;

    if (email == "" || otp == "")
    {
        alert("All fields must be filled out");
        return false;
    }

    // If validation is successful, change the view
    changeView('AccountDeletedView');
    return true;
}

// hides the previous view
function hideView(viewId) {
    document.getElementById(viewId).style.display = "none";
}

function deleteAccount(username) {
    var requestData = { username: username };

    fetch('http://localhost:5198/api/AccountDeletion/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // Handle success response
            changeView('AccountDeletedView');
            console.log(data);
        })
        .catch(error => {
            // Handle error response
            console.error('Error:', error);
        });
}

//function deleteAccount(username) {
//    var requestData = { username: username };

//    $.ajax({
//        url: 'http://localhost:5198/api/AccountDeletion/Delete',
//        type: 'POST',
//        contentType: 'application/json',
//        data: JSON.stringify(requestData),
//        success: function (data) {
//            // Handle success response
//            changeView('AccountDeletedView');
//            console.log(data);
//        },
//        error: function (xhr, status, error) {
//            // Handle error response
//            console.error('Error:', error);
//        }
//    });
//}
