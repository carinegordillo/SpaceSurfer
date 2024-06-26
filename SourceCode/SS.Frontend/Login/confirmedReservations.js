function checkTokenExpiration(accessToken) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
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
        showModal('Confirmation sent successfully!', true);
        return data;
    } catch (error) {
        console.error('Error sending confirmation:', error);
        showModal('Failed to send confirmation: ' + error.message, false);
    }
}

// Function to handle resending a confirmation
async function resendConfirmation(reservationID) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
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
        showModal('Reservation confirmed successfully:', true);
        return data;
    } catch (error) {
        console.error('Error confirming reservation:', error);
        showModal('Error confirming reservation: ' + error.message, false);
    }
}

// Function to handle deleting a confirmation
async function deleteConfirmation(reservationID) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    const url = `${baseUrl}/DeleteConfirmation/${reservationID}`;
    try {
        const response = await fetch(url, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + accessToken
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
        listConfirmations(); // Refresh the list
        return data;
    } catch (error) {
        console.error('Error deleting confirmation:', error);
    }
}

// Function to handle canceling a confirmation
async function cancelConfirmation(hashedUsername, reservationID) {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
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
        showModal('Confirmation cancelled successfully!', true);
        listConfirmations(); // Refresh the list
        return data;
    } catch (error) {
        console.error('Error canceling confirmation:', error);
        showModal('Error canceling confirmation: ' + error.message, false); // Using modal to show error message
    }
}


// Function to handle listing all confirmations for a user
async function listConfirmations() {
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const reservationConfirmationUrl = appConfig.api.EmailConfirmation; 
    const baseUrl = `${reservationConfirmationUrl}/api/v1/reservationConfirmation`;
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    if (!idToken) {
        console.error('idToken is missing');
        return; // Exit the function if there is no idToken.
    }
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
        const data = await response.json();
        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }
        console.log(data);
        

        const confirmations = data.confirmations;
        const detailsContainer = document.querySelector('.reservations-list');
        if (!detailsContainer) {
            console.error('The "reservations-list" element was not found in the DOM.');
            return;
        }
        console.log(confirmations);

        // Check if confirmations is an array and has items
        if (Array.isArray(confirmations) && confirmations.length > 0) {
            console.log('Starting to create reservation details...');
            detailsContainer.innerHTML = ''; // Clear previous entries
            // Iterate over the confirmations array and create HTML for each
            confirmations.forEach(reservation => {
                console.log('Creating detail for reservation ID:', reservation.reservationID);
                const reservationDiv = document.createElement('div');
                reservationDiv.classList.add('confirmation-items');

                // Format the reservation start time
                const formattedStartTime = new Date(reservation.reservationStartTime).toLocaleString();
                const formattedEndTime = new Date(reservation.reservationEndTime).toLocaleString();
            
                 // Create the HTML content for the reservation detail
                reservationDiv.innerHTML = `
                    <p>Reservation ID: ${reservation.reservationID}</p>
                    <p>Company ID: ${reservation.companyID}</p>
                    <p>Floor Plan ID: ${reservation.floorPlanID}</p>
                    <p>Space ID: ${reservation.spaceID}</p>
                    <p>Start Time: ${formattedStartTime}</p>
                    <p>End Time: ${formattedEndTime}</p>
                    <div class="button-group">
                        <button data-reservation-id="${reservation.reservationID}" class="delete-btn">Delete Confirmation</button>
                        <button data-username="${username}" data-reservation-id="${reservation.reservationID}" class="cancel-btn">Cancel Confirmation</button>
                    </div>
                `;

                // Append the reservation detail to the details container
                detailsContainer.appendChild(reservationDiv);
                console.log(reservation);
            });
            console.log('Finished creating reservation details.');
        }else{
            detailsContainer.innerHTML = '<p>You currently do not have confirmed reservations.</p>';
        }

        console.log('List of confirmations retrieved successfully:', data);
        //showModal('List of confirmations retrieved successfully', true);
    } catch (error) {
        console.error('Error listing confirmations:', error);
        showModal('Error listing confirmations: ' + error.message, false);
    }
}

function showModal(message, isSuccess = true) {
    const modalElement = document.createElement('div');
    modalElement.style.position = 'fixed';
    modalElement.style.top = '20%';
    modalElement.style.left = '50%';
    modalElement.style.transform = 'translate(-50%, -50%)';
    modalElement.style.zIndex = '1000';
    modalElement.style.padding = '20px';
    modalElement.style.backgroundColor = isSuccess ? 'lightgreen' : 'salmon';
    modalElement.innerText = message;

    const closeButton = document.createElement('button');
    closeButton.innerText = '&times;';
    closeButton.onclick = function() {
        modalElement.remove();
    };
    modalElement.appendChild(closeButton);

    document.body.appendChild(modalElement);
}

document.addEventListener('DOMContentLoaded', attachClickHandlersToDetailsContainer);
function attachClickHandlersToDetailsContainer() {
    const detailsContainer = document.querySelector('.reservations-list');
    if (detailsContainer) {
        detailsContainer.addEventListener('click', handleReservationButtonClick);
    }
}
function handleReservationButtonClick(event) {
    const target = event.target;
    if (target.tagName === 'BUTTON') {
        const reservationID = target.getAttribute('data-reservation-id');
        if (target.className.includes('delete-btn')) {
            openDialogDelete(reservationID);
        } else if (target.className.includes('cancel-btn')) {
            const username = target.getAttribute('data-username');
            openDialogCancel(username, reservationID);
        }
    }
}

function openDialogCancel(username, reservationID) {

    // Create modal elements
    const modal = document.createElement('div');
    modal.id = 'confirmModal';
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');

    const closeBtn = document.createElement('span');
    closeBtn.classList.add('close-button');
    closeBtn.innerHTML = '&times;';
    closeBtn.onclick = () => leaveModal();

    const message = document.createElement('p');
    message.innerText = 'Are you sure you want to cancel the confirmation for ';

    const reservationIdText = document.createElement('p');
    reservationIdText.innerHTML = `Reservation: <span id="reservationId">${reservationID}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.dataset.testid = "yesBtn"; 
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => {
        cancelConfirmation(username, reservationID);
        fetchInsertUsedFeature('Confirmations');
        leaveModal();
    };

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.onclick = () => leaveModal();

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
    modal.style.color = "#010100"; 
    modal.style.boxShadow = 'none';
}

function openDialogDelete(reservationID) {

    // Create modal elements
    const modal = document.createElement('div');
    modal.id = 'confirmModal';
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');

    const closeBtn = document.createElement('span');
    closeBtn.classList.add('close-button');
    closeBtn.innerHTML = '&times;';
    closeBtn.addEventListener('click', leaveModal);


    const message = document.createElement('p');
    message.innerText = 'You cannot reconfirm the reservation once it is deleted. Are you sure you want to delete the confirmation for:';

    const reservationIdText = document.createElement('p');
    reservationIdText.innerHTML = `Reservation: <span id="reservationId">${reservationID}</span>`;

    const yesBtn = document.createElement('button');
    yesBtn.innerText = 'Yes';
    yesBtn.onclick = () => {
        deleteConfirmation(reservationID);
        fetchInsertUsedFeature('Confirmations');
        leaveModal();
    };

    const noBtn = document.createElement('button');
    noBtn.innerText = 'No';
    noBtn.addEventListener('click', leaveModal);


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
    modal.style.color = "#010100"; 
    modal.style.boxShadow = 'none';
}

function leaveModal() {
    console.log('Close modal function called');
    const modal = document.getElementById('confirmModal');
    if (modal) {
        console.log('Modal element found in DOM');
        //modal.style.display = 'none';
        //modal.innerHTML = '';
        modal.remove(); 
    } else {
        console.log('Modal element not found in DOM');
    }
}

// Function to insert the used feature after every use
async function fetchInsertUsedFeature(featureName) {
    // Retrieve tokens from sessionStorage
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    // Check if tokens are present
    if (!idToken) {
        console.error('ID token not found.');
        return;
    }

    if (!username) {
        console.error('Username not found in token.');
        return;
    }

    if (!accessToken) {
        console.error('Access token not found.');
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const SOurl = appConfig.api.SystemObservability;
    // Construct the URL with the provided time span
    const url = `${SOurl}/api/v1/SystemObservability/UsedFeatureInsertion?FeatureName=${featureName}`;

    try {
        // Fetch data from the API
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        // Parse and return the response data
        const data = await response.json();

        // Check if there's any error in the response
        if (data.hasError) {
            throw new Error(`Error inserting the feature use: ${response.status}`);
        }

        return data;
    }
    catch (error) {
        // Catch and log any errors that occur during the process
        throw new Error(`Caught an error inserting the feature use: ${error.message}`);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('initConfirmationsButton').addEventListener('click', () => {
        listConfirmations();
        fetchInsertUsedFeature('Confirmations');
    });
});
