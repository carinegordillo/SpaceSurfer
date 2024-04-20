function sendCode() {
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);

    fetch('http://localhost:5270/api/auth/sendOTP', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error sending verification code.');
            }
            return response.json();
        })
        .then(data => {
            document.getElementById("requestDataSection").style.display = "none";
            document.getElementById("verifyOTPSection").style.display = "block";
        })
        .catch(error => {
            alert(error.message);
        });
}

function verifyUser() {
    var otp = document.getElementById("otp").value;
    var userIdentity = document.getElementById("username").value;

    fetch('http://localhost:5270/api/auth/authenticate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity, proof: otp })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error verifying OTP.');
            }
            return response.json();
        })
        .then(data => {
            document.getElementById("verifyOTPSection").style.display = "none";
            console.log("otp matches");
        })
        .catch(error => {
            document.getElementById("verifyOTPSection").style.display = "none";
            console.log("otp doesn't match");
        });
}

function createUserProtectionUI() {
    // Get the main content container
    const contentContainer = document.getElementById('contentContainer');

    // Create the buttons for requesting and deleting data
    const requestDataButton = document.createElement('button');
    requestDataButton.textContent = 'Request Data';
    requestDataButton.addEventListener('click', function () {
        document.getElementById('requestDataSection').style.display = 'block';
        document.getElementById('deleteDataSection').style.display = 'none';
        document.getElementById('verifyOTPSection').style.display = 'none'; // Hide verify OTP section
    });

    const deleteDataButton = document.createElement('button');
    deleteDataButton.textContent = 'Delete Data';
    deleteDataButton.addEventListener('click', function () {
        document.getElementById('deleteDataSection').style.display = 'block';
        document.getElementById('requestDataSection').style.display = 'none';
        document.getElementById('verifyOTPSection').style.display = 'none'; // Hide verify OTP section
    });

    contentContainer.appendChild(requestDataButton);
    contentContainer.appendChild(deleteDataButton);

    // Create the request data section
    const requestDataSection = document.createElement('section');
    requestDataSection.id = 'requestDataSection';
    requestDataSection.classList.add('form-container');
    requestDataSection.style.display = 'none'; // Initially hidden
    requestDataSection.innerHTML = `
        <h2>Request Your Data</h2>
        <form id="requestDataForm">
            <label for="username">Enter Your Username:</label>
            <input type="text" id="username" name="username" required>
            <button type="submit">Request Data</button>
        </form>
    `;
    contentContainer.appendChild(requestDataSection);

    // Create the delete data section
    const deleteDataSection = document.createElement('section');
    deleteDataSection.id = 'deleteDataSection';
    deleteDataSection.classList.add('form-container');
    deleteDataSection.style.display = 'none'; // Initially hidden
    deleteDataSection.innerHTML = `
        <h2>Delete Your Data</h2>
        <form id="deleteDataForm">
            <label for="deleteUsername">Enter Your Username:</label>
            <input type="text" id="deleteUsername" name="deleteUsername" required>
            <button type="submit">Delete Data</button>
        </form>
    `;
    contentContainer.appendChild(deleteDataSection);

    // Create the verify OTP section
    const verifyOTPSection = document.createElement('section');
    verifyOTPSection.id = 'verifyOTPSection';
    verifyOTPSection.classList.add('form-container');
    verifyOTPSection.style.display = 'none'; // Initially hidden
    verifyOTPSection.innerHTML = `
        <h2>Verify Your Identity</h2>
        <form id="verifyOTPForm">
            <label for="otp">Enter Verification Code:</label>
            <input type="text" id="otp" name="otp" required>
            <button type="submit">Submit</button>
        </form>
    `;
    contentContainer.appendChild(verifyOTPSection);

    // Attach event listener for request data form
    document.getElementById('requestDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        sendCode();
    });

    // Attach event listener for delete data form
    document.getElementById('deleteDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        deleteData(); // Define this function to handle deletion
    });

    // Attach event listener for verify OTP form
    const verifyOTPForm = document.getElementById('verifyOTPForm');
    if (verifyOTPForm) {
        verifyOTPForm.addEventListener('submit', function (event) {
            event.preventDefault();
            verifyUser();
        });
    }
}

function showUserProtectionSection() {
    const cpraSection = document.getElementById('cpraView');
    cpraSection.style.display = 'block';
    createUserProtectionUI();

    document.getElementById('requestDataSection').addEventListener('submit', function (event) {
        event.preventDefault();
        sendCode();
    });
    document.getElementById('verifyOTPSection').addEventListener('submit', function (event) {
        event.preventDefault();
        verifyUser();
    });
}

function cpraAccess() {
    document.getElementById('cpraView').style.display = 'block';

    document.getElementById('homepageGen').style.display = 'none';
    document.getElementById('homepageManager').style.display = 'none';
    document.getElementById('sendOTPSection').style.display = 'none';
    document.getElementById('enterOTPSection').style.display = 'none';
    document.getElementById('successResult').style.display = 'none';
    document.getElementById('failResult').style.display = 'none';
    document.getElementById('personalOverviewCenter').style.display = 'none';
    document.getElementById('waitlistView').style.display = 'none';

    showUserProtectionSection();

}
