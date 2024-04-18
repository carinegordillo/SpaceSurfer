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
    .catch(error => console.error('Failed to fetch tasks:', error));
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
