//function checkTokenExpiration() {
//    var accessToken = sessionStorage.getItem('accessToken');
//    if (!accessToken) {
//        console.error('Access token not found.');
//        window.location.href = 'UnAuthnAbout/about.html';
//        return;
//    }

//    try {
//        var decodedToken = parseJwt(accessToken);

//        var currentTime = Math.floor(Date.now() / 1000);
//        if (decodedToken.exp < currentTime) {
//            console.log('Token has expired.');
//            sessionStorage.removeItem('accessToken');
//            window.location.href = 'UnAuthnAbout/about.html';
//        }
//    } catch (error) {
//        console.error('Error decoding token:', error);
//    }
//}
//function parseJwt(token) {
//    var base64Url = token.split('.')[1];
//    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
//    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
//        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
//    }).join(''));
//    return JSON.parse(jsonPayload);
//}
//async function refreshToken() {
//    var accessToken = sessionStorage.getItem('accessToken');
//    if (!accessToken) {
//        console.error('Access token not found.');
//        window.location.href = 'UnAuthnAbout/about.html';
//        return;
//    }

//    try {
//        var decodedToken = parseJwt(accessToken);
//        var { role, subject } = decodedToken;

//        var tokenData = { username: subject, roles: { 'Role': role } };

//        var newTokenResponse = await fetch('http://localhost:8081/api/auth/refreshToken', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json',
//            },
//            body: JSON.stringify(tokenData)
//        });

//        if (!newTokenResponse.ok) {
//            throw new Error('Failed to refresh token');
//        }

//        var newToken = await newTokenResponse.text();

//        sessionStorage.removeItem('accessToken');
//        sessionStorage.setItem('accessToken', newToken);
//    } catch (error) {
//        console.error('Error:', error);
//    }
//}


function logout() {
    sessionStorage.removeItem('accessToken');
    window.location.href = '../UnAuthnAbout/about.html';
}

document.getElementById('replaceImage').addEventListener('change', function (event) {

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

document.getElementById('addSpace').addEventListener('click', function () {

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

document.getElementById('addSpaceModify').addEventListener('click', function () {

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

document.addEventListener('DOMContentLoaded', async function () {
    if (!sessionStorage.getItem('accessToken')) {
        // Redirect if accessToken not found
        window.location.href = '../UnAuthnAbout/about.html';
        return; 
    }
    document.getElementById('loadRequestsButton').addEventListener('click', function () {

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

    document.getElementById('accountCreationForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        const companyFloor = {
            FloorPlanName: document.getElementById('floorPlanName').value,
            FloorPlanImage: document.getElementById('imageBase64').value,
            FloorSpaces: collectSpacesAndTimeLimits()
        };
        sendData('http://localhost:8081/api/SpaceManager/postSpace', companyFloor, 'accessToken');
    });

    document.getElementById('modifySpaceForm').addEventListener('submit', async function(e) {
        e.preventDefault();
        const spacesToModify = collectSpacesAndTimeLimitsToModify();
        sendData('http://localhost:8081/api/SpaceManager/modifyTimeLimits', spacesToModify, 'accessToken');
    });

    document.getElementById('modifyImage').addEventListener('click', function () {
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

    document.getElementById('deleteFloor').addEventListener('click', function () {
        const FloorPlanName = document.getElementById('modifyFloorPlanName').value;
        sendData('http://localhost:8081/api/SpaceManager/deleteFloor', { FloorPlanName: FloorPlanName }, 'accessToken');
    });
});

async function sendData(url, data, tokenKey) {
    try {
        const token = sessionStorage.getItem(tokenKey);
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            throw new Error('Failed to send data');
        }
        const responseData = await response.json();
        console.log('Success:', responseData);
        alert('Operation successful!');
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred. Please try again.');
    }
}