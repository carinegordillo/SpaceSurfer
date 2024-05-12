function initSpaceBookingCenter(){

    spaceBookingCenterAccess()
    getReservationCenter()
    initSidebar();
    const formContainer = document.querySelector('.space-form-container');
    if (formContainer) {
        formContainer.innerHTML = '';
        const reservationForm = createReservationForm();

        reservationForm.addEventListener('submit', function (event) {
            event.preventDefault();
            var accessToken = sessionStorage.getItem('accessToken');
            if (!accessToken) {
                console.error('Access token is not available.');
                return;
            }
            handleReservationCreationFormSubmit(event);

        });
        formContainer.appendChild(reservationForm);

    }

    var accessToken = sessionStorage.getItem('accessToken');




    fetchCompanies();
    
    document.getElementById('companiesList').addEventListener('click', function (event) {
        if (event.target && event.target.nodeName === "LI") {
            const companyID = event.target.getAttribute('data-company-id');
            fetchFloorPlans(companyID);
        }
    });
}

function hideAllSBCSections() {
    const sections = document.querySelectorAll('.reservation-center-container, .reservation-container');
    sections.forEach(section => section.style.display = 'none');
}


function initSidebar() {
    const sidebar = document.querySelector('.sidebar');
    sidebar.innerHTML = '';
    const buttons = [
        { id: 'loadReservationCenter', text: 'Reserve Now', onClickFunction: () => {
            hideAllSBCSections();
            document.querySelector('.reservation-center-container').style.display = 'flex';
            getReservationCenter();
        }},
        { id: 'loadReservationOverview', text: 'Your Reservations', onClickFunction: () => {
            hideAllSBCSections();
            document.querySelector('.reservation-container').style.display = 'block';
            getReservationOverview();
        }}
    ];

    buttons.forEach(({ id, text, onClickFunction }) => {
        const button = document.createElement('button');
        button.id = id;
        button.textContent = text;
        button.addEventListener('click', onClickFunction);
        sidebar.appendChild(button);
    });
}

// Function to fetch and display the reservation center
function getReservationCenter() {
    hideAllSBCSections();
    document.querySelector('.reservation-center-container').style.display = 'flex';
    const formContainer = document.querySelector('.space-form-container');
    if (formContainer) {
        formContainer.innerHTML = '';
        const reservationForm = createReservationForm();

        reservationForm.addEventListener('submit', function (event) {
            event.preventDefault();
            var accessToken = sessionStorage.getItem('accessToken');
            if (!accessToken) {
                console.error('Access token is not available.');
                return;
            }
            reservationForm.addEventListener('submit', handleReservationFormSubmit);

        });
        formContainer.appendChild(reservationForm);

    }
    fetchCompanies();
    document.getElementById('companiesList').addEventListener('click', function (event) {
        if (event.target && event.target.nodeName === "LI") {
            const companyID = event.target.getAttribute('data-company-id');
            fetchFloorPlans(companyID);
        }
    });
    document.getElementById('reservationForm').addEventListener('submit', handleReservationCreationFormSubmit);    
}
function getReservationOverview() {
    hideAllSBCSections();
    document.querySelector('.reservation-container').style.display = 'block';
    reservationOverviewButtons();
    const deleteFormContainer = document.querySelector('.space-form-container');
    if (deleteFormContainer) {
        deleteFormContainer.innerHTML = ''; // Clear previous content
        const deleteReservationForm = createDeleteReservationForm();
        deleteFormContainer.appendChild(deleteReservationForm);
    }
}


function createDeleteReservationForm() {
    const form = document.createElement('form');
    form.id = 'deleteReservationForm';
    form.innerHTML = `
        <div class="form-group">
            <label for="reservationIDToDelete">Reservation ID:</label>
            <input type="number" id="reservationIDToDelete" required>
        </div>
        <input type="submit" value="Delete Reservation">
    `;
    form.addEventListener('submit', handleDeleteReservationFormSubmit);
    return form;
}


async function handleDeleteReservationFormSubmit(event) {
    event.preventDefault();

    const reservationID = document.getElementById('reservationIDToDelete').value;
    var accessToken = sessionStorage.getItem('accessToken');

    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    var idToken = sessionStorage.getItem('idToken');

    var parsedIdToken = JSON.parse(idToken);
    var userHash = parsedIdToken.Username; 
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 

    try {
        const response = await fetch(`${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/DeleteReservation`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Accept': 'application/json',
            },
            body: JSON.stringify({ userHash, reservationID: parseInt(reservationID, 10) }),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const responseData = await response.json();
        if (responseData.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        if (responseData.hasError) {
            console.error(`Reservation deletion error: ${responseData.errorMessage}`);
            onError(`Reservation deletion error: ${responseData.errorMessage}`);
        } else {
            console.log('Reservation deleted successfully');
            onSuccess('Reservation deleted successfully!');
            
        }
    } catch (error) {
        console.error('Error deleting reservation:', error);
        onError(`Error deleting reservation: ${error}`);
    }
}


function getReservtionOverview() {
    reservationOverviewButtons();
    const deleteFormContainer = document.querySelector('.space-form-container'); // Adjust the selector as needed
    if (deleteFormContainer) {
        const deleteReservationForm = createDeleteReservationForm();
        deleteFormContainer.appendChild(deleteReservationForm);
    }
}






///RESERVATION OVERVIEW

function reservationOverviewButtons() {
    const content = document.querySelector('.reservation-buttons');
    content.innerHTML = ''; 

    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const buttons = [
        { id: 'loadActiveReservationsBtn', text: 'Active Reservations', onClickFunction: () => filterReservationsByStatus('Active') },
        { id: 'loadPassedReservationsBtn', text: 'Passed Reservations', onClickFunction: () => filterReservationsByStatus('Passed') },
        { id: 'loadCancelledReservationsBtn', text: 'Cancelled Reservations', onClickFunction: () => filterReservationsByStatus('Cancelled') },
        { id: 'loadAllReservationsBtn', text: 'All Reservations', onClickFunction: () => getUsersReservations(username, accessToken) }
    ];

    buttons.forEach(({ id, text, onClickFunction }) => {
        const button = document.createElement('button');
        button.id = id;
        button.textContent = text;
        button.addEventListener('click', onClickFunction);
        content.appendChild(button);
    });
}

function filterReservationsByStatus(statusFilter) {
    const container = document.querySelector('.reservation-list');
    const allReservations = container.querySelectorAll('.reservation-card');

    allReservations.forEach(card => {
        const statusElement = card.querySelector('.status-0, .status-1, .status-2');
        if (statusElement) {
            const statusText = statusElement.textContent.split(': ')[1];
            if (statusText === statusFilter || statusFilter === 'All') {
                card.style.display = ''; 
            } else {
                card.style.display = 'none'; 
            }
        }
    });
}




function getUsersActiveReservations(username) {

    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 
   
    const url = `${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/ListActiveReservations?userName=${encodeURIComponent(username)}`;

    fetch(url, {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken,
            'Accept': 'application/json',
        }
        
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        renderReservations(data, '.reservation-list');
        
    })
    .catch(error => {
        console.error('Error fetching reservations:', error);
        onError("Error fetching active reservations")
    });
}

function getUsersReservations(userName, accessToken) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 
    const url = `${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/ListReservations?userName=${encodeURIComponent(userName)}`;

    fetch(url, {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken,
            'Accept': 'application/json',
        }
        
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {

        renderReservations(data, '.reservation-list');
    })
    .catch(error => {
        console.error('Error fetching reservations:', error);
        onError("Error fetching reservations")
    });
}

function renderReservations(data, containerSelector) {
    const container = document.querySelector(containerSelector);
    container.innerHTML = ''; 

    data.forEach(reservation => {
    let statusText = '';
    let buttonsHtml = '';

    switch (reservation.status) {
        case 0: 
            statusText = 'Active';
            buttonsHtml = `
            <button class="modify-btn" data-reservation='${JSON.stringify(reservation)}'>Modify</button>
            <button class="cancel-btn" data-reservation='${JSON.stringify(reservation)}'>Cancel</button>
            <button class="confirm-btn" data-reservation='${JSON.stringify(reservation)}'>Confirm</button>
            <button class="resend-email-btn" data-reservation='${JSON.stringify(reservation)}'>Resend Email</button>
        `;
            break;
        case 1: 
            statusText = 'Passed';
            buttonsHtml = '';
            break;
        case 2: 
            statusText = 'Cancelled';
            buttonsHtml = '';
            break;
        default: 
            statusText = 'Unknown';
            buttonsHtml = '';
            break;
    }

    const cardHtml = `
        <div class="reservation-card">
            <div class="reservation-detail"><span class="reservation-title">Reservation ID:</span> ${reservation.reservationID}</div>
            <div class="reservation-detail"><span class="reservation-title">Company ID:</span> ${reservation.companyID}</div>
            <div class="reservation-detail"><span class="reservation-title">Floor Plan ID:</span> ${reservation.floorPlanID}</div>
            <div class="reservation-detail"><span class="reservation-title">Space ID:</span> ${reservation.spaceID}</div>
            <div class="reservation-detail"><span class="reservation-title">Start Time:</span> ${new Date(reservation.reservationStartTime).toLocaleString()}</div>
            <div class="reservation-detail"><span class="reservation-title">End Time:</span> ${new Date(reservation.reservationEndTime).toLocaleString()}</div>
            <div class="reservation-detail status-${reservation.status}"><span class="reservation-title">Status:</span> ${statusText}</div>
            ${buttonsHtml}
        </div>
    `;

        container.innerHTML += cardHtml;
    });
    attachEventListeners();
}

function attachEventListeners() {
    document.querySelectorAll('.modify-btn').forEach(button => {
        button.addEventListener('click', function () {
            const reservationData = JSON.parse(this.getAttribute('data-reservation'));
            showModifyModal(reservationData);
        });
    });
    document.querySelectorAll('.cancel-btn').forEach(button => {
        button.addEventListener('click', function () {
            const reservationData = JSON.parse(this.getAttribute('data-reservation'));
            showCancelModal(reservationData);
        });
    });
    document.querySelectorAll('.confirm-btn').forEach(button => {
        button.addEventListener('click', function() {
            const reservationString = this.getAttribute('data-reservation');
            console.log("Reservation string:", reservationString); // For debugging
            try {
                const reservationData = JSON.parse(reservationString);
                console.log("Parsed reservation data:", reservationData); // For debugging
                showConfirmationModal(reservationData);
            } catch (e) {
                console.error("Parsing error:", e);
            }
        });
    });
    document.querySelectorAll('.resend-email-btn').forEach(button => {
        button.addEventListener('click', function() {
            const reservationData = JSON.parse(this.getAttribute('data-reservation'));
            resendEmail(reservationData);
        });
    });
}

function showModifyModal(reservation) {
    // Remove existing modal if present
    const existingModal = document.querySelector('.modal-content');
    if (existingModal) {
        existingModal.remove();
    }

    // Extract start and end time in the required format
    const startTime = toLocalTime(reservation.reservationStartTime);
    const endTime = toLocalTime(reservation.reservationEndTime);

    // Create modal backdrop
    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop';
    backdrop.style.position = 'fixed';
    backdrop.style.top = '0';
    backdrop.style.left = '0';
    backdrop.style.width = '100%';
    backdrop.style.height = '100%';
    backdrop.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
    backdrop.style.display = 'flex';
    backdrop.style.justifyContent = 'center';
    backdrop.style.alignItems = 'center';
    backdrop.addEventListener('click', event => {
        if (event.target === backdrop) {
            backdrop.remove();
        }
    });

    // Create modal content
    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.style.backgroundColor = 'white';
    modalContent.style.padding = '20px';
    modalContent.style.borderRadius = '8px';
    modalContent.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.1)';
    modalContent.style.maxWidth = '500px';
    modalContent.style.width = '100%';
    modalContent.style.position = 'relative';

    // Modal content HTML
    modalContent.innerHTML = `
        <span class="close-button" style="position: absolute; top: 10px; right: 10px; cursor: pointer;">&times;</span>
        <h2>Modify Reservation</h2>
        <form id="modifyReservationForm" data-reservation-id="${reservation.reservationID}">
            <h2>Reservation ID: ${reservation.reservationID}</h2>
            <h2>Company ID: ${reservation.companyID}</h2>
            <h2>FloorPlan ID: ${reservation.floorPlanID}</h2>
            <h2>Space ID: ${reservation.spaceID}</h2>
            <label for="newStartTime">New Start Time:</label>
            <input type="datetime-local" id="newStartTime" value="${startTime}" required>
            <label for="newEndTime">New End Time:</label>
            <input type="datetime-local" id="newEndTime" value="${endTime}" required>
            <button type="submit">Submit</button>
        </form>
    `;

    // Append modal content to backdrop
    backdrop.appendChild(modalContent);

    // Append backdrop to body
    document.body.appendChild(backdrop);

    // Close button event listener
    document.querySelector('.close-button').addEventListener('click', () => {
        backdrop.remove();
    });

    // Form submit event listener
    document.getElementById('modifyReservationForm').addEventListener('submit', function (event) {
        event.preventDefault();
        submitModification(reservation);
    });
}


function toLocalTime(isoString) {
    if (!isoString) return '';
    const date = new Date(isoString);
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60 * 1000);
    return localDate.toISOString().slice(0, 16);
}


async function submitModification(reservation) {
    const accessToken = sessionStorage.getItem('accessToken');
    const idToken = sessionStorage.getItem('idToken');
    const parsedIdToken = JSON.parse(idToken);
    const username = parsedIdToken.Username;

    const tokenExpired = await checkTokenExpiration(accessToken);
    if (!tokenExpired) {
        window.alert("Token expired");
        logout();
        return;
    }

    const form = document.getElementById('modifyReservationForm');
    const newStartTime = form.querySelector('#newStartTime').value;
    const newEndTime = form.querySelector('#newEndTime').value;

    const modificationData = {
        reservationID: reservation.reservationID,
        companyID: reservation.companyID,
        floorPlanID: reservation.floorPlanID,
        spaceID: reservation.spaceID,
        reservationStartTime: newStartTime,
        reservationEndTime: newEndTime,
        status: reservation.status,
        userHash: username
    };

    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 

    try {
        const response = await fetch(`${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/UpdateReservation`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(modificationData),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const responseData = await response.json();

        if (responseData.newToken) {
            accessToken = responseData.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (responseData.hasError) {
            console.log(`Reservation error: ${responseData.errorMessage}`);
            onError(`Reservation error: ${responseData.errorMessage}`);
        } else {
            console.log('Reservation modified successfully');
            onSuccess('Reservation modified successfully!');
        }
    } catch (error) {
        console.error('Error modifying reservation:', error);
        onError(`Error modifying reservation: ${error}`);
    }
}

/////////////////////////////////////////

function showCancelModal(reservation) {
    // Remove existing modal if present
    const existingModal = document.querySelector('.modal-content');
    if (existingModal) {
        existingModal.remove();
    }

    // Create modal backdrop
    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop';
    backdrop.style.position = 'fixed';
    backdrop.style.top = '0';
    backdrop.style.left = '0';
    backdrop.style.width = '100%';
    backdrop.style.height = '100%';
    backdrop.style.backgroundColor = 'rgba(0, 0, 0, 0.5)';
    backdrop.style.display = 'flex';
    backdrop.style.justifyContent = 'center';
    backdrop.style.alignItems = 'center';

    // Create modal content
    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';
    modalContent.style.backgroundColor = 'white';
    modalContent.style.padding = '20px';
    modalContent.style.borderRadius = '8px';
    modalContent.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.1)';
    modalContent.style.maxWidth = '500px';
    modalContent.style.width = '100%';
    modalContent.style.position = 'relative';

    modalContent.innerHTML = `
        <span class="close-button" style="position: absolute; top: 10px; right: 10px; cursor: pointer;">&times;</span>
        <h2>Cancel Reservation</h2>
        <h3>Are you sure you want to cancel this reservation?</h3>
        <h4>Reservation ID: ${reservation.reservationID}</h4>
        <h4>Company ID: ${reservation.companyID}</h4>
        <h4>FloorPlan ID: ${reservation.floorPlanID}</h4>
        <h4>Space ID: ${reservation.spaceID}</h4>
        <h4>Start Time: ${new Date(reservation.reservationStartTime).toLocaleString()}</h4>
        <h4>End Time: ${new Date(reservation.reservationEndTime).toLocaleString()}</h4>
        <div style="display: flex; justify-content: space-between; margin-top: 20px;">
            <button id="confirmCancel" style="background-color: red; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer;">Yes</button>
            <button id="cancelCancel" style="background-color: grey; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer;">No</button>
        </div>
    `;

    backdrop.appendChild(modalContent);
    document.body.appendChild(backdrop);

    var accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
        document.getElementById('confirmCancel').addEventListener('click', function () {
            submitCancellation(reservation, accessToken);
            backdrop.remove(); // Close modal on confirmation
        });
        document.getElementById('cancelCancel').addEventListener('click', function () {
            backdrop.remove(); // Close modal on cancellation
        });
        document.querySelector('.close-button').addEventListener('click', function () {
            backdrop.remove(); // Close modal on close button click
        });
    } else {
        console.error('Access token is not available.');
        return;
    }
}


 async function submitCancellation(reservation, accessToken) {
    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    var idToken = sessionStorage.getItem('idToken');

    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const modificationData = {
        reservationID: reservation.reservationID,
        companyID: reservation.companyID,
        floorPlanID: reservation.floorPlanID,
        spaceID: reservation.spaceID,
        reservationStartTime: reservation.reservationStartTime,
        reservationEndTime: reservation.reservationEndTime,
        status: reservation.status,
        userHash: username
    };
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 

    try {
        const response =  await fetch(`${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/CancelReservation`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(modificationData),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data =  await response.json();

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (data.hasError) {
            console.log(`Reservation error: ${data.errorMessage}`);
            onError(data.errorMessage);
        } else {
            console.log('Reservation cancelled successfully');
            onSuccess('Reservation was cancelled.');
        }
    } catch (error) {
        console.error('Error cancelling reservation:', error);
        onError(`Error cancelling reservation: ${error}`);
    }
}

function getUsersReservationID(companyID, floorID, spaceID, startTime, endTime, userName, accessToken) {

    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 
    const url = `${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/ListReservations?userName=${encodeURIComponent(userName)}`;
    
    fetch(url, {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken,
            'Accept': 'application/json',
        }
        
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        console.log(data);
        // Assuming `data` is an array of reservations
        const foundReservation = data.find(res =>
            res.companyID === companyID &&
            res.floorPlanID === floorID &&
            res.spaceID === spaceID &&
            res.reservationStartTime.slice(0, 16) === startTime &&
            res.reservationEndTime.slice(0, 16) === endTime
        );
        
        if (foundReservation) {
            console.log("Found Reservation ID:", foundReservation.reservationID);
            sendConfirmation(foundReservation);
        } else {
            console.log("No matching reservation found.");
        }
    })
    .catch(error => {
        console.error('Error fetching reservations:', error);
        onError("Error fetching reservations")
    });
}

//////////////Send Confirmation Email//////////////
async function sendConfirmation(reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    const reservationID = reservation.reservationID;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const EmailConfirmationUrl = appConfig.api.EmailConfirmation; 
    const url = `${EmailConfirmationUrl}/api/v1/reservationConfirmation/SendConfirmation?ReservationID=${encodeURIComponent(reservationID)}`;
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
            const errorData = await response.json();
            throw new Error(`HTTP error! status: ${response.status} - ${errorData.detail || errorData.title}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Confirmation sent successfully:', data);
        onSuccess("Confirmation sent successfully!");
        return data;
    } catch (error) {
        console.error('Error sending confirmation:', error);
    }
}

//////////////Reservation Confirmation//////////////
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

function showConfirmationModal(reservation) {
    const reservationID = reservation.reservationID;
    if (!reservation || !reservationID) {
        console.error('Invalid reservation object or missing reservationID');
        return;
    }
    // Ensure only one modal exists at a time
    const existingModal = document.querySelector('.modal-content');
    if (existingModal) {
        existingModal.remove(); // Remove existing modal to prevent duplicates
    }

    // Create modal content
    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-confirm-content');

    // Set innerHTML safely, and ensure IDs and data attributes are used correctly
    modalContent.innerHTML = `
        <span class="close-button">×</span>
        <h2>Confirm Reservation</h2>
        <form id="confirmReservationForm" data-reservation-id="${reservationID}">
            <h2>Reservation ID: ${reservationID}</h2> 
            <input type="text" id="confirmationCodeInput" placeholder="Enter confirmation code" required />
            <button type="submit">Submit</button> 
        </form>
    `;

    // Append modal content to the body or a specific modal container if you have one
    document.body.appendChild(modalContent);

    // Attach an event listener to the close button
    const closeButton = modalContent.querySelector('.close-button');
    closeButton.addEventListener('click', function() {
        modalContent.remove();
    });

    // Attach event listener to the form for handling submissions
    document.getElementById('confirmReservationForm').addEventListener('submit', function(event) {
        event.preventDefault(); // Prevent default form submission behavior
        const confirmationCode = document.getElementById('confirmationCodeInput').value;
        confirmReservation(reservationID, confirmationCode); // Call confirmReservation with the right parameters
    });
}

async function confirmReservation(reservationID, code) {

    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    
    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const EmailConfirmationUrl = appConfig.api.EmailConfirmation; 
    const url = `${EmailConfirmationUrl}/api/v1/reservationConfirmation/ConfirmReservation?reservationID=${reservationID}&otp=${encodeURIComponent(code)}`;
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken
            }
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(`HTTP error! status: ${response.status} - ${errorData.detail || errorData.title}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Reservation confirmed successfully:', data);
        onSuccess('Reservation confirmed successfully!');
        return data;
    } catch (error) {
        console.error('Error confirming reservation:', error);
        onError('Error confirming reservation. Please try again or check if reservation is already confirmed.');
    }
}
function closeModal() {
    const modal = document.getElementById('confirmModal');
    modal.style.display = 'none';
}

////////// Resend Email ///////////
async function resendEmail(reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    const reservationID = reservation.reservationID;
    
    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const EmailConfirmationUrl = appConfig.api.EmailConfirmation; 
    const url = `${EmailConfirmationUrl}/api/v1/reservationConfirmation/ResendConfirmation?reservationID=${encodeURIComponent(reservationID)}`;
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
            const errorData = await response.json();
            throw new Error(`HTTP error! status: ${response.status} - ${errorData.detail || errorData.title}`);
        }
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Confirmation resent successfully:', data);
        onSuccess('Confirmation resent successfully!');
        return data;
    } catch (error) {
        console.error('Error resending confirmation:', error);
        onError('Error resending confirmation. Please try again later.');
    }
}






//////reservtions Center View



function createLocationsContainer() {
    const locationsContainer = document.createElement('div');
    locationsContainer.className = 'locations-container';

    // Company Info
    const companyInfo = document.createElement('div');
    companyInfo.className = 'company-info';

    // Companies List
    const companiesDiv = document.createElement('div');
    companiesDiv.id = 'companies';
    const companiesTitle = document.createElement('h2');
    companiesTitle.textContent = 'Companies';
    const companiesList = document.createElement('ul');
    companiesList.id = 'companiesList';
    companiesDiv.appendChild(companiesTitle);
    companiesDiv.appendChild(companiesList);

    // Floor Plans
    const floorPlansDiv = document.createElement('div');
    floorPlansDiv.id = 'floorPlans';
    const floorPlansTitle = document.createElement('h2');
    floorPlansTitle.textContent = 'Floor Plans';
    const floorPlansContent = document.createElement('div');
    floorPlansContent.id = 'floorPlansContent';
    floorPlansDiv.appendChild(floorPlansTitle);
    floorPlansDiv.appendChild(floorPlansContent);

    // Append to company info
    companyInfo.appendChild(companiesDiv);
    companyInfo.appendChild(floorPlansDiv);

    // Append company info to locations container
    locationsContainer.appendChild(companyInfo);

    return locationsContainer;
}

function createReservationForm() {
    const form = document.createElement('form');
    form.id = 'reservationForm';

    const formElements = [
        { label: 'Start Time:', type: 'datetime-local', id: 'reservation-startTime', required: true },
        { label: 'End Time:', type: 'datetime-local', id: 'reservation-endTime', required: true },
        { label: 'Company ID:', type: 'hidden', id: 'reservation-companyId' },
        { label: 'Floor Plan ID:', type: 'hidden', id: 'reservation-floorPlanId' },
        { label: 'Space ID:', type: 'hidden', id: 'reservation-spaceId' }
    ];

    formElements.forEach(elem => {
        const div = document.createElement('div');
        div.className = 'form-group';

        const label = document.createElement('label');
        label.htmlFor = elem.id;
        label.textContent = elem.label;

        const input = document.createElement('input');
        input.type = elem.type;
        input.id = elem.id;
        input.required = elem.required;

        div.appendChild(label);
        div.appendChild(input);
        form.appendChild(div);
    });

    const submitButton = document.createElement('input');
    submitButton.type = 'submit';
    submitButton.value = 'Reserve';
    form.appendChild(submitButton);

    const checkAvailabilityButton = document.createElement('button');
    checkAvailabilityButton.type = 'button'; 
    checkAvailabilityButton.textContent = 'Check Availability';
    checkAvailabilityButton.id = 'checkAvailabilityButton';

    checkAvailabilityButton.addEventListener('click', function() {
        checkAvailability() 
    });

    
    form.appendChild(checkAvailabilityButton);

    return form;
}
function checkAvailability() {

    var accessToken = sessionStorage.getItem('accessToken', accessToken);
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;


    const companyIdInput = document.getElementById('reservation-companyId').value;
    const startTimeInput = document.getElementById('reservation-startTime').value;
    const endTimeInput = document.getElementById('reservation-endTime').value;

    const companyId = parseInt(companyIdInput, 10);

    
    const startTime = startTimeInput;
    const endTime = endTimeInput;


    if (!companyId || !startTime || !endTime) {
        console.error('Invalid input values');
        return;
    }

    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const BookingCenterUrl = appConfig.api.SpaceBookingCenter; 

    const apiUrl = `${BookingCenterUrl}/api/v1/spaceBookingCenter/reservations/CheckAvailability?companyId=${companyId}&startTime=${startTime}&endTime=${endTime}`;
    fetch(apiUrl, {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken,
            'Accept': 'application/json',
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Received data:", data)
            updateSpaceAvailabilityUI(data);
        })
        .catch(error => {
            console.error('Error checking availability:', error);
        });
}

function updateSpaceAvailabilityUI(data) {
    console.log("Received availability data:", data);

    data.forEach(space => {
        console.log(`Updating SpaceID: ${space.spaceID}, IsAvailable: ${space.isAvailable}`);
        const spaceElement = document.querySelector(`[data-space-id="${space.spaceID}"]`);
        
        if (spaceElement) {
            console.log(`Found element for SpaceID: ${space.spaceID}`);
            spaceElement.style.backgroundColor = space.isAvailable ? 'green' : 'red';
        } else {
            console.error(`Element for SpaceID: ${space.spaceID} not found.`);
        }
    });
}

async function fetchCompanies() {
    const accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    const accountInfo = await fetchUserAccount();

    if (accountInfo) {
        await getActivityStatus(accountInfo.isActive);
    }

    const userRole = sessionStorage.getItem('role');
    console.log(userRole);

    try {
        let response;
        let url;

        if (userRole === '1') {
            console.log("this is an admin");
            url = `http://localhost:5279/api/v1/spaceBookingCenter/companies/ListCompanies`;
        } else {
            console.log('not admin');
            if (accountInfo.companyId !== null && accountInfo.companyId !== undefined) {
                console.log("this is an employee user");
                url = `http://localhost:5279/api/v1/spaceBookingCenter/companies/ListCompaniesForUsers?companyID=${accountInfo.companyId}`;
            } else {
                console.log("this is a general user");
                url = `http://localhost:5279/api/v1/spaceBookingCenter/companies/ListCompaniesForUsers`;
            }
        }

        response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        console.log('Received companies data:', data);
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        const companiesList = document.getElementById('companiesList');
        companiesList.innerHTML = '';

        if (data.length === 0) {
            companiesList.innerHTML = '<p>Sorry, no companies available.</p>';
        } else {
            data.forEach(company => {
                let htmlContent = '';
                const li = document.createElement('li');
                li.classList.add('company-item');
                if (company.companyType === 2) {
                    li.classList.add('user-company');
                    htmlContent = '<h5>Workplace</h5>';
                }
                htmlContent += `
                    <div class="company-name clickable" data-company-id="${company.companyID}">${company.companyName}</div>
                    <div class="company-info">
                        <p class="company-address">Address: ${company.address}</p>
                        <p class="company-hours">Hours: ${formatTime(company.openingHours)} - ${formatTime(company.closingHours)}</p>
                        <p class="company-days">Days Open: ${company.daysOpen}</p>
                    </div>
                `;
                li.innerHTML = htmlContent;
                companiesList.appendChild(li);
            });

            document.querySelectorAll('.clickable').forEach(item => {
                item.addEventListener('click', function (event) {
                    const companyID = event.target.getAttribute('data-company-id');
                    fetchFloorPlans(companyID);
                });
            });
        }
    } catch (error) {
        console.error('Error fetching companies:', error);
        document.getElementById('companiesList').innerHTML = '<p>sorry we currently have no participating facilities.</p>';
    }
}


function formatTime(timeString) {
   
    const time = timeString.split(':'); 
    const hours = parseInt(time[0], 10);
    const minutes = time[1];
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const formattedHours = hours % 12 || 12; 
    return `${formattedHours}:${minutes} ${ampm}`;
}

async function fetchFloorPlans(companyID) {
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
    const CompanyUrl = appConfig.api.CompanyAPI; 
    try {
        const response = await fetch(`${CompanyUrl}/api/v1/spaceBookingCenter/companies/FloorPlans/${companyID}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error("HTTP error! status: " + response.status);
        }

        const data = await response.json();
        console.log(data);

        if (data.newToken) {
            sessionStorage.setItem('accessToken', data.newToken);
            console.log('New access token stored:', data.newToken);
        }

        const floorPlansContent = document.getElementById('floorPlansContent');
        floorPlansContent.innerHTML = '';

        const floorPlansArray = data.floorPlans || data;

        if (floorPlansArray.length === 0) {
            floorPlansContent.innerHTML = '<p>Sorry, this company has not uploaded any floor plans.</p>';
        } else {
            floorPlansArray.forEach(floorPlan => {
                const floorDiv = document.createElement('div');
                floorDiv.classList.add('floor-plan');
                floorDiv.innerHTML = `
                    <h3>${floorPlan.floorPlanName}</h3>
                    <img src="data:image/png;base64,${floorPlan.floorPlanImageBase64}" alt="${floorPlan.floorPlanName}" />
                    <div>FloorSpaces:</div>
                `;

                const spacesList = document.createElement('ul');
                Object.entries(floorPlan.floorSpaces).forEach(([spaceId, timeLimit]) => {
                    const spaceItem = document.createElement('li');
                    spaceItem.textContent = `SpaceID: ${spaceId}, TimeLimit: ${timeLimit}`;
                    spaceItem.classList.add('clickable-space');
                    spaceItem.setAttribute('data-company-id', companyID);
                    spaceItem.setAttribute('data-floor-plan-id', floorPlan.floorPlanID);
                    spaceItem.setAttribute('data-space-id', spaceId);
                    spacesList.appendChild(spaceItem);
                });

                floorDiv.appendChild(spacesList);
                floorPlansContent.appendChild(floorDiv);
            });

            const clickableSpaces = floorPlansContent.getElementsByClassName('clickable-space');
            Array.from(clickableSpaces).forEach(space => {
                space.addEventListener('click', function () {
                    updateReservationForm(
                        space.getAttribute('data-company-id'),
                        space.getAttribute('data-floor-plan-id'),
                        space.getAttribute('data-space-id')
                    );
                });
            });
        }
    } catch (error) {
        console.error('Error fetching floor plans:', error);
        document.getElementById('floorPlansContent').innerHTML = '<h5>This company has no available floor plans.</h5>';
    }
}


async function checkTokenExpiration(accessToken) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const SpaceBookingCenterUrl = appConfig.api.SpaceBookingCenter; 
    try {
        const response = await fetch(`${SpaceBookingCenterUrl}/api/v1/spaceBookingCenter/reservations/checkTokenExp`, {
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


async function handleReservationCreationFormSubmit(event) {
    event.preventDefault();

    var accessToken = sessionStorage.getItem('accessToken', accessToken);
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenValid =  await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    const reservationData = {
        companyId: parseInt(document.getElementById('reservation-companyId').value, 10),
        floorPlanId: parseInt(document.getElementById('reservation-floorPlanId').value, 10),
        spaceId: document.getElementById('reservation-spaceId').value,
        reservationStartTime: document.getElementById('reservation-startTime').value,
        reservationEndTime: document.getElementById('reservation-endTime').value,
        userHash: username
    };

    const requestData = JSON.stringify(reservationData);
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const SpaceBookingCenterUrl = appConfig.api.SpaceBookingCenter; 
    try {
        const response =  await fetch(`${SpaceBookingCenterUrl}/api/v1/spaceBookingCenter/reservations/CreateReservation`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: requestData
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data =  await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (data.hasError) {
            showWaitlistModal();
            console.log(`Reservation error: ${data.errorMessage}`);
            //onError(data.errorMessage);
        } else {
            console.log(reservationData.companyId);
            getUsersReservationID(reservationData.companyId, reservationData.floorPlanId,
                                    reservationData.spaceId, reservationData.reservationStartTime, 
                                    reservationData.reservationEndTime, username, accessToken);
            console.log('Reservation created successfully');
            onSuccess('Reservation created successfully!');
        }
    } catch (error) {
        console.error('Error creating reservation:', error);
        //onError("Error creating reservation. Please try again later.");
    }
}



function updateReservationForm(companyId, floorPlanId, spaceId) {
    const companyIdInput = document.getElementById('reservation-companyId');
    const floorPlanIdInput = document.getElementById('reservation-floorPlanId');
    const spaceIdInput = document.getElementById('reservation-spaceId');

    companyIdInput.value = companyId;
    floorPlanIdInput.value = floorPlanId;
    spaceIdInput.value = spaceId;

    
    companyIdInput.type = 'text';
    floorPlanIdInput.type = 'text';
    spaceIdInput.type = 'text';

    companyIdInput.required = true;
    floorPlanIdInput.required = true;
    spaceIdInput.required = true;
}

////// WAITLIST ////////

function showWaitlistModal() {
    // Check if the modal container element exists
    let modal = document.getElementById('reservationConflictModal');
    if (!modal) {
        // If it doesn't exist, create it
        modal = document.createElement('div');
        modal.id = 'reservationConflictModal';
        modal.classList.add('modal');
        document.body.appendChild(modal);
    }

    // Create modal content
    let modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.innerHTML = `
        <span class="close-button">&times;</span>
        <p>This space is already reserved for this time. Would you like to join the waitlist?</p>
        <button id="joinWaitlistBtn">Yes</button>
        <button id="cancelWaitlistBtn">No</button>
    `;

    // Append modal content to modal container
    modal.innerHTML = ''; // Clear existing content
    modal.appendChild(modalContent);

    // Create success message element
    let successMessage = document.createElement('div');
    successMessage.id = 'successMessage';
    successMessage.textContent = 'You have successfully joined the waitlist!';
    successMessage.style.display = 'none';

    // Append success message to modal container
    modal.appendChild(successMessage);

    // Create error message element
    let errorMsg = document.createElement('div');
    errorMsg.id = 'errorMsg';
    errorMsg.textContent = 'You have already been added to this waitlist.';
    errorMsg.style.display = 'none';

    // Append error message to modal container
    modal.appendChild(errorMsg);

    // Show the modal
    modal.style.display = 'block';

    const closeButton = modal.querySelector('.close-button');
    const cancelButton = modal.querySelector('#cancelWaitlistBtn');
    const joinWaitlistButton = modal.querySelector('#joinWaitlistBtn');

    // Add event listeners
    closeButton.addEventListener('click', closeModal);
    cancelButton.addEventListener('click', closeModal);
    joinWaitlistButton.addEventListener('click', joinWaitlist);
}

async function joinWaitlist() {
    console.log('Joining waitlist...');
    var accessToken = sessionStorage.getItem('accessToken', accessToken);
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    const reservationData = {
        companyId: parseInt(document.getElementById('reservation-companyId').value, 10),
        floorPlanId: parseInt(document.getElementById('reservation-floorPlanId').value, 10),
        spaceId: document.getElementById('reservation-spaceId').value,
        reservationStartTime: document.getElementById('reservation-startTime').value,
        reservationEndTime: document.getElementById('reservation-endTime').value,
        userHash: username
    };

    const requestData = JSON.stringify(reservationData);
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const SpaceBookingCenterUrl = appConfig.api.SpaceBookingCenter; 
    try {
        const response = await fetch(`${SpaceBookingCenterUrl}/api/v1/spaceBookingCenter/reservations/addToWaitlist`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: requestData
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

        if (data.hasError) {
            // Show error message
            const errorMsg = document.getElementById('errorMsg');
            errorMsg.style.display = 'block';

            setTimeout(() => {
                closeModal();
                errorMsg.style.display = 'none';
            }, 5000);

            console.log(`Joining waitlist error: User already on waitlist`);
        } else {
            // Show success message
            const successMessage = document.getElementById('successMessage');
            successMessage.style.display = 'block';

            setTimeout(() => {
                closeModal();
                successMessage.style.display = 'none';
            }, 5000);

            console.log('Successfully joined waitlist');
        }
    } catch (error) {
        console.error('Error joining waitlist:', error);
    }
}


function closeModal() {
    const modal = document.getElementById('reservationConflictModal');
    modal.style.display = 'none';
}

///////////////////////


/// modal 

function onSuccess(message) {
    createAndShowModal(message); 
}


function onError(errorMessage) {
    createAndShowModal(errorMessage); 
}

function createAndShowModal(message) {
    document.querySelectorAll('.modal-backdrop').forEach(modal => modal.remove());

    const backdrop = document.createElement('div');
    backdrop.className = 'modal-backdrop';

    backdrop.addEventListener('click', event => {
        if (event.target === backdrop) {
            backdrop.style.display = 'none';
        }
    });

    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';

    const closeBtn = document.createElement('span');
    closeBtn.className = 'modal-close-btn';
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = function() {
        backdrop.style.display = 'none';
    };

    const text = document.createElement('p');
    text.textContent = message;

    modalContent.appendChild(closeBtn);
    modalContent.appendChild(text);
    backdrop.appendChild(modalContent);
    document.body.appendChild(backdrop);

    backdrop.style.display = 'flex';
    backdrop.style.color = "#010100"; 
}
