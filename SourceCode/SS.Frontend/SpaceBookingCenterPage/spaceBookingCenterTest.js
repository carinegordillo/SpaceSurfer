document.addEventListener('DOMContentLoaded', function() {
    
    initSidebar();
    
    const formContainer = document.querySelector('.form-container'); // Adjusted selector to class
    if (formContainer) { 
        const reservationForm = createReservationForm();
        reservationForm.addEventListener('submit', function(event) {
            event.preventDefault(); 
            handleFormSubmit(event);

        });
        formContainer.appendChild(reservationForm);

        
    }

    
    fetchCompanies();

    document.addEventListener('DOMContentLoaded', function() {
        fetchCompanies();
        document.getElementById('companiesList').addEventListener('click', function(event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    
        document.getElementById('reservationForm').addEventListener('submit', handleFormSubmit);
        
    });
});


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
    // Attach event listeners to buttons
    
    document.getElementById('reserveNowBtn').addEventListener('click', handleReserveNowClick);
    document.getElementById('yourReservationsBtn').addEventListener('click', handleYourReservationsClick);

}



function getReservationCenter() {
    alert('Load Reserve Now View');
    fetchCompanies();

    document.addEventListener('DOMContentLoaded', function() {
        fetchCompanies();
        document.getElementById('companiesList').addEventListener('click', function(event) {
            if (event.target && event.target.nodeName === "LI") {
                const companyID = event.target.getAttribute('data-company-id');
                fetchFloorPlans(companyID);
            }
        });
    
        document.getElementById('reservationForm').addEventListener('submit', handleFormSubmit);
        
    });
}


function getReservtionOverview() {


    alert('Load Your Reservations View')
    
    reservationOverviewButtons();
    
}





///RESERVATION OVERVIEW

function reservationOverviewButtons() {
    const content = document.querySelector('.reservation-buttons');
    content.innerHTML = ''; // Clear previous buttons if needed

    const buttons = [
        { id: 'loadActiveReservationsBtn', text: 'Active Reservations', onClickFunction: () => getUsersActiveReservations('hashed_user3') },
        { id: 'loadAllReservationsBtn', text: 'All Reservations', onClickFunction: () => getUsersReservations('hashed_user3') }
    ];

    buttons.forEach(({ id, text, onClickFunction }) => {
        const button = document.createElement('button');
        button.id = id;
        button.textContent = text;
        button.addEventListener('click', onClickFunction);
        content.appendChild(button);
    });
}



function getUsersActiveReservations(userName) {
   
   
    const url = `http://localhost:5005/api/v1/spaceBookingCenter/reservations/ListActiveReservations?userName=${encodeURIComponent(userName)}`;

    fetch(url, {
        method: 'GET',
        headers: {
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
        window.alert("Error fetching reservations");
    });
}

function getUsersReservations(userName) {
   
    const url = `http://localhost:5005/api/v1/spaceBookingCenter/reservations/ListReservations?userName=${encodeURIComponent(userName)}`;

    fetch(url, {
        method: 'GET',
        headers: {
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
        window.alert("Error fetching reservations");
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

function submitModification(reservation) {
    const form = document.getElementById('modifyReservationForm');
    const newStartTime = form.querySelector('#newStartTime').value;
    const newEndTime = form.querySelector('#newEndTime').value;

    const modificationData = {
        reservationID: reservation.reservationID,
        //company id
        companyID: reservation.companyID,
        //floorplan id
        floorPlanID: reservation.floorPlanID,
        //space id
        spaceID: reservation.spaceID,
        reservationStartTime: newStartTime,
        reservationEndTime: newEndTime,
        //status
        status: reservation.status,
        //userhash
        userHash: "hashed_user3"
    };

    // Send the modification request to your backend
    fetch(`http://localhost:5005/api/v1/spaceBookingCenter/reservations/UpdateReservation`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(modificationData),
    })
    .then(response => response.json())
    .then(data => {
        alert('Reservation modified successfully');
        // Close the modal and refresh the reservations list or handle as needed
        document.querySelector('.modal-content').remove();
        getUsersActiveReservations('hashed_user3'); // Reload active reservations
    })
    .catch(error => {
        console.error('Error modifying reservation:', error);
        alert('Error modifying reservation');
    });
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
    document.getElementById('confirmCancel').addEventListener('click', function() {
        submitCancellation(reservation);
    });
    document.getElementById('cancelCancel').addEventListener('click', function() {
        document.querySelector('.modal-content').remove();
    });
}

function submitCancellation(reservation) {

    const modificationData = {
        reservationID: reservation.reservationID,
        //company id
        companyID: reservation.companyID,
        //floorplan id
        floorPlanID: reservation.floorPlanID,
        //space id
        spaceID: reservation.spaceID,
        reservationStartTime: reservation.reservationStartTime,
        reservationEndTime: reservation.reservationEndTime,
        //status
        status: reservation.status,
        //userhash
        userHash: "hashed_user3"
    };

    fetch(`http://localhost:5005/api/v1/spaceBookingCenter/reservations/CancelReservation`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(modificationData),
    })
    .then(response => response.json())
    .then(data => {
        alert('Reservation Cancelled successfully');
        document.querySelector('.modal-content').remove();
        getUsersActiveReservations('hashed_user3'); 
    })
    .catch(error => {
        console.error('Error modifying reservation:', error);
        alert('Error modifying reservation');
    });
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
    checkAvailabilityButton.type = 'button'; // It's not a submit button
    checkAvailabilityButton.textContent = 'Check Availability';
    checkAvailabilityButton.id = 'checkAvailabilityButton';

    checkAvailabilityButton.addEventListener('click', function() {
        checkAvailability() // Replace this with your actual functionality
    });

    
    form.appendChild(checkAvailabilityButton);

    return form;
}
function checkAvailability() {
    window.alert("Checking availability");
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
    fetch(apiUrl)
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









function fetchCompanies() {
    fetch('http://localhost:5001/api/v1/spaceBookingCenter/companies/ListCompanies') 
        .then(response => response.json())
        .then(data => {
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
        })
        .catch(error => console.error('Error fetching companies:', error));
}




function formatTime(timeString) {
   
    const time = timeString.split(':'); 
    const hours = parseInt(time[0], 10);
    const minutes = time[1];
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const formattedHours = hours % 12 || 12; 
    return `${formattedHours}:${minutes} ${ampm}`;
}

function fetchFloorPlans(companyID) {
    fetch(`http://localhost:5001/api/v1/spaceBookingCenter/companies/FloorPlans/${companyID}`) 
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (!Array.isArray(data)) {
                throw new TypeError('Expected an array of floor plans');
            }
            
            const floorPlansContent = document.getElementById('floorPlansContent');
            floorPlansContent.innerHTML = ''; // Clear previous content

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
        })
        .catch(error => console.error('Error fetching floor plans:', error));
}

function handleFormSubmit(event) {
    event.preventDefault(); 

    // Gather data from the form
    const reservationData = {
        companyId: parseInt(document.getElementById('reservation-companyId').value, 10),
        floorPlanId: parseInt(document.getElementById('reservation-floorPlanId').value, 10),
        spaceId: document.getElementById('reservation-spaceId').value,
        reservationStartTime: document.getElementById('reservation-startTime').value,
        reservationEndTime: document.getElementById('reservation-endTime').value,
        userHash: "hashed_user5"

    };


    const requestData = JSON.stringify(reservationData);
    window.alert("making a resertvion with:" + requestData);

    
    fetch('http://localhost:5005/api/v1/spaceBookingCenter/reservations/CreateReservation', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: requestData
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('Reservation created successfully:', data);
        window.alert("Reservation created successfully");
    })
    .catch(error => {
        console.error('Error creating reservation:', error);
        window.alert("Error creating reservation");
    });
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
}



