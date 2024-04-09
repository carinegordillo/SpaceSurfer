window.addEventListener('DOMContentLoaded', (event) => {
    loadContent(window.location.hash);
    window.addEventListener('hashchange', (event) => {
        loadContent(window.location.hash);
    });
});

function loadContent(route) {
    const mainContent = document.getElementById('main-content');
    switch (route) {
        case '#home':
            mainContent.innerHTML = '<h1>Home Page</h1><p>Welcome to the home page</p>';
            break;
        case '#login':
            loadPage('../Login/index.html', mainContent, loadLoginScript);
            break;
        case '#about':
            loadPage('../UnAuthnAbout/about.html', mainContent);
            break;
        default:
            mainContent.innerHTML = '<p>Starting page</p>';
    }
}

function loadPage(pagePath, targetElement, callback) {
    fetch(pagePath)
        .then(response => response.text())
        .then(html => {
            targetElement.innerHTML = html;
            if (typeof callback === 'function') {
                callback();
            }
        })
        .catch(error => {
            console.error('Error loading page:', error);
        });
}

function loadLoginScript() {
    const scriptElement = document.createElement('script');
    scriptElement.src = '../Login/script.js';
    document.body.appendChild(scriptElement);
}
