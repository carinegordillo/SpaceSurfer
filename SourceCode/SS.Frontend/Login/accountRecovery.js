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
    hideAllSections();
    // Hide the OTP section and show the recovery form section
    document.getElementById("accountRecoverySection").style.display = "block";
    document.getElementById('noLogin').style.display = 'block';
    document.getElementById('sendOTPSection').style.display = 'none';
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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const loginUrl = appConfig.api.Login;  
    const recoveryUrl = appConfig.api.AccountRecovery;  
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
        showModal('OTP sent. Check your email for the verification code.');
        document.getElementById("accountRecoverySection").style.display = "none";
        document.getElementById("enterAccountRecoveryOTPSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error sending verification code.');
    });
}

function showModal(message, isSuccess = true) {
    var modal = document.getElementById('modal');
    var modalMessage = document.getElementById('modal-message');
    modalMessage.textContent = message;
    modal.style.display = 'block';

    var closeButton = document.querySelector('.close-button');
    closeButton.onclick = function() {
        modal.style.display = 'none';
    };
}


function authenticateRecoveryUser() {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const loginUrl = appConfig.api.Login;  
    const recoveryUrl = appConfig.api.AccountRecovery;  
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
        showModal("OTP verified successfully. Please provide additional information.");
        document.getElementById("enterAccountRecoveryOTPSection").style.display = "none";
        document.getElementById("additionalInfoSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Invalid OTP. Please try again.');
    });
}


function sendRecoveryRequest(event) {
    event.preventDefault();
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const loginUrl = appConfig.api.Login;  
    const recoveryUrl = appConfig.api.AccountRecovery;  

    var email = document.getElementById("userRecoveryIdentity").value;
    var additionalInformation = document.getElementById("additionalInformation").value;
    if (additionalInformation.length > 49) {
        showModal('Additional Information must not exceed 49 characters.');
        return; 
    }

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
        showModal('Recovery request sent successfully. Please wait for admin to approve your request.');
        document.getElementById("additionalInfoSection").style.display = "none";
        document.getElementById("accountRecoverySection").style.display = "none";
        document.getElementById("noLogin").style.display = "block";
        document.getElementById("sendOTPSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error sending recovery request. Try again later');
    });
}



// get the user acccount

// async function fetchUserAccount() {
//     const idToken = sessionStorage.getItem('idToken');
    
//         if (!idToken) {
//             console.error('idToken not found in sessionStorage');
//             return;
//         }
//         const parsedIdToken = JSON.parse(idToken);

//         // Log parsed object for debugging
//         console.log('Parsed idToken:', parsedIdToken);

//         if (!parsedIdToken || !parsedIdToken.Username) {
//             console.error('Parsed idToken does not have Username');
//             return;
//         }

//         const email = parsedIdToken.Username;

//     try {
//         const response = await fetch(`http://localhost:5176/api/requestRecovery/getUserAccountDetails?email=${encodeURIComponent(email)}`);
//         const data = await response.json();
//         console.log(data)
//         return data || null;
//     } catch (error) {
//         console.error('Error fetching user Account:', error);
//         return null;
//     }
// }

//if there is a userAcccount.CompanyID

//add the comapny intot he list of reservable places




var allRequests = [];

async function initUserRequests() {
    hideAllSections();
    document.getElementById("userRequestsView").style.display = "block";

    console.log("get userReuests clicked  clicked");
    startTimer('User Request');

    

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
    document.getElementById('filterByUsernameBtn').addEventListener('click', function() {
        const username = document.getElementById('usernameInput').value.trim();
        filterRequestsByUsername(username);
    });


    await fetchUserRequests();
}

// Function to filter requests by username
function filterRequestsByUsername(username) {
    if (!username) {
        console.log("No username entered, displaying all requests");
        return;
    }

    console.log("Filtering by username:", username);

    const filteredRequests = window.allRequests.filter(request => {
        return request.userName.toLowerCase().includes(username.toLowerCase());
    });

    console.log(filteredRequests);
    renderRequests(filteredRequests);
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


async function fetchUserRequests() {
    console.log("Fetching user requests");
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }

    const recoveryUrl = appConfig.api.AdminAccountRecovery;
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
    try {
        const response = await fetch(`${recoveryUrl}/api/adminAccountRecovery/getAllRequests`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${accessToken}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        allRequests = data;
        console.log('Received user requests:', data);
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
            <h4>User Name: ${request.userName}</h4>
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


function filterRequestsByStatus(status) {
    console.log("Filtering by status:", status);

    const filteredRequests = window.allRequests.filter(request => {
        return status === 'all' || request.status.trim().toLowerCase() === status.toLowerCase();
    });

    console.log(filteredRequests); 
    renderRequests(filteredRequests); 
}







function approveRequestsByUserHash(userHashes) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const recoveryUrl = appConfig.api.AdminAccountRecovery;  
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
    

    if (userHashes.length === 0) {
        console.warn('No user hashes provided for approval.');
        return;
    }

    console.log('Approving requests for users:', userHashes);

    fetch(`${recoveryUrl}/api/adminAccountRecovery/acceptRequests`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw new Error(`Error: ${response.status} ${errorData.message || 'Unknown error'}`);
            });
        }
        return response.json();
    })
    .then(data => {
        showModal("Successfully approved requests");
        console.log('Successfully approved requests:', data);
        fetchUserRequests(); // Refresh the data after approval
    })
    .catch(error => {
        console.error('Error approving requests:', error);
        showModal(`Failed to approve requests: ${error.message}`);
    });
}


// Function to deny user requests
function denyRequestsByUserHash(userHashes) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }

    const recoveryUrl = appConfig.api.AdminAccountRecovery;  
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

    if (userHashes.length === 0) {
        console.warn('No user hashes provided for denial.');
        return;
    }

    console.log('Denying requests for users:', userHashes);

    fetch(`${recoveryUrl}/api/adminAccountRecovery/denyRequests`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw new Error(`Error: ${response.status} ${errorData.message || 'Unknown error'}`);
            });
        }
        return response.json();
    })
    .then(data => {

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        showModal("Successfully denied requests");
        console.log('Successfully denied requests:', data);
        fetchUserRequests(); // Refresh the data after denial
    })
    .catch(error => {
        console.error('Error denying requests:', error);
        showModal(`Failed to deny requests: ${error.message}`);
    });
}


document.getElementById('deactivateUserBtn').addEventListener('click', function() {
    var userHash = document.getElementById('userHashInput').value.trim();
    console.log(userHash)
    if (userHash) {
        deactivateUserAccount(userHash);
    } else {
        showModal('Please enter a valid user hash.');
    }
});

// Function to deactivate user account
function deactivateUserAccount(userHash) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const recoveryUrl = appConfig.api.AdminAccountRecovery;  
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

    if (!userHash) {
        console.warn('User hash is not provided.');
        return;
    }

    console.log('Deactivating account for user:', userHash);

    fetch(`${recoveryUrl}/api/adminAccountRecovery/disableAccount`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(userHash)  // Send the userHash as a JSON string
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw new Error(`Error: ${response.status} ${errorData.message || 'Unknown error'}`);
            });
        }
        return response.json();
    })
    .then(data => {
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        showModal('User deactivation ' + (data.success ? 'successful' : 'failed') + ': ' + data.message);
        console.log('User deactivation result:', data);
        fetchUserRequests(); // Optionally refresh the list if needed
    })
    .catch(error => {
        console.error('Error disabling the user account:', error);
        showModal('Error disabling the user account. Please try again.');
    });
}


// Function to delete user requests
function deleteRequestsByUserHash(userHashes) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return "error";
    }
    
    const recoveryUrl = appConfig.api.AdminAccountRecovery;  
    
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

    if (userHashes.length === 0) {
        console.warn('No user hashes provided for deletion.');
        return;
    }

    console.log('Deleting Completed User Requests:', userHashes);

    fetch(`${recoveryUrl}/api/adminAccountRecovery/deleteRequestByUserHash`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(userHashes),
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(errorData => {
                throw new Error(`Error: ${response.status} ${errorData.message || 'Unknown error'}`);
            });
        }
        return response.json();
    })
    .then(data => {
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        showModal("Successfully Deleted User Requests");
        console.log('Successfully deleted requests:', data);
        fetchUserRequests(); // Refresh the data after deletion
    })
    .catch(error => {
        console.error('Error deleting user requests:', error);
        showModal('Error deleting user requests. Please try again.');
    });
}
