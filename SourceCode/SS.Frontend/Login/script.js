

function sendOTP() {
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);

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
            var accessToken = response.accessToken;
            var idToken = response.idToken;
            sessionStorage.setItem('accessToken', accessToken);
            sessionStorage.setItem('idToken', idToken);
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";

            var accessToken = sessionStorage.getItem('accessToken');

            $.ajax({
                url: 'http://localhost:5270/api/auth/getRole',
                type: 'POST',
                contentType: 'application/json',
                data: accessToken,
                success: function (response) {
                    if (response === "2" || response === "3") {
                        document.getElementById("homepageManager").style.display = "block";
                        document.getElementById("homepageGen").style.display = "none";
                    }
                    else {
                        document.getElementById("homepageGen").style.display = "block";
                     }
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching role:', error);
                }
            });
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
    sessionStorage.removeItem('idToken');
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";
}

// Function to toggle the display of views
function toggleView(viewId) {
    document.getElementById('spaceBookingViewGen').style.display = 'none';
    document.getElementById('waitlistViewGen').style.display = 'none';
    document.getElementById('spaceBookingViewManager').style.display = 'none';
    document.getElementById('waitlistViewManager').style.display = 'none';
    document.getElementById(viewId).style.display = 'block';
}

//1 if general, 2 if manager
var page = 0;

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('showWaitlistButtonGen').addEventListener('click', () => {
        toggleView('waitlistViewGen');
        page = 1;
    });
    document.getElementById('showSpaceBookingButtonGen').addEventListener('click', () => {
        toggleView('spaceBookingViewGen');
        page = 1;
    });

    document.getElementById('showWaitlistButtonManager').addEventListener('click', () => {
        toggleView('waitlistViewManager');
        page = 2;
    });
    document.getElementById('showSpaceBookingButtonManager').addEventListener('click', () => {
        toggleView('spaceBookingViewManager');
        page = 2;
    });
});