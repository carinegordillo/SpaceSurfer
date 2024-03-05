'use strict';

// Immediately Invoke Function Execution (IIFE or IFE)
// Protects functions from being exposed to the global object
(function (root, ajaxClient) {
    // Dependency check
    const isValid = root && ajaxClient;

    if(!isValid){
        // Handle missing dependencies
        alert("Missing dependencies");
    }

    const webServiceUrl = 'http://localhost:8080';

    // NOT exposed to the global object ("Private" functions)
    function getDataHandler() {
        var request = ajaxClient.get(webServiceUrl);

         request.then(function (response) {

            const contentElement = document.getElementsByClassName('dynamic-content')[0];
            
            // Dynamically build HTML
            var content = '<ul>';

            var promisePayload = response.data;
            var apiPayload = promisePayload.numbers;

            for(var i = 0; i < apiPayload.length; i++) {
                content += '<li>' + apiPayload[i] + '</li>'
            }

            content += '</ul>'

            /* 
                This was not a production-ready method to dynamically build views.
                Instead, use either createDocumentFragmen, createElement/append or a JS frameworks.
            */
            contentElement.innerHTML = content; 
        });
    }

    // NOT exposed to the global object ("Private" functions)
    function myEndpointHandler() {
        var request = ajaxClient.get(webServiceUrl + '/myEndpoint');

        request.then(function (response) {
            const contentElement = document.getElementsByClassName('dynamic-content')[0];
            contentElement.innerHTML = response.data;
        })
    }

    // NOT exposed to the global object ("Private" functions)
    function sendDataHandler(url) {
        var request = ajaxClient.send(url, {
                data: ['Alice', 'Bob', 'John'] // Hard-coding data
            });

        const contentElement = document.getElementsByClassName('dynamic-content')[0];

        request
            .then(function (response) {
                
                const timestamp = new Date();

                contentElement.innerHTML = response.data + " " + timestamp.toString(); 
            })
            .catch(function (error) {
                console.log("Send", url, error.response.data); // Payload error message

                contentElement.innerHTML = error; // Only showing top level error message
            });
    }

    function loginHandler(username, password){
        var request = ajaxClient.post(webServiceUrl + '/login', {username, password});

        const contentElement = document.getElementsByClassName('dynamic-content')[0];

        request
            .then(function (response) {
                const token = response.data.token;
                localStorage.setItem('token', token);
                console.log('Login successful');
                contentElement.innerHTML = response.data;
            })
            .catch(function (error) {
                console.error('Login failed', error);
                contentElement.innerHTML = error;
            });

    }

    function fetchDataHandler(){
        
        const token = localStorage.getItem('token');
        var request = ajaxClient.get(webServiceUrl + '/protected', {
            headers:{
                'Authorization': `Bearer ${token}`
            }
        });

        const contentElement = document.getElementsByClassName('dynamic-content')[0];

        request
            .then(function (response) {
                console.log('Data:', response.data);
                contentElement.innerHTML = response.data;
            })
            .catch(function (error) {
                console.error('Error fetching data:', error);
                contentElement.innerHTML = error;
            });
    }


    root.myApp = root.myApp || {};

    // Show or Hide private functions
    //root.myApp.getData = getDataHandler;
    //root.myApp.sendData = sendDataHandler;

    // Initialize the current view by attaching event handlers 
    function init() {
        // Note that normally you would want to use Event Bubbling to register events for child elements
        // that could grow over time 
        var getDataElement = document.getElementById('get');
        var myEndpointElement = document.getElementById('myEndpoint');
        var sendDataElement = document.getElementById('send');
        var sendDataErrorElement = document.getElementById('sendError');
        var loginElement = document.getElementById('login');
        var fetchDataElement = document.getElementById('protected');

        // Dynamic event registration
        getDataElement.addEventListener('click', getDataHandler);
        myEndpointElement.addEventListener('click', myEndpointHandler);
        sendDataElement.addEventListener('click', () => sendDataHandler(webServiceUrl));
        sendDataErrorElement.addEventListener('click', () => sendDataHandler(webServiceUrl + "/error") );
        loginElement.addEventListener('click', () => loginHandler('username', 'password'));
        fetchDataElement.addEventListener('click', () => fetchDataHandler);
    }

    init();

})(window, window.ajaxClient);

