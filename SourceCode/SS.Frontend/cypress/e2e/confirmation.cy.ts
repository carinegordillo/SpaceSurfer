describe('Confirmations Tests', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=","Aud":"spacesurfers","Exp":638502328017962250,"Iat":638502292017962240,"Claims":{"Role":"2"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"ebGqpeyd7Yb1PH7fZVm1frgmzh7JpU9IYSmJ5Odna20"}';
    beforeEach(() => {
        cy.intercept('POST', 'http://localhost:5270/api/auth/sendOTP', {
            statusCode: 200
        }).as('sendOTP');

        cy.intercept('POST', 'http://localhost:5270/api/auth/authenticate', {
            statusCode: 200,
            body: {
                accessToken: token,
                idToken: '{"Username":"7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="}'
            }
        }).as('authenticate');

        cy.intercept('POST', 'http://localhost:5116/api/v1/reservationConfirmation/ListConfirmations?hashedUsername=${username}', {
            statusCode: 200,
            body: {
                success: true,
                message: 'List confirmations successfully'
            }
        }).as('listConfirmations');

        cy.visit('http://localhost:3000/Login/index.html');

        cy.get('#userIdentity').type('testUser');
        cy.get('button').contains('Send Verification').click();
        cy.wait('@sendOTP');
        cy.get('#otp').type('123456');
        cy.get('button').contains('Login').click();
        cy.wait('@authenticate');

        cy.get('[data-testid="confirmation-view"]').click({ force: true });
        cy.get('button').contains('Confirmations').click();

        cy.get('[data-testid="initConfirmation-view"]').click({ force: true });
        //cy.get('button').contains('Initialize Confirmations').click();

    });

    context('Cancel Confirmation', () => {
        it('allows a user to cancel a confirmation', () => {
            const hashedUsername = '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=';
            const reservationID = '15';
            // cy.get('[data-testid="confirmation-view"]').click({ force: true });
            // cy.get('button').contains('Confirmations').click();

            // cy.get('[data-testid="initConfirmation-view"]').click({ force: true });
            // cy.get('button').contains('Initialize Confirmations').click();

            cy.intercept('POST', `http://localhost:5116/api/v1/reservationConfirmation/CancelConfirmation?hashedUsername=${encodeURIComponent(hashedUsername)}&reservationID=${encodeURIComponent(reservationID)}`, {
                statusCode: 200,
                body: { message: 'Confirmation cancelled successfully!' }
            }).as('cancelConfirmation');
            
            cy.get(`button[data-username="${hashedUsername}"][data-reservation-id="${reservationID}"].cancel-btn`).click();
            cy.get('body').should('contain', 'Cancel Confirmation'); // Checks if the text is anywhere in the body

            cy.get('div.modal').should('be.visible');
            cy.get('div.modal').first().within(() => {
                cy.get('button[data-test-id="yesBtn"]').contains('Yes').should('be.visible').click({ force: true });
            });

            cy.wait('@cancelConfirmation').its('request.body').should('deep.equal', {
                hashedUsername: hashedUsername,
                reservationID: reservationID
            });
            cy.get('.modal-content').should('contain', 'Confirmation cancelled successfully!');
        });
    });

    context('Delete Confirmation', () => {
        it('allows a user to delete a confirmation', () => {
            const reservationID = '1191';
            // cy.get('[data-testid="confirmation-view"]').click({ force: true });
            // cy.get('button').contains('Confirmations').click();

            // cy.get('[data-testid="initConfirmation-view"]').click({ force: true });
            // cy.get('button').contains('Initialize Confirmations').click();
            cy.intercept('DELETE', `http://localhost:5116/api/v1/reservationConfirmation/DeleteConfirmation/${reservationID}`, {
                statusCode: 200,
                body: { message: 'Confirmation deleted successfully' }
            }).as('deleteConfirmation');
            cy.get(`button[data-reservation-id="${reservationID}"].delete-btn`).click();
            
            cy.get('div.modal').should('be.visible');
            cy.get('div.modal').first().within(() => {
                cy.get('button[data-test-id="yesBtn"]').contains('Yes').should('be.visible').click({ force: true });
            });

            cy.wait('@deleteConfirmation').then(interception => {
                expect(interception.request.url).to.include(reservationID);
                cy.get('.modal-content').should('contain', 'Confirmation deleted successfully');
            });
            cy.get('.reservations-list').should('not.contain', reservationID);
        });
    });

    
});