
// document.getElementById('employeeForm').addEventListener('submit', function(e) {
//     e.preventDefault();
//     submiEmployeeCreationForm();
// });

// function submitEmployeeCreationForm() {
//     var userInfo = {
//         username: document.getElementById('username').value,
//         dob: document.getElementById('dob').value,
//         firstname: document.getElementById('firstname').value,
//         lastname: document.getElementById('lastname').value,
//         role: 4,
//         status: "no",
//         backupEmail: "",
//     };
//     var companyInfo = {
//         companyName: '',
//         address: '',
//         openingHours: '',
//         closingHours: '',
//         daysOpen: ''
//     };
//     var accountCreationRequest = {
//         userInfo: userInfo,
//         companyInfo: companyInfo
//     };
//     fetch('http://localhost:8080/api/registration/postAccount', {
//         method: 'POST',
//         headers: {
//             'Content-Type': 'application/json',
//         },
//         body: JSON.stringify(accountCreationRequest),
//     })
//     .then(response => response.json())
//     .then(data => {
//         alert('Account created successfully!');
//     })
//     .catch(error => {
//         console.error('Error:', error);
//         alert('Error creating account. ' + error.message);
//     });
// }

