// postAuthenticate.test.js
describe('postAuthenticate function', () => {
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
  
    it('stores accessToken on successful authentication', async () => {
      const { postAuthenticate } = require('../Security/securityServer.js');
  
      // Mock preventDefault to prevent actual form submission
      const event = { preventDefault: jest.fn() };
  
      await postAuthenticate(event);
        
      //check fetch and alert
      expect(global.fetch).toHaveBeenCalled();
      expect(localStorage.setItem).toHaveBeenCalledWith('accessToken', 'fakeToken');
      expect(global.alert).toHaveBeenCalledWith("Authentication successful.");
    });
  });
  