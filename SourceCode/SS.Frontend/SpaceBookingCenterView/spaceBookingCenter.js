

document.addEventListener('DOMContentLoaded', function() {
    fetchCompanies();
    document.getElementById('companiesList').addEventListener('click', function(event) {
        if (event.target && event.target.nodeName === "LI") {
            const companyID = event.target.getAttribute('data-company-id');
            fetchFloorPlans(companyID);
        }
    });


});




function fetchCompanies() {
    fetch('http://localhost:5001/api/v1/spaceBookingCenter/companies/ListCompanies') 
        .then(response => response.json())
        .then(data => {
            const companiesList = document.getElementById('companiesList');
            companiesList.innerHTML = ''; // Clear existing entries
            
            data.forEach(company => {
                const li = document.createElement('li');
                li.classList.add('company-item'); // For styling
                
                // Enhanced HTML content with clickable area for floor plans
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
    event.preventDefault(); // Prevent the default form submission

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
document.getElementById('reservationForm').addEventListener('submit', handleFormSubmit);

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



