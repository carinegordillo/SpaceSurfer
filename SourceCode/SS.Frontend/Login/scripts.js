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
            sessionStorage.setItem('accessToken', accessToken);
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";

            getRole(function (roles) {
                var isManager = roles.some(function (role) {
                    return role == "2" || role == "3";
                });

                if (isManager) {
                    document.getElementById("homepageManager").style.display = "block";
                    document.getElementById("homepageGen").style.display = "none";
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
    sessionStorage.removeItem('accessToken');
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";
}
function getRole(callback) {
    var token = sessionStorage['accessToken'];

    if (!token) {
        var accessTokenContainer = document.getElementById("accessTokenContainer");
        accessTokenContainer.innerHTML = "<p>No access token found</p>";
        return;
    }

    $.ajax({
        url: 'http://localhost:5270/api/auth/decodeToken',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(token),
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
    var token = sessionStorage['accessToken'];

    if (!token) {
        var accessTokenContainer = document.getElementById("accessTokenContainer");
        accessTokenContainer.innerHTML = "<p>No access token found</p>";
        return;
    }

    $.ajax({
        url: 'http://localhost:5270/api/auth/decodeToken',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(token),
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


