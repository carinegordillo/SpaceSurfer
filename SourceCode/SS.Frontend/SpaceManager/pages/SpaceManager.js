function logout() {
    localStorage.removeItem('accessToken');
    window.location.href = '../UnAuthnAbout/about.html';
}

document.getElementById('replaceImage').addEventListener('change', function(event) {
    var fileModify = event.target.files[0];
    if (!fileModify) {
        alert('No file selected.');
        return;
    }
    if (fileModify.size > 5242880) {
        alert('The file is too large. Please select a file smaller than 5MB.');
        return;
    }
    var reader = new FileReader();
    reader.onload = function(e) {
        var imagePreviewContainer = document.getElementById('modifyImagePreviewContainer');
        imagePreviewContainer.innerHTML = '';
        var img = new Image();
        img.src = e.target.result;
        img.style.maxWidth = '200px';
        img.style.maxHeight = '200px';
        imagePreviewContainer.appendChild(img);
        document.getElementById('modifyImageBase64').value = e.target.result.split(',')[1];
    };
    reader.readAsDataURL(fileModify);
});

document.getElementById('imageUpload').addEventListener('change', handleImageUpload);

function handleImageUpload(event) {
    var file = event.target.files[0];
    if (!file) {
        alert('No file selected.');
        return;
    }
    if (file.size > 5242880) {
        alert('The file is too large. Please select a file smaller than 5MB.');
        return;
    }
    var reader = new FileReader();
    reader.onload = function(e) {
        var imagePreviewContainer = document.getElementById('imagePreviewContainer');
        imagePreviewContainer.innerHTML = '';
        var img = new Image();
        img.src = e.target.result;
        img.style.maxWidth = '200px';
        img.style.maxHeight = '200px';
        imagePreviewContainer.appendChild(img);
        document.getElementById('imageBase64').value = e.target.result.split(',')[1];
    };
    reader.readAsDataURL(file);
}

function addSpaceRow(containerId, rowClass) {
    const container = document.getElementById(containerId);
    const currentSpaces = container.getElementsByClassName(rowClass).length;
    if (currentSpaces >= 20) {
        alert('You cannot add more than 20 spaces.');
        return;
    }
    const newRow = document.createElement('div');
    newRow.className = rowClass;
    newRow.innerHTML = `
        <input type="text" class="spaceID" name="spaceID[]" placeholder="Space ID">
        <input type="text" class="timeLimit" name="timeLimit[]" placeholder="Time Limit">
    `;
    if (rowClass === 'spaceRowModify') {
        const deleteButton = document.createElement('button');
        deleteButton.type = 'button';
        deleteButton.className = 'btn btn-danger deleteSpace';
        deleteButton.textContent = 'Delete Space';
        deleteButton.onclick = function() { this.closest('.'+rowClass).remove(); };
        newRow.appendChild(deleteButton);
    }
    container.appendChild(newRow);
}

document.getElementById('addSpace').addEventListener('click', function() {
    const container = document.getElementById('spacesContainer');
    const currentSpaces = container.getElementsByClassName('spaceRow').length;

    if (currentSpaces >= 20) {
        alert('You cannot add more than 20 spaces.');
        return;
    }

    const newRow = document.createElement('div');
    newRow.classList.add('spaceRow');
    newRow.innerHTML = `
        <input type="text" class="spaceID" name="spaceID[]" placeholder="Space ID">
        <input type="text" class="timeLimit" name="timeLimit[]" placeholder="Time Limit">
    `;
    container.appendChild(newRow);
});

document.getElementById('addSpaceModify').addEventListener('click', function() {
    const container = document.getElementById('spacesModificationContainer');
    const currentModifiedSpaces = container.getElementsByClassName('spaceRowModify').length;

    if (currentModifiedSpaces >= 20) {
        alert('You cannot add more than 20 modified spaces.');
        return;
    }

    const newRow = document.createElement('div');
    newRow.classList.add('spaceRowModify');
    newRow.innerHTML = `
        <input type="text" class="modifySpaceID" name="spaceID[]" placeholder="Space ID">
        <input type="text" class="modifyTimeLimit" name="modifytimeLimit[]" placeholder="Time Limit">
        <button type="button" class="btn btn-danger deleteSpace">Delete Space</button>
    `;
    container.appendChild(newRow);

    newRow.querySelector('.deleteSpace').addEventListener('click', function() {
        this.closest('.spaceRowModify').remove();
    });
});

function collectSpacesAndTimeLimits() {
    let spacesDict = {};
    // Assuming each spaceID input is directly followed by its corresponding timeLimit input
    const spaceIDs = document.querySelectorAll('.spaceID');
    const timeLimits = document.querySelectorAll('.timeLimit');

    spaceIDs.forEach((spaceInput, index) => {
        const spaceID = spaceInput.value;
        const timeLimit = timeLimits[index].value; // Assuming the same index
        if (spaceID && timeLimit) {
            spacesDict[spaceID] = parseInt(timeLimit);
        }
    });

    return spacesDict;
}

function collectSpacesAndTimeLimitsToModify() {
    let spacesDict = {};
    // Assuming each spaceID input is directly followed by its corresponding timeLimit input
    const spaceIDs = document.querySelectorAll('.modifySpaceID');
    const timeLimits = document.querySelectorAll('.modifyTimeLimit');

    spaceIDs.forEach((spaceInput, index) => {
        const spaceID = spaceInput.value;
        const timeLimit = timeLimits[index].value; // Assuming the same index
        if (spaceID && timeLimit) {
            spacesDict[spaceID] = parseInt(timeLimit);
        }
    });

    return spacesDict;
}

document.querySelectorAll('.accordion').forEach(function(button) {
    button.addEventListener('click', function() {
        this.classList.toggle('active');
        var formContainer = this.nextElementSibling;
        formContainer.style.display = formContainer.style.display === 'flex' ? 'none' : 'flex';
    });
});

document.addEventListener('DOMContentLoaded', function() {
    // if (!localStorage.getItem('accessToken')) {
    //     // Redirect if accessToken not found
    //     window.location.href = '../UnAuthnAbout/about.html';
    //     return; 
    // }
    document.getElementById('loadRequestsButton').addEventListener('click', function() {
        fetch('http://localhost:8081/api/SpaceManager/createSpace')
            .then(response => response.json())
            .then(data => {
                const recoveryRequestsList = document.getElementById('recoveryRequestsList');
                recoveryRequestsList.innerHTML = '';
                data.forEach(function(request) {
                    const listItem = document.createElement('li');
                    listItem.textContent = `Name: ${request.floorPlanName}, Spaces: ${request.spaceList}`;
                    if(request.resolveDate) {
                        listItem.textContent += `, Resolve Date: ${request.resolveDate}`;
                    }
                    recoveryRequestsList.appendChild(listItem);
                });
            })
            .catch(error => console.error('Error:', error));
    });

    document.getElementById('accountCreationForm').addEventListener('submit', function(e) {
        e.preventDefault();
        const companyFloor = {
            FloorPlanName: document.getElementById('floorPlanName').value,
            FloorPlanImage: document.getElementById('imageBase64').value,
            FloorSpaces: collectSpacesAndTimeLimits()
        };
        sendData('http://localhost:8081/api/SpaceManager/postSpace', companyFloor, 'accessToken');
    });

    document.getElementById('modifySpaceForm').addEventListener('submit', function(e) {
        e.preventDefault();
        const spacesToModify = collectSpacesAndTimeLimitsToModify();
        sendData('http://localhost:8081/api/SpaceManager/modifyTimeLimits', spacesToModify, 'accessToken');
    });

    document.getElementById('modifyImage').addEventListener('click', function() {
        const newFloor = {
            FloorPlanName: document.getElementById('modifyFloorPlanName').value,
            NewFloorPlanImage: document.getElementById('modifyImageBase64').value
        };
        sendData('http://localhost:8081/api/SpaceManager/modifyFloorPlan', newFloor, 'accessToken');
    });

    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('deleteSpace')) {
            const spaceID = e.target.closest('.spaceRowModify').querySelector('.modifySpaceID').value;
            sendData('http://localhost:8081/api/SpaceManager/deleteSpace', { spaceID: spaceID }, 'accessToken');
        }
    });

    document.getElementById('deleteFloor').addEventListener('click', function() {
        const FloorPlanName = document.getElementById('modifyFloorPlanName').value;
        sendData('http://localhost:8081/api/SpaceManager/deleteFloor', { FloorPlanName: FloorPlanName }, 'accessToken');
    });
});

function sendData(url, data, tokenKey) {
    const token = localStorage.getItem(tokenKey);
    fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(data)
    })
    .then(response => response.json())
    .then(data => {
        console.log('Success:', data);
        alert('Operation successful!');
    })
    .catch((error) => {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
    });
}



// function logout() {
//     // Remove token from localStorage
//     localStorage.removeItem('accessToken');
//     window.location.href = '../UnAuthnAbout/about.html';
// }

// document.getElementById('replaceImage').addEventListener('change', function(event) {
//     var fileModify = event.target.files[0];

//     if (!fileModify) {
//         alert('No file selected.');
//         return;
//     }

//     if (fileModify.size > 5242880) { // 5MB in bytes
//         alert('The file is too large. Please select a file smaller than 5MB.');
//         return;
//     }

//     var reader = new FileReader();
//     reader.onload = function(e) {
//         var imagePreviewContainer = document.getElementById('modifyImagePreviewContainer');
//         imagePreviewContainer.innerHTML = '';
//         var img = document.createElement('img');
//         img.src = e.target.result;
//         img.style.maxWidth = '200px';
//         img.style.maxHeight = '200px';
//         imagePreviewContainer.appendChild(img);

//         var base64String = e.target.result.split(',')[1];
//         document.getElementById('modifyImageBase64').value = base64String;
//     };

//     reader.readAsDataURL(fileModify);
// });

// document.getElementById('imageUpload').addEventListener('change', function(event) {
//     var file = event.target.files[0];

//     if (!file) {
//         alert('No file selected.');
//         return;
//     }

//     if (file.size > 5242880) { // 5MB in bytes
//         alert('The file is too large. Please select a file smaller than 5MB.');
//         return;
//     }

//     var reader = new FileReader();
//     reader.onload = function(e) {
//         var imagePreviewContainer = document.getElementById('imagePreviewContainer');
//         imagePreviewContainer.innerHTML = '';
//         var img = document.createElement('img');
//         img.src = e.target.result;
//         img.style.maxWidth = '200px';
//         img.style.maxHeight = '200px';
//         imagePreviewContainer.appendChild(img);

//         var base64String = e.target.result.split(',')[1];
//         document.getElementById('imageBase64').value = base64String;
//     };

//     reader.readAsDataURL(file);
// });

// document.getElementById('addSpace').addEventListener('click', function() {
//     const container = document.getElementById('spacesContainer');
//     const currentSpaces = container.getElementsByClassName('spaceRow').length;

//     if (currentSpaces >= 20) {
//         alert('You cannot add more than 20 spaces.');
//         return;
//     }

//     const newRow = document.createElement('div');
//     newRow.classList.add('spaceRow');
//     newRow.innerHTML = `
//         <input type="text" class="spaceID" name="spaceID[]" placeholder="Space ID">
//         <input type="text" class="timeLimit" name="timeLimit[]" placeholder="Time Limit">
//     `;
//     container.appendChild(newRow);
// });

// document.getElementById('addSpaceModify').addEventListener('click', function() {
//     const container = document.getElementById('spacesModificationContainer');
//     const currentModifiedSpaces = container.getElementsByClassName('spaceRowModify').length;

//     if (currentModifiedSpaces >= 20) {
//         alert('You cannot add more than 20 modified spaces.');
//         return;
//     }

//     const newRow = document.createElement('div');
//     newRow.classList.add('spaceRowModify');
//     newRow.innerHTML = `
//         <input type="text" class="modifySpaceID" name="spaceID[]" placeholder="Space ID">
//         <input type="text" class="modifyTimeLimit" name="modifytimeLimit[]" placeholder="Time Limit">
//         <button type="button" class="btn btn-danger deleteSpace">Delete Space</button>
//     `;
//     container.appendChild(newRow);

//     newRow.querySelector('.deleteSpace').addEventListener('click', function() {
//         this.closest('.spaceRowModify').remove();
//     });
// });

// function collectSpacesAndTimeLimits() {
//     let spacesDict = {};
//     // Assuming each spaceID input is directly followed by its corresponding timeLimit input
//     const spaceIDs = document.querySelectorAll('.spaceID');
//     const timeLimits = document.querySelectorAll('.timeLimit');

//     spaceIDs.forEach((spaceInput, index) => {
//         const spaceID = spaceInput.value;
//         const timeLimit = timeLimits[index].value; // Assuming the same index
//         if (spaceID && timeLimit) {
//             spacesDict[spaceID] = parseInt(timeLimit);
//         }
//     });

//     return spacesDict;
// }

// function collectSpacesAndTimeLimitsToModify() {
//     let spacesDict = {};
//     // Assuming each spaceID input is directly followed by its corresponding timeLimit input
//     const spaceIDs = document.querySelectorAll('.modifySpaceID');
//     const timeLimits = document.querySelectorAll('.modifyTimeLimit');

//     spaceIDs.forEach((spaceInput, index) => {
//         const spaceID = spaceInput.value;
//         const timeLimit = timeLimits[index].value; // Assuming the same index
//         if (spaceID && timeLimit) {
//             spacesDict[spaceID] = parseInt(timeLimit);
//         }
//     });

//     return spacesDict;
// }



// var acc = document.getElementsByClassName("accordion");
// var i;

// for (i = 0; i < acc.length; i++) {
// acc[i].addEventListener("click", function() {
//     this.classList.toggle("active");
//     var panel = this.nextElementSibling;
//     if (panel.style.maxHeight) {
//     panel.style.maxHeight = null;
//     } else {
//     panel.style.maxHeight = panel.scrollHeight + "px";
//     }
// });
//  }
//  document.querySelectorAll('.accordion').forEach(button => {
//     button.addEventListener('click', () => {
//         const formContainer = button.nextElementSibling;
//         if (formContainer.style.display === 'none' || formContainer.style.display === '') {
//             formContainer.style.display = 'flex';
//         } else {
//             formContainer.style.display = 'none';
//         }
//     });
// });
// $(document).ready(function() {
//     // $('.accordion').click(function() {
//     //     this.classList.toggle("active");
//     //     var formContainer = $(this).next('.form-container');
//     //     if (formContainer.css('display') === "none") {
//     //         formContainer.css('display', 'flex'); // Show the form
//     //     } else {
//     //         formContainer.css('display', 'none'); // Hide the form
//     //     }
//     // });

//     $('#loadRequestsButton').click(function() {
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/createSpace',
//             type: 'GET',
//             dataType: 'json',
//             success: function(data) {
//                 $('#recoveryRequestsList').empty(); 
//                 data.forEach(function(request) {
//                     var listItem = '<li>Name : ' + request.floorPlanName + ', Spaces : ' + request.spaceList;
//                     if(request.resolveDate) {
//                         listItem += ', Resolve Date: ' + request.resolveDate;
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
//     $('#accountCreationForm').submit(function(e) {
//         e.preventDefault();
        

//         var companyFloor = {
//             FloorPlanName: $('#floorPlanName').val(),
//             FloorPlanImage: $('#imageBase64').val(),
//             FloorSpaces: collectSpacesAndTimeLimits()
            
//         };
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/postSpace',
//             type: 'POST',
//             contentType: 'application/json',
//             beforeSend: function(xhr) {
//                 xhr.setRequestHeader("Authorization", "Bearer " + localStorage["accessToken"]);
//             },
//             data: JSON.stringify(companyFloor),
//             success: function(response) {
//                 alert('Space created successfully!');
//                 console.log(response); // Log response for debugging
//             },
//             error: function(xhr, status, error) {
//                 console.error(xhr.responseText); // Log response text for debugging
//                 alert('Error creating space. ' + xhr.responseText);
//             }
//         });
//     });




//     $('#modifySpaceForm').submit(function(e) {
//         e.preventDefault();
    
//         var spacesToModify = collectSpacesAndTimeLimitsToModify();
//         console.log("Submitting with data:", spacesToModify); // Debug output
    
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/modifyTimeLimits',
//             type: 'POST',
//             contentType: 'application/json',
//             beforeSend: function(xhr) {
//                 xhr.setRequestHeader("Authorization", "Bearer " + localStorage["accessToken"]);
//             },
//             data: JSON.stringify(spacesToModify),
//             success: function(response) {
//                 console.log("Response from server:", response); // Debug output
//                 alert('Modified Time Limit successfully!');
//             },
//             error: function(xhr, status, error) {
//                 var errorMsg = xhr.responseJSON?.message || xhr.statusText;
//                 console.error("Error from server:", errorMsg); // Debug output
//                 alert('Error Modifying Time Limit: ' + errorMsg);
//             }
//         });
//     });

//     $('#modifyImage').click(function(e) {
//         e.preventDefault();
//         console.log('Modify image button clicked');
    
//         var newFloor = {
//             FloorPlanName: $('#modifyFloorPlanName').val(),
//             NewFloorPlanImage: $('#modifyImageBase64').val()
//         };
    
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/modifyFloorPlan',
//             type: 'POST',
//             contentType: 'application/json',
//             beforeSend: function(xhr) {
//                 xhr.setRequestHeader("Authorization", "Bearer " + localStorage["accessToken"]);
//             },
//             data: JSON.stringify(newFloor),
//             success: function(response) {
//                 console.log("Response from server:", response); // Debug output
//                 alert('Modified Image successfully!');
//             },
//             error: function(xhr, status, error) {
//                 var errorMsg = xhr.responseJSON?.message || xhr.statusText;
//                 console.error("Error from server:", errorMsg); // Debug output
//                 alert('Error Modifying Image: ' + errorMsg);
//             }
//         });
//     });

//     $('#deleteSpace').click(function(e) {
//         e.preventDefault();
//         console.log('Delete button clicked');
//         var spaceID = $(this).closest('.spaceRowModify').find('.modifySpaceID').val();
//         console.log("Space ID to delete:", spaceID);
    
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/deleteSpace',
//             type: 'POST',
//             contentType: 'application/json',
//             beforeSend: function(xhr) {
//                 xhr.setRequestHeader("Authorization", "Bearer " + localStorage["accessToken"]);
//             },
//             data: JSON.stringify({ spaceID: spaceID }),
//             success: function(response) {
//                 console.log("Response from server:", response); // Debug output
//                 alert('Deleted Space successfully!');
//             },
//             error: function(xhr, status, error) {
//                 var errorMsg = xhr.responseJSON?.message || xhr.statusText;
//                 console.error("Error from server:", errorMsg); // Debug output
//                 alert('Error Deleting Space: ' + errorMsg);
//             }
//         });
//     });

//     $('#deleteFloor').click(function(e) {
//         e.preventDefault();
//         console.log('Delete Floor button clicked');
//         var FloorPlanName = $('#modifyFloorPlanName').val();
//         console.log("Floor Plan to delete:", FloorPlanName);
    
//         $.ajax({
//             url: 'http://localhost:8081/api/SpaceManager/deleteFloor',
//             type: 'POST',
//             contentType: 'application/json',
//             beforeSend: function(xhr) {
//                 // Include the JWT token in the request header
//                 xhr.setRequestHeader("Authorization", "Bearer " + localStorage["accessToken"]);
//             },
//             data: JSON.stringify({ FloorPlanName: FloorPlanName }),
//             success: function(response) {
//                 console.log("Response from server:", response); // Debug output
//                 alert('Deleted Floor successfully!');
//             },
//             error: function(xhr, status, error) {
//                 var errorMsg = xhr.responseJSON?.message || xhr.statusText;
//                 console.error("Error from server:", errorMsg); // Debug output
//                 alert('Error Deleting Floor: ' + errorMsg);
//             }
//         });
//     });
// });