function logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('idToken');
    window.location.href = '/UnAuthnAbout/about.html';
}

