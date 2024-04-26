const baseUrl = "http://localhost:5116/api/v1/reservationConfirmation";

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

function checkTokenExpiration(accessToken) {
    try {
        var response = fetch(`${baseUrl}/checkTokenExp`, {
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

// Function to handle sending a confirmation
async function sendConfirmation(reservationID) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/SendConfirmation?reservationID=${reservationID}`;
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ ReservationID: reservationID })
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Confirmation sent successfully:', data);
        return data;
    } catch (error) {
        console.error('Error sending confirmation:', error);
    }
}

// Function to handle resending a confirmation
async function resendConfirmation(reservationID) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }
    
    const url = `${baseUrl}/ResendConfirmation?reservationID=${reservationID}`;
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ reservationID })
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Confirmation resent successfully:', data);
        return data;
    } catch (error) {
        console.error('Error resending confirmation:', error);
    }
}

// Function to handle confirming a reservation
async function confirmReservation(reservationID, otp) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/ConfirmReservation?reservationID=${encodeURIComponent(reservationID)}&otp=${encodeURIComponent(otp)}`;
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Reservation confirmed successfully:', data);
        return data;
    } catch (error) {
        console.error('Error confirming reservation:', error);
    }
}

// Function to handle deleting a confirmation
async function deleteConfirmation(reservationID) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/DeleteConfirmation?reservationID=${reservationID}`;
    try {
        const response = await fetch(url, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        console.log('Confirmation deleted successfully:', data);
        return data;
    } catch (error) {
        console.error('Error deleting confirmation:', error);
    }
}

// Function to handle canceling a confirmation
async function cancelConfirmation(hashedUsername, reservationID) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/CancelConfirmation?hashedUsername=${encodeURIComponent(hashedUsername)}&reservationID=${encodeURIComponent(reservationID)}`;
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ hashedUsername, reservationID })
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Confirmation cancelled successfully:', data);
        return data;
    } catch (error) {
        console.error('Error canceling confirmation:', error);
    }
}


// Function to handle listing all confirmations for a user
async function listConfirmations() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/ListConfirmations?hashedUsername=${username}`;
    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username })
        });
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        const detailsContainer = document.getElementById('reservation-details');
        detailsContainer.innerHTML = ''; // Clear previous entries

        data.forEach(reservation => {
            const div = document.createElement('div');
            div.className = 'reservation-entry'; // This class should have corresponding styles in style.css
            div.innerHTML = `
                <h3>Reservation ID: ${reservation.reservationID}</h3>
                <p>Start Time: ${new Date(reservation.reservationStartTime).toLocaleString()}</p>
                <p>End Time: ${new Date(reservation.reservationEndTime).toLocaleString()}</p>
                <div class="button-group">
                    <button onclick="deleteConfirmation(${reservation.reservationID})">Delete Confirmation</button>
                    <button onclick="cancelConfirmation('${username}', ${reservation.reservationID})">Cancel Confirmation</button>
                </div>
            `;
            detailsContainer.appendChild(div);
        });

        console.log('List of confirmations retrieved successfully:', data);
        return data;
    } catch (error) {
        console.error('Error listing confirmations:', error);
    }
}

// Function to display reservation details
function displayReservationDetails(reservation) {
    // Display reservation details in the reservation-details container
    const reservationDetails = document.getElementById('reservation-details');
    reservationDetails.innerHTML = `
        <h3>${reservation.companyName}</h3>
        <p>Company ID: ${reservation.companyID}</p>
        <p>FloorPlan ID: ${reservation.floorPlanID}</p>
        <p>Space ID: ${reservation.spaceID}</p>
        <p>Start Time: ${reservation.startTime}</p>
        <p>End Time: ${reservation.endTime}</p>
        <img src="path_to_floor_plan_image" alt="Floor Plan">
        <button id="cancelConfirmationBtn">Cancel Confirmation</button>
        <button id="deleteConfirmationBtn">Delete Confirmation</button>
    `;

    const cancelConfirmationBtn = document.getElementById('cancelConfirmationBtn');
    cancelConfirmationBtn.addEventListener('click', () => openDialogCancel(reservation));

    const deleteConfirmationBtn = document.getElementById('deleteConfirmationBtn');
    deleteConfirmationBtn.addEventListener('click', () => openDialogDelete(reservation));
}

function openDialogCancel(reservation) {
    var reservationId = reservation.reservationID;
    var username = reservation.userHash;

    // Create modal elements
    const modal = document.createElement('div');
    modal.id = 'confirmModal';
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');

    const closeBtn = document.createElement('span');
    closeBtn.classList.add('close-button');
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = closeLeaveModal;

    const message = document.createElement('p');
    message.innerText = 'Are you sure you want to cancel the confirmation for:';

    const reservationIdText = document.createElement('p');
    spaceIdText.innerHTML = `Reservation: <span id="reservationId">${reservationId}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => cancelConfirmation(username, reservationId);

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.onclick = closeLeaveModal;

    // Append elements
    modalContent.appendChild(closeBtn);
    modalContent.appendChild(message);
    modalContent.appendChild(reservationIdText);
    modalContent.appendChild(yesBtn);
    modalContent.appendChild(noBtn);

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    // Show the modal
    modal.style.display = 'block';
}

function openDialogDelete(reservation) {
    var reservationId = reservation.reservationID;

    // Create modal elements
    const modal = document.createElement('div');
    modal.id = 'confirmModal';
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');

    const closeBtn = document.createElement('span');
    closeBtn.classList.add('close-button');
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = closeLeaveModal;

    const message = document.createElement('p');
    message.innerText = 'You cannot reconfirm the reservation once it is deleted. Are you sure you want to delete the confirmation for:';

    const reservationIdText = document.createElement('p');
    spaceIdText.innerHTML = `Reservation: <span id="reservationId">${reservationId}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => deleteConfirmation(reservationId);

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.onclick = closeLeaveModal;

    // Append elements
    modalContent.appendChild(closeBtn);
    modalContent.appendChild(message);
    modalContent.appendChild(reservationIdText);
    modalContent.appendChild(yesBtn);
    modalContent.appendChild(noBtn);

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    // Show the modal
    modal.style.display = 'block';
}

function closeLeaveModal() {
    console.log('Close modal function called');
    const modal = document.getElementById('confirmModal');
    if (modal && modal.style) {
        console.log('Modal element found in DOM');
        modal.style.display = 'none';
    } else {
        console.log('Modal element not found in DOM');
    }
}

function toggleButtons() {
    const buttonsContainer = document.getElementById('buttons-container');
    if (buttonsContainer.style.display === "none") {
        buttonsContainer.style.display = "block";
        // Optionally, also call listConfirmations here if you want to load data
        listConfirmations('hashedUsername1');
    } else {
        buttonsContainer.style.display = "none";
    }
}

////// mock reservations test/////
const mockReservations = [
    {
        reservationID: 1,
        companyID: 1,
        floorPlanID: 101,
        spaceID: 'A1',
        reservationStartTime: '2024-04-20T09:00:00',
        reservationEndTime: '2024-04-20T17:00:00',
        status: 'Active',
        userHash: 'hashedUsername1'
    },
    {
        reservationID: 2,
        companyID: 1,
        floorPlanID: 102,
        spaceID: 'A2',
        reservationStartTime: '2024-04-21T10:00:00',
        reservationEndTime: '2024-04-21T18:00:00',
        status: 'Active',
        userHash: 'hashedUsername2'
    },
    {
        reservationID: 3,
        companyID: 2,
        floorPlanID: 201,
        spaceID: 'B1',
        reservationStartTime: '2024-04-22T08:00:00',
        reservationEndTime: '2024-04-22T16:00:00',
        status: 'Active',
        userHash: 'hashedUsername3'
    }
];

function displayReservations() {
    const container = document.getElementById('reservation-details');
    container.innerHTML = ''; // Clear existing entries

    mockReservations.forEach(res => {
        const div = document.createElement('div');
        div.className = 'reservation-entry';
        div.innerHTML = `
        <div class="container">
            <div class="reservation-details-container content-container">
                <h3>Reservation ID: ${res.reservationID}</h3>
                <p>Start Time: ${res.reservationStartTime}</p>
                <p>End Time: ${res.reservationEndTime}</p>
                <p>Status: ${res.status}</p>
                <div class="button-container">
                    <button onclick="cancelConfirmation(${res.reservationID})">Cancel</button>
                    <button onclick="deleteConfirmation(${res.reservationID})">Delete</button>
                </div>
                <div id="reservation-details">
                </div>
            </div>
        </div>

        `;
        container.appendChild(div);
    });
}

// Dummy function for cancelling confirmation
function cancelConfirmation(reservationID) {
    alert(`Confirmation cancelled for reservation ID ${reservationID}`);
    // Update the display by filtering out the cancelled reservation
    const index = mockReservations.findIndex(res => res.reservationID === reservationID);
    if (index > -1) {
        mockReservations.splice(index, 1);
        displayReservations(); // Refresh the list
    }
}

// Dummy function for deleting confirmation
function deleteConfirmation(reservationID) {
    alert(`Confirmation deleted for reservation ID ${reservationID}`);
    // Update the display by filtering out the deleted reservation
    const index = mockReservations.findIndex(res => res.reservationID === reservationID);
    if (index > -1) {
        mockReservations.splice(index, 1);
        displayReservations(); // Refresh the list
    }
}

// Load and display reservations when the page loads or a button is clicked
//window.onload = displayReservations;

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('initConfirmationsButton').addEventListener('click', () => {
        displayWaitlistedReservations();
    });
});
