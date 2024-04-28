// listConfirmations.test.js
const fetchMock = require('jest-fetch-mock');
fetchMock.enableMocks();

// Since we're not in a browser environment, we'll need to mock sessionStorage.
const mockSessionStorage = (function() {
  let store = {};
  return {
    getItem: function(key) {
      return store[key] || null;
    },
    setItem: function(key, value) {
      store[key] = value.toString();
    },
    removeItem: function(key) {
      delete store[key];
    },
    clear: function() {
      store = {};
    },
  };
})();

// Mock the global sessionStorage before importing the script
Object.defineProperty(window, 'sessionStorage', { value: mockSessionStorage });

const {
    sendConfirmation,
    deleteConfirmation,
    cancelConfirmation,
    listConfirmations
} = require('../Login/confirmedReservations.js');

beforeEach(() => {
    document.body.innerHTML = `
        <div id="homepageGen"></div>
        <div id="homepageManager"></div>
        <div id="sendOTPSection"></div>
        <div id="reservation-details"></div>
        <button id="initConfirmationsButton"></button>
    `;
    
     // Clear all instances and calls to constructor and all methods:
     jest.resetModules();
     global.sessionStorage = (function() {
         let store = {};
         return {
             getItem: jest.fn((key) => store[key] || null),
             setItem: jest.fn((key, value) => {
                 store[key] = value.toString();
             }),
             removeItem: jest.fn((key) => {
                 delete store[key];
             }),
             clear: jest.fn(() => {
                 store = {};
             })
         };
     })();
 
     // Pre-populate sessionStorage with required data
     sessionStorage.setItem('idToken', JSON.stringify({ Username: 'fakeUser' }));

    // Mock console log to prevent logging during tests
    console.log = jest.fn();

    // Setup initial DOM elements if needed
    document.body.innerHTML = `
        <div id="reservation-details"></div>
        <button id="initConfirmationsButton"></button>
    `;
});

describe('Simple sessionStorage Test', () => {
    it('should retrieve mocked idToken correctly', () => {
        expect(sessionStorage.getItem('idToken')).toEqual(JSON.stringify({ Username: 'fakeUser' }));
        expect(sessionStorage.getItem).toHaveBeenCalledWith('idToken');
    });
});


describe('listConfirmations', () => {
    it('handles successful confirmation listings', async () => {
        const mockResponse = [{ reservationID: 1, reservationStartTime: new Date(), reservationEndTime: new Date(), username: '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=' }];
        fetch.mockResponseOnce(JSON.stringify(mockResponse));

        document.body.innerHTML = '<div id="reservation-details"></div>'; // Mocking the DOM element

        await listConfirmations();

        expect(fetch).toHaveBeenCalledWith(`http://localhost:5116/api/v1/reservationConfirmation/ListConfirmations?hashedUsername=${mockResponse.username}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer fake-token',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: '7mLYo1Gu98LGqqtvSQcZ31hJhDEit2iDK4BCD3DM8ZU=' })
        });

        const detailsContainer = document.getElementById('reservation-details');
        expect(detailsContainer.innerHTML).toContain('Reservation ID: 1'); // Test if the DOM was updated correctly
        expect(sessionStorage.setItem).toHaveBeenCalledWith('accessToken', 'new-token'); // Assuming new token logic
    });

    it('handles no confirmations case', async () => {
        fetch.mockResponseOnce(JSON.stringify([])); // Empty array for no confirmations

        document.body.innerHTML = '<div id="reservation-details"></div>';

        await listConfirmations();

        const detailsContainer = document.getElementById('reservation-details');
        expect(detailsContainer.innerHTML).toContain('You currently do not have confirmed reservations');
    });

    // it('handles fetch failure', async () => {
    //     fetch.mockReject(new Error('API failure'));

    //     await listConfirmations();

    //     // You can also check for error handling logic, such as displaying an error message to the user
    //     expect(console.error).toHaveBeenCalledWith('Error listing confirmations:', expect.any(Error));
    // });
});