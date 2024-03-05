// document.addEventListener('DOMContentLoaded', function() {
//     // Initial view setup or any other initialization
//     showView('about'); // Show about by default or based on some logic
// });

// function showView(viewId) {
//     // Hide all views
//     document.querySelectorAll('.view').forEach(function(view) {
//         view.style.display = 'none';
//     });

//     // Show the selected view
//     document.getElementById(viewId + 'View').style.display = 'block';
// }





document.getElementById('accountCreationForm').onsubmit = function(event) {
    event.preventDefault();

    let firstName = document.getElementById('firstName').value;
    let lastName = document.getElementById('lastName').value;
    let dob = document.getElementById('dob').value;
    let email = document.getElementById('email').value;
    let isCompany = document.getElementById('isCompany').checked;
    let isFacility = document.getElementById('isFacility').checked;
    let name = document.getElementById('name').value;
    let location = document.getElementById('location').value;
    let openingHours = document.getElementById('openingHours').value;
    let closingHours = document.getElementById('closingHours').value;

    // Check required fields
    if (!firstName || !lastName || !dob || !email || (isCompany && !name) || (isFacility && !name)) {
        document.getElementById('errorMessage').innerText = "Cannot create account. Please enter all required fields";
        return false;
    }

    // Further processing, such as sending data to a server, can be added here.
    alert("Form Submitted Successfully!");
};

function handleCheckboxChange() {
    let isCompany = document.getElementById('isCompany').checked;
    let isFacility = document.getElementById('isFacility').checked;
    let additionalFields = document.getElementById('additionalFields');
    let name = document.getElementById('name');
    let location = document.getElementById('location');
    let openingHours = document.getElementById('openingHours');
    let closingHours = document.getElementById('closingHours');

    if (isCompany || isFacility) {
        // If either checkbox is checked, display and require the additional fields
        additionalFields.style.display = 'block';
        // name.required = true;
        // location.required = true;
        // openingHours.required = true;
        // closingHours.required = true;
    }
    // } else {

    //     // If neither checkbox is checked, hide and make the additional fields optional
    //     additionalFields.style.display = 'none';
    //     name.required = false;
    //     location.required = false;
    //     openingHours.required = false;
    //     closingHours.required = false;
    // }

    // Prevent both checkboxes from being checked simultaneously
    if (isCompany) {
        document.getElementById('isFacility').checked = false;
    } else if (isFacility) {
        document.getElementById('isCompany').checked = false;
    }
}

