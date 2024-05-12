describe('System Observability Tests', () => {
  let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"qGkjtLi\u002BR/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=","Aud":"spacesurfers","Exp":638498904024001560,"Iat":638498868024001559,"Claims":{"Role":"1"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"wB4V2rkQXiwFTvymzTboPImSnfAQ0vpoJZggFu95n60"}';

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
            window.sessionStorage.setItem('idToken', '{"Username": "qGkjtLi\u002BR/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY="}');
        });
    });

      cy.intercept('GET', 'http://localhost:5295/api/v1/SystemObservability/Information?timeSpan=6%20months', {
        statusCode: 200,
        body: {
          "loginsCount": [
            { "month": 2, "year": 2024, "failedLogins": 43, "successfulLogins": 257 },
            { "month": 3, "year": 2024, "failedLogins": 30, "successfulLogins": 180 },
            { "month": 4, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 5, "year": 2024, "failedLogins": 41, "successfulLogins": 255 }
          ],
          "registrationCount": [
            { "month": 2, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 45 },
            { "month": 3, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 4, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 5, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 }
          ],
          "viewsDurationCount": [
            { "viewName": "Home View", "durationInSeconds": 74 },
            { "viewName": "Waitlist View", "durationInSeconds": 37 },
            { "viewName": "System Observability View", "durationInSeconds": 23 }
          ],
          "usedFeatureCount": [
            { "featureName": "System Observability", "usageCount": 7 },
            { "featureName": "Feature 2", "usageCount": 4 },
            { "featureName": "Usage Analysis", "usageCount": 4 }
          ],
          "topCompanyReservationCount": [
            { "companyName": "99CentStore", "reservationCount": 11 },
            { "companyName": "SomeCompany", "reservationCount": 7 },
            { "companyName": "DaveAndBusters", "reservationCount": 11 }
          ],
          "topCompanySpaceCount": [
            { "companyName": "DaveAndBusters", "spaceCount": 6 },
            { "companyName": "SomeCompany", "spaceCount": 1 },
            { "companyName": "99CentStore", "spaceCount": 11 }
          ]
        }
      }).as('getAnalysis6months');

      cy.intercept('GET', 'http://localhost:5295/api/v1/SystemObservability/Information?timeSpan=12%20months', {
        statusCode: 200,
        body: {
          "loginsCount": [
            { "month": 2, "year": 2024, "failedLogins": 43, "successfulLogins": 257 },
            { "month": 3, "year": 2024, "failedLogins": 30, "successfulLogins": 180 },
            { "month": 4, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 5, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 6, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 7, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 8, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 9, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 10, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 11, "year": 2024, "failedLogins": 41, "successfulLogins": 255 }
          ],
          "registrationCount": [
            { "month": 2, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 45 },
            { "month": 3, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 4, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 5, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 },
            { "month": 6, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 7, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 8, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 },
            { "month": 9, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 10, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 11, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 }
          ],
          "viewsDurationCount": [
            { "viewName": "Personal Overview View", "durationInSeconds": 325 },
            { "viewName": "Space Manager View", "durationInSeconds": 90 },
            { "viewName": "System Observability View", "durationInSeconds": 55 }
          ],
          "usedFeatureCount": [
            { "featureName": "Waitlist", "usageCount": 195 },
            { "featureName": "System Observability", "usageCount": 62 },
            { "featureName": "Space Manager", "usageCount": 41 }
          ],
          "topCompanyReservationCount": [
            { "companyName": "99CentStore", "reservationCount": 525 },
            { "companyName": "SomeCompany", "reservationCount": 310 },
            { "companyName": "DaveAndBusters", "reservationCount": 115 }
          ],
          "topCompanySpaceCount": [
            { "companyName": "99CentStore", "spaceCount": 55 },
            { "companyName": "DaveAndBusters", "spaceCount": 53 },
            { "companyName": "SomeCompany", "spaceCount": 25 }
          ]
        }
      }).as('getAnalysis12months');

      cy.intercept('GET', 'http://localhost:5295/api/v1/SystemObservability/Information?timeSpan=24%20months', {
        statusCode: 200,
        body: {
          "loginsCount": [
            { "month": 2, "year": 2024, "failedLogins": 43, "successfulLogins": 257 },
            { "month": 3, "year": 2024, "failedLogins": 30, "successfulLogins": 180 },
            { "month": 4, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 5, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 6, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 7, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 8, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 9, "year": 2024, "failedLogins": 41, "successfulLogins": 255 },
            { "month": 10, "year": 2024, "failedLogins": 50, "successfulLogins": 320 },
            { "month": 11, "year": 2024, "failedLogins": 41, "successfulLogins": 255 }
          ],
          "registrationCount": [
            { "month": 2, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 45 },
            { "month": 3, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 4, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 5, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 },
            { "month": 6, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 7, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 8, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 },
            { "month": 9, "year": 2024, "failedRegistrations": 8, "successfulRegistrations": 90 },
            { "month": 10, "year": 2024, "failedRegistrations": 15, "successfulRegistrations": 70 },
            { "month": 11, "year": 2024, "failedRegistrations": 5, "successfulRegistrations": 18 }
          ],
          "viewsDurationCount": [
            { "viewName": "Personal Overview View", "durationInSeconds": 325 },
            { "viewName": "Space Manager View", "durationInSeconds": 90 },
            { "viewName": "System Observability View", "durationInSeconds": 55 }
          ],
          "usedFeatureCount": [
            { "featureName": "Waitlist", "usageCount": 195 },
            { "featureName": "System Observability", "usageCount": 62 },
            { "featureName": "Space Manager", "usageCount": 41 }
          ],
          "topCompanyReservationCount": [
            { "companyName": "99CentStore", "reservationCount": 525 },
            { "companyName": "SomeCompany", "reservationCount": 310 },
            { "companyName": "DaveAndBusters", "reservationCount": 115 }
          ],
          "topCompanySpaceCount": [
            { "companyName": "99CentStore", "spaceCount": 55 },
            { "companyName": "DaveAndBusters", "spaceCount": 53 },
            { "companyName": "SomeCompany", "spaceCount": 25 }
          ]
        }
      }).as('getAnalysis12months');


      
      
      cy.visit('http://localhost:3000/Login/index.html');
      cy.get('#userIdentity').type('brandongalich@gmail.com');
      cy.get('button').contains('Send Verification').click();
      cy.wait('@sendOTP',);
      cy.get('#otp').type('password', { force: true });
      cy.contains('Login').click({ force: true });
      cy.wait('@authenticate');
  });

  it('System Observability View 6 months', () => {
    cy.get('[data-testid="system-observability-view"]').eq(0).click();
  
    // Act
    cy.get('#timeSpan').select('6 months').should('have.value', '6 months')
  
    // Assert
    cy.wait('@getAnalysis6months').then(interception => {
      if (interception && interception.response && interception.response.body) {
        const analysis = interception.response.body;
        expect(analysis).to.be.an('array').and.not.to.be.empty;
        
        // Additional assertions
        cy.get('#loginsTrendChart').should('exist');
        cy.get('#registrationsTrendChart').should('exist');
        cy.get('#viewsDurationList').should('have.descendants');
        cy.get('#usedFeaturesList').should('have.descendants');
        cy.get('#companyReservationsList').should('have.descendants');
        cy.get('#companySpacesList').should('have.descendants');
      } else {
        // If interception or its response is undefined, fail the test
        throw new Error('Interception or response body is undefined');
      }
    });
  });

    it('System Observability View 12 months', () => {
      cy.get('[data-testid="system-observability-view"]').eq(0).click();
    
      // Act
      cy.get('#timeSpan').select('24 months').should('have.value', '6 months')
    
      // Assert
      cy.wait('@getAnalysis12months').then(interception => {
        if (interception && interception.response && interception.response.body) {
          const analysis = interception.response.body;
          expect(analysis).to.be.an('array').and.not.to.be.empty;
          
          // Additional assertions
          cy.get('#loginsTrendChart').should('exist');
          cy.get('#registrationsTrendChart').should('exist');
          cy.get('#viewsDurationList').should('have.descendants');
          cy.get('#usedFeaturesList').should('have.descendants');
          cy.get('#companyReservationsList').should('have.descendants');
          cy.get('#companySpacesList').should('have.descendants');
        } else {
          // If interception or its response is undefined, fail the test
          throw new Error('Interception or response body is undefined');
        }
      });
    });

      it('System Observability View 24 months', () => {
        cy.get('[data-testid="system-observability-view"]').eq(0).click();
      
        // Act
        cy.get('#timeSpan').select('24 months').should('have.value', '6 months')
      
        // Assert
        cy.wait('@getAnalysis24months').then(interception => {
          if (interception && interception.response && interception.response.body) {
            const analysis = interception.response.body;
            expect(analysis).to.be.an('array').and.not.to.be.empty;
            
            // Additional assertions
            cy.get('#loginsTrendChart').should('exist');
            cy.get('#registrationsTrendChart').should('exist');
            cy.get('#viewsDurationList').should('have.descendants');
            cy.get('#usedFeaturesList').should('have.descendants');
            cy.get('#companyReservationsList').should('have.descendants');
            cy.get('#companySpacesList').should('have.descendants');
          } else {
            // If interception or its response is undefined, fail the test
            throw new Error('Interception or response body is undefined');
          }
        });

  });

});