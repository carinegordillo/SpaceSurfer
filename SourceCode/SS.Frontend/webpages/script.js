function sendOTP() {
    var userIdentity = document.getElementById("userIdentity").value;

    $.ajax({
        url: 'http://localhost:5270/api/auth/sendOTP',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({userIdentity: userIdentity }),
        success: function (response) {
            var otp = response.otp;

            var otpWindow = window.open('', '_blank', 'width=400,height=200');
            otpWindow.document.write('<h2>Your OTP:</h2><p>' + otp + '</p>');

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
        data: JSON.stringify({userIdentity: userIdentity, proof: otp }),
        success: function (response) {
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "block";
            document.getElementById("failResult").style.display = "none";
                },
        error: function (xhr, status, error) {
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";
            document.getElementById("failResult").style.display = "block";
        }
    });
}