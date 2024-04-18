document.getElementById('viewTasksBtn').addEventListener('click', function(event) {
    event.preventDefault();
    console.log("This button has been clicked");
    fetchAndDisplayTasks();
});

async function fetchAndDisplayTasks() {
    const accessToken = sessionStorage.getItem('accessToken');
    // if (!accessToken || !(await checkTokenExpiration(accessToken))) {
    //     console.error('Token expired or invalid');
    //     logout();
    //     return;
    // }

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
        console.log(data); // Log the full response to see its structure
        displayTasks(data); // Make sure 'tasks' is the correct key
    })
    .catch(error => console.error('Failed to fetch tasks: this is the username ',userName,  error));
}

function displayTasks(tasks) {
    const taskListElement = document.getElementById('taskList');
    taskListElement.innerHTML = ''; // Clear current list

    tasks.forEach(task => {
        const taskItem = document.createElement('div');
        taskItem.classList.add('task-item');

        // Determine the color based on the priority
        let priorityColor = '';
        switch (task.priority) {
            case 'High':
                priorityColor = 'red';
                break;
            case 'Medium':
                priorityColor = 'orange';
                break;
            case 'Low':
                priorityColor = 'green';
                break;
            default:
                priorityColor = 'grey'; // Default color if priority is undefined or different
        }

        // Construct the HTML with the due date and colored priority
        taskItem.innerHTML = `
            <div><strong>Title:</strong> ${task.title}</div>
            <div><strong>Description:</strong> ${task.description}</div>
            <div><strong>Due Date:</strong> ${new Date(task.dueDate).toLocaleDateString()}</div>
            <div><strong>Priority:</strong> <span style="color: ${priorityColor};">${task.priority}</span></div>
            <div><strong>Notification Setting:</strong> ${task.notificationSetting}</div>
            <button onclick="modifyTask('${task.id}', '${task.title}')">Modify</button>
            <button onclick="deleteTask('${task.id}')">Delete</button>
        `;
        taskListElement.appendChild(taskItem);
    });
}



document.getElementById('createTaskForm').addEventListener('submit', function(event) {
    event.preventDefault(); // This stops the form from submitting traditionally, which prevents page refresh
    console.log("Create task form has been submitted");

    const task = {
        title: document.getElementById('taskTitle').value,
        description: document.getElementById('taskDescription').value,
        dueDate: document.getElementById('taskDueDate').value,
        priority: document.getElementById('taskPriority').value,
        notificationSetting: parseInt(document.getElementById('taskNotificationSetting').value, 10)
    };

    createTask(task);
});

function createTask(task) {
    const accessToken = sessionStorage.getItem('accessToken'); // Assume token is stored in sessionStorage
    fetch(`http://localhost:8089/api/v1/taskManagerHub/CreateTask?userName=${encodeURIComponent(userName)}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify(task)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Failed to create task');
        }
        return response.json();
    })
    .then(data => {
        console.log('Task created successfully', data);
        alert('Task created successfully!');
        // Optionally clear the form or update the UI here
        document.getElementById('createTaskForm').reset(); // This resets the form fields after successful submission
    })
    .catch(error => {
        console.error('Failed to create task', error);
        alert('Failed to create task: ' + error.message);
    });
}


// async function checkTokenExpiration(accessToken) {
//     try {
//         const response = await fetch('http://localhost:5099/api/auth/checkTokenExp', {
//             headers: {
//                 'Authorization': `Bearer ${accessToken}`
//             }
//         });
//         const data = await response.json();
//         return data.isTokenValid; // assuming API returns { isTokenValid: true/false }
//     } catch (error) {
//         console.error('Error checking token expiration:', error);
//         return false;
//     }
// }

// function logout() {
//     sessionStorage.removeItem('accessToken');
//     sessionStorage.removeItem('idToken');
//     alert('Session expired. Please login again.');
//     window.location.href = 'login.html'; // Adjust to your login page URL
// }
