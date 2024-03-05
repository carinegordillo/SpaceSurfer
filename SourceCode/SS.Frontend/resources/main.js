
document.addEventListener('click', function () {
    document.getElementById('loadRequests').addEventListener('click', function () {
        console.log('Button clicked'); // Print statement for debugging
        fetch('http://localhost:8080/api/employees/getAllDummyRequests')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Data received:', data); // Print received data for debugging
                const container = document.getElementById('requestsContainer');
                container.innerHTML = ''; // Clear previous content
                let content = '<ul>';
                data.forEach(employee => {
                    console.log('Processing employee:', employee); // Print each employee for debugging
                    content += `<li>Name: ${employee.Name}, Position: ${employee.Position}</li>`;
                });
                content += '</ul>';
                container.innerHTML = content;
            })
            .catch(error => console.error('Fetch error:', error));
    });
});

function init() {
    console.log('Initializing main.js'); // Print statement for debugging
    var getDataElement = document.getElementById('get');

    getDataElement.addEventListener('click', function () {
        console.log('Button clicked'); // Print statement for debugging
        getDataHandler();
    });
}

init();