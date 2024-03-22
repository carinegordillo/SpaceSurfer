// postAuthorize.test.js
describe('postAuthorize function', () => {
    beforeEach(() => {
      global.fetch = jest.fn(() => Promise.resolve({ ok: true }));
      //global.localStorage = { getItem: jest.fn().mockReturnValue('fakeToken') };
      Object.defineProperty(window, 'localStorage', {
        value: {
            setItem: jest.fn(),
            getItem: jest.fn().mockReturnValue('fakeToken'),
            removeItem: jest.fn(),
            clear: jest.fn()
        },
        writable: true
    });
      global.alert = jest.fn();
      document.getElementById = jest.fn().mockImplementation(id => ({ value: id }));
    });
  
    afterEach(() => {
      jest.clearAllMocks();
    });
  
    it('sends authorization request with access token', async () => {
      const { postAuthorize } = require('../Security/securityServer.js');
  
      await postAuthorize();
  
      expect(global.fetch).toHaveBeenCalledWith(expect.any(String), expect.objectContaining({
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer fakeToken"
        },
        body: expect.any(String),
      }));
    });
  });
  