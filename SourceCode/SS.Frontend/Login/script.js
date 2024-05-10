

document.addEventListener('DOMContentLoaded', function() {
    const accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
        hideAllSections();
        document.getElementById("homepageGen").style.display = "block";
        document.getElementById("userNav").style.display = "block";
        document.getElementById("CMview").style.display = "block";
        document.getElementById("welcomeSection").style.display = "block";
        document.getElementById("identity").textContent = `Logged in as: ${sessionStorage.getItem('userIdentity')}`;

    } else {
        // No valid token, show login
        hideAllSections();
        document.getElementById("homepageGen").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
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
            hideAllSections();
            document.getElementById("sendOTPSection").style.display = "none";
            document.getElementById("enterOTPSection").style.display = "block";
            document.getElementById("noLogin").style.display = "block";

        },
        error: function (xhr, status, error) {
            alert('Error sending verification code.');
        }
    });
}

function authenticateUser() {
    var otp = document.getElementById("otp").value;
    var userIdentity = document.getElementById("userIdentity").value;
    var userIsActive = false;
    $.ajax({
        url: 'http://localhost:5270/api/auth/authenticate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ userIdentity: userIdentity, proof: otp }),
        success: async function (response) {
            var accessToken = response.accessToken;
            var idToken = response.idToken;
            sessionStorage.setItem('accessToken', accessToken);
            sessionStorage.setItem('idToken', idToken);
            document.getElementById("enterOTPSection").style.display = "none";
            document.getElementById("successResult").style.display = "none";

            var accessToken = sessionStorage.getItem('accessToken');

            const accountInfo = await fetchUserAccount();

            if (accountInfo) {
                userIsActive = await getActivityStatus(accountInfo.isActive);
            }
            console.log(userIsActive)
            if(userIsActive){
                console.log("the user is active")
                $.ajax({
                    url: 'http://localhost:5270/api/auth/getRole',
                    type: 'POST',
                    contentType: 'application/json',
                    data: accessToken,
                    success: function (response) {
                        
                        
                        showModal("You're logged in!");
                        sessionStorage.setItem('role', response);
                        if (response === "2") {
                        
                            showModal("You're logged in!");
                            hideAllSections();

                            document.getElementById("homepageGen").style.display = "block";
                            document.getElementById("userNav").style.display = "block";
                            document.getElementById("CMview").style.display = "block";
                            document.getElementById("FMview").style.display = "none";
                            sessionStorage.setItem('userIdentity', userIdentity);
                            document.getElementById("identity").style.display = "block";
                            document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                            document.getElementById("welcomeSection").style.display = "block";

                        } else if (response === "3"){
                            showModal("You're logged in!");
                            hideAllSections();
                            document.getElementById("homepageGen").style.display = "block";
                            document.getElementById("userNav").style.display = "block";
                            document.getElementById("FMview").style.display = "block";
                            document.getElementById("CMview").style.display = "none";
                            sessionStorage.setItem('userIdentity', userIdentity);
                            document.getElementById("identity").style.display = "block";
                            document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                            document.getElementById("welcomeSection").style.display = "block";
                        } else if (response === "4" || response === "5"){
                            showModal("You're logged in!");
                            hideAllSections();
                            document.getElementById("homepageGen").style.display = "block";
                            document.getElementById("userNav").style.display = "block";
                            document.getElementById("FMview").style.display = "none";
                            document.getElementById("CMview").style.display = "none";
                            sessionStorage.setItem('userIdentity', userIdentity);
                            document.getElementById("identity").style.display = "block";
                            document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                            document.getElementById("welcomeSection").style.display = "block";
                        }
                        else {
                            showModal("You're logged in!");
                            hideAllSections();
                            document.getElementById("homepageGen").style.display = "block";
                            document.getElementById("userNav").style.display = "block";
                            sessionStorage.setItem('userIdentity', userIdentity);
                            document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
                            document.getElementById("welcomeSection").style.display = "block";
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Error fetching role:', error);
                    }
                });
            }
            else{
                console.log(userIsActive)
                showModal('Sorry, your account is not active. Please submit a recovery request.');
                console.log("account is not active")
                logout()
            }
        },
        error: function (xhr, status, error) {
            showModal('Incorrect verification code. Please double-check and try again.');
        }
    });
}
async function fetchUserAccount() {
    const idToken = sessionStorage.getItem('idToken');
    
        if (!idToken) {
            console.error('idToken not found in sessionStorage');
            return;
        }
        const parsedIdToken = JSON.parse(idToken);

        // Log parsed object for debugging
        console.log('Parsed idToken:', parsedIdToken);

        if (!parsedIdToken || !parsedIdToken.Username) {
            console.error('Parsed idToken does not have Username');
            return;
        }

        const email = parsedIdToken.Username;

    try {
        const response = await fetch(`http://localhost:5176/api/requestRecovery/getUserAccountDetails?email=${encodeURIComponent(email)}`);
        const data = await response.json();
        console.log(data)
        return data || null;
    } catch (error) {
        console.error('Error fetching user Account:', error);
        return null;
    }
}

async function getActivityStatus(activityStatus) {
    return activityStatus.toLowerCase() === 'yes';
}

function logout() {
    console.log("logout clicked")
    console.log("logout clicked")
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    sessionStorage.removeItem('userIdentity');
    sessionStorage.clear();
    var identityDiv = document.getElementById("identity");
    if (identityDiv) {
        console.log("Identity element found, current display:", identityDiv.style.display);
        identityDiv.style.display = "none";
        console.log("Identity should now be hidden, new display:", identityDiv.style.display);
    } else {
        console.log("Identity element not found");
    }
    hideAllSections();
    document.getElementById("sendOTPSection").style.display = "block";
    document.getElementById("noLogin").style.display = "block";
    document.getElementById("userNav").style.display = "none";
    document.getElementById("FMview").style.display = "none";
    document.getElementById("CMview").style.display = "none";
    document.getElementById("homepageGen").style.display = "none";
}

function getLogin(){
    console.log("get login page clicked  clicked");
    hideAllSections();
    document.getElementById('sendOTPSection').style.display = 'block';
    document.getElementById('noLogin').style.display = 'block';
}

function getAbout(){
    console.log("get about page clicked  clicked");
    hideAllSections();
    document.getElementById('UnAuthnAbout').style.display = 'block';
    document.getElementById('noLogin').style.display = 'block';
}

function getUserProfile(){
    console.log("get userprofile clicked  clicked");
    hideAllSections();
    document.getElementById('userProfileView').style.display = 'block';
}


function spaceBookingCenterAccess() {
    hideAllSections();
    document.getElementById('spaceBookingView').style.display = 'block';
}

function registrationAccess() {
    hideAllSections();
    document.getElementById('Registration').style.display = 'block';
    document.getElementById('noLogin').style.display = 'block';
}

function taskHubAccess() {
    hideAllSections();
    document.getElementById('taskManagerView').style.display = 'block';
}

function personalOverviewAccess() {
    hideAllSections();
    document.getElementById('personalOverviewCenter').style.display = 'block';
}

function waitlistAccess() {
    hideAllSections();
    document.getElementById('waitlistView').style.display = 'block';
}
function spaceManagerAccess() {
    hideAllSections();
    document.getElementById('spaceManagerView').style.display = 'block';
}

function getHomePage() {
    hideAllSections();
    document.getElementById("welcomeSection").style.display = "block";
}

function employeeSetupAccess() {
    hideAllSections();
    document.getElementById('employeeSetup').style.display = 'block';
}

function confirmationAccess() {
    hideAllSections();
    document.getElementById('confirmationView').style.display = 'block';
}

function SOAccess() {
    hideAllSections();
    document.getElementById('systemObservability').style.display = 'block';
}

function hideAllSections() {
    document.getElementById('employeeSetup').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById('UnAuthnAbout').style.display = 'none';
    document.getElementById('Registration').style.display = 'none';
    document.getElementById('spaceManagerView').style.display = 'none';
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById('noLogin').style.display = 'none';
    document.getElementById('confirmationView').style.display = 'none';
    document.getElementById('systemObservability').style.display = 'none';
    document.getElementById("accountRecoverySection").style.display = "none";
    document.getElementById("userRequestsView").style.display = "none";
}


