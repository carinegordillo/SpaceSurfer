
document.addEventListener('click', function(event) {
    var target = event.target;
    if (target.classList.contains('initProfileButton')) {


        var idToken = sessionStorage.getItem('idToken');
        var parsedIdToken = JSON.parse(idToken);
        var username = parsedIdToken.Username;

        fetchUserProfile(username).then(profile => {
            if (profile) {
                const profileContainer = document.querySelector('.userProfileContainer');
                profileContainer.appendChild(profile); 
            }
        }).catch(error => console.error("Failed to fetch or append profile:", error));
    }
});

async function fetchUserProfile(email) {

    var accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }
    fetch(`http://localhost:5048/api/profile/getUserProfile?email=${encodeURIComponent(email)}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            if(data && data.length > 0){
                displayUserProfile(data[0]);
            } else {
                console.error('User profile data is empty');
            }
        })
        .catch(error => console.error('Error fetching user profile:', error));
}

function displayUserProfile(userProfile) {
    const leftPanel = document.getElementById('leftPanel');
    const accountDetails = document.getElementById('accountDetails');
    const userNameSpan = document.getElementById('userName');
    
    if (userNameSpan) {
        userNameSpan.textContent = `Hi, ${userProfile.firstName}`;
    } else {
        console.error("Element #userName not found");
    }

    // Populate left panel with editable fields
    leftPanel.innerHTML = `
        <h2>Edit Profile</h2>
        <input id="firstName" value="${userProfile.firstName}" />
        <input id="lastName" value="${userProfile.lastName}" />
        <textarea id="about">${userProfile.about || "User biography not provided."}</textarea>
        <button onclick="saveProfileChanges()">Save Changes</button>
    `;

    // Display non-editable account details
    accountDetails.innerHTML = `
        <h2>Account Details</h2>
        <p>Email: ${userProfile.email}</p>
        <p>Backup Email: ${userProfile.backupEmail || "Not provided"}</p>
        <p>Role: ${userProfile.appRole || "User role not specified"}</p>
    `;
}

function toggleEdit() {
    console.log('Edit button clicked');
}

function logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";

    const existingModal = document.querySelector('.modal-content');
        if (existingModal) {
            existingModal.remove();
        }
    const modalBackdrop = document.querySelector('.modal-backdrop');
    if (modalBackdrop) {
        modalBackdrop.style.display = 'none';
    }

}

async function checkTokenExpiration(accessToken) {
    try {
        const response = await fetch('http://localhost:5005/api/v1/spaceBookingCenter/reservations/checkTokenExp', {
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