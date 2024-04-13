document.addEventListener('DOMContentLoaded', function() {
    // Handle loading of requests
    document.getElementById('loadRequestsButton').addEventListener('click', function() {
        fetch('http://localhost:8080/api/registration/createAccount')
            .then(response => response.json())
            .then(data => {
                const recoveryRequestsList = document.getElementById('recoveryRequestsList');
                recoveryRequestsList.innerHTML = ''; // Empty the list
                data.forEach(function(request) {
                    const listItem = document.createElement('li');
                    listItem.textContent = `Name: ${request.firstname}, Last Name: ${request.lastname}`;
                    if (request.resolveDate) {
                        listItem.textContent += `, Resolve Date: ${request.resolveDate}`;
                    }
                    recoveryRequestsList.appendChild(listItem);
                });
            })
            .catch(error => console.error('Error:', error));
    });

    // Handle form submission
    document.getElementById('accountCreationForm').addEventListener('submit', function(e) {
        e.preventDefault();
        submitAccountCreationForm();
    });

    // Initialize display state for additional fields upon page load
    updateAdditionalFieldsDisplay();
});

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

// $(document).ready(function() {
//     // Handle loading of requests
//     $('#loadRequestsButton').click(function() {
//         $.ajax({
//             url: 'http://localhost:8080/api/registration/createAccount',
//             type: 'GET',
//             dataType: 'json',
//             success: function(data) {
//                 $('#recoveryRequestsList').empty();
//                 data.forEach(function(request) {
//                     var listItem = `<li>Name : ${request.firstname}, Last Name : ${request.lastname}`;
//                     if (request.resolveDate) {
//                         listItem += `, Resolve Date: ${request.resolveDate}`;
//                     }
//                     listItem += '</li>';
//                     $('#recoveryRequestsList').append(listItem);
//                 });
//             },
//             error: function(error) {
//                 console.error(error);
//             }
//         });
//     });

//     // Handle form submission
//     $('#accountCreationForm').submit(function(e) {
//         e.preventDefault();
//         submitAccountCreationForm();
//     });
// });

// function handleCheckboxChange(checkedBox) {
//     // Logic to ensure only one checkbox is checked
//     if (checkedBox.id === "isCompany") {
//         document.getElementById('isFacility').checked = false;
//     } else if (checkedBox.id === "isFacility") {
//         document.getElementById('isCompany').checked = false;
//     }
//     updateAdditionalFieldsDisplay();
// }

// function updateAdditionalFieldsDisplay() {
//     // Update the display of additional fields based on checkbox state
//     let additionalFields = document.getElementById('additionalFields');
//     additionalFields.style.display = document.getElementById('isCompany').checked || document.getElementById('isFacility').checked ? 'block' : 'none';
// }

// function submitAccountCreationForm() {
//     var userInfo = {
//         username: $('#username').val(),
//         dob: $('#dob').val(),
//         firstname: $('#firstname').val(),
//         lastname: $('#lastname').val(),
//         role: $('#isCompany').is(':checked') ? 2 : $('#isFacility').is(':checked') ? 3 : 5,
//         status: "yes",
//         backupEmail: $('#backupEmail').val(),
//     };

//     var companyInfo = {
//         companyName: '',
//         address: '',
//         openingHours: '',
//         closingHours: '',
//         daysOpen: ''
//     };

//     // If either checkbox is checked, populate companyInfo with actual values
//     if ($('#isCompany').is(':checked') || $('#isFacility').is(':checked')) {
//         companyInfo = {
//             companyName: $('#companyName').val(),
//             address: $('#address').val(),
//             openingHours: $('#openingHours').val(),
//             closingHours: $('#closingHours').val(),
//             daysOpen: Array.from($("input[name='daysOpen']:checked")).map(cb => cb.value).join(', ')
//         };
//     }

//     var accountCreationRequest = {
//         userInfo: userInfo,
//         companyInfo: companyInfo
//     };

//     $.ajax({
//         url: 'http://localhost:8080/api/registration/postAccount',
//         type: 'POST',
//         contentType: 'application/json',
//         data: JSON.stringify(accountCreationRequest),
//         success: function(response) {
//             alert('Account created successfully!');
//         },
//         error: function(xhr, status, error) {
//             alert('Error creating account. ' + xhr.responseText);
//         }
//     });
// }

// // Initialize display state for additional fields upon page load
// updateAdditionalFieldsDisplay();

