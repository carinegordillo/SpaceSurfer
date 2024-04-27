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
    fetch.resetMocks();
    sessionStorage.clear();

    // Mock sessionStorage
    Storage.prototype.setItem = jest.fn();
    Storage.prototype.getItem = jest.fn(() => JSON.stringify({ accessToken: 'fake-token', idToken: 'fake-id-token' }));

    // Mock console log to prevent logging during tests
    console.log = jest.fn();

    // Set up the required HTML structure for the tests
    document.body.innerHTML = `
      <div id="reservation-details"></div>
      <button id="initConfirmationsButton"></button>
    `;

    // Now the button exists, and we can attach the event listener without causing an error
    document.getElementById('initConfirmationsButton').addEventListener('click', () => {
        listConfirmations();
    });
});


describe('listConfirmations', () => {
    it('handles successful confirmation listings', async () => {
        const mockResponse = [{ reservationID: 1, reservationStartTime: new Date(), reservationEndTime: new Date(), username: 'user1' }];
        fetch.mockResponseOnce(JSON.stringify(mockResponse));

        document.body.innerHTML = '<div id="reservation-details"></div>'; // Mocking the DOM element

        await listConfirmations();

        expect(fetch).toHaveBeenCalledWith('http://localhost:5116/api/v1/reservationConfirmation/ListConfirmations?hashedUsername=user1', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer fake-token',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: 'user1' })
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

    it('handles fetch failure', async () => {
        fetch.mockReject(new Error('API failure'));

        await listConfirmations();

        // You can also check for error handling logic, such as displaying an error message to the user
        expect(console.error).toHaveBeenCalledWith('Error listing confirmations:', expect.any(Error));
    });
});