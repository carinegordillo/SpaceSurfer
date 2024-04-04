// Function to fetch waitlisted reservations and display them
async function displayWaitlistedReservations() {
    const accessToken = sessionStorage.getItem('accessToken');
    const isTokenValid = checkTokenExpiration(accessToken);
    if (!isTokenValid) {
        logout();
        return;
    }

    try {
        const response = await fetch(`http://localhost:5099//api/waitlist/getWaitlists`, {
            method: 'GET', // Assuming GET is the method since it wasn't specified but commonly used for fetching data
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

        // Clear the existing list of waitlisted reservations
        const reservationsList = document.getElementById('reservations-list');
        reservationsList.innerHTML = '';

        // Iterate through the waitlisted reservations and create list items
        data.waitlistedReservations.forEach((reservation, index) => {
            const listItem = document.createElement('li');
            listItem.textContent = `${reservation.companyName} (${reservation.spaceID})`;
            listItem.classList.add('waitlist-item');

            // Add click event listener to display reservation details on click
            listItem.addEventListener('click', () => displayReservationDetails(reservation));

            reservationsList.appendChild(listItem);
        });
    } catch (error) {
        console.error('Error fetching waitlisted reservations:', error);
    }
}

// Function to display reservation details
function displayReservationDetails(reservation) {
    // Display reservation details in the reservation-details container
    const reservationDetails = document.getElementById('reservation-details');
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
    leaveWaitlistBtn.addEventListener('click', () => confirmLeaveWaitlist(reservation));
}

// Function to confirm leaving the waitlist
function confirmLeaveWaitlist(reservation) {
    // Show confirmation pop-up
    const confirmPopup = confirm('Are you sure you want to leave the waitlist?');
    if (confirmPopup) {
        // Call API endpoint to leave the waitlist
        // Handle success or error response accordingly
    }
}

function logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    window.location.href = '/UnAuthnAbout/about.html';
}

// Call the function to display waitlisted reservations when the page loads
document.addEventListener('DOMContentLoaded', () => {
    displayWaitlistedReservations();
});
function test() {
    var accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Access token is not available.');
        return;
    }

    $.ajax({
        url: 'http://localhost:5099/api/waitlist/checkTokenExp',
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken
        },
        contentType: 'application/json',
        success: function (response) {
            console.log(response);
            if (response == true) {
                logout();
            }
            else {
                $.ajax({
                    url: 'http://localhost:5099/api/waitlist/button',
                    type: 'GET',
                    headers: {
                        'Authorization': 'Bearer ' + accessToken
                    },
                    contentType: 'application/json',
                    success: function (response) {
                        console.log(response);
                        if (response) {
                            var message = "Test successful";
                            var messageElement = document.createElement('p');
                            messageElement.textContent = message;
                            messageElement.style.color = 'green';

                            var testSection = document.getElementById('testSection');
                            testSection.appendChild(messageElement);

                            if (response.newToken) {
                                sessionStorage.setItem('accessToken', response.newToken);
                                console.log('New access token stored:', response.newToken);
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Error:', error);
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}
