
function checkTokenExpiration(accessToken) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const waitlistUrl = appConfig.api.Waitlist;  
    try {
        var response = fetch(`${waitlistUrl}/api/waitlist/checkTokenExp`, {
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

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        console.log("no token expiration ");
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const waitlistUrl = appConfig.api.Waitlist;  
    try {
        const response = await fetch(`${waitlistUrl}/api/waitlist/getWaitlists`, {
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
        const reservationsList = document.getElementById('reservations-list');
        reservationsList.innerHTML = '';

        // Check if waitlistedReservations exist before iterating over it
        if (Array.isArray(data) && data.length > 0) {
            data.forEach((reservation, index) => {
                const listItem = document.createElement('li');
                listItem.textContent = `${reservation.companyName} (${reservation.spaceID})`;
                listItem.classList.add('waitlist-item');

                listItem.addEventListener('click', () => displayReservationDetails(reservation), fetchInsertUsedFeature('Waitlist'));

                reservationsList.appendChild(listItem);
                console.log(data);
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
async function displayReservationDetails(reservation) {
    const startDate = new Date(reservation.startTime);
    const endDate = new Date(reservation.endTime);

    // Format date and time
    const formattedStartDate = startDate.toLocaleDateString('en-US', { month: 'long', day: 'numeric' });
    const formattedStartTime = startDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true });
    const formattedEndTime = endDate.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true });

    // Display reservation details in the reservation-details container
    const reservationDetails = document.getElementById('reservation-details');

    //FETCH//

    var accessToken = sessionStorage.getItem('accessToken');
    
    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        console.log("no token expiration ");
        logout();
        return;
    }
    var compName = reservation.companyName
    var data;
    console.log(compName);
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const waitlistUrl = appConfig.api.Waitlist;  
    try {
        const response = await fetch(`${waitlistUrl}/api/waitlist/getFloorplan?cid=${reservation.companyID}&fid=${reservation.floorID}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        data = await response.json();

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log("floorplan name: " + response.floorPlanName);

    } catch (error) {
        console.error('Display reservation details error:', error);
    }

    /////////

    data.forEach(floorPlan => {
        reservationDetails.innerHTML = `
        <h3>${reservation.companyName}</h3>
        <img src="data:image/png;base64,${floorPlan.floorPlanImageBase64}" alt="${floorPlan.floorPlanName}" />
        <p>Space ID: ${reservation.spaceID}</p>
        <p>Date: ${formattedStartDate}</p>
        <p>From: ${formattedStartTime}</p>
        <p>To: ${formattedEndTime}</p>
        <p>Current position on waitlist: ${reservation.position}</p>
        <button id="leaveWaitlistBtn">Leave Waitlist</button>
    `;
    });

    // Add click event listener to the leave waitlist button
    const leaveWaitlistBtn = document.getElementById('leaveWaitlistBtn');
    leaveWaitlistBtn.addEventListener('click', () => openDialog(reservation), fetchInsertUsedFeature('Waitlist'));
}


async function getReservationId(reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        console.log("no token expiration ");
        logout();
        return;
    }

    var cname = reservation.companyName;
    var sid = reservation.spaceID;
    var stime = reservation.startTime;
    var etime = reservation.endTime;
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const waitlistUrl = appConfig.api.Waitlist;  
    try {
        const response = await fetch(`${waitlistUrl}/api/waitlist/getResId`, {
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
    closeBtn.onclick = closeLeaveModal;

    const message = document.createElement('p');
    message.innerText = 'Are you sure you want to leave the waitlist for:';

    const spaceIdText = document.createElement('p');
    spaceIdText.innerHTML = `Space: <span id="spaceId">${spaceId}</span>`;

    const timeText = document.createElement('p');
    timeText.innerHTML = `From: <span id="reservationTime">${startTime}</span> - <span id="reservationEndTime">${endTime}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => leaveWaitlist(true, reservation), fetchInsertUsedFeature('Waitlist');

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.onclick = closeLeaveModal;

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


async function leaveWaitlist(confirm, reservation) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        console.log("no token expiration ");
        logout();
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const waitlistUrl = appConfig.api.Waitlist;  

    if (confirm) {
        try {
            var resId = await getReservationId(reservation);
            const response = await fetch(`${waitlistUrl}/api/waitlist/leaveWaitlist`, {
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
            const reservationsList = document.getElementById('reservations-list');
            const listItemToRemove = document.querySelector(`.waitlist-item[data-space-id="${reservation.spaceID}"]`);
            if (listItemToRemove) {
                listItemToRemove.remove();
            }

            // Clear the reservation details container
            const reservationDetails = document.getElementById('reservation-details');
            reservationDetails.innerHTML = '';
            setTimeout(closeLeaveModal, 3000); // 3000 milliseconds = 3 seconds
        } catch (error) {
            console.error('Error leaving waitlist:', error);
        }
    }
}

//document.addEventListener('DOMContentLoaded', () => {
//    document.getElementById('initWaitlistButton').addEventListener('click', () => {
//        displayWaitlistedReservations();
//    });
//});