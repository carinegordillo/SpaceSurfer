
document.getElementById('accountCreationForm').addEventListener('submit', function(e) {
    e.preventDefault();
    console.log("regusration form has been submitted ")
    submitAccountCreationForm();
});
updateAdditionalFieldsDisplay();

function handleCheckboxChange(checkedBox) {
    // Logic to ensure only one checkbox is checked
    if (checkedBox.id === "isCompany") {
        document.getElementById('isFacility').checked = false;
    } else if (checkedBox.id === "isFacility") {
        document.getElementById('isCompany').checked = false;
    }
    updateAdditionalFieldsDisplay();
}

function updateAdditionalFieldsDisplay() {
    // Update the display of additional fields based on checkbox state
    let additionalFields = document.getElementById('additionalFields');
    additionalFields.style.display = document.getElementById('isCompany').checked || document.getElementById('isFacility').checked ? 'block' : 'none';
}

function submitAccountCreationForm() {
    const irvineZipCodes = ["92602", "92603", "92604", "92606", "92612", "92614", "92616", "92617", "92618", "92619", "92620", "92623", "92650", "92697", "92709", "92710"];

    var userInfo = {
        username: document.getElementById('username').value,
        dob: document.getElementById('dob').value,
        firstname: document.getElementById('firstname').value,
        lastname: document.getElementById('lastname').value,
        role: document.getElementById('isCompany').checked ? 2 : document.getElementById('isFacility').checked ? 3 : 5,
        status: "no",
        backupEmail: document.getElementById('backupEmail').value,
    };

    var companyInfo = {
        companyName: '',
        address: '',
        openingHours: '',
        closingHours: '',
        daysOpen: ''
    };

    if (document.getElementById('isCompany').checked || document.getElementById('isFacility').checked) {
        companyInfo = {
            companyName: document.getElementById('companyName').value,
            address: document.getElementById('address').value,
            openingHours: document.getElementById('openingHours').value,
            closingHours: document.getElementById('closingHours').value,
            daysOpen: Array.from(document.querySelectorAll("input[name='daysOpen']:checked")).map(cb => cb.value).join(', ')
        };
        const zipCode = companyInfo.address.split(',').pop().trim();

        if (!irvineZipCodes.includes(zipCode)) {
            showModal('Error: Company must be located in Irvine, California.');
            return; 
        }
    }


    var accountCreationRequest = {
        userInfo: userInfo,
        companyInfo: companyInfo
    };
    
    console.log("THIS IS THE REQUEST", accountCreationRequest);

    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }

    const RegistrationUrl = appConfig.api.Registration; 

    fetch(`${RegistrationUrl}/api/registration/postAccount`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(accountCreationRequest),
    })
    .then(response => response.json())
    .then(data => {
        showModal('Account successfully submitted! Check your email for OTP to verify accunt');
        sendRegistrationOTP(userInfo.username);
        // getLogin();
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error creating account. ' + error.message);
    });
}

document.getElementById('employeeForm').addEventListener('submit', function(e) {
    e.preventDefault();
    submiEmployeeCreationForm();
});

function submitEmployeeCreationForm() {
    var userInfo = {
        username: document.getElementById('username').value,
        dob: document.getElementById('dob').value,
        firstname: document.getElementById('firstname').value,
        lastname: document.getElementById('lastname').value,
        role: 4,
        status: "no",
        backupEmail: "",
    };
    var companyInfo = {
        companyName: '',
        address: '',
        openingHours: '',
        closingHours: '',
        daysOpen: ''
    };
    var accountCreationRequest = {
        userInfo: userInfo,
        companyInfo: companyInfo,
        manager_hashedUsername : JSON.parse(sessionStorage.getItem('idToken')).Username
    };
    if (!appConfig) {
        console.error('Configuration is not loaded!');
        return;
    }
    const RegistrationUrl = appConfig.api.Registration; 
    fetch(`${RegistrationUrl}/api/registration/postAccount`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(accountCreationRequest),
    })
    .then(response => response.json())
    .then(data => {
        alert('Account created successfully!');
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error creating account. ' + error.message);
    });
}

function sendRegistrationOTP(userIdentity) {
    const loginUrl = appConfig.api.Login;  
    fetch(`${loginUrl}/api/auth/sendOTP`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity })
    })
    .then(response => response.json())
    .then(data => {
        document.getElementById("accountCreationForm").style.display = "none";
        document.getElementById("enterRegistrationOTPSection").style.display = "block";
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error sending verification code.');
    });
}

function authenticateRegisterUser() {
    var otp = document.getElementById("registrationOtp").value;
    var userIdentity = document.getElementById("registrationUsername").value;
    const loginUrl = appConfig.api.Login; // Ensure loginUrl is defined in your appConfig or locally

    fetch(`${loginUrl}/api/auth/authenticate`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userIdentity: userIdentity, proof: otp })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        const RegistrationUrl = appConfig.api.Registration; 
        return fetch(`${RegistrationUrl}/api/registration/verifyAccount`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ Username: userIdentity })
        });
    })
    .then(verifyResponse => {
        if (!verifyResponse.ok) {
            throw new Error('Network response was not ok during account verification');
        }
        return verifyResponse.json();
    })
    .then(verifyData => {
        showModal("Account verified successfully. You may now login.");
        document.getElementById("enterRegistrationOTPSection").style.display = "none";
        getLogin(); 
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error during OTP verification or account finalization: ' + error.message);
    });
}

