
// Sets current date
const currentDate = new Date();
var currentView = null;
var reservations = [];

document.getElementById('prev-month').addEventListener('click', () => changeMonth(-1));
document.getElementById('next-month').addEventListener('click', () => changeMonth(1));
document.getElementById('calendar-button').addEventListener('click', calendarView);
document.getElementById('list-button').addEventListener('click', listView);
document.getElementById('confirm-button').addEventListener('click', confirmSelection);
// document.getElementById('space-form-container').addEventListener('click', getReservationOverview);
document.getElementById('reservation-delete-button').addEventListener('click', fetchReservationDeletion);

// Function to fetch user reservations from the API
async function fetchUserReservations(fromDateValue, toDateValue) {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

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

    const url = `http://localhost:5275/api/v1/PersonalOverview/Reservations?fromDate=${fromDateValue}&toDate=${toDateValue}`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json"
            }
        });

        if (!response.ok) {
            throw new Error(`Error fetching user reservations: ${response.status}`);
        }

        const data = await response.json();
        return data;
    }
    catch (error) {
        throw new Error(`Error fetching user reservations: ${error.message}`);
    }
}

async function fetchReservationDeletion() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    var reservationIDValue = document.getElementById('reservation-id').value;

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

    const url = `http://localhost:5275/api/v1/PersonalOverview/ReservationDeletion?ReservationID=${reservationIDValue}`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        if (!response.ok) {
            throw new Error(`Error deleting reservations: ${response.status}`);
        }

        const data = await response.json();
        return data;
    }
    catch (error) {
        throw new Error(`Error deleting reservations: ${error.message}`);
    }

}


// Function to apply date filter and fetch reservations
async function applyDateFilter() {
    var fromDateValue = document.getElementById('from-date').value;
    var toDateValue = document.getElementById('to-date').value;

    try {
        reservations = await fetchUserReservations(fromDateValue, toDateValue);

    } catch (error) {
        console.error(error.message);
    }

}

// Function to handle user selection and trigger reservation fetching
async function confirmSelection() {
    // Handle user selection
    await applyDateFilter();

    if (currentView === 'list') {
        displayReservationList();
        document.getElementById('calendar').style.display = 'none';
        document.getElementById('reservation-list').style.display = 'block';
    }
    if (currentView === 'calendar') {
        createCalendar(currentDate.getFullYear(), currentDate.getMonth());
        document.getElementById('reservation-list').style.display = 'none';
        document.getElementById('calendar').style.display = 'block';
    }
}



// Function to switch to calendar view
function calendarView() {
    currentView = 'calendar';
    document.getElementById('calendar-button').classList.add('selected-button');
    document.getElementById('list-button').classList.remove('selected-button');

}

// Function to switch to reservation list view
function listView() {
    currentView = 'list';
    document.getElementById('list-button').classList.add('selected-button');
    document.getElementById('calendar-button').classList.remove('selected-button');
}

function displayReservationList() {

    var container = document.querySelector("#reservation-list");
    container.innerHTML = "";
    reservations.forEach(reservation => {
        var listItem = document.createElement("div");
        //listItem.setAttribute('class','<class>');
        container.appendChild(listItem);
        addReservation(reservation, listItem)

    });
}

function addReservation(reservation, div) {
    // Get the template element
    const template = document.getElementById("reservation-detail-template");

    // Clone the template's content
    const reservationItem = template.content.cloneNode(true);

    // Access elements inside the clone
    const reservationIDLabel = reservationItem.querySelector("#labelReservationID");
    reservationIDLabel.textContent = reservation.reservationID;

    const companyLabel = reservationItem.querySelector("#labelCompany");
    companyLabel.textContent = reservation.companyName;

    const companyIDLabel = reservationItem.querySelector("#labelCompanyID");
    companyIDLabel.textContent = reservation.companyID;

    const addressLabel = reservationItem.querySelector("#labelAddress");
    addressLabel.textContent = reservation.address;

    const FloorPlanIDLabel = reservationItem.querySelector("#labelFloorPlanID");
    FloorPlanIDLabel.textContent = reservation.floorPlanID;

    const SpaceIDLabel = reservationItem.querySelector("#labelSpaceID");
    SpaceIDLabel.textContent = reservation.spaceID;

    const ReservationDateLabel = reservationItem.querySelector("#labelReservationDate");
    ReservationDateLabel.textContent = reservation.reservationDate;

    const startTimeFormatted = convertTo12HourFormat(reservation.reservationStartTime);
    const endTimeFormatted = convertTo12HourFormat(reservation.reservationEndTime);

    const ReservationStartTimeLabel = reservationItem.querySelector("#labelReservationTimeStart");
    ReservationStartTimeLabel.textContent = startTimeFormatted;

    const ReservationEndTimeLabel = reservationItem.querySelector("#labelReservationTimeEnd");
    ReservationEndTimeLabel.textContent = endTimeFormatted;

    const StatusLabel = reservationItem.querySelector("#labelStatus");
    StatusLabel.textContent = reservation.status;

    // Append the cloned content to the specified div
    div.appendChild(reservationItem);
}


function changeMonth(offset) {
    currentDate.setMonth(currentDate.getMonth() + offset);
    createCalendar(currentDate.getFullYear(), currentDate.getMonth());
}
function createCalendar(year, month) {
    const calendarBody = document.querySelector('#calendar tbody');
    const monthYearContainer = document.querySelector('#month-year');

    // Clear previous calendar
    calendarBody.innerHTML = '';

    // Set the month and year header
    const monthNames = {
        0: 'January', 1: 'February', 2: 'March', 3: 'April', 4: 'May', 5: 'June',
        6: 'July', 7: 'August', 8: 'September', 9: 'October', 10: 'November', 11: 'December'
    };

    monthYearContainer.textContent = `${monthNames[month]} ${year}`;

    // Get the current date
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth();
    const currentDay = currentDate.getDate();
    const currentYear = currentDate.getFullYear();

    // Get the first day of the month
    const firstDay = new Date(year, month, 1).getDay();

    // Get the last day of the month
    const lastDay = new Date(year, month + 1, 0).getDate();

    let dayCounter = 1;

    // Create the calendar rows and cells
    for (let i = 0; i < 6; i++) {
        const row = calendarBody.insertRow();

        for (let j = 0; j < 7; j++) {
            const cell = row.insertCell();

            if (i === 0 && j < firstDay) {
                // Add empty cells for days before the first day of the month
                cell.textContent = '';
            } else if (dayCounter <= lastDay) {
                // Add cells with day numbers
                const dayNumber = document.createElement('div');
                dayNumber.className = 'day-number';
                dayNumber.textContent = dayCounter;

                // Add the day of the month to the top right corner of the cell
                cell.appendChild(dayNumber);

                // Check if there is a reservation for the current day
                const reservation = reservations.find(res => isSameDay(reservationDateFromString(res.reservationDate), new Date(year, month, dayCounter)));

                if (reservation) {
                    // Display reservation details (name of the place and time)
                    const reservationDetails = document.createElement('div');
                    reservationDetails.className = 'reservation-details';

                    const reservationElement = document.createElement('div');
                    reservationElement.className = 'reservationID';
                    reservationElement.textContent = `ReservationID: ${reservation.reservationID}`;
                    reservationDetails.appendChild(reservationElement);

                    const companyElement = document.createElement('div');
                    companyElement.className = 'companyNameID';
                    companyElement.textContent = `Company Name - ID: ${reservation.companyName} - ${reservation.companyID}`;
                    reservationDetails.appendChild(companyElement);

                    const addressElement = document.createElement('div');
                    addressElement.className = 'address';
                    addressElement.textContent = `Address: ${reservation.address}`;
                    reservationDetails.appendChild(addressElement);

                    const startTimeFormatted = convertTo12HourFormat(reservation.reservationStartTime);
                    const endTimeFormatted = convertTo12HourFormat(reservation.reservationEndTime);

                    const timeElement = document.createElement('div');
                    timeElement.className = 'reservationTime';
                    timeElement.textContent = `Reservation Time: ${startTimeFormatted} - ${endTimeFormatted}`;
                    reservationDetails.appendChild(timeElement);

                    cell.appendChild(reservationDetails);
                }

                // Highlight the current date in the current month with a yellow background
                if (month === currentMonth && dayCounter === currentDay && year === currentYear) {
                    cell.classList.add('current-date');
                }

                dayCounter++;
            }
        }
    }
}

function convertTo12HourFormat(time) {
    // Split the time string into hours, minutes, and seconds
    const [hours, minutes, seconds] = time.split(':').map(Number);

    // Determine if it's AM or PM
    const period = hours >= 12 ? 'PM' : 'AM';

    // Convert hours to 12-hour format
    const hours12 = hours % 12 || 12;

    // Format the time as HH:MM AM/PM
    return `${hours12}:${minutes.toString().padStart(2, '0')} ${period}`;
}

function reservationDateFromString(dateString) {
    const [year, month, day] = dateString.split('-');
    return new Date(year, month - 1, day);
}

// Function to check if two dates are the same day
function isSameDay(date1, date2) {
    return (
        date1.getFullYear() === date2.getFullYear() &&
        date1.getMonth() === date2.getMonth() &&
        date1.getDate() === date2.getDate()
    );
}


//function createDeleteReservationForm() {
//    const form = document.createElement('form');
//    form.id = 'deleteReservationForm';
//    form.innerHTML = `
//        <div class="form-group">
//            <label for="reservationIDToDelete">Reservation ID:</label>
//            <input type="number" id="reservationIDToDelete" required>
//        </div>
//        <input type="submit" value="Delete Reservation">
//    `;
//    form.addEventListener('submit', handleDeleteReservationFormSubmit);
//    return form;
//}


//async function handleDeleteReservationFormSubmit(event) {
//    event.preventDefault();

//    const reservationID = document.getElementById('reservation-id').value;
//    var accessToken = sessionStorage.getItem('accessToken');

//    const isTokenValid = await checkTokenExpiration(accessToken);
//    if (!isTokenValid) {
//        logout();
//        return;
//    }

//    var idToken = sessionStorage.getItem('idToken');

//    var parsedIdToken = JSON.parse(idToken);
//    var userHash = parsedIdToken.Username;


//    try {
//        const response = await fetch(`http://localhost:5005/api/v1/spaceBookingCenter/reservations/DeleteReservation`, {
//            method: 'POST',
//            headers: {
//                'Authorization': `Bearer ${accessToken}`,
//                'Content-Type': 'application/json',
//            },
//            body: JSON.stringify({ userHash, reservationID: parseInt(reservationID, 10) }),
//        });

//        if (!response.ok) {
//            throw new Error(`HTTP error! status: ${response.status}`);
//        }

//        const responseData = await response.json();
//        if (responseData.newToken) {
//            accessToken = data.newToken;
//            sessionStorage.setItem('accessToken', accessToken);
//            console.log('New access token stored:', accessToken);
//        }
//        if (responseData.hasError) {
//            console.error(`Reservation deletion error: ${responseData.errorMessage}`);
//            onError(`Reservation deletion error: ${responseData.errorMessage}`);
//        } else {
//            console.log('Reservation deleted successfully');
//            onSuccess('Reservation deleted successfully!');

//        }
//    } catch (error) {
//        console.error('Error deleting reservation:', error);
//        onError(`Error deleting reservation: ${error}`);
//    }
//}


//function getReservationOverview() {
//    const deleteFormContainer = document.querySelector('.space-form-container');
//    if (deleteFormContainer) {
//        const deleteReservationForm = createDeleteReservationForm();
//        deleteFormContainer.appendChild(deleteReservationForm);
//    }
//}