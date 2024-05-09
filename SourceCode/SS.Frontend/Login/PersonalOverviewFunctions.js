
// Sets current date
const currentDate = new Date();
var currentView = null;
var reservations = [];

document.getElementById('prev-month').addEventListener('click', () => changeMonth(-1));
document.getElementById('next-month').addEventListener('click', () => changeMonth(1));
document.getElementById('calendar-button').addEventListener('click', calendarView);
document.getElementById('list-button').addEventListener('click', listView);
document.getElementById('confirm-button').addEventListener('click', confirmSelection);
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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const OverviewUrl = appConfig.api.PersonalOverview; 

    const url = `${OverviewUrl}/api/v1/PersonalOverview/Reservations?fromDate=${fromDateValue}&toDate=${toDateValue}`;

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
    var errorPrompt = document.getElementById('error-prompt');

    //resets error prompt
    errorPrompt.textContent = "";
    errorPrompt.style.display = "none";

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

    // Check if Reservation ID is empty or not a number
    if (!reservationIDValue || isNaN(reservationIDValue)) {
        // Show prompt
        errorPrompt.textContent = "Please enter the Reservation ID.";
        errorPrompt.style.display = "block";
        return;
    }
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const OverviewUrl = appConfig.api.PersonalOverview; 

    const url = `${OverviewUrl}/api/v1/PersonalOverview/ReservationDeletion?ReservationID=${reservationIDValue}`;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ` + accessToken,
                'Content-Type': "application/json",
            }
        });

        const data = await response.json();

        if (data.hasError) {
            errorPrompt.textContent = 'Something went wrong. Please try again.';
            errorPrompt.style.display = 'block';
            throw new Error(`Error deleting reservations: ${response.status}`);
        }

        errorPrompt.textContent = 'Reservetion ID ' + reservationIDValue + ' was deleted.';
        errorPrompt.style.display = 'block';
        return data;
    }
    catch (error) {
        errorPrompt.textContent = 'Something went wrong. Please try again.';
        errorPrompt.style.display = 'block';
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

    // Check if a view is selected
    if (!currentView) {
        // Show the prompt
        document.getElementById("view-prompt").style.display = "block";
        return; // Stops execution
    }

    // Handle user selection
    await applyDateFilter();

    if (currentView === 'list') {
        displayReservationList();
        document.getElementById('calendar-poc').style.display = 'none';
        document.getElementById('reservation-list-poc').style.display = 'block';
        document.getElementById('view-prompt').style.display = 'none';
    }
    if (currentView === 'calendar') {
        createCalendar(currentDate.getFullYear(), currentDate.getMonth());
        document.getElementById('reservation-list-poc').style.display = 'none';
        document.getElementById('calendar-poc').style.display = 'block';
        document.getElementById('view-prompt').style.display = 'none';
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

    var container = document.querySelector("#reservation-list-poc");
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

    // Add the appropriate class based on the status value
    if (reservation.status === "Active") {
        StatusLabel.classList.add("active");
    } else if (reservation.status === "Cancelled") {
        StatusLabel.classList.add("cancelled");
    } else if (reservation.status === "Passed") {
        StatusLabel.classList.add("passed");
    }

    // Append the cloned content to the specified div
    div.appendChild(reservationItem);
}


function changeMonth(offset) {
    currentDate.setMonth(currentDate.getMonth() + offset);
    createCalendar(currentDate.getFullYear(), currentDate.getMonth());
}
function createCalendar(year, month) {
    const calendarBody = document.querySelector('#calendar-poc tbody');
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