document.getElementById('loadRequestsButton').addEventListener('click', function() {
    console.log("This button haas been clicked")
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














document.getElementById('viewTasksBtn').addEventListener('click', function() {
    fetchAndDisplayTasks(userName, accessToken);
    console.log("BUTTON CLICKED")
});
async function checkTokenExpiration(accessToken) {
    try {
        var response = await fetch('http://localhost:5099/api/waitlist/checkTokenExp', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        return response.ok;

    } catch (error) {
        console.error('Error:', error);
        return false;
    }
}

// Function to fetch waitlisted reservations and display them
async function fetchAndDisplayTasks() {
    console.log(" thisBUTTON CLICKED")
    var accessToken = sessionStorage.getItem('accessToken');
    var idToken = sessionStorage.getItem('idToken');
    var parsedIdToken = JSON.parse(idToken);
    var username = parsedIdToken.Username;

    const isTokenExp = await checkTokenExpiration(accessToken);
    if (isTokenExp) {
        logout();
        return;
    }

    try {
        const response = await fetch(`http://localhost:8089/api/v1/taskManagerHub/ListTasks?userName=${encodeURIComponent(userName)}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + accessToken,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        var tasks = await response.json();
        displayTasks(tasks);

        if (data.newToken) {
            accessToken = data.newToken;
            sessionStorage.setItem('accessToken', accessToken);
            console.log('New access token stored:', accessToken);
        }

        // Clear the existing list of waitlisted reservations
        
    } catch (error) {
        console.error('Error fetching waitlisted reservations:', error);
    }
}



function displayTasks(tasks) {
    const taskListElement = document.getElementById('taskList');
    taskListElement.innerHTML = ''; // Clear current list

    tasks.forEach(task => {
        const taskItem = document.createElement('div');
        taskItem.classList.add('task-item');
        taskItem.innerHTML = `
            <span class="task-title">${task.title}</span>
            <button onclick="modifyTask('${task.id}', '${task.title}')">Modify</button>
            <button onclick="deleteTask('${task.id}')">Delete</button>
        `;
        taskListElement.appendChild(taskItem);
    });
}












function createTask(taskTitle, accessToken) {
    fetch(`http://localhost:8089/api/v1/taskManagerHub/CreateTask`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + accessToken
        },
        body: JSON.stringify({ title: taskTitle })
    })
    .then(response => {
        if (!response.ok) throw new Error('Failed to create task');
        alert('Task created!');
        fetchAndDisplayTasks(); // Refresh the list
    })
    .catch(error => console.error('Failed to create task', error));
}

function modifyTask(taskId, taskTitle) {
    const newTitle = prompt("Enter new title for the task:", taskTitle);
    if (!newTitle) return;

    fetch(`http://localhost:8089/api/v1/taskManagerHub/ModifyTask/${taskId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${sessionStorage.getItem('accessToken')}`
        },
        body: JSON.stringify({ title: newTitle })
    })
    .then(response => {
        if (!response.ok) throw new Error('Failed to modify task');
        alert('Task modified!');
        fetchAndDisplayTasks(); // Refresh the list
    })
    .catch(error => console.error('Failed to modify task', error));
}

function deleteTask(taskId) {
    if (!confirm("Are you sure you want to delete this task?")) return;

    fetch(`http://localhost:8089/api/v1/taskManagerHub/DeleteTask/${taskId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${sessionStorage.getItem('accessToken')}`
        }
    })
    .then(response => {
        if (!response.ok) throw new Error('Failed to delete task');
        alert('Task deleted!');
        fetchAndDisplayTasks(); // Refresh the list
    })
    .catch(error => console.error('Failed to delete task', error));
}
