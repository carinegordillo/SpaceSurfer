function sendOTP() {
    var userIdentity = document.getElementById("userIdentity").value;

    $.ajax({
        url: 'http://localhost:5270/api/auth/sendOTP',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ userIdentity: userIdentity }),
        success: function (response) {
            document.getElementById("sendOTPSection").style.display = "none";
            document.getElementById("enterOTPSection").style.display = "block";
        },
        error: function (xhr, status, error) {
            alert('Error sending verification code.');
        }
    });
}
function authenticateUser() {
    var otp = document.getElementById("otp").value;
    var userIdentity = document.getElementById("userIdentity").value;

    $.ajax({
        url: 'http://localhost:5270/api/auth/authenticate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ userIdentity: userIdentity, proof: otp }),
        success: function (response) {
            var accessToken = response;
            localStorage.setItem('accessToken', accessToken);
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";
            document.getElementById("homepageGen").style.display = "block";
            authorize();

            //var accessTokenContainer = document.getElementById("accessTokenContainer");
            //accessTokenContainer.innerHTML = "<p>Access Token: " + accessToken + "</p>";
        },
        error: function (xhr, status, error) {
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";
            document.getElementById("failResult").style.display = "block";
        }
    });
}
function logout() {
    // Remove token from localStorage
    localStorage.removeItem('accessToken');
    document.getElementById("homepage").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";
}
function authorize() {
    var token = localStorage['accessToken'];

    if (!token) {
        // Handle case where token is not found in localStorage
        var accessTokenContainer = document.getElementById("accessTokenContainer");
        accessTokenContainer.innerHTML = "<p>No access token found</p>";
        return;
    }

    $.ajax({
        url: 'http://localhost:5270/api/auth/decodeToken',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(token), // Ensure token is properly serialized
        success: function (response) {
            var accessTokenContainer = document.getElementById("accessTokenContainer");
            accessTokenContainer.innerHTML = "<p>Token Info: " + response + "</p>";
        },
        error: function (xhr, status, error) {
            var accessTokenContainer = document.getElementById("accessTokenContainer");
            accessTokenContainer.innerHTML = "<p>Error retrieving token info</p>";
        }
    });
}
