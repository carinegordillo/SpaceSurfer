// JavaScript source code

// Sets current date
let currentDate = new Date();
let fromDate = null;
let toDate = null;
let currentView = null;

// Test reservations to check if the view works
let reservations = [
    { date: new Date(2023, 11, 5), place: 'Example Place 1', time: '10:00 AM' }, // December
    { date: new Date(2024, 1, 15), place: 'Example Place 2', time: '02:30 PM' },
    { date: new Date(2024, 12, 5), place: 'Example Place 3', time: '10:00 AM' }, // December
    { date: new Date(2024, 7, 15), place: 'Example Place 4', time: '02:30 PM' },
    { date: new Date(2024, 5, 25), place: 'Example Place 5', time: '10:00 AM' }, // December
    { date: new Date(2025, 1, 15), place: 'Example Place 6', time: '02:30 PM' } // February
];


document.getElementById('from-date').addEventListener('change', applyDateFilter);
document.getElementById('to-date').addEventListener('change', applyDateFilter);

function selectCalendar() {
    currentView = 'calendar';
    document.getElementById('calendar-button').classList.add('selected-button'); // Highlight calendar button
    document.getElementById('list-button').classList.remove('selected-button'); // Remove highlight from list button
    applyDateFilter(); // Apply date filter and update confirm button visibility
}

function selectReservationList() {
    currentView = 'list';
    document.getElementById('list-button').classList.add('selected-button'); // Highlight list button
    document.getElementById('calendar-button').classList.remove('selected-button'); // Remove highlight from calendar button
    applyDateFilter(); // Apply date filter and update confirm button visibility
}
function confirmSelection() {
    if (currentView === null) {
        alert("Please select a view (calendar or list).");
        return;
    }

    if (currentView === 'calendar') {
        showCalendar();
    } else if (currentView === 'list') {
        showReservationList();
    }

    // Reapply date filters if they are set
    applyDateFilter();
}

function applyDateFilter() {
    if (currentView === null) {
        alert("Please select a view (calendar or list).");
        return;
    }

    const fromDateInput = document.getElementById('from-date').value;
    const toDateInput = document.getElementById('to-date').value;

    // Parse the input values to Date objects
    fromDate = fromDateInput ? new Date(fromDateInput) : null;
    toDate = toDateInput ? new Date(toDateInput) : null;

    console.log("From Date:", fromDate);
    console.log("To Date:", toDate);

    // Update the appropriate view based on the current view selection
    if (currentView === 'calendar') {
        createCalendar(currentDate.getFullYear(), currentDate.getMonth(), fromDate, toDate);
    } else if (currentView === 'list') {
        displayReservationList();
    }
}




// Function to display calendar
function showCalendar() {
    document.getElementById('calendar').style.display = 'block';
    document.getElementById('reservation-list').style.display = 'none';
    applyDateFilter(); // Apply date filter when switching view
}

// Function to display reservation list
function showReservationList() {
    document.getElementById('calendar').style.display = 'none';
    document.getElementById('reservation-list').style.display = 'block';
    applyDateFilter(); // Apply date filter when switching view
}

function displayReservationList() {
    const reservationListContainer = document.getElementById('reservation-list');
    reservationListContainer.innerHTML = '';

    reservations.forEach(reservation => {
        // Check if the reservation date falls within the selected date range
        if ((!fromDate || reservation.date >= fromDate) && (!toDate || reservation.date <= toDate)) {
            const reservationItem = document.createElement('div');
            reservationItem.textContent = `${reservation.date.toLocaleDateString()} - Place: ${reservation.place}, Time: ${reservation.time}`;
            reservationListContainer.appendChild(reservationItem);
        }
    });
}

function changeMonth(offset) {
    currentDate.setMonth(currentDate.getMonth() + offset);
    createCalendar(currentDate.getFullYear(), currentDate.getMonth());
}

// Function to create and update the calendar
function createCalendar(year, month, fromDate, toDate) {
    const calendarBody = document.querySelector('#calendar tbody');
    const monthYearContainer = document.querySelector('#month-year');

    // Clear previous calendar
    calendarBody.innerHTML = '';

    // Set the month and year header
    const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    monthYearContainer.textContent = `${monthNames[month]} ${year}`;

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

                const currentDate = new Date(year, month, dayCounter);

                // Check if the date is within the selected date range
                if ((!fromDate || !toDate) || (fromDate && toDate && currentDate >= fromDate && currentDate <= toDate) || (fromDate && currentDate >= fromDate) || (toDate && currentDate <= toDate)) {
                    cell.appendChild(dayNumber);

                    // Check if there's a reservation for this date
                    const reservation = reservations.find(res => {
                        const resDate = new Date(res.date);
                        return resDate.getFullYear() === year && resDate.getMonth() === month && resDate.getDate() === dayCounter;
                    });

                    if (reservation) {
                        // Display reservation details
                        const reservationDetails = document.createElement('div');
                        reservationDetails.className = 'reservation-details';
                        reservationDetails.textContent = `${reservation.place} - ${reservation.time}`;
                        cell.appendChild(reservationDetails);
                    }
                } else {
                    cell.classList.add('out-of-range');
                }

                dayCounter++;
            }
        }
    }
}





// Function to display reservation list
function displayReservationList() {
    const reservationListContainer = document.getElementById('reservation-list');
    reservationListContainer.innerHTML = '';

    reservations.forEach(reservation => {
        // Check if the reservation date falls within the selected date range
        if ((!fromDate || reservation.date >= fromDate) && (!toDate || reservation.date <= toDate)) {
            const reservationItem = document.createElement('div');
            reservationItem.classList.add('reservation-item'); // Add the 'reservation-item' class
            reservationItem.textContent = `${reservation.date.toLocaleDateString()} - Place: ${reservation.place}, Time: ${reservation.time}`;
            reservationListContainer.appendChild(reservationItem);
        }
    });
}


// Function to check if two dates are the same day
function isSameDay(date1, date2) {
    return (
        date1.getFullYear() === date2.getFullYear() &&
        date1.getMonth() === date2.getMonth() &&
        date1.getDate() === date2.getDate()
    );
}

