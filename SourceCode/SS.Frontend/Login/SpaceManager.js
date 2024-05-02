
document.getElementById('replaceImage').addEventListener('change', function(event) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    event.preventDefault();

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
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    event.preventDefault();
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
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
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
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
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
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
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
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
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


document.getElementById('spaceCreationForm').addEventListener('submit', function(e) {
    e.preventDefault();
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    const companyFloor = {
        hashedUsername : JSON.parse(sessionStorage.getItem('idToken')).Username,
        FloorPlanName: document.getElementById('floorPlanName').value,
        FloorPlanImage: document.getElementById('imageBase64').value,
        FloorSpaces: collectSpacesAndTimeLimits()
    };
    sendData('http://localhost:8081/api/SpaceManager/postSpace', companyFloor, 'accessToken');
});

document.getElementById('modifySpaceForm').addEventListener('submit', function(e) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    e.preventDefault();
    const spacesToModify = collectSpacesAndTimeLimitsToModify();
    const idToken = JSON.parse(sessionStorage.getItem('idToken'));
    if (idToken && idToken.Username) {
        spacesToModify.hashedUsername = idToken.Username;
    } else {
        console.error('ID Token is missing or malformed');
        return; 
    }
    sendData('http://localhost:8081/api/SpaceManager/modifyTimeLimits', spacesToModify, 'accessToken');
});

document.getElementById('modifyImage').addEventListener('click', function(e) {
    e.preventDefault();
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    const newFloor = {
        hashedUsername: JSON.parse(sessionStorage.getItem('idToken')).Username,
        FloorPlanName: document.getElementById('modifyFloorPlanName').value,
        FloorPlanImage: document.getElementById('modifyImageBase64').value
    };
    sendData('http://localhost:8081/api/SpaceManager/modifyFloorPlan', newFloor, 'accessToken');
});

document.addEventListener('click', function(e) {
    e.preventDefault();
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    if (e.target.classList.contains('deleteSpace')) {
        const spaceModifier = {
            hashedUsername: JSON.parse(sessionStorage.getItem('idToken')).Username,
            spaceID : e.target.closest('.spaceRowModify').querySelector('.modifySpaceID').value
        }
        sendData('http://localhost:8081/api/SpaceManager/deleteSpace', spaceModifier, 'accessToken');
    }
});

document.getElementById('deleteFloor').addEventListener('click', function() {
    e.preventDefault();
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    const spaceModifier = {
        hashedUsername: JSON.parse(sessionStorage.getItem('idToken')).Username,
        FloorPlanName : document.getElementById('modifyFloorPlanName').value
    }
    sendData('http://localhost:8081/api/SpaceManager/deleteFloor', spaceModifier, 'accessToken');
});


function sendData(url, data, tokenKey) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
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
