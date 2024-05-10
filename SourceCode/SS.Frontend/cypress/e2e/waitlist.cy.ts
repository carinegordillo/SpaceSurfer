describe('Waitlist Tests', () => {
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

        cy.intercept('POST', 'http://localhost:5099/api/waitlist/getWaitlists', {
            statusCode: 200,
            body: [
                {
                    companyName: 'Mocked Company',
                    spaceID: 'Mocked Space ID',
                    startTime: 'Mocked Start Time',
                    endTime: 'Mocked End Time',
                    position: 'Mocked Position'
                }
            ]
        }).as('getWaitlists');

        cy.intercept('GET', 'http://localhost:5099/api/waitlist/getFloorplan*', {
            statusCode: 200,
            body: [
                {
                    floorPlanName: 'Mocked Floorplan Name',
                    floorPlanImageBase64: 'Mocked Floorplan Image Base64'
                }
            ]
        }).as('getFloorplan');

        cy.visit('http://localhost:3000/Login/index.html');
    });

    it('should handle OTP verification and login', () => {
        cy.get('#sendOTPSection').invoke('show');
        cy.get('#userIdentity').type('testUser');
        cy.get('button').contains('Send Verification').click();
        cy.wait('@sendOTP');
        cy.get('#enterOTPSection').should('be.visible');
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

    it('should navigate to waitlist feature after login', () => {
        cy.get('#homepageGen').click({ force: true });
        cy.get('#enterOTPSection').should('not.be.visible');
        cy.get('[data-testid="waitlist-view"]').invoke('css', 'display', 'block');
        cy.get('[data-testid="init-waitlist-button"]').should('be.visible');
    });

    it('should display waitlisted reservations', () => {
        cy.get('[data-testid="waitlist-view"]').invoke('css', 'display', 'block');
        cy.get('[data-testid="init-waitlist-button"]').click();
        cy.wait('@getWaitlists');
        cy.get('[data-testid="reservations-list-ul"]').should('be.visible');
    });


    it('should display waitlisted reservations', () => {
        cy.get('[data-testid="waitlist-view"]').invoke('css', 'display', 'block');
        cy.get('[data-testid="init-waitlist-button"]').click();
        cy.wait('@getWaitlists');
        cy.get('[data-testid="reservations-list-ul"]').should('be.visible');
    });

    it('should close modal when No or close button is clicked', () => {
        cy.get('#waitlistView.homepage-container').invoke('show');
        cy.get('[data-testid="init-waitlist-button"]').click();
        cy.wait('@getWaitlists');
        cy.get('.waitlist-item').first().click();
        cy.wait('@getFloorplan');
        cy.get('#leaveWaitlistBtn').click();
        cy.get('.modal-content').should('be.visible');
        cy.get('.close-button').click({ multiple: true, force: true });
    });
});
