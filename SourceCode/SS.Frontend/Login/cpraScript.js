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
        console.log(JSON.stringify(username));
        if (!appConfig) {
            console.error('Configuration is not loaded!');
            return;
        }
        const CPRAUrl = appConfig.api.CPRA;  
        const response = await fetch(`${CPRAUrl}/api/userDataProtection/accessData`, {
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
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const CPRAUrl = appConfig.api.CPRA; 

    try {
        console.log("running user data protection endpoint");
        const deleteResponse = await fetch(`${CPRAUrl}/api/userDataProtection/deleteData`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(username)
        });

        if (!deleteResponse.ok) {
            throw new Error(`HTTP error! status: ${deleteResponse.status}`);
        }

        const contentType = deleteResponse.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            const deleteData = await deleteResponse.json();
            console.log('Delete response:', deleteData);
            if (deleteData.newToken) {
                accessToken = deleteData.newToken;
                sessionStorage.setItem('accessToken', accessToken);
                console.log('New access token stored:', accessToken);
            }
        } else {
            console.log('Delete operation successful.');
        }

        console.log('Successfully deleted data.');

        console.log("running account deletion endpoint");
        const DeletionUrl = appConfig.api.AccountDeletion; 
        const secondResponse = await fetch(`${DeletionUrl}/api/AccountDeletion/Delete`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        if (!secondResponse.ok) {
            throw new Error(`HTTP error! status: ${secondResponse.status}`);
        }

        const secondAccountDeletionData = await secondResponse.json();
        if (secondAccountDeletionData.newToken) {
            accessToken = secondAccountDeletionData.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        console.log('Successfully completed account deletion.');

        showDeleteSuccessMessage();
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
        logout();
        document.getElementById('cpraView').style.display = 'none';
    });

    modal.appendChild(modalContent);
    document.body.appendChild(modal);

    modal.style.display = 'block';
}

function sendCode_Access() {
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const LoginUrl = appConfig.api.Login;  
    fetch(`${LoginUrl}/api/auth/sendOTP`, {
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
            document.getElementById("verifyOTPSection_Access").style.display = "block";
        })
        .catch(error => {
            alert(error.message);
        });
}

function sendCode_Deletion() {
    var userIdentity = document.getElementById("userIdentity").value;
    console.log(userIdentity);
    const loginUrl = appConfig.api.Login; 
    fetch(`${loginUrl}/api/auth/sendOTP`, {
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
            document.getElementById("deleteDataSection").style.display = "none";
            document.getElementById("verifyOTPSection_Deletion").style.display = "block";
        })
        .catch(error => {
            alert(error.message);
        });
}

function verifyUser_Access() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    var otp = document.getElementById('access_otp').value;
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const LoginUrl = appConfig.api.Login;
    fetch(`${LoginUrl}/api/auth/verifyCode`, {
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
            document.getElementById("verifyOTPSection_Access").style.display = "none";
            accessData();
            console.log("otp matches");
        })
        .catch (error => {
            document.getElementById("verifyOTPSection_Access").style.display = "none";
            console.log("Error:", error);
        });
}

function verifyUser_Deletion() {
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;
    var otp = document.getElementById('deletion_otp').value;
    const LoginUrl = appConfig.api.Login;
    fetch(`${LoginUrl}/api/auth/verifyCode`, {
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
            document.getElementById("verifyOTPSection_Deletion").style.display = "none";
            deleteData();
            console.log("otp matches");
        })
        .catch(error => {
            document.getElementById("verifyOTPSection_Deletion").style.display = "none";
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
        document.getElementById('verifyOTPSection_Access').style.display = 'none';
        document.getElementById('verifyOTPSection_Deletion').style.display = 'none';
        fetchInsertUsedFeature('User Data Protection');
    });

    const deleteDataButton = document.createElement('button');
    deleteDataButton.textContent = 'Delete Data';
    deleteDataButton.addEventListener('click', function () {
        document.getElementById('deleteDataSection').style.display = 'block';
        document.getElementById('requestDataSection').style.display = 'none';
        document.getElementById('verifyOTPSection_Access').style.display = 'none';
        document.getElementById('verifyOTPSection_Deletion').style.display = 'none';
        fetchInsertUsedFeature('User Data Protection');
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

    const verifyOTPSection_Access = document.createElement('section');
    verifyOTPSection_Access.id = 'verifyOTPSection_Access';
    verifyOTPSection_Access.classList.add('form-container');
    verifyOTPSection_Access.style.display = 'none';
    verifyOTPSection_Access.innerHTML = `
        <h2>Verify Your Identity</h2>
        <form id="verifyOTPForm_Access">
            <label for="access_otp">Enter Verification Code:</label>
            <input type="text" id="access_otp" name="access_otp" required>
            <button type="submit">Submit</button>
        </form>
    `;
    contentContainer.appendChild(verifyOTPSection_Access);

    const verifyOTPSection_Deletion = document.createElement('section');
    verifyOTPSection_Deletion.id = 'verifyOTPSection_Deletion';
    verifyOTPSection_Deletion.classList.add('form-container');
    verifyOTPSection_Deletion.style.display = 'none';
    verifyOTPSection_Deletion.innerHTML = `
        <h2>Verify Your Identity</h2>
        <form id="verifyOTPForm_Deletion">
            <label for="deletion_otp">Enter Verification Code:</label>
            <input type="text" id="deletion_otp" name="deletion_otp" required>
            <button type="submit">Submit</button>
        </form>
    `;
    contentContainer.appendChild(verifyOTPSection_Deletion);
    document.getElementById('requestDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        sendCode_Access();
    });

    document.getElementById('deleteDataForm').addEventListener('submit', function (event) {
        event.preventDefault();
        sendCode_Deletion();
    });

    const verifyOTPForm_Access = document.getElementById('verifyOTPForm_Access');
    if (verifyOTPForm_Access) {
        verifyOTPForm_Access.addEventListener('submit', function (event) {
            event.preventDefault();
            verifyUser_Access();
        });
    }

    const verifyOTPForm_Deletion = document.getElementById('verifyOTPForm_Deletion');
    if (verifyOTPForm_Deletion) {
        verifyOTPForm_Deletion.addEventListener('submit', function (event) {
            event.preventDefault();
            verifyUser_Deletion();
        });
    }
}

function showUserProtectionSection() {
    const cpraSection = document.getElementById('cpraView');
    cpraSection.style.display = 'block';
    createUserProtectionUI();
}

function cpraAccess() {
    hideAllSections();
    document.getElementById('cpraView').style.display = 'block';
    showUserProtectionSection();

}