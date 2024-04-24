document.getElementById('viewTasksBtn').addEventListener('click', function(event) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    event.preventDefault();
    console.log("This button has been clicked");
    fetchAndDisplayTasks();
});

document.getElementById('ScoreTasks').addEventListener('click', function(event) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    event.preventDefault();
    console.log("This score tasks button has been clicked");
    fetchAndScoreTasks();
});

let allTasks = [];
function fetchAndDisplayTasks() {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }

    const userName = JSON.parse(sessionStorage.getItem('idToken')).Username;

    fetch(`http://localhost:8089/api/v1/taskManagerHub/ListTasks?userName=${encodeURIComponent(userName)}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${accessToken}`,
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        allTasks = data;
        console.log(data); // Log the full response to see its structure
        displayTasks(data); // Make sure 'tasks' is the correct key
    })
    .catch(error => console.error('Failed to fetch tasks: this is the username ',userName,  error));
}
function filterTasksByPriority() {
    const selectedPriority = document.getElementById('priorityFilter').value;
    const filteredTasks = allTasks.filter(task => {
        return selectedPriority === 'all' || task.priority.toLowerCase() === selectedPriority;
    });
    displayTasks(filteredTasks);
}

document.getElementById('filterTasksBtn').addEventListener('click', function(event) {
    event.preventDefault();
    filterTasksByPriority();
});
function fetchAndScoreTasks() {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }

    const userName = JSON.parse(sessionStorage.getItem('idToken')).Username;

    fetch(`http://localhost:8089/api/v1/taskManagerHub/ScoreTasks?userName=${encodeURIComponent(userName)}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${accessToken}`,
            'Content-Type': 'application/json'
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log(data); 
        displayTasks(data); 
    })
    .catch(error => console.error('Failed to score tasks: this is the username ',userName,  error));
}

function displayTasks(tasks) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    const taskListElement = document.getElementById('taskList');
    taskListElement.innerHTML = ''; 
    const userName = JSON.parse(sessionStorage.getItem('idToken')).Username;

    const taskListActive = document.getElementById('taskListActive');
    const taskListCompleted = document.getElementById('taskListCompleted');

    taskListActive.innerHTML = '';
    taskListCompleted.innerHTML = '';

    tasks.forEach(task => {
        const taskItem = document.createElement('li');
        taskItem.classList.add('task-item');
        let priorityColor = getPriorityColor(task.priority);

        // Check if the task is completed
        let completedTasks = JSON.parse(localStorage.getItem('completedTasks') || '{}');
        let isChecked = completedTasks[task.title] ? 'checked' : '';

        taskItem.innerHTML = `
            <div class="task-header">
                <input type="checkbox" ${isChecked} onchange="toggleTaskCompletion('${task.title}', this.checked)">
                <strong>Title:</strong> ${task.title}
            </div>
            <div><strong>Description:</strong> ${task.description}</div>
            <div><strong>Due Date:</strong> ${new Date(task.dueDate).toLocaleDateString()}</div>
            <div><strong>Priority:</strong> <span style="color: ${priorityColor};">${task.priority}</span></div>
            <button onclick="showModifyForm('${task.title}', '${task.description}', '${task.dueDate}', '${task.priority}', '${userName}')">Modify</button>
            <button onclick="deleteTask('${task.title}', '${userName}')">Delete</button>
        `;

        if (completedTasks[task.title]) {
            taskItem.classList.add('completed');
            taskListCompleted.appendChild(taskItem);
        } else {
            taskListActive.appendChild(taskItem);
        }
    });
}

function toggleTaskCompletion(taskTitle, completed) {
    let completedTasks = JSON.parse(localStorage.getItem('completedTasks') || '{}');
    if (completed) {
        completedTasks[taskTitle] = true;
    } else {
        delete completedTasks[taskTitle];
    }
    localStorage.setItem('completedTasks', JSON.stringify(completedTasks));
    displayTasks(allTasks); // Assuming you have a way to retrieve all tasks, like a global variable or function
}


function getPriorityColor(priority) {
    switch (priority) {
        case 'High': return 'red';
        case 'Medium': return 'orange';
        case 'Low': return 'green';
        default: return 'grey';
    }
}



document.getElementById('createTaskForm').addEventListener('submit', function(event) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    event.preventDefault(); 
    console.log("Create task form has been submitted");

    const title = document.getElementById('taskTitle').value.trim();
    const description = document.getElementById('taskDescription').value.trim();
    const dueDate = new Date(document.getElementById('taskDueDate').value);
    const priority = document.getElementById('taskPriority').value;

    // Validation
    if (!title || !description || !dueDate || !priority) {
        showModal('All fields must be filled out correctly.');
        return;
    }

    if(title.length > 20){
        showModal('Task title is too long');
        return;
    }

    if (dueDate < new Date()) {
        showModal('Due date cannot be in the past.');
        return;
    }

    if (!['High', 'Medium', 'Low'].includes(priority)) {
        showModal('Invalid priority selected.');
        return;
    }

    const task = {
        hashedUsername: JSON.parse(sessionStorage.getItem('idToken')).Username,
        title: title,
        description: description,
        dueDate: dueDate,
        priority: priority,
    };
    // const userName = JSON.parse(sessionStorage.getItem('idToken')).Username;
    createTask(task);
    fetchAndDisplayTasks();
    document.getElementById('createTaskForm').reset();

});

function createTask(task) {
    const accessToken = sessionStorage.getItem('accessToken'); 
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    fetch('http://localhost:8089/api/v1/taskManagerHub/CreateTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(task)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(data => Promise.reject(data));
        }
        return response.json();
    })
    .then(data => {
        console.log('Task created successfully', data);
        // alert('Task created successfully!');
        showModal('Task created successfully!');
    })
    .catch(error => {
        console.error('Failed to create task', error);
        // alert('Failed to create task: ' + (error.message || JSON.stringify(error)));
        showModal('Failed to create task: ' + (error.message || JSON.stringify(error)));
    });
}

function deleteTask(taskTitle, userName) {
    const task = {
        hashedUsername: userName,
        title: taskTitle
    };
    const accessToken = sessionStorage.getItem('accessToken'); 
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }
    fetch(`http://localhost:8089/api/v1/taskManagerHub/DeleteTask`, {
        method: 'POST', // Change to 'DELETE' if your server supports it
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(task)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to delete task');
        }
        return response.json();
    })
    .then(() => {
        // alert('Task deleted successfully!');
        showModal('Task deleted successfully!');
        fetchAndDisplayTasks(); // Refresh the task list after deletion
    })
    .catch(error => {
        console.error('Failed to delete task', error);
        showModal('Failed to delete task'+ error.message);
        // alert('Failed to delete task: ' + error.message);
    });
}


function showModifyForm(title, description, dueDate, priority, userName) {
    console.log("Showing modify form for: " + title);
    const modifyForm = document.getElementById('modifyTaskForm'); 
    const formContainer = document.getElementById('modifyTaskFormContainer');

    const newForm = modifyForm.cloneNode(true);
    modifyForm.parentNode.replaceChild(newForm, modifyForm);
    
    document.getElementById('modTitle').textContent = title;
    document.getElementById('modDescription').value = description;
    document.getElementById('modDueDate').value = dueDate.split('T')[0];
    document.getElementById('modPriority').value = priority;

    formContainer.style.display = 'block';

    // Add an event listener to the new form
    newForm.addEventListener('submit', function(event) {
        event.preventDefault();

        const updatedDescription = document.getElementById('modDescription').value.trim();
        const updatedDueDate = new Date(document.getElementById('modDueDate').value);
        const updatedPriority = document.getElementById('modPriority').value;

        // Validation before sending the request
        if (!updatedDescription || updatedDueDate < new Date() || !['High', 'Medium', 'Low'].includes(updatedPriority)) {
            showModal('Please ensure all fields are correctly filled and valid.');
            return;
        }

        // Call the modifyTask function with updated fields
        modifyTask(title, userName, {
            description: updatedDescription,
            dueDate: updatedDueDate,
            priority: updatedPriority
        });

        formContainer.style.display = 'none';
    });
}

function modifyTask(originalTitle, userName, updatedFields) {
    const accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Token expired or invalid');
        logout();
        return;
    }

    const modifyRequest = {
        UserName: userName,
        TaskTitle: originalTitle,
        FieldsToUpdateJson: JSON.stringify(updatedFields)
    };

    fetch('http://localhost:8089/api/v1/taskManagerHub/ModifyTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(modifyRequest)
    })
    .then(response => response.json())
    .then(data => {
        console.log('Task modified successfully', data);
        showModal('Task modified successfully!');
        fetchAndDisplayTasks(); // Assuming this is a function that refreshes the task list
    })
    .catch(error => {
        console.error('Failed to modify task', error);
        showModal('Failed to modify task: ' + error.message);
    });
}

function showModal(message) {
    var modal = document.getElementById('modal');
    var modalMessage = document.getElementById('modal-message');
    modalMessage.textContent = message;
    modal.style.display = 'block';

    var closeButton = document.querySelector('.close-button');
    closeButton.onclick = function() {
        modal.style.display = 'none';
    };
}


async function checkTokenExpiration(accessToken) {
    try {
        const response = await fetch('http://localhost:5099/api/auth/checkTokenExp', {
            headers: {
                'Authorization': `Bearer ${accessToken}`
            }
        });
        const data = await response.json();
        return data.isTokenValid; // assuming API returns { isTokenValid: true/false }
    } catch (error) {
        console.error('Error checking token expiration:', error);
        return false;
    }
}

function logout() {
    console.log("logout cliced")
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    sessionStorage.removeItem('userIdentity')
    var identityDiv = document.getElementById("identity");
    if (identityDiv) {
        console.log("Identity element found, current display:", identityDiv.style.display);
        identityDiv.style.display = "none";
        console.log("Identity should now be hidden, new display:", identityDiv.style.display);
    } else {
        console.log("Identity element not found");
    }
    document.getElementById("homepageGen").style.display = "none";
    document.getElementById("homepageManager").style.display = "none";
    document.getElementById("sendOTPSection").style.display = "block";
    document.getElementById("taskManagerView").style.display = "none";
}
