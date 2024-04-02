


document.getElementById('initAppButton').addEventListener('click', function() {
    
    initSidebar();
    
    const formContainer = document.querySelector('.space-form-container'); 
    if (formContainer) { 
        const reservationForm = createReservationForm();

        reservationForm.addEventListener('submit', function(event) {
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
        document.getElementById('companiesList').addEventListener('click', function(event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    
        
});

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



function initSidebar() {
    const sidebar = document.querySelector('.sidebar');

    const buttons = [
        { id: 'loadReservationCenter', text: 'Reserve Now', onClickFunction: () => getReservationCenter() },
        { id: 'loadReservationOverview', text: 'Your Reservations', onClickFunction: () => getReservtionOverview() }
    ];
    buttons.forEach(({ id, text, onClickFunction }) => {
        const button = document.createElement('button');
        button.id = id;
        button.textContent = text;
        button.addEventListener('click', onClickFunction);
        sidebar.appendChild(button);
    });
}


function initReservationOverviewButtons() {
    
    document.getElementById('reserveNowBtn').addEventListener('click', handleReserveNowClick);
    document.getElementById('yourReservationsBtn').addEventListener('click', handleYourReservationsClick);

}



function getReservationCenter() {

        fetchCompanies();
        document.getElementById('companiesList').addEventListener('click', function(event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    
        document.getElementById('reservationForm').addEventListener('submit', handleReservationCreationFormSubmit);
        
}


function getReservtionOverview() {
    reservationOverviewButtons();
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
        { id: 'loadActiveReservationsBtn', text: 'Active Reservations', onClickFunction: () => getUsersActiveReservations(username) },
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



function getUsersActiveReservations(username) {

    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

   
   
    const url = `http://localhost:5005/api/v1/spaceBookingCenter/reservations/ListActiveReservations?userName=${encodeURIComponent(username)}`;

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
   
    const url = `http://localhost:5005/api/v1/spaceBookingCenter/reservations/ListReservations?userName=${encodeURIComponent(userName)}`;

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
        console.log("in here");
        console.log(data);

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
        button.addEventListener('click', function() {
            const reservationData = JSON.parse(this.getAttribute('data-reservation'));
            showModifyModal(reservationData);
        });
    });
    document.querySelectorAll('.cancel-btn').forEach(button => {
        button.addEventListener('click', function() {
            const reservationData = JSON.parse(this.getAttribute('data-reservation'));
            showCancelModal(reservationData);
        });
    });
}
function showModifyModal(reservation) {

    
    const existingModal = document.querySelector('.modal-content');
    if (existingModal) {
        existingModal.remove();
    }

    const startTime = reservation.reservationStartTime ? new Date(reservation.reservationStartTime).toISOString().slice(0, 16) : '';
    const endTime = reservation.reservationEndTime ? new Date(reservation.reservationEndTime).toISOString().slice(0, 16) : '';

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.innerHTML = `
        <h2>Modify Reservation</h2>
        <form id="modifyReservationForm" data-reservation-id="${reservation.reservationID}">
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
    document.body.appendChild(modalContent);

    
    document.getElementById('modifyReservationForm').addEventListener('submit', function(event) {
        event.preventDefault();
        submitModification(reservation);
    });
    

   
    
}

async function submitModification(reservation) {

    var accessToken = sessionStorage.getItem('accessToken', accessToken);
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

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

    try {
        const response = await fetch(`http://localhost:5005/api/v1/spaceBookingCenter/reservations/UpdateReservation`, {
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
    const existingModal = document.querySelector('.modal-content');
    if (existingModal) {
        existingModal.remove();
    }

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.innerHTML = `
        <h2>Cancel Reservation</h2>
        <h3>Are you sure you want to cancel this reservation?</h3>
        <h4>Reservation ID: ${reservation.reservationID}</h4>
        <h4>Company ID: ${reservation.companyID}</h4>
        <h4>FloorPlan ID: ${reservation.floorPlanID}</h4>
        <h4>Space ID: ${reservation.spaceID}</h4>
        <h4>Start Time: ${new Date(reservation.reservationStartTime).toLocaleString()}</h4>
        <h4>End Time: ${new Date(reservation.reservationEndTime).toLocaleString()}</h4>
        <button id="confirmCancel">Yes</button>
        <button id="cancelCancel">No</button>
    `;

    document.body.appendChild(modalContent);

    var accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
        document.getElementById('confirmCancel').addEventListener('click', function() {
            submitCancellation(reservation, accessToken);
        });
        document.getElementById('cancelCancel').addEventListener('click', function() {
            document.querySelector('.modal-content').remove();
        });
    }
    else {
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

    try {
        const response =  await fetch(`http://localhost:5005/api/v1/spaceBookingCenter/reservations/CancelReservation`, {
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



    const apiUrl = `http://localhost:5005/api/v1/spaceBookingCenter/reservations/CheckAvailability?companyId=${companyId}&startTime=${startTime}&endTime=${endTime}`;
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
    const isTokenValid = await checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    try {
        const response = await fetch('http://localhost:5001/api/v1/spaceBookingCenter/companies/ListCompanies', {
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

        data.forEach(company => {
            const li = document.createElement('li');
            li.classList.add('company-item');
            const htmlContent = `
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
            item.addEventListener('click', function(event) {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            });
        });
    } catch (error) {
        console.error('Error fetching companies:', error);
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

    try {
        const response = await fetch(`http://localhost:5001/api/v1/spaceBookingCenter/companies/FloorPlans/${companyID}`, {
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
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        const floorPlansContent = document.getElementById('floorPlansContent');
        floorPlansContent.innerHTML = '';

        const floorPlansArray = data.floorPlans || data; 
        data.forEach(floorPlan => {
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
            space.addEventListener('click', function() {
                updateReservationForm(
                    space.getAttribute('data-company-id'),
                    space.getAttribute('data-floor-plan-id'),
                    space.getAttribute('data-space-id')
                );
            });
        });
    } catch (error) {
        console.error('Error fetching floor plans:', error);
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

    try {
        const response =  await fetch('http://localhost:5005/api/v1/spaceBookingCenter/reservations/CreateReservation', {
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
            console.log(`Reservation error: ${data.errorMessage}`);
            onError(data.errorMessage);
        } else {
            console.log('Reservation created successfully');
            onSuccess('Reservation created successfully!');
        }
    } catch (error) {
        console.error('Error creating reservation:', error);
        onError("Error creating reservation. Please try again later.");
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
}