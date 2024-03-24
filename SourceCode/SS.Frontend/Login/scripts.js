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

            getRole(function (roles) {
                var isManager = roles.some(function (role) {
                    return role == "2" || role == "3";
                });

                if (isManager) {
                    document.getElementById("homepageManager").style.display = "block";
                } else {
                    document.getElementById("homepageGen").style.display = "block";
                }
            });

            document.getElementById("homepageGen").style.display = "block";
        },
        error: function (xhr, status, error) {
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";
            document.getElementById("failResult").style.display = "block";
        }
    });
}

function logout() {
    localStorage.removeItem('accessToken');
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";
}
function getRole(callback) {
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
            var role = response.role;
            callback(role);
        },
        error: function (xhr, status, error) {
            var accessTokenContainer = document.getElementById("accessTokenContainer");
            accessTokenContainer.innerHTML = "<p>Error retrieving token info</p>";
        }
    });
}

function getExpirationTime(callback) {
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
            var time = response.exp_time;
            callback(time);
        },
        error: function (xhr, status, error) {
            var accessTokenContainer = document.getElementById("accessTokenContainer");
            accessTokenContainer.innerHTML = "<p>Error retrieving token info</p>";
        }
    });
}
