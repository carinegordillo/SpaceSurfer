document.addEventListener('DOMContentLoaded', function() {
    const accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
        // Assuming accessToken is sufficient to determine logged in state
        // displayHomePage();
        document.getElementById("homepageGen").style.display = "block";
        document.getElementById("welcomeSection").style.display = "block";
        document.getElementById("noLogin").style.display = "none";
        document.getElementById("sendOTPSection").style.display = "none";
        document.getElementById("identity").textContent = `Logged in as: ${sessionStorage.getItem('userIdentity')}`;
        document.getElementById('UnAuthnAbout').style.display = 'none';
        document.getElementById('Registration').style.display = 'none';
    } else {
        // No valid token, show login
        document.getElementById("homepageGen").style.display = "none";
        document.getElementById("welcomeSection").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
        document.getElementById('UnAuthnAbout').style.display = 'none';
        document.getElementById('waitlistView').style.display = 'none';
        document.getElementById('successResult').style.display = 'none';
        document.getElementById('failResult').style.display = 'none';
        document.getElementById('personalOverviewCenter').style.display = 'none';
        document.getElementById("taskManagerView").style.display = "none";
        document.getElementById('spaceBookingView').style.display = 'none';
        document.getElementById('userProfileView').style.display = 'none';
        document.getElementById('Registration').style.display = 'none';
    }
});

function sendOTP() {
    console.log("otp is sending")
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
                        // document.getElementById("homepageManager").style.display = "block";
                        document.getElementById("homepageGen").style.display = "block";
                        sessionStorage.setItem('userIdentity', userIdentity);
                        document.getElementById("identity").style.display = "block";
                        document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                        document.getElementById("welcomeSection").style.display = "block";
                        document.getElementById("noLogin").style.display = "none";
                        document.getElementById('UnAuthnAbout').style.display = 'none';
                        document.getElementById('Registration').style.display = 'none';

                    }
                    else {
                        document.getElementById("homepageGen").style.display = "block";
                        sessionStorage.setItem('userIdentity', userIdentity);
                        document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                        document.getElementById("welcomeSection").style.display = "block";
                        document.getElementById("noLogin").style.display = "none";
                        document.getElementById('UnAuthnAbout').style.display = 'none';
                        document.getElementById('Registration').style.display = 'none';
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
    console.log("logout clicked")
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    sessionStorage.removeItem('userIdentity')
    var identityDiv = document.getElementById("identity");
    if (identityDiv) {
        console.log("Identity element found, current display:", identityDiv.style.display);
        identityDiv.style.display = "none";
        console.log("Identity should now be hidden, new display:", identityDiv.style.display);
    } else {
        console.log("Identity element not found");
    }
    document.getElementById("sendOTPSection").style.display = "block";
    document.getElementById("noLogin").style.display = "block";
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';
}

function getLogin(){
    console.log("get login page clicked  clicked");
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    // document.getElementById('homepageGen').style.display = 'none';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'block';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('Registration').style.display = 'none';
}

function getAbout(){
    console.log("get about page clicked  clicked");
    document.getElementById('UnAuthnAbout').style.display = 'block';
    document.getElementById('userProfileView').style.display = 'none';
    // document.getElementById('homepageGen').style.display = 'none';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('Registration').style.display = 'none';
}

function getUserProfile(){
    console.log("get userprofile clicked  clicked");
    document.getElementById('userProfileView').style.display = 'block';
    // document.getElementById('homepageGen').style.display = 'none';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';


}

function spaceBookingCenterAccess() {
    document.getElementById('spaceBookingView').style.display = 'block';
    // document.getElementById('homepageGen').style.display = 'none';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';

}

function registrationAccess() {
    document.getElementById('taskManagerView').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';

    document.getElementById('Registration').style.display = 'block';
}
function taskHubAccess() {
    document.getElementById('taskManagerView').style.display = 'block';
    
// do an if user role is whatever then display the manager page 
    // document.getElementById('homepageGen').style.display = 'block';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';

    document.getElementById('Registration').style.display = 'none';
}

function personalOverviewAccess() {
    // Show the personalOverviewCenter section
    document.getElementById('personalOverviewCenter').style.display = 'block';
    // document.getElementById('homepageGen').style.display = 'block';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';

}
    // Hide other sections if needed
function waitlistAccess() {
    document.getElementById('waitlistView').style.display = 'block';

    // document.getElementById('homepageGen').style.display = 'block';
    // document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';

}
function getHomePage() {
    document.getElementById("welcomeSection").style.display = "block";
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';
}
