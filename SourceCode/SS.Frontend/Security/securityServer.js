/*'use strict';

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

    
    function sendOTP() {
        // Define the request body
        const requestBody = {
            // Provide the necessary data for sending OTP
            // For example:
            username: 'exampleUsername',
        };

        // make POST request to endpoint to send OTP
        fetch('http://localhost:8080/api/securityAuth/postSendOTP', {
            metho: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestBody)
        })
        .then(response => {
            if (!response.ok){
                throw new Error('Netowrk response was not ok');
            }
            return response.json();
        })
        .then(data => {
            //handle response data
            console.log('OTP sent: ', data);
        })
        .catch(error => {
            //handle errors
            console.error("Error sending OTP: ", error);
        });
    }

    function authenticate() {
        // Define the authentication request body
        const authRequestBody = {
            // Provide the necessary data for authentication
            // For example:
            username: 'test@email',
            proof: 'ZNi9NfJDIyEeP6dnmtczFDK3eNeGSIvSSuhN4AzWhkI='//,
            //principal: ''
        };

        //make POST request to the endpoint for authN
        fetch('http://localhost:8080/api/securityAuth/postAuthenticate', {
            metho: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(authRequestBody)
        })
        .then(response => {
            if (!response.ok){
                throw new Error('Netowrk response was not ok');
            }
            return response.json();
        })
        .then(data => {
            //handle response data
            console.log('Authentication successful: ', data);
        })
        .catch(error => {
            //handle errors
            console.error("Authentication error: ", error);
        });
    }

    function authorize() {
        // Define the authorization request body
        const authRequestBody = {
            // Provide the necessary data for authorization
            // For example:
            currentPrincipal: { userId: 'test@email' },
            requiredClaims: { claim1: 'Admin'}
        };

        //make POST request to the endpoint for authZ
        fetch('http://localhost:8080/api/securityAuth/postAuthorize', {
            metho: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(authRequestBody)
        })
        .then(response => {
            if (!response.ok){
                throw new Error('Netowrk response was not ok');
            }
            return response.json();
        })
        .then(data => {
            //handle response data
            console.log('Authorization successful: ', data);
        })
        .catch(error => {
            //handle errors
            console.error("Authorization error: ", error);
        });
    }

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

            
                //This was not a production-ready method to dynamically build views.
                //Instead, use either createDocumentFragmen, createElement/append or a JS frameworks.
            
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
        //var loginElement = document.getElementById('login');
        var fetchDataElement = document.getElementById('protected');

        var sendOTPElement = document.getElementById('sendOTP');
        var authenticateElement = document.getElementById('authenticate');
        var authorizeElement = document.getElementById('authorize');

        // Dynamic event registration
        sendOTPElement.addEventListener('click', sendOTP);
        authenticateElement.addEventListener('click', authenticate);
        authorizeElement.addEventListener('click', authorize);
        
        // Dynamic event registration
        getDataElement.addEventListener('click', getDataHandler);
        myEndpointElement.addEventListener('click', myEndpointHandler);
        sendDataElement.addEventListener('click', () => sendDataHandler(webServiceUrl));
        sendDataErrorElement.addEventListener('click', () => sendDataHandler(webServiceUrl + "/error") );
        //loginElement.addEventListener('click', () => loginHandler('username', 'password'));
        fetchDataElement.addEventListener('click', () => fetchDataHandler);
    }

    init();
    */
// Define the base URL for your API

const baseUrl = "http://localhost:8080/api/securityAuth";

function postSendOTP() {
  const email = document.getElementById("emailForOTP").value;
  fetch(`${baseUrl}/postSendOTP`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: email }),
  })
    .then((response) => {
      if (!response.ok) throw new Error("Network response was not ok.");
      return response.json();
    })
    .then((data) => alert("OTP sent successfully."))
    .catch((error) => alert("Error sending OTP: " + error.message));
}

function postAuthenticate(event) {
  event.preventDefault(); // Prevent the form from submitting in the traditional way
  const email = document.getElementById("authEmail").value;
  const otp = document.getElementById("authOTP").value;
  fetch(`${baseUrl}/postAuthenticate`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: email, OTP: otp }),
  })
    .then((response) => {
      if (!response.ok) throw new Error("Network response was not ok.");
      return response.json();
    })
    .then((principal) => {
      console.log(principal);
      alert("Authentication successful.");
    })
    .catch((error) => alert("Authentication failed: " + error.message));
}

function postAuthorize() {
  const userId = document.getElementById("userId").value;
  const userRole = document.getElementById("userRole").value;
  const requiredPermission = document.getElementById("requiredPermission").value;
  const requiredResource = document.getElementById("requiredResource").value;

  const requestBody = {
    currentPrincipal: {
      userId: userId,
      role: userRole,
    },
    requiredClaims: {
      permission: requiredPermission,
      resource: requiredResource,
    },
  };
  fetch(`${baseUrl}/postAuthorize`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(requestBody),
  })
    .then((response) => {
      if (!response.ok) throw new Error("Network response was not ok.");
      return response.json();
    })
    .then((data) => alert("Authorization check passed."))
    .catch((error) => alert("Authorization check failed: " + error.message));
}


// Example usage:
// sendOTP('user@example.com');
// authenticate('user@example.com', 'OTP_received');
// authorize({userId: 'user1'}, {claim1: 'Admin'});

//})(window, window.ajaxClient);
