let viewStartTimer = null;
let appConfig = null;

function loadConfig() {
    fetch('csp-config.json')  
        .then(response => response.json())
        .then(config => {
            appConfig = config;  
            console.log('Configuration loaded successfully.');
        })
        .catch(error => {
            console.error('Failed to load configuration:', error);
        });
}
document.addEventListener('DOMContentLoaded', loadConfig);

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
        var userEmail = document.getElementById("userIdentity").value;
        var role = sessionStorage.getItem('role');
        hideAllSections();
        document.getElementById("homepageGen").style.display = "block";
        document.getElementById("userNav").style.display = "block";
        manageUserViews(role, userEmail);
        document.getElementById("welcomeSection").style.display = "block";
        document.getElementById("identity").textContent = `Logged in as: ${sessionStorage.getItem('userIdentity')}`;

    } else {
        // No valid token, show login
        hideAllSections();
        document.getElementById("homepageGen").style.display = "none";
        document.getElementById("userNav").style.display = "none";
        document.getElementById("CMview").style.display = "none";
        document.getElementById("FMview").style.display = "none";
        document.getElementById("SAview").style.display = "none";
        document.getElementById("welcomeSection").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
    }
});

function sendOTP() {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const loginUrl = appConfig.api.Login;  
    console.log("otp is sending")
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);

    fetch(`${loginUrl}/api/auth/sendOTP`, {
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

async function authenticateUser() {
    var otp = document.getElementById("otp").value;
    var userIdentity = document.getElementById("userIdentity").value;
    sessionStorage.setItem('userIdentity', userIdentity);
    var userIsActive = false;
    const loginUrl = appConfig.api.Login; 
    fetch(`${loginUrl}/api/auth/authenticate`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity, proof: otp })
    })
    .then(response => response.json())
    .then(data => {
        var accessToken = data.accessToken;
        var idToken = data.idToken;
        sessionStorage.setItem('accessToken', accessToken);
        sessionStorage.setItem('idToken', idToken);
        document.getElementById("enterOTPSection").style.display = "none";
        document.getElementById("successResult").style.display = "none";
        return fetch(`${loginUrl}/api/auth/getRole`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: accessToken
        });
    })
    .then(response => response.text())
    .then(async role => {
        const accountInfo = await fetchUserAccount();

            if (accountInfo) {
                userIsActive = await getActivityStatus(accountInfo.isActive);
            }
            console.log(userIsActive)
            if(userIsActive){
                showModal("You're logged in!");
                sessionStorage.setItem('role', role);
                manageUserViews(role, userIdentity);
            }else{
                console.log(userIsActive)
                showModal('Sorry, your account is not active. Please submit a recovery request.');
                console.log("account is not active")
                logout()
            }
    })
    .catch(error => {
        showModal('Incorrect verification code. Please double-check and try again.');
        console.error('Error during authentication:', error);
    });
}
async function fetchUserAccount() {
    const idToken = sessionStorage.getItem('idToken');
    console.log("in fetch user account")
    
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
        const recoveryUrl = appConfig.api.AccountRecovery;  
        const response = await fetch(`${recoveryUrl}/api/requestRecovery/getUserAccountDetails?email=${encodeURIComponent(email)}`);
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

function manageUserViews(role, userIdentity) {
    hideAllSections();
    document.getElementById("homepageGen").style.display = "block";
    document.getElementById("userNav").style.display = "block";
    // sessionStorage.setItem('userIdentity', userIdentity);
    document.getElementById("identity").style.display = "block";
    document.getElementById("identity").textContent = `Logged in as: ${userIdentity}`;
    document.getElementById("welcomeSection").style.display = "block";

    if (role === "2") {
        document.getElementById("CMview").style.display = "block";
        document.getElementById("FMview").style.display = "none";
    } else if (role === "3") {
        document.getElementById("FMview").style.display = "block";
        document.getElementById("CMview").style.display = "none";
    } else if (role === "4" || role === "5") {
        document.getElementById("FMview").style.display = "none";
        document.getElementById("CMview").style.display = "none";
    } else if (role === "1") {
        document.getElementById("SAview").style.display = "block";
        document.getElementById("FMview").style.display = "none";
        document.getElementById("CMview").style.display = "none";
    }

    startTimer('Dashboard View');
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
    document.getElementById("SAview").style.display = "none";
    document.getElementById("homepageGen").style.display = "none";
}

function getLogin(){
    console.log("get login page clicked  clicked");
    hideAllSections();
    document.getElementById('sendOTPSection').style.display = 'block';
    document.getElementById('noLogin').style.display = 'block';
    startTimer('Login View');
}

function getAbout(){
    console.log("get about page clicked  clicked");
    hideAllSections();
    document.getElementById('UnAuthnAbout').style.display = 'block';
    document.getElementById('noLogin').style.display = 'block';
    startTimer('About View');
}

function getUserProfile(){
    console.log("get userprofile clicked  clicked");
    hideAllSections();
    document.getElementById('userProfileView').style.display = 'block';
    startTimer('User Profile View');
}


function spaceBookingCenterAccess() {
    hideAllSections();
    document.getElementById('spaceBookingView').style.display = 'block';
    startTimer('Space Booking Center View');
}

function registrationAccess() {
    hideAllSections();
    document.getElementById('Registration').style.display = 'block';
    document.getElementById("accountCreationForm").style.display = "block";
    document.getElementById("enterRegistrationOTPSection").style.display = "none";
    document.getElementById('noLogin').style.display = 'block';    
}

function taskHubAccess() {
    hideAllSections();
    document.getElementById('taskManagerView').style.display = 'block';
    startTimer('Task Manager Hub View');
}

function personalOverviewAccess() {
    hideAllSections();
    document.getElementById('personalOverviewCenter').style.display = 'block'; 
    startTimer('Personal Overview View');
}

function waitlistAccess() {
    hideAllSections();
    document.getElementById('waitlistView').style.display = 'block';
    startTimer('Waitlist View');
}

function spaceManagerAccess() {
    hideAllSections();
    document.getElementById('spaceManagerView').style.display = 'block';
    startTimer('Space Manager View');
}

function getHomePage() {
    hideAllSections();
    document.getElementById("welcomeSection").style.display = "block";
    startTimer('Home View');
}

function employeeSetupAccess() {
    hideAllSections();
    document.getElementById('employeeSetup').style.display = 'block';
    startTimer('Employee Setup View');
}

function confirmationAccess() {
    hideAllSections();
    document.getElementById('confirmationView').style.display = 'block';
    startTimer('Confirmation View');
}

function SOAccess() {
    hideAllSections();
    document.getElementById('systemObservability').style.display = 'block';
    startTimer('System Observability View');
}

function hideAllSections() {
    const currentView = sessionStorage.getItem('currentView');

    if (currentView)
    {
        endTimerAndInsertDuration(currentView);
    }

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
    document.getElementById("enterRegistrationOTPSection").style.display = "none";
}

function startTimer(viewName) {
    const viewStartTime = new Date(); 

    sessionStorage.setItem('viewStartTime', viewStartTime.getTime());
    sessionStorage.setItem('currentView', viewName)
}

function clearTimer() {

    sessionStorage.removeItem('viewStartTime');
    sessionStorage.removeItem('currentView');
}

function endTimerAndInsertDuration(viewName) {
    const viewStartTime = sessionStorage.getItem('viewStartTime');
    const currentView = sessionStorage.getItem('currentView');
    if (viewStartTime && currentView === viewName) {
        const durationInSeconds = Math.round((new Date() - new Date(parseInt(viewStartTime))) / 1000);
        fetchInsertViewDuration(viewName, durationInSeconds)
            .then(() => {
                console.log(`Inserted view duration for ${currentView}: ${durationInSeconds} seconds`);
            })
            .catch(error => {
                console.error(`Error inserting view duration for ${currentView}:`, error);
            });
    }
    clearTimer();
}


// Function to insert the view duration insertion data for each view
async function fetchInsertViewDuration(viewName, duration) {
    // Retrieve tokens from sessionStorage
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    // Check if tokens are present
    if (!idToken) {
        console.error('ID token not found.');
        return;
    }

    if (!username) {
        console.error('Username not found in token.');
        return;
    }

    if (!accessToken) {
        console.error('Access token not found.');
        return;
    }

    // Construct the URL with the provided view name and duration
    const url = `http://localhost:5295/api/v1/SystemObservability/ViewDurationInsertion?viewName=${viewName}&durationInSeconds=${duration}`;

    try {
        // Fetch data from the API
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        // Parse and return the response data
        const data = await response.json();

        // Check if there's any error in the response
        if (data.hasError) {
            throw new Error(`Error inserting the view duration: ${response.status}`);
        }

        return data;
    }
    catch (error) {
        // Catch and log any errors that occur during the process
        throw new Error(`Caught an error inserting the view duration: ${error.message}`);
    }
}