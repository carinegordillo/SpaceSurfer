document.getElementById('initAppButtonGen').addEventListener('click', function () {
    initSidebar();
    let formContainer;
    if (page === 1) {
        formContainer = document.querySelector('.space-form-container-gen'); 
    } else {
        formContainer = document.querySelector('.space-form-container-man'); 
    }
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
    
    if (page === 1) {
        document.getElementById('companiesList-gen').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    } else {
        document.getElementById('companiesList-man').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    }        
});

document.getElementById('initAppButtonManager').addEventListener('click', function () {
    initSidebar();
    let formContainer;
    if (page === 1) {
        formContainer = document.querySelector('.space-form-container-gen');
    } else {
        formContainer = document.querySelector('.space-form-container-man');
    }
    if (formContainer) {
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
    
    if (page === 1) {
        document.getElementById('companiesList-gen').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id-gen');
                fetchFloorPlans(companyID);
            }
        });
    } else {
        document.getElementById('companiesList-man').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id-man');
                fetchFloorPlans(companyID);
            }
        });
    }
});

function logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";

    let existingModal;
    if (page === 1) {
        existingModal = document.querySelector('.modal-content-gen');
    } else {
        existingModal = document.querySelector('.modal-content-man');
    }
        if (existingModal) {
            existingModal.remove();
        }
    let modalBackdrop;
    if (page === 1) {
        modalBackdrop = document.querySelector('.modal-backdrop-gen');
    } else {
        modalBackdrop = document.querySelector('.modal-backdrop-man');
    }
    if (modalBackdrop) {
        modalBackdrop.style.display = 'none';
    }

}



function initSidebar() {
    let sidebar;
    if (page === 1) {
        sidebar = document.querySelector('.sidebar-gen');
    } else {
        sidebar = document.querySelector('.sidebar-man');
    }
    let buttons;
    
    if (page === 1) {
        buttons = [
            { id: 'loadReservationCenter-gen', text: 'Reserve Now', onClickFunction: () => getReservationCenter() },
            { id: 'loadReservationOverview-gen', text: 'Your Reservations', onClickFunction: () => getReservtionOverview() }
        ];
    } else {
        buttons = [
            { id: 'loadReservationCenter-man', text: 'Reserve Now', onClickFunction: () => getReservationCenter() },
            { id: 'loadReservationOverview-man', text: 'Your Reservations', onClickFunction: () => getReservtionOverview() }
        ];
    }

    buttons.forEach(({ id, text, onClickFunction }) => {
        const button = document.createElement('button');
        button.id = id;
        button.textContent = text;
        button.addEventListener('click', onClickFunction);
        sidebar.appendChild(button);
    });
}



function initReservationOverviewButtons() {
    
    if (page === 1) {
        document.getElementById('reserveNowBtn-gen').addEventListener('click', handleReserveNowClick);
        document.getElementById('yourReservationsBtn-gen').addEventListener('click', handleYourReservationsClick);
    } else {
        document.getElementById('reserveNowBtn-man').addEventListener('click', handleReserveNowClick);
        document.getElementById('yourReservationsBtn-man').addEventListener('click', handleYourReservationsClick);
    }
}



function getReservationCenter() {

    fetchCompanies();
    
    if (page === 1) {
        document.getElementById('companiesList-gen').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id-gen');
                fetchFloorPlans(companyID);
            }
        });
    } else {
        document.getElementById('companiesList-man').addEventListener('click', function (event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id-man');
                fetchFloorPlans(companyID);
            }
        });
    }
    
    if (page === 1) {
        document.getElementById('reservationForm-gen').addEventListener('submit', handleReservationCreationFormSubmit);
    } else {
        document.getElementById('reservationForm-man').addEventListener('submit', handleReservationCreationFormSubmit);
    }    
}


function getReservtionOverview() {
    reservationOverviewButtons();
}





///RESERVATION OVERVIEW

function reservationOverviewButtons() {
    let content;
    if (page === 1) {
        content = document.querySelector('.reservation-buttons-gen');
    } else {
        content = document.querySelector('.reservation-buttons-man');
    }
    content.innerHTML = ''; 

    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    let buttons;
    
    if (page === 1) {
        buttons = [
            { id: 'loadActiveReservationsBtn-gen', text: 'Active Reservations', onClickFunction: () => getUsersActiveReservations(username) },
            { id: 'loadAllReservationsBtn-gen', text: 'All Reservations', onClickFunction: () => getUsersReservations(username, accessToken) }
        ];
    } else {
        buttons = [
            { id: 'loadActiveReservationsBtn-man', text: 'Active Reservations', onClickFunction: () => getUsersActiveReservations(username) },
            { id: 'loadAllReservationsBtn-man', text: 'All Reservations', onClickFunction: () => getUsersReservations(username, accessToken) }
        ];
    }

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
        if (page === 1) {
            renderReservations(data, '.reservation-list-gen');
        } else {
            renderReservations(data, '.reservation-list-man');
        }
        
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
        if (page === 1) {
            renderReservations(data, '.reservation-list-gen');
        } else {
            renderReservations(data, '.reservation-list-man');
        }
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
            if (page === 1) {
                buttonsHtml = `
            <button class="modify-btn-gen" data-reservation='${JSON.stringify(reservation)}'>Modify</button>
            <button class="cancel-btn-gen" data-reservation='${JSON.stringify(reservation)}'>Cancel</button>
        `;
            } else {
                buttonsHtml = `
            <button class="modify-btn-man" data-reservation='${JSON.stringify(reservation)}'>Modify</button>
            <button class="cancel-btn-man" data-reservation='${JSON.stringify(reservation)}'>Cancel</button>
        `;
            }
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
    if (page === 1) {
        document.querySelectorAll('.modify-btn-gen').forEach(button => {
            button.addEventListener('click', function () {
                const reservationData = JSON.parse(this.getAttribute('data-reservation'));
                showModifyModal(reservationData);
            });
        });
        document.querySelectorAll('.cancel-btn-gen').forEach(button => {
            button.addEventListener('click', function () {
                const reservationData = JSON.parse(this.getAttribute('data-reservation'));
                showCancelModal(reservationData);
            });
        });
    } else {
        document.querySelectorAll('.modify-btn-man').forEach(button => {
            button.addEventListener('click', function () {
                const reservationData = JSON.parse(this.getAttribute('data-reservation'));
                showModifyModal(reservationData);
            });
        });
        document.querySelectorAll('.cancel-btn-man').forEach(button => {
            button.addEventListener('click', function () {
                const reservationData = JSON.parse(this.getAttribute('data-reservation'));
                showCancelModal(reservationData);
            });
        });
    }
}
function showModifyModal(reservation) {

    
    let existingModal;
    if (page === 1) {
        existingModal = document.querySelector('.modal-content-gen');
    } else {
        existingModal = document.querySelector('.modal-content-man');
    }
    if (existingModal) {
        existingModal.remove();
    }

    const startTime = reservation.reservationStartTime ? new Date(reservation.reservationStartTime).toISOString().slice(0, 16) : '';
    const endTime = reservation.reservationEndTime ? new Date(reservation.reservationEndTime).toISOString().slice(0, 16) : '';

    const modalContent = document.createElement('div');
    if (page === 1) {
        modalContent.classList.add('modal-content-gen');
    } else {
        modalContent.classList.add('modal-content-man');
    }
    
    if (page === 1) {
        modalContent.innerHTML = `
        <h2>Modify Reservation</h2>
        <form id="modifyReservationForm-gen" data-reservation-id="${reservation.reservationID}">
            <h2>Company ID: ${reservation.companyID}</h2>
            <h2>FloorPlan ID: ${reservation.floorPlanID}</h2>
            <h2>Space ID: ${reservation.spaceID}</h2>
            <label for="newStartTime">New Start Time:</label>
            <input type="datetime-local" id="newStartTime-gen" value="${startTime}" required>
            <label for="newEndTime">New End Time:</label>
            <input type="datetime-local" id="newEndTime-gen" value="${endTime}" required>
            <button type="submit">Submit</button>
        </form>
    `;
    } else {
        modalContent.innerHTML = `
        <h2>Modify Reservation</h2>
        <form id="modifyReservationForm-man" data-reservation-id="${reservation.reservationID}">
            <h2>Company ID: ${reservation.companyID}</h2>
            <h2>FloorPlan ID: ${reservation.floorPlanID}</h2>
            <h2>Space ID: ${reservation.spaceID}</h2>
            <label for="newStartTime">New Start Time:</label>
            <input type="datetime-local" id="newStartTime-man" value="${startTime}" required>
            <label for="newEndTime">New End Time:</label>
            <input type="datetime-local" id="newEndTime-man" value="${endTime}" required>
            <button type="submit">Submit</button>
        </form>
    `;
    }
    document.body.appendChild(modalContent);
    
    if (page === 1) {
        document.getElementById('modifyReservationForm-gen').addEventListener('submit', function (event) {
            event.preventDefault();
            submitModification(reservation);
        });
    } else {
        document.getElementById('modifyReservationForm-man').addEventListener('submit', function (event) {
            event.preventDefault();
            submitModification(reservation);
        });
    }
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

    let form;
    
    if (page === 1) {
        form = document.getElementById('modifyReservationForm-gen');
    } else {
        form = document.getElementById('modifyReservationForm-man');
    }
    let newStartTime;
    if (page === 1) {
        newStartTime = form.querySelector('#newStartTime-gen').value;
    } else {
        newStartTime = form.querySelector('#newStartTime-man').value;
    }
    let newEndTime;
    if (page === 1) {
        newEndTime = form.querySelector('#newEndTime-gen').value;
    } else {
        newEndTime = form.querySelector('#newEndTime-man').value;
    }

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
    let existingModal;
    if (page === 1) {
        existingModal = document.querySelector('.modal-content-gen');
    } else {
        existingModal = document.querySelector('.modal-content-man');
    }
    if (existingModal) {
        existingModal.remove();
    }

    const modalContent = document.createElement('div');
    if (page === 1) {
        modalContent.classList.add('modal-content-gen');
    } else {
        modalContent.classList.add('modal-content-man');
    }
    
    if (page === 1) {
        modalContent.innerHTML = `
        <h2>Cancel Reservation</h2>
        <h3>Are you sure you want to cancel this reservation?</h3>
        <h4>Reservation ID: ${reservation.reservationID}</h4>
        <h4>Company ID: ${reservation.companyID}</h4>
        <h4>FloorPlan ID: ${reservation.floorPlanID}</h4>
        <h4>Space ID: ${reservation.spaceID}</h4>
        <h4>Start Time: ${new Date(reservation.reservationStartTime).toLocaleString()}</h4>
        <h4>End Time: ${new Date(reservation.reservationEndTime).toLocaleString()}</h4>
        <button id="confirmCancel-gen">Yes</button>
        <button id="cancelCancel-gen">No</button>
    `;
    } else {
        modalContent.innerHTML = `
        <h2>Cancel Reservation</h2>
        <h3>Are you sure you want to cancel this reservation?</h3>
        <h4>Reservation ID: ${reservation.reservationID}</h4>
        <h4>Company ID: ${reservation.companyID}</h4>
        <h4>FloorPlan ID: ${reservation.floorPlanID}</h4>
        <h4>Space ID: ${reservation.spaceID}</h4>
        <h4>Start Time: ${new Date(reservation.reservationStartTime).toLocaleString()}</h4>
        <h4>End Time: ${new Date(reservation.reservationEndTime).toLocaleString()}</h4>
        <button id="confirmCancel-man">Yes</button>
        <button id="cancelCancel-man">No</button>
    `;
    }

    document.body.appendChild(modalContent);

    var accessToken = sessionStorage.getItem('accessToken');
    if (accessToken) {
        
        if (page === 1) {
            document.getElementById('confirmCancel-gen').addEventListener('click', function () {
                submitCancellation(reservation, accessToken);
            });
            document.getElementById('cancelCancel-gen').addEventListener('click', function () {
                document.querySelector('.modal-content-gen').remove();
            });
        } else {
            document.getElementById('confirmCancel-man').addEventListener('click', function () {
                submitCancellation(reservation, accessToken);
            });
            document.getElementById('cancelCancel-man').addEventListener('click', function () {
                document.querySelector('.modal-content-man').remove();
            });
        }
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
    if (page === 1) {
        companyInfo.className = 'company-info-gen';
    } else {
        companyInfo.className = 'company-info-man';
    }

    // Companies List
    const companiesDiv = document.createElement('div');
    
    if (page === 1) {
        companiesDiv.id = 'companies-gen';
    } else {
        companiesDiv.id = 'companies-man';
    }
    const companiesTitle = document.createElement('h2');
    companiesTitle.textContent = 'Companies';
    const companiesList = document.createElement('ul');
    
    if (page === 1) {
        companiesList.id = 'companiesList-gen';
    } else {
        companiesList.id = 'companiesList-man';
    }
    companiesDiv.appendChild(companiesTitle);
    companiesDiv.appendChild(companiesList);

    // Floor Plans
    const floorPlansDiv = document.createElement('div');
    
    if (page === 1) {
        floorPlansDiv.id = 'floorPlans-gen';
    } else {
        floorPlansDiv.id = 'floorPlans-man';
    }
    const floorPlansTitle = document.createElement('h2');
    floorPlansTitle.textContent = 'Floor Plans';
    const floorPlansContent = document.createElement('div');
    
    if (page === 1) {
        floorPlansContent.id = 'floorPlansContent-gen';
    } else {
        floorPlansContent.id = 'floorPlansContent-man';
    }
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
    
    if (page === 1) {
        form.id = 'reservationForm-gen';
    } else {
        form.id = 'reservationForm-man';
    }

    let formElements;
    
    if (page === 1) {
        formElements = [
            { label: 'Start Time:', type: 'datetime-local', id: 'reservation-startTime-gen', required: true },
            { label: 'End Time:', type: 'datetime-local', id: 'reservation-endTime-gen', required: true },
            { label: 'Company ID:', type: 'hidden', id: 'reservation-companyId-gen' },
            { label: 'Floor Plan ID:', type: 'hidden', id: 'reservation-floorPlanId-gen' },
            { label: 'Space ID:', type: 'hidden', id: 'reservation-spaceId-gen' }
        ];
    } else {
        formElements = [
            { label: 'Start Time:', type: 'datetime-local', id: 'reservation-startTime-man', required: true },
            { label: 'End Time:', type: 'datetime-local', id: 'reservation-endTime-man', required: true },
            { label: 'Company ID:', type: 'hidden', id: 'reservation-companyId-man' },
            { label: 'Floor Plan ID:', type: 'hidden', id: 'reservation-floorPlanId-man' },
            { label: 'Space ID:', type: 'hidden', id: 'reservation-spaceId-man' }
        ];
    }

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
    
    if (page === 1) {
        checkAvailabilityButton.id = 'checkAvailabilityButton-gen';
    } else {
        checkAvailabilityButton.id = 'checkAvailabilityButton-man';
    }

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


    let companyIdInput;
    let startTimeInput;
    let endTimeInput;
    
    if (page === 1) {
        companyIdInput = document.getElementById('reservation-companyId-gen').value;
        startTimeInput = document.getElementById('reservation-startTime-gen').value;
        endTimeInput = document.getElementById('reservation-endTime-gen').value;
    } else {
        companyIdInput = document.getElementById('reservation-companyId-man').value;
        startTimeInput = document.getElementById('reservation-startTime-man').value;
        endTimeInput = document.getElementById('reservation-endTime-man').value;
    }

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
        let spaceElement;
        if (page === 1) {
            spaceElement = document.querySelector(`[data-space-id-gen="${space.spaceID}"]`);
        } else {
            spaceElement = document.querySelector(`[data-space-id-man="${space.spaceID}"]`);
        }
        
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

        let companiesList;
        
        if (page === 1) {
            companiesList = document.getElementById('companiesList-gen');
        } else {
            companiesList = document.getElementById('companiesList-man');
        }
        companiesList.innerHTML = '';

        data.forEach(company => {
            const li = document.createElement('li');
            if (page === 1) {
                li.classList.add('company-item-gen');
            } else {
                li.classList.add('company-item-man');
            }
            let htmlContent;
            if (page === 1) {
                htmlContent = `
                <div class="company-name-gen clickable-gen" data-company-id-gen="${company.companyID}">${company.companyName}</div>
                <div class="company-info-gen">
                    <p class="company-address">Address: ${company.address}</p>
                    <p class="company-hours">Hours: ${formatTime(company.openingHours)} - ${formatTime(company.closingHours)}</p>
                    <p class="company-days">Days Open: ${company.daysOpen}</p>
                </div>
            `;
            } else {
                htmlContent = `
                <div class="company-name-man clickable-man" data-company-id-man="${company.companyID}">${company.companyName}</div>
                <div class="company-info-man">
                    <p class="company-address">Address: ${company.address}</p>
                    <p class="company-hours">Hours: ${formatTime(company.openingHours)} - ${formatTime(company.closingHours)}</p>
                    <p class="company-days">Days Open: ${company.daysOpen}</p>
                </div>
            `;
            }

            li.innerHTML = htmlContent;
            companiesList.appendChild(li);
        });
        let companyID;
        if (page === 1) {
            document.querySelectorAll('.clickable-gen').forEach(item => {
                item.addEventListener('click', function (event) {
                    companyID = event.target.getAttribute('data-company-id-gen');
                    fetchFloorPlans(companyID);
                });
            });
        } else {
            document.querySelectorAll('.clickable-man').forEach(item => {
                item.addEventListener('click', function (event) {
                    companyID = event.target.getAttribute('data-company-id-man');
                    fetchFloorPlans(companyID);
                });
            });
        }
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

        let floorPlansContent;
        
        if (page === 1) {
            floorPlansContent = document.getElementById('floorPlansContent-gen');
        } else {
            floorPlansContent = document.getElementById('floorPlansContent-man');
        }
        floorPlansContent.innerHTML = '';

        const floorPlansArray = data.floorPlans || data; 
        data.forEach(floorPlan => {
            const floorDiv = document.createElement('div');
            if (page === 1) {
                floorDiv.classList.add('floor-plan-gen');
                floorDiv.innerHTML = `
                <h3>${floorPlan.floorPlanName}</h3>
                <img src="data:image/png;base64,${floorPlan.floorPlanImageBase64}" alt="${floorPlan.floorPlanName}" />
                <div>FloorSpaces:</div>
            `;
            } else {
                floorDiv.classList.add('floor-plan-man');
                floorDiv.innerHTML = `
                <h3>${floorPlan.floorPlanName}</h3>
                <img src="data:image/png;base64,${floorPlan.floorPlanImageBase64}" alt="${floorPlan.floorPlanName}" />
                <div>FloorSpaces:</div>
            `;
            }

            const spacesList = document.createElement('ul');
            Object.entries(floorPlan.floorSpaces).forEach(([spaceId, timeLimit]) => {
                const spaceItem = document.createElement('li');
                spaceItem.textContent = `SpaceID: ${spaceId}, TimeLimit: ${timeLimit}`;
                if (page === 1) {
                    spaceItem.classList.add('clickable-space-gen');
                    spaceItem.setAttribute('data-company-id-gen', companyID);
                    spaceItem.setAttribute('data-floor-plan-id-gen', floorPlan.floorPlanID);
                    spaceItem.setAttribute('data-space-id-gen', spaceId);
                } else {
                    spaceItem.classList.add('clickable-space-man');
                    spaceItem.setAttribute('data-company-id-man', companyID);
                    spaceItem.setAttribute('data-floor-plan-id-man', floorPlan.floorPlanID);
                    spaceItem.setAttribute('data-space-id-man', spaceId);
                }
                spacesList.appendChild(spaceItem);
            });

            floorDiv.appendChild(spacesList);
            floorPlansContent.appendChild(floorDiv);
        });

        let clickableSpaces;
        if (page === 1) {
            clickableSpaces = floorPlansContent.getElementsByClassName('clickable-space-gen');
            Array.from(clickableSpaces).forEach(space => {
                space.addEventListener('click', function () {
                    updateReservationForm(
                        space.getAttribute('data-company-id-gen'),
                        space.getAttribute('data-floor-plan-id-gen'),
                        space.getAttribute('data-space-id-gen')
                    );
                });
            });
        } else {
            clickableSpaces = floorPlansContent.getElementsByClassName('clickable-space-man');
            Array.from(clickableSpaces).forEach(space => {
                space.addEventListener('click', function () {
                    updateReservationForm(
                        space.getAttribute('data-company-id-man'),
                        space.getAttribute('data-floor-plan-id-man'),
                        space.getAttribute('data-space-id-man')
                    );
                });
            });
        }
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

    let reservationData;
    
    if (page === 1) {
        reservationData = {
            companyId: parseInt(document.getElementById('reservation-companyId-gen').value, 10),
            floorPlanId: parseInt(document.getElementById('reservation-floorPlanId-gen').value, 10),
            spaceId: document.getElementById('reservation-spaceId-gen').value,
            reservationStartTime: document.getElementById('reservation-startTime-gen').value,
            reservationEndTime: document.getElementById('reservation-endTime-gen').value,
            userHash: username
        };
        console.log("gen reservation data");
        console.log("companyid: " + reservationData.companyId);
        console.log("floorplanid: " + reservationData.floorPlanId);
        console.log("spaceid: " + reservationData.spaceId);
        console.log("stime: " + reservationData.reservationStartTime);
        console.log("etime: " + reservationData.reservationEndTime);
        console.log("userHash: " + reservationData.userHash);
    } else {
        reservationData = {
            companyId: parseInt(document.getElementById('reservation-companyId-man').value, 10),
            floorPlanId: parseInt(document.getElementById('reservation-floorPlanId-man').value, 10),
            spaceId: document.getElementById('reservation-spaceId-man').value,
            reservationStartTime: document.getElementById('reservation-startTime-man').value,
            reservationEndTime: document.getElementById('reservation-endTime-man').value,
            userHash: username
        };
        console.log("man reservation data");
        console.log("companyid: " + reservationData.companyId);
        console.log("floorplanid: " + reservationData.floorPlanId);
        console.log("spaceid: " + reservationData.spaceId);
        console.log("stime: " + reservationData.reservationStartTime);
        console.log("etime: " + reservationData.reservationEndTime);
        console.log("userHash: " + reservationData.userHash);
    }

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
            showWaitlistModal();
            console.log(`Reservation error: ${data.errorMessage}`);
            //onError(data.errorMessage);
        } else {
            console.log('Reservation created successfully');
            onSuccess('Reservation created successfully!');
        }
    } catch (error) {
        console.error('Error creating reservation:', error);
        //onError("Error creating reservation. Please try again later.");
    }
}



function updateReservationForm(companyId, floorPlanId, spaceId) {
    let companyIdInput;
    let floorPlanIdInput;
    let spaceIdInput;
    
    if (page === 1) {
        companyIdInput = document.getElementById('reservation-companyId-gen');
        floorPlanIdInput = document.getElementById('reservation-floorPlanId-gen');
        spaceIdInput = document.getElementById('reservation-spaceId-gen');
    } else {
        companyIdInput = document.getElementById('reservation-companyId-man');
        floorPlanIdInput = document.getElementById('reservation-floorPlanId-man');
        spaceIdInput = document.getElementById('reservation-spaceId-man');
    }

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
    let modal;
    
    if (page === 1) {
        modal = document.getElementById('reservationConflictModal-gen');
    } else {
        modal = document.getElementById('reservationConflictModal-man');
    }
    if (!modal) {
        // If it doesn't exist, create it
        modal = document.createElement('div');
        
        if (page === 1) {
            modal.id = 'reservationConflictModal-gen';
            modal.classList.add('modal-gen');
        } else {
            modal.id = 'reservationConflictModal-man';
            modal.classList.add('modal-man');
        }
        document.body.appendChild(modal);
    }

    // Create modal content
    const modalContent = document.createElement('div');
    if (page === 1) {
        modalContent.classList.add('modal-content-gen');
        modalContent.innerHTML = `
        <span class="close-button-gen">&times;</span>
        <p>This space is already reserved for this time. Would you like to join the waitlist?</p>
        <button id="joinWaitlistBtn-gen">Yes</button>
        <button id="cancelWaitlistBtn-gen">No</button>
    `;
    } else {
        modalContent.classList.add('modal-content-man');
        modalContent.innerHTML = `
        <span class="close-button-man">&times;</span>
        <p>This space is already reserved for this time. Would you like to join the waitlist?</p>
        <button id="joinWaitlistBtn-man">Yes</button>
        <button id="cancelWaitlistBtn-man">No</button>
    `;
    }

    // Append modal content to modal container
    modal.innerHTML = ''; // Clear existing content
    modal.appendChild(modalContent);

    // Create success message element
    const successMessage = document.createElement('div');
    
    if (page === 1) {
        successMessage.id = 'successMessage-gen';
    } else {
        successMessage.id = 'successMessage-man';
    }
    successMessage.textContent = 'You have successfully joined the waitlist!';
    successMessage.style.display = 'none';

    // Append success message to modal container
    modal.appendChild(successMessage);

    // Create error message element
    const errorMsg = document.createElement('div');
    
    if (page === 1) {
        errorMsg.id = 'errorMsg-gen';
    } else {
        errorMsg.id = 'errorMsg-man';
    }
    errorMsg.textContent = 'You have already been added to this waitlist.';
    errorMsg.style.display = 'none';

    // Append error message to modal container
    modal.appendChild(errorMsg);

    // Show the modal
    modal.style.display = 'block';

    let closeButton;
    let cancelButton;
    let joinWaitlistButton;
    if (page === 1) {
        closeButton = modal.querySelector('.close-button-gen');
        cancelButton = modal.querySelector('#cancelWaitlistBtn-gen');
        joinWaitlistButton = modal.querySelector('#joinWaitlistBtn-gen');
    } else {
        closeButton = modal.querySelector('.close-button-man');
        cancelButton = modal.querySelector('#cancelWaitlistBtn-man');
        joinWaitlistButton = modal.querySelector('#joinWaitlistBtn-man');
    }

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

    let reservationData;
    if (page === 1) {
        reservationData = {
            companyId: parseInt(document.getElementById('reservation-companyId-gen').value, 10),
            floorPlanId: parseInt(document.getElementById('reservation-floorPlanId-gen').value, 10),
            spaceId: document.getElementById('reservation-spaceId-gen').value,
            reservationStartTime: document.getElementById('reservation-startTime-gen').value,
            reservationEndTime: document.getElementById('reservation-endTime-gen').value,
            userHash: username
        };
    } else {
        reservationData = {
            companyId: parseInt(document.getElementById('reservation-companyId-man').value, 10),
            floorPlanId: parseInt(document.getElementById('reservation-floorPlanId-man').value, 10),
            spaceId: document.getElementById('reservation-spaceId-man').value,
            reservationStartTime: document.getElementById('reservation-startTime-man').value,
            reservationEndTime: document.getElementById('reservation-endTime-man').value,
            userHash: username
        };
    }

    const requestData = JSON.stringify(reservationData);

    try {
        const response = await fetch('http://localhost:5005/api/v1/spaceBookingCenter/reservations/addToWaitlist', {
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
            let errorMsg;
            
            if (page === 1) {
                errorMsg = document.getElementById('errorMsg-gen');
            } else {
                errorMsg = document.getElementById('errorMsg-man');
            }
            errorMsg.style.display = 'block';

            setTimeout(() => {
                closeModal();
                errorMsg.style.display = 'none';
            }, 5000);

            console.log(`Joining waitlist error: User already on waitlist`);
        } else {
            // Show success message
            let successMessage;
            
            if (page === 1) {
                successMessage = document.getElementById('successMessage-gen');
            } else {
                successMessage = document.getElementById('successMessage-man');
            }
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
    let modal;
    
    if (page === 1) {
        modal = document.getElementById('reservationConflictModal-gen');
    } else {
        modal = document.getElementById('reservationConflictModal-man');
    }
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
    if (page === 1) {
        document.querySelectorAll('.modal-backdrop-gen').forEach(modal => modal.remove());
    } else {
        document.querySelectorAll('.modal-backdrop-man').forEach(modal => modal.remove());
    }

    const backdrop = document.createElement('div');
    if (page === 1) {
        backdrop.className = 'modal-backdrop-gen';
    } else {
        backdrop.className = 'modal-backdrop-man';
    }

    backdrop.addEventListener('click', event => {
        if (event.target === backdrop) {
            backdrop.style.display = 'none';
        }
    });

    const modalContent = document.createElement('div');
    if (page === 1) {
        modalContent.className = 'modal-content-gen';
    } else {
        modalContent.className = 'modal-content-man';
    }

    const closeBtn = document.createElement('span');
    if (page === 1) {
        closeBtn.className = 'modal-close-btn-gen';
    } else {
        closeBtn.className = 'modal-close-btn-man';
    }
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