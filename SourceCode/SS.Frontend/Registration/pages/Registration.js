
$(document).ready(function() {
    // Handle loading of requests
    $('#loadRequestsButton').click(function() {
        $.ajax({
            url: 'http://localhost:8080/api/registration/createAccount',
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                $('#recoveryRequestsList').empty();
                data.forEach(function(request) {
                    var listItem = `<li>Name : ${request.firstname}, Last Name : ${request.lastname}`;
                    if (request.resolveDate) {
                        listItem += `, Resolve Date: ${request.resolveDate}`;
                    }
                    listItem += '</li>';
                    $('#recoveryRequestsList').append(listItem);
                });
            },
            error: function(error) {
                console.error(error);
            }
        });
    });

    // Handle form submission
    $('#accountCreationForm').submit(function(e) {
        e.preventDefault();
        submitAccountCreationForm();
    });
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
        username: $('#username').val(),
        dob: $('#dob').val(),
        firstname: $('#firstname').val(),
        lastname: $('#lastname').val(),
        role: $('#isCompany').is(':checked') ? 2 : $('#isFacility').is(':checked') ? 3 : 5,
        status: "yes",
        backupEmail: $('#backupEmail').val(),
    };

    var companyInfo = {
        companyName: '',
        address: '',
        openingHours: '',
        closingHours: '',
        daysOpen: ''
    };

    // If either checkbox is checked, populate companyInfo with actual values
    if ($('#isCompany').is(':checked') || $('#isFacility').is(':checked')) {
        companyInfo = {
            companyName: $('#companyName').val(),
            address: $('#address').val(),
            openingHours: $('#openingHours').val(),
            closingHours: $('#closingHours').val(),
            daysOpen: Array.from($("input[name='daysOpen']:checked")).map(cb => cb.value).join(', ')
        };
    }

    var accountCreationRequest = {
        userInfo: userInfo,
        companyInfo: companyInfo
    };

    $.ajax({
        url: 'http://localhost:8080/api/registration/postAccount',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(accountCreationRequest),
        success: function(response) {
            alert('Account created successfully!');
        },
        error: function(xhr, status, error) {
            alert('Error creating account. ' + xhr.responseText);
        }
    });
}

// Initialize display state for additional fields upon page load
updateAdditionalFieldsDisplay();

