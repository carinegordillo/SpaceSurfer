// JavaScript source code
function showPassword() {
    var visibility = document.getElementById("OTP");
    if (visibility.type === "password") {
        visibility.type = "text";
    }
    else {
        visibility.type = "password";
    }
}

function changeView(viewId) {
    // Get all the view elements
    var views = document.getElementsByClassName("center-box");

    // Hide all the views
    for (var i = 0; i < views.length; i++) {
        views[i].style.display = "none";
    }

    // Show the requested view
    document.getElementById(viewId).style.display = "block";
}

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

function hideView(viewId) {
    document.getElementById(viewId).style.display = "none";
}

