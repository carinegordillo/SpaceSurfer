describe('Task Management Tests', () => {
    // let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":2}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}';
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"kj3VOKOk9Dh0pY5Fh41Dr7knV3/qR9FI6I7FmZlRVtc=","Aud":"spacesurfers","Exp":638494325499418601,"Iat":638494289499417369,"Claims":{"Role":"2"},"Nbf":null,"Scope":"","Permissions":[]},"Signature":"mzAJJzqPELCJGZ_a7oyacIsozDSpsao-21jrG-bmDpk"}';
    beforeEach(() => {
        cy.visit('http://localhost:3000/');
        window.localStorage.setItem('token-local', token);
        cy.wait(500);
        cy.get('[data-testid=task-view]').click(); // Assuming there's a data-testid for the task view button
        cy.wait(1000);
    });

    it('Create a new task', () => {
        // Act
        cy.get('#taskTitle').type('New Task Title');
        cy.get('#taskDescription').type('This is a new task.');
        cy.get('#taskDueDate').type('2024-03-25');
        cy.get('#taskPriority').select('High');
        cy.get('#taskNotificationSetting').type('30');
        cy.get('#submit-task-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#taskList').should('contain', 'New Task Title');
        cy.get('#taskList').should('contain', 'This is a new task.');
    });

    it('Modify an existing task', () => {
        // Prepare a task to modify
        cy.get('#taskTitle').type('Task to Edit');
        cy.get('#submit-task-button').click();
        cy.wait(1000);

        // Modify the task
        cy.get(`[data-test-id='edit-${encodeURIComponent('Task to Edit')}']`).click(); // Assuming buttons for editing have unique identifiers
        cy.get('#taskTitle').clear().type('Edited Task Title');
        cy.get('#submit-task-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#taskList').should('contain', 'Edited Task Title');
        cy.get('#taskList').should('not.contain', 'Task to Edit');
    });

    it('Delete a task', () => {
        // Prepare a task to delete
        cy.get('#taskTitle').type('Task to Delete');
        cy.get('#submit-task-button').click();
        cy.wait(1000);

        // Delete the task
        cy.get(`[data-test-id='delete-${encodeURIComponent('Task to Delete')}']`).click(); // Assuming buttons for deleting have unique identifiers

        // Assert
        cy.wait(1000);
        cy.get('#taskList').should('not.contain', 'Task to Delete');
    });

    it('View tasks by date filter', () => {
        // Prepare tasks with different dates
        cy.get('#taskDueDate').type('2024-03-25');
        cy.get('#taskTitle').type('Task on March 25');
        cy.get('#submit-task-button').click();
        cy.wait(1000);
        cy.get('#taskDueDate').type('2024-03-26');
        cy.get('#taskTitle').clear().type('Task on March 26');
        cy.get('#submit-task-button').click();
        cy.wait(1000);

        // Filter tasks by date
        cy.get('#filter-date-input').type('2024-03-25');
        cy.get('#filter-date-button').click();

        // Assert
        cy.wait(1000);
        cy.get('#taskList').should('contain', 'Task on March 25');
        cy.get('#taskList').should('not.contain', 'Task on March 26');
    });
});
