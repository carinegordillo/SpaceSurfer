

let isEditing = false;



function initProfile() {
    document.getElementById('userProfileView').style.display = 'block';

    document.getElementById("userRequestsView").style.display = "none";

    console.log("get userReuests clicked  clicked");

    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById("taskManagerView").style.display = "none";
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';
    document.getElementById('spaceBookingView').style.display = 'none';
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById("accountRecoverySection").style.display = "none";

    try {
        const idToken = sessionStorage.getItem('idToken');
    
        if (!idToken) {
            console.error('idToken not found in sessionStorage');
            return;
        }
        const accessToken = sessionStorage.getItem('accessToken');
        const isTokenValid = checkTokenExpiration(accessToken);
        if (!isTokenValid) {
            logout();
            return;
        }
        const parsedIdToken = JSON.parse(idToken);

        if (!parsedIdToken || !parsedIdToken.Username) {
            console.error('Parsed idToken does not have Username');
            return;
        }

        const username = parsedIdToken.Username;
        

        fetchUserProfile(username).then(profile => {
            if (profile) {
                displayUserProfile(profile);
            }
        }).catch(error => console.error("Failed to fetch or append profile:", error));
    } catch (error) {
        console.error('Error parsing idToken:', error);
    }
}




// Event listener for profile actions
document.addEventListener('click', function (event) {
    const target = event.target;

    if (target.id === 'editProfile') {
        event.preventDefault();
        event.stopPropagation();
        toggleEditProfile();
    } else if (target.id === 'saveChangesButton') {
        event.preventDefault();
        event.stopPropagation();
        saveProfileChanges();
    } else if (target.id === 'cancelChangesButton') {
        event.preventDefault();
        event.stopPropagation();
        cancelEditProfile();
    }
});

async function fetchUserProfile(email) {
    const accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = await checkTokenExpiration(accessToken);
    
    if (!isTokenValid) {
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const profileUrl = appConfig.api.UserProfile; 
    try {
        const response = await fetch(`${profileUrl}/api/profile/getUserProfile?email=${encodeURIComponent(email)}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${accessToken}`,
                'Content-Type': 'application/json'
            }
        })
        const data = await response.json();
        console.log('Received companies data:', data);
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        return data.length > 0 ? data[0] : null;
    } catch (error) {
        console.error('Error fetching user profile:', error);
        return null;
    }
}



function displayUserProfile(userProfile) {
    isEditing = false; // Set edit mode to false when displaying profile

    // Get email directly from sessionStorage without parsing it as JSON
    const email = sessionStorage.getItem('userIdentity');
    console.log('Email:', email);

    // Check for missing properties in userProfile
    if (!userProfile || typeof userProfile.firstName === 'undefined' || typeof userProfile.lastName === 'undefined') {
        console.error('User profile has missing properties:', userProfile);
        return;
    }

    // Find the panel elements
    const leftPanel = document.querySelector('.left-panel');
    const rightPanel = document.querySelector('.right-panel');

    if (!leftPanel || !rightPanel) {
        console.error('Panel elements not found in the DOM');
        return;
    }

    // Display profile details
    leftPanel.innerHTML = `
        <h2>Profile</h2>
        <p>Name: <span id="displayFirstName">${userProfile.firstName}</span> <span id="displayLastName">${userProfile.lastName}</span></p>
        <p><span id="displayAbout">${userProfile.about || " "}</span></p>
        <button id="editProfile">Edit Profile</button>
    `;

    // Display non-editable account details
    rightPanel.innerHTML = `
        <h2>Account Details</h2>
        <p>Email: ${email}</p>
        <p>Backup Email: ${userProfile.backupEmail || "Not provided"}</p>
        <p>Role: ${userProfile.appRole || "User role not specified"}</p>
    `;
}




// Toggle profile to editable mode
function toggleEditProfile() {
    isEditing = true; // Set edit mode to true

    const leftPanel = document.querySelector('.left-panel');

    // Change the displayed profile to an editable form
    leftPanel.innerHTML = `
    <form id="modifyUserProfileForm">
        <h2>Edit Profile</h2>
        <input id="firstName" value="${document.getElementById('displayFirstName').innerText}" />
        <input id="lastName" value="${document.getElementById('displayLastName').innerText}" />
        <p3 id="about">${document.getElementById('displayAbout').innerText}</p>
        <button type="submit" id="saveChangesButton">Save Changes</button>
        <button id="cancelChangesButton">Cancel</button>
        </form>
    `;

}

async function saveProfileChanges() {
    const updatedProfile = {
        username: JSON.parse(sessionStorage.getItem('idToken')).Username, // Parse idToken correctly
        firstname: document.getElementById('firstName').value,
        lastname: document.getElementById('lastName').value,
    };
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const profileUrl = appConfig.api.UserProfile; 

    const idToken = sessionStorage.getItem('idToken');
    
    if (!idToken) {
        console.error('idToken not found in sessionStorage');
        return;
    }
    const accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }
    const parsedIdToken = JSON.parse(idToken);

    if (!parsedIdToken || !parsedIdToken.Username) {
        console.error('Parsed idToken does not have Username');
        return;
    }

    const username = parsedIdToken.Username;

    try {
        const accessToken = sessionStorage.getItem('accessToken');
        const response = await fetch(`${profileUrl}/api/profile/updateUserProfile`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${accessToken}`
            },
            body: JSON.stringify(updatedProfile)
        });

        const result = await response.json(); // Correctly await the JSON parsing

        if (result.newToken) {
            accessToken = result.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (response.ok) {
            fetchUserProfile(updatedProfile.username).then(profile => {
                if (profile) {
                    displayUserProfile(profile);
                }
            }).catch(error => console.error("Failed to fetch or append profile:", error));
        } else {
            alert('Profile update failed: ' + result.message);
        }
    } catch (error) {
        console.error('Error updating profile:', error);
        alert('An error occurred while updating the profile');
    }
}

function cancelEditProfile() {
    isEditing = false; // Set edit mode to false
    initProfile();
}




async function checkTokenExpiration(accessToken) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const bookingUrl = appConfig.api.SpaceBookingCenter; 
    try {
        const response = await fetch(`${bookingUrl}/api/v1/spaceBookingCenter/reservations/checkTokenExp`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });
        return response.ok;
    } catch (error) {
        console.error('Error:', error);
        return false;
    }
}
