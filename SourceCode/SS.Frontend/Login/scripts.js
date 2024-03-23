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
            document.getElementById("homepage").style.display = "block";

            var accessTokenContainer = document.getElementById("accessTokenContainer");
            accessTokenContainer.innerHTML = "<p>Access Token: " + accessToken + "</p>";
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
    document.getElementByID("sendOTPSection").style.display = "block";
}