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
        cy.get('#calendar').should('be.visible');
        cy.get('#reservation-list').should('not.be.visible');
    });

    it('ListView', () => {
        // Act
        cy.get('#list-button').click();
        cy.get('#confirm-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#calendar').should('not.be.visible');
        cy.get('#reservation-list').should('be.visible');
    });


});