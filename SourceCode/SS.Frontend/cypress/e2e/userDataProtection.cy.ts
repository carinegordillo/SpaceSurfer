describe('User Data Protection Tests', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"AFFKahiwAYT+JMsdsVLyF+WWV9nKpyTvU1gaTp9Z+q4=","Aud":"spacesurfers","Exp":638503722849984500,"Iat":638494289499417369,"Claims":{"Role":"2"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"mzAJJzqPELCJGZ_a7oyacIsozDSpsao-21jrG-bmDpk"}';
    beforeEach(() => {
        cy.intercept('POST', 'http://localhost:5270/api/auth/sendOTP', {
            statusCode: 200
        }).as('sendOTP');

        cy.intercept('POST', 'http://localhost:5270/api/auth/authenticate', {
            statusCode: 200,
            body: {
                accessToken: token,
                idToken: '{"Username": "AFFKahiwAYT+JMsdsVLyF+WWV9nKpyTvU1gaTp9Z+q4="}'
            }
        }).as('authenticate').then(() => {
            cy.window().then(window => {
                window.sessionStorage.setItem('accessToken', token);
                window.sessionStorage.setItem('idToken', '{"Username": "AFFKahiwAYT+JMsdsVLyF+WWV9nKpyTvU1gaTp9Z+q4="}');
            });
        });

        cy.visit('http://localhost:3000/Login/index.html');

    });

    it('should handle OTP verification and login', () => {
        cy.get('#sendOTPSection').invoke('show');
        cy.get('#userIdentity', { force: true }).type('testUser');
        cy.get('button').contains('Send Verification', { force: true }).click();
        cy.wait('@sendOTP');

        cy.get('#enterOTPSection').invoke('attr', 'style', 'display: block !important');

        cy.get('#otp').type('123456');
        cy.get('button').contains('Login').click();
        cy.wait('@authenticate');
        cy.get('body').then($body => {
            if ($body.find('#homepageGen').css('display') === 'none') {
                cy.get('#homepageGen').invoke('show');
            }
        });

        cy.get('#homepageGen').should('be.visible');
        cy.get('#enterOTPSection').invoke('css', 'display', 'none');
    });

    it('should navigate to the user data protection view when clicking on the feature button', () => {
        cy.get('[onclick="initProfile()"]').click();
        cy.get('#userProfileView').should('be.visible');
        cy.get('#userProfileView button').contains('User Data Protection').click();
        cy.get('#cpraView').should('be.visible');
    });

    it('should navigate to request data', () => {
        cy.get('[onclick="initProfile()"]').click();
        cy.get('#userProfileView').should('be.visible');
        cy.get('#userProfileView button').contains('User Data Protection').click();
        cy.get('#cpraView').should('be.visible');
        cy.contains('Request Data').click();
        cy.get('#requestDataSection').should('be.visible').should('contain', 'Request Your Data');
        cy.get('#username').type('testUser', { force: true });
        cy.get('#requestDataForm').submit();
    });

    it('should navigate to delete data', () => {
        cy.get('[onclick="initProfile()"]').click();
        cy.get('#userProfileView').should('be.visible');
        cy.get('#userProfileView button').contains('User Data Protection').click();
        cy.get('#cpraView').should('be.visible');
        cy.contains('Delete Data').click();
        cy.get('#deleteDataSection').should('be.visible').should('contain', 'Delete Your Data');
        cy.get('#username').type('testUser', { force: true });
        cy.get('#deleteDataForm').submit();
    });
});
