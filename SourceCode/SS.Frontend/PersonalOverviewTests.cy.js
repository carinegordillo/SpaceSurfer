describe('Personal Overview Tests', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"qGkjtLi\u002BR/fQXcdKnAYWDYjkEVPvJ3E8SPCYTrU0hvY=","Aud":"spacesurfers","Exp":638498904024001560,"Iat":638498868024001559,"Claims":{"Role":"3"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"wB4V2rkQXiwFTvymzTboPImSnfAQ0vpoJZggFu95n60"}';

    beforeEach(() => {
        cy.visit('http://localhost:3000/');
        window.localStorage.setItem('token-local', token);
        cy.wait(500);
        cy.get('[data-testid=personal-overview-view]').click();
        cy.wait(1000);
    });

    it('Calendar View', () => {
        // Act
        cy.get('#calendar-button').click();
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar-poc').should('be.visible');
        cy.get('#reservation-list-poc').should('not.be.visible');
    });

    it('ListView', () => {
        // Act
        cy.get('#list-button').click();
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar-poc').should('not.be.visible');
        cy.get('#reservation-list-poc').should('be.visible');
    });

    it('ListView with From-Date filter', () => {
        
        // Clear fields
        cy.get('#from-date').clear();
        cy.get('#to-date').clear();
        cy.wait(1000);

        // Act
        cy.get('#list-button').click();
        cy.get('#from-date').type('2024-03-28');
        cy.get('#to-date').clear();
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar-poc').should('not.be.visible');
        cy.get('#reservation-list-poc').should('be.visible');

        // Check that specific reservation details are not contained in any reservation item
        cy.get('.reservation-item').each(($item) => {
            cy.wrap($item).should('not.contain.text', 'ReservationID: 24');
            cy.wrap($item).should('not.contain.text', 'CompanyName: 99CentStore');
            cy.wrap($item).should('not.contain.text', 'CompanyID: 4');
            cy.wrap($item).should('not.contain.text', 'Address: 123 Bogus St. Irvine CA, 91323');
            cy.wrap($item).should('not.contain.text', 'Floor Plan - Space ID: 2 - F2');
            cy.wrap($item).should('not.contain.text', 'Reservation Date: 2024-03-15');
            cy.wrap($item).should('not.contain.text', 'Reservation Time: 12:00 PM - 1:00 PM');
            cy.wrap($item).should('not.contain.text', 'Cancelled');
        });
    });

    it('ListView with To-Date filter', () => {

        // Clear fields
        cy.get('#from-date').clear();
        cy.get('#to-date').clear();
        cy.wait(1000);

        // Act
        cy.get('#list-button').click();
        cy.get('#from-date').clear();
        cy.get('#to-date').type('2024-04-10');
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar-poc').should('not.be.visible');
        cy.get('#reservation-list-poc').should('be.visible');

        // Check that specific reservation details are not contained in any reservation item
        cy.get('.reservation-item').each(($item) => {
            cy.wrap($item).should('not.contain.text', 'ReservationID: 23');
            cy.wrap($item).should('not.contain.text', 'CompanyName: SomeCompany');
            cy.wrap($item).should('not.contain.text', 'CompanyID: 3');
            cy.wrap($item).should('not.contain.text', 'Address: 456 Help Ave. Irvine CA, 90000');
            cy.wrap($item).should('not.contain.text', 'Floor Plan - Space ID: 1 - CE5');
            cy.wrap($item).should('not.contain.text', 'Reservation Date: 2024-04-18');
            cy.wrap($item).should('not.contain.text', 'Reservation Time: 12:00 PM - 1:00 PM');
            cy.wrap($item).should('not.contain.text', 'Active');
        });
    });

    it('ListView with From-Date and To-Date filter', () => {

        // Clear fields
        cy.get('#from-date').clear();
        cy.get('#to-date').clear();
        cy.wait(1000);

        //Act
        cy.get('#list-button').click();
        cy.get('#from-date').type('2024-03-28');
        cy.get('#to-date').type('2024-04-10');
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar-poc').should('not.be.visible');
        cy.get('#reservation-list-poc').should('be.visible');

        // Check that specific reservation details are not contained in any reservation item
        cy.get('.reservation-item').each(($item) => {
            cy.wrap($item).should('not.contain.text', 'ReservationID: 24');
            cy.wrap($item).should('not.contain.text', 'CompanyName: 99CentStore');
            cy.wrap($item).should('not.contain.text', 'CompanyID: 4');
            cy.wrap($item).should('not.contain.text', 'Address: 123 Bogus St. Irvine CA, 91323');
            cy.wrap($item).should('not.contain.text', 'Floor Plan - Space ID: 2 - F2');
            cy.wrap($item).should('not.contain.text', 'Reservation Date: 2024-03-15');
            cy.wrap($item).should('not.contain.text', 'Reservation Time: 12:00 PM - 1:00 PM');
            cy.wrap($item).should('not.contain.text', 'Cancelled');

            cy.wrap($item).should('not.contain.text', 'ReservationID: 23');
            cy.wrap($item).should('not.contain.text', 'CompanyName: SomeCompany');
            cy.wrap($item).should('not.contain.text', 'CompanyID: 3');
            cy.wrap($item).should('not.contain.text', 'Address: 456 Help Ave. Irvine CA, 90000');
            cy.wrap($item).should('not.contain.text', 'Floor Plan - Space ID: 1 - CE5');
            cy.wrap($item).should('not.contain.text', 'Reservation Date: 2024-04-18');
            cy.wrap($item).should('not.contain.text', 'Reservation Time: 12:00 PM - 1:00 PM');
            cy.wrap($item).should('not.contain.text', 'Active');
        });
    });


});