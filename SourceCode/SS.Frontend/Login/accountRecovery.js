// Event delegation
document.addEventListener('click', function (event) {
    // Check if the clicked element has the class 'recover-link'
    if (event.target && event.target.classList.contains('recover-link')) {
        // Prevent default link behavior
        event.preventDefault();
        // Execute your function to show the recovery form
        showRecoveryForm();
    }
});

function showRecoveryForm() {
    window.alert("in here")
    // Hide the OTP section and show the recovery form section
    document.getElementById("accountRecoverySection").style.display = "block";
    document.getElementById('sendOTPSection').style.display = 'none';
    
    // Display recovery clicked alert
    window.alert("Recovery clicked");

    // Add event listener to form submission
    const recoveryForm = document.getElementById('recoveryForm');
    recoveryForm.addEventListener('submit', sendRecoveryRequest);
}

async function sendRecoveryRequest(event) {
    event.preventDefault();
    
    const recoveryForm = document.getElementById('recoveryForm');
    if (recoveryForm) {
        // This alert will confirm the form was found and the event handler was attached
        window.alert("Form found and submitted");

        const email = document.getElementById('email').value;
        const additionalInformation = document.getElementById('additionalInformation').value;

        fetch('http://localhost:5176/api/requestRecovery/sendRecoveryRequest', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: new URLSearchParams({
                email: email,
                additionalInformation: additionalInformation
            })
        })
        .then(response => response.json())
        .then(data => {
            console.log(data);
            let message = "Request failed. Please try again"; // Default message
            if (!data.HasError) {
                message = "Request sent successfully";
            }
            
            createAndShowModal(message);
        })
        .catch(error => {
            console.error('Error:', error);
            createAndShowModal("There was an error processing your request.");
        });
    } else {
        window.alert("Recovery form not found");
        console.error('Recovery form not found.');
    }
}
function createAndShowModal(message) {
    const modal = document.createElement('div');
    modal.id = 'dynamicModal';
    modal.className = 'modal'; 
    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';

    const closeButton = document.createElement('button');
    closeButton.textContent = 'OK';
    closeButton.className = 'close-button';
    closeButton.onclick = function() {
        modal.style.display = 'none';
        document.body.removeChild(modal);
    };

    const resultMessage = document.createElement('p');
    resultMessage.textContent = message;

    modalContent.appendChild(resultMessage);
    modalContent.appendChild(closeButton);
    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    modal.style.display = 'flex'; 
}

