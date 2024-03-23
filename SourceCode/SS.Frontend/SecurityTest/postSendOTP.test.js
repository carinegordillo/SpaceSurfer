// postSendOTP.test.js
describe('postSendOTP function', () => {
    beforeEach(() => {
        // Setup global.fetch mock
      global.fetch = jest.fn(() => Promise.resolve({
        ok: true,
        json: () => Promise.resolve({ message: "OTP sent successfully." })
      }));

      //mocking alert
      global.alert = jest.fn();
    });
  
    // clear mocks
    afterEach(() => {
      jest.clearAllMocks();
    });
  
    it('sends OTP successfully', async () => {
      const { postSendOTP } = require('../Security/securityServer.js');
  
      document.getElementById = jest.fn().mockReturnValue({ value: 'test@example.com' });
  
      await postSendOTP();
        
      // check fetch
      expect(global.fetch).toHaveBeenCalledWith(expect.any(String), expect.objectContaining({
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email: 'test@example.com' }),
      }));
      // check alert
      expect(global.alert).toHaveBeenCalledWith("OTP sent successfully.");
    });
  });
  