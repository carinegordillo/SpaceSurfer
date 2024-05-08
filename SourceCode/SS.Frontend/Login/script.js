document.addEventListener('DOMContentLoaded', function() {
    fetch('csp-config.json')
        .then(response => response.json())
        .then(cspConfig => {
            const cspContent = [
                `default-src ${cspConfig.defaultSrc.join(' ')}`,
                `script-src ${cspConfig.scriptSrc.join(' ')}`,
                `style-src ${cspConfig.styleSrc.join(' ')}`,
                `connect-src ${cspConfig.connectSrc.join(' ')}`,
                `img-src ${cspConfig.imgSrc.join(' ')}`
            ].join('; ');

            const meta = document.createElement('meta');
            meta.httpEquiv = "Content-Security-Policy";
            meta.content = cspContent;
            document.head.appendChild(meta);
        })
        .catch(error => console.error('Failed to load CSP configuration:', error));
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
        document.getElementById("userNav").style.display = "none";
        document.getElementById("CMview").style.display = "none";
        document.getElementById("welcomeSection").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
    }
});

function sendOTP() {
    console.log("otp is sending")
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);

    fetch('http://localhost:5270/api/auth/sendOTP', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        hideAllSections();
        document.getElementById("sendOTPSection").style.display = "none";
        document.getElementById("enterOTPSection").style.display = "block";
        document.getElementById("noLogin").style.display = "block";
    })
    .catch(error => {
        console.error('Error sending verification code:', error);
        alert('Error sending verification code.');
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
        },
        error: function (xhr, status, error) {
            showModal('Incorrect verification code. Please double-check and try again.');
        }
    });
}

function logout() {
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
}
