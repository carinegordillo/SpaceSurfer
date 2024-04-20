'use strict';

(function (root) {


    function fromHTML(html, trim = true) {
       
        html = trim ? html.trim() : html;
        if (!html) return null;

       
        const template = document.createElement('template');
        template.innerHTML = html;
        const result = template.content.children;

        if (result.length === 1) return result[0];
        return result;
    }

    root.myApp = root.myApp || {};
    async function init() {
        window.location = "UnAuthnAbout/about.html"
        //window.location = "SpaceManager/index.html"

        if (localStorage.length != 0) {
            token = localStorage["accessToken"]
        }

        if (!token) {
            window.location = "UnAuthnAbout/about.html"
        } else {
            window.location = "UnAuthnAbout/about.html"
        }
    }

    init();

})(window, window.ajaxClient);