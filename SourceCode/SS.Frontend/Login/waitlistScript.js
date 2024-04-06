
async function checkTokenExpiration(accessToken) {
    try {
        var response = await fetch('http://localhost:5099/api/waitlist/checkTokenExp', {
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

// Function to fetch waitlisted reservations and display them
async function displayWaitlistedReservations() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = await checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    try {
        const response = await fetch(`http://localhost:5099/api/waitlist/getWaitlists`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(username)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        var data = await response.json();

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log(data);

        // Clear the existing list of waitlisted reservations
        let reservationsList;
        if (page === 1) {
            reservationsList = document.getElementById('reservations-list-gen');
        } else {
            reservationsList = document.getElementById('reservations-list-man');
        }
        reservationsList.innerHTML = '';

        // Check if waitlistedReservations exist before iterating over it
        if (Array.isArray(data) && data.length > 0) {
            data.forEach((reservation, index) => {
                const listItem = document.createElement('li');
                listItem.textContent = `${reservation.companyName} (${reservation.spaceID})`;
                listItem.classList.add('waitlist-item');

                listItem.addEventListener('click', () => displayReservationDetails(reservation));

                reservationsList.appendChild(listItem);
            });
        } else {
            // If there are no waitlisted reservations, display a message
            reservationsList.innerHTML = '<p>You are currently not waitlisted for any reservations.</p>';
        }
    } catch (error) {
        console.error('Error fetching waitlisted reservations:', error);
    }
}

// Function to display reservation details
function displayReservationDetails(reservation) {
    // Display reservation details in the reservation-details container
    let reservationDetails;
    if (page === 1) {
        reservationDetails = document.getElementById('reservation-details-gen');
    } else {
        reservationDetails = document.getElementById('reservation-details-man');
    }
    reservationDetails.innerHTML = `
        <h3>${reservation.companyName}</h3>
        <p>Space ID: ${reservation.spaceID}</p>
        <p>From: ${reservation.startTime}</p>
        <p>To: ${reservation.endTime}</p>
        <img src="path_to_floor_plan_image" alt="Floor Plan">
        <p>Current position on waitlist: ${reservation.position}</p>
        <button id="leaveWaitlistBtn">Leave Waitlist</button>
    `;

    // Add click event listener to the leave waitlist button
    const leaveWaitlistBtn = document.getElementById('leaveWaitlistBtn');
    leaveWaitlistBtn.addEventListener('click', () => openDialog(reservation));
}

async function getReservationId(reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = await checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    var cname = reservation.companyName;
    var sid = reservation.spaceID;
    var stime = reservation.startTime;
    var etime = reservation.endTime;

    try {
        const response = await fetch(`http://localhost:5099/api/waitlist/getResId`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                companyName: cname,
                spaceId: sid,
                start: stime,
                end: etime
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        var data = await response.json();

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        return data;
    } catch (error) {
        console.error('Error leaving waitlist:', error);
    }
}

function openDialog(reservation) {
    var spaceId = reservation.spaceID;
    var startTime = reservation.startTime;
    var endTime = reservation.endTime;

    // Create modal elements
    const modal = document.createElement('div');
    modal.id = 'confirmModal';
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');

    const closeBtn = document.createElement('span');
    closeBtn.classList.add('close-button');
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = closeModal;

    const message = document.createElement('p');
    message.innerText = 'Are you sure you want to leave the waitlist for:';

    const spaceIdText = document.createElement('p');
    spaceIdText.innerHTML = `Space: <span id="spaceId">${spaceId}</span>`;

    const timeText = document.createElement('p');
    timeText.innerHTML = `From: <span id="reservationTime">${startTime}</span> - <span id="reservationEndTime">${endTime}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => leaveWaitlist(true, reservation);

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.onclick = closeModal;

    // Append elements
    modalContent.appendChild(closeBtn);
    modalContent.appendChild(message);
    modalContent.appendChild(spaceIdText);
    modalContent.appendChild(timeText);
    modalContent.appendChild(yesBtn);
    modalContent.appendChild(noBtn);

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    // Show the modal
    modal.style.display = 'block';
}

function closeModal() {
    const modal = document.getElementById('confirmModal');
    if (modal) {
        modal.remove();
    }
    displayWaitlistedReservations();
}

async function leaveWaitlist(confirm, reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = await checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    if (confirm) {
        try {
            var resId = await getReservationId(reservation);
            const response = await fetch(`http://localhost:5099/api/waitlist/leaveWaitlist`, {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + accessToken,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    UserHash: username,
                    ReservationId: resId
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            var data = await response.json();

            if (data.newToken) {
                accessToken = data.newToken;
                sessionStorage.setItem('accessToken', accessToken);
                console.log('New access token stored:', accessToken);
            }

            // Remove the reservation from the waitlisted reservations container
            let reservationsList;
            if (page === 1) {
                reservationsList = document.getElementById('reservations-list-gen');
            } else {
                reservationsList = document.getElementById('reservations-list-man');
            }
            const listItemToRemove = document.querySelector(`.waitlist-item[data-space-id="${reservation.spaceID}"]`);
            if (listItemToRemove) {
                listItemToRemove.remove();
            }

            // Clear the reservation details container
            let reservationDetails;
            if (page === 1) {
                reservationDetails = document.getElementById('reservation-details-gen');
            } else {
                reservationDetails = document.getElementById('reservation-details-man');
            }
            reservationDetails.innerHTML = '';

        } catch (error) {
            console.error('Error leaving waitlist:', error);
        }
    }
    closeModal();
}

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('initWaitlistButtonGen').addEventListener('click', () => {
        displayWaitlistedReservations();
    });
    document.getElementById('initWaitlistButtonManager').addEventListener('click', () => {
        displayWaitlistedReservations();
    });
});