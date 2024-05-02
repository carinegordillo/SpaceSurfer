async function accessData() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    try {
        const response = await fetch(`http://localhost:5084/api/userDataProtection/accessData`, {
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

        var data = await response;

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (response.ok) {
            showAccessSuccessMessage();
            console.log('Successfully accessed data.');
        }

    } catch (error) {
        console.error('Error accessing data:', error);
    }
}

function showAccessSuccessMessage() {
    const modal = document.createElement('div');
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.innerHTML = `
        <span class="close-button">&times;</span>
        <h2>Success!</h2>
        <p>Your data request was successful. Please refer to the email sent to view the requested data.</p>
    `;

    const closeButton = modalContent.querySelector('.close-button');
    closeButton.addEventListener('click', function () {
        modal.style.display = 'none';
    });

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    modal.style.display = 'block';
}

async function deleteData() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = checkTokenExpiration(accessToken);
    if (!isTokenExp) {
        logout();
        return;
    }

    try {
        const response = await fetch(`http://localhost:5084/api/userDataProtection/deleteData`, {
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

        var data = await response;

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        if (response.ok) {
            showDeleteSuccessMessage();
            console.log('Successfully deleted data.');
        }

    } catch (error) {
        console.error('Error deleting data:', error);
    }
}

function showDeleteSuccessMessage() {
    const modal = document.createElement('div');
    modal.classList.add('modal');

    const modalContent = document.createElement('div');
    modalContent.classList.add('modal-content');
    modalContent.innerHTML = `
        <span class="close-button">&times;</span>
        <h2>Success!</h2>
        <p>Your data has been successfully deleted. Please refer to the email sent to view the requested data.
           You will be redirected to the home screen soon. Thank you for using Space Surfer. We hope to see you again soon!
        </p>
    `;

    const closeButton = modalContent.querySelector('.close-button');
    closeButton.addEventListener('click', function () {
        modal.style.display = 'none';
    });

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    modal.style.display = 'block';
}

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
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    var otp = document.getElementById('access_otp').value;

    fetch('http://localhost:5270/api/auth/verifyCode', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: username, proof: otp })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error verifying OTP.');
            }
            return response;
        })
        .then(data => {
            document.getElementById("verifyOTPSection").style.display = "none";
            accessData();
            console.log("otp matches");
        })
        .catch (error => {
            document.getElementById("verifyOTPSection").style.display = "none";
            console.log("Error:", error);
        });
}

function createUserProtectionUI() {
    const contentContainer = document.getElementById('contentContainer');

    const requestDataButton = document.createElement('button');
    requestDataButton.textContent = 'Request Data';
    requestDataButton.addEventListener('click', function () {
        document.getElementById('requestDataSection').style.display = 'block';
        document.getElementById('deleteDataSection').style.display = 'none';
        document.getElementById('verifyOTPSection').style.display = 'none';
    });

    const deleteDataButton = document.createElement('button');
    deleteDataButton.textContent = 'Delete Data';
    deleteDataButton.addEventListener('click', function () {
        document.getElementById('deleteDataSection').style.display = 'block';
        document.getElementById('requestDataSection').style.display = 'none';
        document.getElementById('verifyOTPSection').style.display = 'none';
    });

    contentContainer.appendChild(requestDataButton);
    contentContainer.appendChild(deleteDataButton);

    const requestDataSection = document.createElement('section');
    requestDataSection.id = 'requestDataSection';
    requestDataSection.classList.add('form-container');
    requestDataSection.style.display = 'none';
    requestDataSection.innerHTML = `
        <h2>Request Your Data</h2>
        <p>By requesting your data, you acknowledge that we will send an email containing your requested data to the provided email address. This process is in compliance with CPRA and PII data laws in California.</p>
        <form id="requestDataForm">
            <label for="username">Enter Your Username:</label>
            <input type="text" id="username" name="username" required>
            <button type="submit">Request Data</button>
        </form>
    `;
    contentContainer.appendChild(requestDataSection);

    const deleteDataSection = document.createElement('section');
    deleteDataSection.id = 'deleteDataSection';
    deleteDataSection.classList.add('form-container');
    deleteDataSection.style.display = 'none';
    deleteDataSection.innerHTML = `
        <h2>Delete Your Data</h2>
        <p>By deleting your data, you acknowledge that all your personal data will be permanently removed from our system. This action is irreversible.</p>
        <form id="deleteDataForm">
            <label for="deleteUsername">Enter Your Username:</label>
            <input type="text" id="deleteUsername" name="deleteUsername" required>
            <button type="submit">Delete Data</button>
        </form>
    `;
    contentContainer.appendChild(deleteDataSection);

    const verifyOTPSection = document.createElement('section');
    verifyOTPSection.id = 'verifyOTPSection';
    verifyOTPSection.classList.add('form-container');
    verifyOTPSection.style.display = 'none';
    verifyOTPSection.innerHTML = `
        <h2>Verify Your Identity</h2>
        <form id="verifyOTPForm">
            <label for="access_otp">Enter Verification Code:</label>
            <input type="text" id="access_otp" name="access_otp" required>
            <button type="submit">Submit</button>
        </form>
    `;
    contentContainer.appendChild(verifyOTPSection);

    document.getElementById('requestDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        sendCode();
    });

    document.getElementById('deleteDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        deleteData();
    });

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