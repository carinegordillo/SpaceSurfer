if (!appConfig) {
    console.error('Configuration is not loaded!');
    return;
}
const loginUrl = appConfig.api.Login;  
const recoveryUrl = appConfig.api.AccountRecovery;  
document.addEventListener('click', function (event) {
    // Check if the clicked element has the class 'recover-link'
    if (event.target && event.target.classList.contains('recover-link')) {
        // Prevent default link behavior
        event.preventDefault();
        // Execute your function to show the recovery form
        showRecoveryForm();
    }
});

function showRecoveryForm() {
    window.alert("in here")
    // Hide the OTP section and show the recovery form section
    document.getElementById("accountRecoverySection").style.display = "block";
    document.getElementById('sendOTPSection').style.display = 'none';

    window.alert("Recovery clicked");

    const recoveryForm = document.getElementById('recoveryForm');
    recoveryForm.addEventListener('submit', sendRecoveryRequest);
}


function createAndShowModal(message) {
    const modal = document.createElement('div');
    modal.id = 'dynamicModal';
    modal.className = 'modal'; 
    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';

    const closeButton = document.createElement('button');
    closeButton.textContent = 'OK';
    closeButton.className = 'close-button';
    closeButton.onclick = function() {
        modal.style.display = 'none';
        document.body.removeChild(modal);
    };

    const resultMessage = document.createElement('p');
    resultMessage.textContent = message;

    modalContent.appendChild(resultMessage);
    modalContent.appendChild(closeButton);
    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    modal.style.display = 'flex'; 
}

// Function to send OTP based on user email
function sendRecoveryOTP() {
    var userIdentity = document.getElementById("userRecoveryIdentity").value;

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
        alert('OTP sent. Check your email for the verification code.');
        document.getElementById("accountRecoverySection").style.display = "none";
        document.getElementById("enterAccountRecoveryOTPSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error sending verification code.');
    });
}

// Function to authenticate the user with the provided OTP
function authenticateRecoveryUser() {
    var otp = document.getElementById("RecoveryOtp").value;
    var userIdentity = document.getElementById("userRecoveryIdentity").value;

    fetch(`${loginUrl}/api/auth/authenticate`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity, proof: otp })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        alert("OTP verified successfully. Please provide additional information.");
        document.getElementById("enterAccountRecoveryOTPSection").style.display = "none";
        document.getElementById("additionalInfoSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Invalid OTP. Please try again.');
    });
}

// Function to submit the final recovery request
function sendRecoveryRequest(event) {
    event.preventDefault();

    var email = document.getElementById("userRecoveryIdentity").value;
    var additionalInformation = document.getElementById("additionalInformation").value;

    var formData = new URLSearchParams({
        email: email,
        additionalInformation: additionalInformation
    });

    console.log(email);
    console.log(additionalInformation);

    fetch(`${recoveryUrl}/api/requestRecovery/sendRecoveryRequest`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        alert('Recovery request sent successfully. Please wait for admin to approve your request.');
        document.getElementById("additionalInfoSection").style.display = "none";
        document.getElementById("accountRecoverySection").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error sending recovery request. Try again later');
    });
}




// Function to initialize user requests and add event listeners to filter buttons
let allRequests = [];

async function initUserRequests() {
    document.getElementById("userRequestsView").style.display = "block";

    console.log("get userReuests clicked  clicked");
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
    document.getElementById("accountRecoverySection").style.display = "none";

    

    document.getElementById('approvalForm').addEventListener('submit', function (e) {
        e.preventDefault();
        const selectedRequests = document.querySelectorAll('.request-card input[type="checkbox"]:checked');
        const approvedUserHashes = [];
        const deniedUserHashes = [];
        const deleteUserHashes =[];
        
        selectedRequests.forEach(checkbox => {
            if (checkbox.dataset.action === 'approve') {
                approvedUserHashes.push(checkbox.dataset.userHash);
            } else if (checkbox.dataset.action === 'deny') {
                deniedUserHashes.push(checkbox.dataset.userHash);
            }
            else if (checkbox.dataset.action === 'delete') {
                deleteUserHashes.push(checkbox.dataset.userHash);
            }
        });

        document.getElementById('approveRequestsBtn').addEventListener('click', function(event) {
            approveRequestsByUserHash(approvedUserHashes);
        });

        document.getElementById('denyRequestsBtn').addEventListener('click', function(event) {
            denyRequestsByUserHash(deniedUserHashes);
        });
        document.getElementById('deleteRequestsBtn').addEventListener('click', function(event) {
            console.log(deleteUserHashes)
            deleteRequestsByUserHash(deleteUserHashes);
        });
        
        
        
        
    });

    document.getElementById('loadRequestsButton').addEventListener('click', fetchUserRequests);

    await fetchUserRequests();
}

document.addEventListener('click', function (event) {
    const target = event.target;

    if (target.id === 'showPending') {
        event.preventDefault();
        event.stopPropagation();
        filterRequestsByStatus('Pending');
    } else if (target.id === 'showApproved') {
        event.preventDefault();
        event.stopPropagation();
        filterRequestsByStatus('accepted');
    } else if (target.id === 'showDenied') {
        event.preventDefault();
        event.stopPropagation();
        filterRequestsByStatus('denied');
    }
});

// Function to fetch user requests
async function fetchUserRequests() {
    const accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    try {
        const response = await fetch(`${recoveryUrl}/api/requestRecovery/getAllRequests`);
        const data = await response.json();
        window.allRequests = data; // Store all requests globally to enable filtering
        allRequests = data;
        renderRequests(data);
    } catch (error) {
        console.error('Error fetching user requests:', error);

        renderRequests([]);
    }
}

function renderRequests(data) {
    const container = document.getElementById('pendingRequests');
    container.innerHTML = '';

    data.forEach(request => {
        const card = document.createElement('div');
        card.className = 'request-card';

        let actions = '';
        let resolveDateText = '';
        
        if (request.status.trim().toLowerCase() === 'pending') {
            actions = `
                <label>
                    <input type="checkbox" data-user-hash="${request.userHash}" data-action="approve"> Approve
                </label>
                <label>
                    <input type="checkbox" data-user-hash="${request.userHash}" data-action="deny"> Deny
                </label>
                
            `;
        } else {
            // Include the resolved date for resolved requests and add a delete checkbox
            resolveDateText = `
                <p>Resolved Date: ${new Date(request.resolveDate).toLocaleString()}</p>
                <label>
                    <input type="checkbox" data-user-hash="${request.userHash}" data-action="delete"> Delete
                </label>
            `;
        }

        card.innerHTML = `
            <h4>User Hash: ${request.userHash}</h4>
            <p>Request Date: ${new Date(request.requestDate).toLocaleString()}</p>
            <p>Status: ${request.status}</p>
            <p>Request Type: ${request.requestType}</p>
            <p>Additional Info: ${request.additionalInformation || 'None'}</p>
            ${resolveDateText}
            ${actions}
        `;
        container.appendChild(card);
    });
}

function deleteRequestsByUserHash(userHashes) {
    window.alert("deleting request")
    // if (userHashes.length === 0) return; // Skip if no requests to delete
    // console.log('Deleting requests for users:', userHashes);

    // fetch('http://localhost:5176/api/requestRecovery/deleteRequests', {
    //     method: 'POST',
    //     headers: { 'Content-Type': 'application/json' },
    //     body: JSON.stringify(userHashes),
    // })
    // .then(response => {
    //     if (!response.ok) throw new Error('Network response was not ok');
    //     return response.json();
    // })
    // .then(data => {
    //     console.log('Successfully deleted requests:', data);
    //     fetchUserRequests(); // Refresh the data after deletion
    // })
    // .catch(error => console.error('Error deleting requests:', error));
}

function filterRequestsByStatus(status) {
    console.log("Filtering by status:", status);

    const filteredRequests = window.allRequests.filter(request => {
        return status === 'all' || request.status.trim().toLowerCase() === status.toLowerCase();
    });

    console.log(filteredRequests); 
    renderRequests(filteredRequests); 
}







function approveRequestsByUserHash(userHashes) {
    if (userHashes.length === 0) return; 
    console.log('Approving requests for users:', userHashes);

    fetch(`${recoveryUrl}/api/requestRecovery/acceptRequests`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) throw new Error('Network response was not ok');
        return response.json();
    })
    .then(data => {
        console.log('Successfully approved requests:', data);
        fetchUserRequests(); // Refresh the data after approval
    })
    .catch(error => console.error('Error approving requests:', error));
}

// Function to deny user requests
function denyRequestsByUserHash(userHashes) {
    if (userHashes.length === 0) return; // Skip if no requests to deny
    console.log('Denying requests for users:', userHashes);

    fetch(`${recoveryUrl}/api/requestRecovery/denyRequests`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) throw new Error('Network response was not ok');
        return response.json();
    })
    .then(data => {
        console.log('Successfully denied requests:', data);
        fetchUserRequests(); // Refresh the data after denial
    })
    .catch(error => console.error('Error denying requests:', error));
}

document.getElementById('deactivateUserBtn').addEventListener('click', function() {
    var userHash = document.getElementById('userHashInput').value.trim();
    console.log(userHash)
    if (userHash) {
        deactivateUserAccount(userHash);
    } else {
        alert('Please enter a valid user hash.');
    }
});

function deactivateUserAccount(userHash) {
    fetch(`${recoveryUrl}/api/requestRecovery/disableAccount`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userHash)  // Send the userHash as a JSON string
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error, status = ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        alert('User deactivation ' + (data.success ? 'successful' : 'failed') + ': ' + data.message);
        fetchUserRequests(); // Optionally refresh the list if needed
    })
    .catch(error => {
        console.error('Error disabling the user account:', error);
        alert('Error disabling the user account. Please try again.');
    });
}

function deleteRequestsByUserHash(userHashes) {
    if (userHashes.length === 0) return; 
    console.log('Deleting Completed User Requests:', userHashes);

    fetch(`${recoveryUrl}/api/requestRecovery/deleteRequestByUserHash`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) throw new Error('Network response was not ok');
        return response.json();
    })
    .then(data => {
        window.alert("Successfully Deleted User Requests")
        console.log('Successfully deleted requests:', data);
        fetchUserRequests(); // Refresh the data after approval
    })
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
    document.getElementById('userProfileView').style.display = 'none';
    document.getElementById("accountRecoverySection").style.display = "none";
    document.getElementById("userRequestsView").style.display = "none";

}