window.onload = function() {
    handlePath(window.location.href);  // Changed to href to include the full URL
};

function handlePath(fullUrl) {
    const url = new URL(fullUrl);
    const pathSegments = url.pathname.split('/').filter(p => p);

    // Check if the 'employee' query parameter is present
    if (url.searchParams.has('employee')) {
        verifyEmployee();
    } else if (pathSegments.length) {
        switch (pathSegments[0]) {
            case 'employee':
                verifyEmployee();
                break;
            default:
                console.log('No employee handler for this path');
        }
    }
}

function verifyEmployee() {
    hideAllSections();
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("userNav").style.display = "none";
    document.getElementById("CMview").style.display = "none";
    document.getElementById("welcomeSection").style.display = "none";
    document.getElementById("noLogin").style.display = "block";
    // document.getElementById("sendOTPSection").style.display = "block";
    console.log('Fetching details for employee:');
    document.getElementById("Registration").style.display = "block";
    document.getElementById("accountCreationForm").style.display = "none";
    document.getElementById("enterRegistrationOTPSection").style.display = "block";
    authenticateRegisterUser();
}


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
    submitEmployeeCreationForm();
});

function submitEmployeeCreationForm() {
    if (!appConfig) {
        console.error('Configuration in register is not loaded!');
        return;
    }
    var employeeuserInfo = {
        username: document.getElementById('employeeusername').value,
        dob: document.getElementById('employeedob').value,
        firstname: document.getElementById('employeefirstname').value,
        lastname: document.getElementById('employeelastname').value,
        role: 4,
        status: "no",
        backupEmail: "",
    };
    // Assuming you have a way to define or retrieve userIdentity
    var userIdentity = employeeuserInfo.username; 

    var employeecompanyInfo = {
        companyName: '',
        address: '',
        openingHours: '',
        closingHours: '',
        daysOpen: ''
    };
    var managerHashedUsername = JSON.parse(sessionStorage.getItem('idToken')).Username;

    var employeeCreationRequest = {
        userInfo: employeeuserInfo,
        companyInfo: employeecompanyInfo, 
        manager_hashedUsername: managerHashedUsername
    };
    
    console.log("THIS IS THE REQUEST", employeeCreationRequest);
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
        body: JSON.stringify(employeeCreationRequest),
    })
    .then(response => {
        if (!response.ok) {
          // Convert non-JSON response to error
          return response.text().then(text => Promise.reject(new Error(text)));
        }
        return response.json();
      })
    .then(data => {
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
            showModal('Account successfully submitted! Check employee email for OTP to verify account');
        })
        .catch(error => {
            console.error('Error:', error);
            showModal('Error sending verification code.');
        });
    })
    .catch(error => {
        console.error('Error:', error);
        showModal('Error creating account. ' + error.message);
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

