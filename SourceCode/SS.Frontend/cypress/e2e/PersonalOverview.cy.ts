describe('Personal Overview Tests', () => {
  let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"qGkjtLi\u002BR/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=","Aud":"spacesurfers","Exp":638498904024001560,"Iat":638498868024001559,"Claims":{"Role":"3"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"wB4V2rkQXiwFTvymzTboPImSnfAQ0vpoJZggFu95n60"}';

  beforeEach(() => {

    cy.intercept('POST', 'http://localhost:5270/api/auth/sendOTP', {
        statusCode: 200
      }).as('sendOTP');
    
      cy.intercept('POST', 'http://localhost:5270/api/auth/authenticate', {
        statusCode: 200,
        body: {
          accessToken: token,
          idToken: '{"Username":"qGkjtLi\u002BR/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY="}'
        }
      }).as('authenticate');

      cy.intercept('GET', 'http://localhost:5275/api/v1/PersonalOverview/Reservations?fromDate=&toDate=', {
        statusCode: 200,
        body: [
          {
            "reservationID": 24,
            "companyName": "99CentStore",
            "companyID": 4,
            "address": "123 Bogus St. Irvine CA, 91323",
            "floorPlanID": 2,
            "spaceID": "F2",
            "reservationDate": "2024-03-15",
            "reservationStartTime": "12:00:00",
            "reservationEndTime": "13:00:00",
            "status": "Cancelled"
          },
          {
            "reservationID": 65,
            "companyName": "99CentStore",
            "companyID": 4,
            "address": "123 Bogus St. Irvine CA, 91323",
            "floorPlanID": 2,
            "spaceID": "F3",
            "reservationDate": "2024-03-28",
            "reservationStartTime": "12:00:00",
            "reservationEndTime": "15:00:00",
            "status": "Passed"
          },
          {
            "reservationID": 66,
            "companyName": "99CentStore",
            "companyID": 4,
            "address": "123 Bogus St. Irvine CA, 91323",
            "floorPlanID": 3,
            "spaceID": "G1",
            "reservationDate": "2024-04-06",
            "reservationStartTime": "14:30:00",
            "reservationEndTime": "16:00:00",
            "status": "Passed"
          },
          {
            "reservationID": 22,
            "companyName": "99CentStore",
            "companyID": 4,
            "address": "123 Bogus St. Irvine CA, 91323",
            "floorPlanID": 3,
            "spaceID": "G1",
            "reservationDate": "2024-04-06",
            "reservationStartTime": "14:30:00",
            "reservationEndTime": "16:00:00",
            "status": "Passed"
          },
          {
            "reservationID": 23,
            "companyName": "SomeCompany",
            "companyID": 3,
            "address": "456 Help Ave. Irvine CA, 90000",
            "floorPlanID": 1,
            "spaceID": "CE5",
            "reservationDate": "2024-04-18",
            "reservationStartTime": "12:00:00",
            "reservationEndTime": "13:00:00",
            "status": "Active"
          }
        ]
      }).as('getReservations');

      cy.intercept('POST', 'http://localhost:5275/api/v1/PersonalOverview/ReservationDeletion?ReservationID=22', {
        statusCode: 200,
        body: {
            success: true,
            message: 'Reservation deleted successfully'
        }
    }).as('deleteReservation');
      
      cy.visit('http://localhost:3000/Login/index.html');
      cy.get('#userIdentity').type('brandongalich@gmail.com');
      cy.get('button').contains('Send Verification').click();
      cy.wait('@sendOTP');
      cy.get('#otp').type('password');
      cy.get('button').contains('Login').click();
      cy.wait('@authenticate');
  });

  it('Calendar View', () => {
    cy.get('[data-testid="personal-overview-view"]').eq(0).click();
  
    // Act
    cy.get('#calendar-button').click();
    cy.get('#confirm-button').click();
  
    // Assert
    cy.wait('@getReservations').then(interception => {
      const reservations = interception.response.body;
      expect(reservations).to.be.an('array').and.not.to.be.empty;
    });
  
    cy.get('#calendar-poc').should('be.visible');
    cy.get('#reservation-list-poc').should('not.be.visible');
  });

  it('List View', () => {
      cy.get('[data-testid="personal-overview-view"]').eq(0).click();
      // Act
      cy.get('#list-button').click();
      cy.get('#confirm-button').click();

      // Assert
      cy.wait('@getReservations').then(interception => {
        const reservations = interception.response.body;
        expect(reservations).to.be.an('array').and.not.to.be.empty;
      });

      cy.get('#calendar-poc').should('not.be.visible');
      cy.get('#reservation-list-poc').should('be.visible');
  });

  it('Delete Reservation', () => {
    cy.get('[data-testid="personal-overview-view"]').eq(0).click();
    // Act
    cy.get('#list-button').click();
    cy.get('#confirm-button').click();

    cy.get('#reservation-id').type('22');
    cy.get('#reservation-delete-button').click();
    cy.wait('@deleteReservation')

    cy.get('#calendar-poc').should('not.be.visible');
    cy.get('#reservation-list-poc').should('be.visible');

});


});