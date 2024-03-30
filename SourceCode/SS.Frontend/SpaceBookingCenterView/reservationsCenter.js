
document.addEventListener('DOMContentLoaded', function() {

    document.getElementById('loadActiveReservationsBtn').addEventListener('click', function() {
        getUsersActiveReservations('hashed_user3');
    });

    document.getElementById('loadAllReservationsBtn').addEventListener('click', function() {
        getUsersReservations('hashed_user3');
    });
    
});

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

        renderReservations(data, '.dynamic-content');
        
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

        renderReservations(data, '.dynamic-content');
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


/// modify reservation

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




