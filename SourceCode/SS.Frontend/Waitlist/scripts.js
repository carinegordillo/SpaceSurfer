function logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    window.location.href = '/UnAuthnAbout/about.html';
}

function test() {
    var accessToken = sessionStorage.getItem('accessToken');
    if (!accessToken) {
        console.error('Access token is not available.');
        return;
    }

    $.ajax({
        url: 'http://localhost:5099/api/waitlist/checkTokenExp',
        type: 'GET',
        headers: {
            'Authorization': 'Bearer ' + accessToken
        },
        contentType: 'application/json',
        success: function (response) {
            console.log(response);
            if (response == true) {
                logout();
            }
            else {
                $.ajax({
                    url: 'http://localhost:5099/api/waitlist/button',
                    type: 'GET',
                    headers: {
                        'Authorization': 'Bearer ' + accessToken
                    },
                    contentType: 'application/json',
                    success: function (response) {
                        console.log(response);
                        if (response) {
                            var message = "Test successful";
                            var messageElement = document.createElement('p');
                            messageElement.textContent = message;
                            messageElement.style.color = 'green';

                            var testSection = document.getElementById('testSection');
                            testSection.appendChild(messageElement);

                            if (response.newToken) {
                                sessionStorage.setItem('accessToken', response.newToken);
                                console.log('New access token stored:', response.newToken);
                            }
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error('Error:', error);
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}


