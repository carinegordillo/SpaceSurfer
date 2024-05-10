// describe('User Profile Tests', () => {
//     let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=","Aud":"spacesurfers","Exp":638502328017962250,"Iat":638502292017962240,"Claims":{"Role":"2"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"ebGqpeyd7Yb1PH7fZVm1frgmzh7JpU9IYSmJ5Odna20"}';
//     beforeEach(() => {
//         cy.intercept('POST', 'http://localhost:5270/api/auth/sendOTP', {
//             statusCode: 200
//         }).as('sendOTP');

//         cy.intercept('POST', 'http://localhost:5270/api/auth/authenticate', {
//             statusCode: 200,
//             body: {
//                 accessToken: token,
//                 idToken: '{"Username":"7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU="}'
//             }
//         }).as('authenticate');

//         cy.intercept('GET','http://localhost:5048/api/profile/getUserProfile?email=${encodeURIComponent(email)}', {
//             statusCode: 200,
//             body: {
//                 success: true,
//                 message: 'Get user profile successfully'
//             }
//         }).as('getUserProfile');
//         // Clear session storage before each test
//         //sessionStorage.clear();
//         cy.visit('http://localhost:3000/Login/index.html');

//         cy.get('#userIdentity').type('testUser');
//         cy.get('button').contains('Send Verification').click();
//         cy.wait('@sendOTP');
//         cy.get('#otp').type('123456');
//         cy.get('button').contains('Login').click();
//         cy.wait('@authenticate');

//         cy.get('[data-testid="profile-view"]').click({ force: true });
//         cy.get('button').contains('Profile').click();

        
//     });

//     it('should display an error if no idToken is found in sessionStorage', () => {
//         cy.window().then((win) => {
//             // Stub the console.error to check if it gets called
//             cy.stub(win.console, 'error').as('consoleError');
//             win.initProfile();
//             cy.get('@consoleError').should('be.calledWith', 'idToken not found in sessionStorage');
//         });
//         cy.get('[data-testid="profile-view"]').click({ force: true });
//         cy.get('button').contains('Task Manager Hub').click();
//     });

//     // it('should handle the idToken without a Username', () => {
//     //     // Set an idToken without a Username
//     //     sessionStorage.setItem('idToken', JSON.stringify({}));
//     //     cy.window().then((win) => {
//     //         cy.stub(win.console, 'error').as('consoleError');
//     //         win.initProfile();
//     //         cy.get('@consoleError').should('be.calledWith', 'Parsed idToken does not have Username');
//     //     });
//     // });

//     // it('should fetch and display the user profile if idToken is valid', () => {
//     //     // Set a valid idToken and mock the fetchUserProfile function
//     //     sessionStorage.setItem('idToken', JSON.stringify({ Username: 'testUser' }));
//     //     cy.window().then((win) => {
//     //         cy.stub(win, 'fetchUserProfile').resolves({ firstName: 'John', lastName: 'Doe' });
//     //         cy.stub(win, 'displayUserProfile').as('displayUserProfile');
//     //         win.initProfile();
//     //         cy.get('@displayUserProfile').should('be.calledWith', { firstName: 'John', lastName: 'Doe' });
//     //     });
//     // });

//     // it('should toggle the edit mode and update the form', () => {
//     //     cy.get('#editProfile').click();
//     //     cy.get('.left-panel').should('contain.html', 'form');
//     //     cy.get('#firstName').should('exist');
//     //     cy.get('#lastName').should('exist');
//     // });

//     // it('should save profile changes and re-fetch the profile', () => {
//     //     // Prepare environment
//     //     sessionStorage.setItem('idToken', JSON.stringify({ Username: 'testUser' }));
//     //     sessionStorage.setItem('accessToken', 'validToken');

//     //     // Simulate profile editing
//     //     cy.visit('/'); // Reload the page to simulate user interaction
//     //     cy.get('#editProfile').click();
//     //     cy.get('#firstName').type('Jane');
//     //     cy.get('#lastName').type('Smith');
//     //     cy.get('#saveChangesButton').click();

//     //     // Mock server response
//     //     cy.intercept('POST', 'http://localhost:5048/api/profile/updateUserProfile', {
//     //         statusCode: 200,
//     //         body: { message: 'Success' }
//     //     });

//     //     cy.get('.left-panel').should('contain', 'Jane Smith'); // Check if the updated name is displayed
//     // });

//     // it('should cancel profile edits and restore the original profile', () => {
//     //     cy.get('#editProfile').click();
//     //     cy.get('#firstName').clear().type('NewName');
//     //     cy.get('#cancelChangesButton').click();

//     //     cy.window().then((win) => {
//     //         cy.stub(win, 'initProfile').as('initProfile');
//     //         win.cancelEditProfile();
//     //         cy.get('@initProfile').should('be.calledOnce'); // Ensure that initProfile is called to restore the original state
//     //     });
//     // });
// });
