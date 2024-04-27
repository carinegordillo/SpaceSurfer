const {
    sendConfirmation,
    deleteConfirmation,
    cancelConfirmation,
    listConfirmations
  } = require('../Login/confirmedReservations.js');
  
describe('sendConfirmation', () => {
    beforeEach(() => {
        // Setup global.fetch mock
        global.fetch = jest.fn(() => Promise.resolve({
          ok: true,
          json: () => Promise.resolve({ accessToken: "fakeToken" })
        }));

        // Mocking localStorage and its methods
      Object.defineProperty(window, 'localStorage', {
        value: {
            setItem: jest.fn(),
            getItem: jest.fn(),
            removeItem: jest.fn(),
            clear: jest.fn()
        },
        writable: true
    });

    //mocking alert and document.getElementId
    global.alert = jest.fn();
    document.getElementById = jest.fn(id => ({ value: id === "authEmail" ? "user@example.com" : "123456" }));
    });

    // clear mocks
    afterEach(() => {
        jest.clearAllMocks();
      });

  it('sends a confirmation and handles a successful response', async () => {
    fetch.mockResponseOnce(JSON.stringify({ newToken: "newAccessToken123", message: "Confirmation sent" }), { status: 200 });
    
    const data = await sendConfirmation(1);
    
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining('/SendConfirmation?reservationID=3'), // Assuming this is how you form your URL
      expect.objectContaining({
        method: 'POST',
        headers: expect.objectContaining({
          'Authorization': expect.stringContaining('Bearer '),
          'Content-Type': 'application/json'
        }),
        body: JSON.stringify({ ReservationID: 3 })
      })
    );
    expect(data).toEqual(expect.objectContaining({ newToken: "newAccessToken123", message: "Confirmation sent" }));
  });

  it('handles non-200 responses gracefully', async () => {
    fetch.mockResponseOnce('Not found', { status: 404 });

    await expect(sendConfirmation(3)).rejects.toThrow('HTTP error');
  });
});

