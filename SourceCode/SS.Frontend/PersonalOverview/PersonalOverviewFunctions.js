// JavaScript source code

// Sets current date
let currentDate = new Date();

// Test reservations to check if the view works
let reservations = [
    { date: new Date(2023, 11, 5), place: 'Example Place 1', time: '10:00 AM' }, // December
    { date: new Date(2024, 1, 15), place: 'Example Place 2', time: '02:30 PM' }, // February
];

function changeMonth(offset) {
    currentDate.setMonth(currentDate.getMonth() + offset);
    createCalendar(currentDate.getFullYear(), currentDate.getMonth());
}

function showCalendar() {
    document.getElementById('calendar').style.display = 'block';
    document.getElementById('reservation-list').style.display = 'none';
    // Initial calendar display
    createCalendar(currentDate.getFullYear(), currentDate.getMonth());
}

function showReservationList() {
    document.getElementById('calendar').style.display = 'none';
    document.getElementById('reservation-list').style.display = 'block';
    displayReservationList();
}

// Function to create and update the calendar
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

                // Check if there is a reservation for the current day
                const reservation = reservations.find(res => isSameDay(res.date, new Date(year, month, dayCounter)));

                if (reservation) {
                    // Display reservation details (name of the place and time)
                    const reservationDetails = document.createElement('div');
                    reservationDetails.className = 'reservation-details';

                    const placeElement = document.createElement('div');
                    placeElement.className = 'place';
                    placeElement.textContent = `Place: ${reservation.place}`;
                    reservationDetails.appendChild(placeElement);

                    const timeElement = document.createElement('div');
                    timeElement.className = 'time';
                    timeElement.textContent = `Time: ${reservation.time}`;
                    reservationDetails.appendChild(timeElement);

                    cell.appendChild(reservationDetails);
                }

                // Highlight the current date in the current month with a yellow background
                if (month === currentMonth && dayCounter === currentDay && year === currentYear) {
                    cell.classList.add('current-date');
                }

                cell.appendChild(dayNumber);

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
        const reservationItem = document.createElement('div');
        reservationItem.textContent = `${reservation.date.toLocaleDateString()} - Place: ${reservation.place}, Time: ${reservation.time}`;
        reservationListContainer.appendChild(reservationItem);
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

