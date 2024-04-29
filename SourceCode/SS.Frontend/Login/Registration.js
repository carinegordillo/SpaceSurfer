
document.getElementById('accountCreationForm').addEventListener('submit', function(e) {
    e.preventDefault();
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

    // If either checkbox is checked, populate companyInfo with actual values
    if (document.getElementById('isCompany').checked || document.getElementById('isFacility').checked) {
        companyInfo = {
            companyName: document.getElementById('companyName').value,
            address: document.getElementById('address').value,
            openingHours: document.getElementById('openingHours').value,
            closingHours: document.getElementById('closingHours').value,
            daysOpen: Array.from(document.querySelectorAll("input[name='daysOpen']:checked")).map(cb => cb.value).join(', ')
        };
    }

    var accountCreationRequest = {
        userInfo: userInfo,
        companyInfo: companyInfo
    };
    console.log("THIS IS THE REQUEST", accountCreationRequest)
    console.log("THIS ISI THE USERINOF ", accountCreationRequest.userInfo)
    fetch('http://localhost:8080/api/registration/postAccount', {
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
