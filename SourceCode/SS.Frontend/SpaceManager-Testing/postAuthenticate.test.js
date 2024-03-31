describe('Space Manager Functionality', () => {
  beforeEach(() => {
      // Mock fetch globally
      global.fetch = jest.fn(() =>
          Promise.resolve({
              json: () => Promise.resolve({ message: "Operation successful!" }),
          })
      );

      // Mock localStorage
      Object.defineProperty(window, 'localStorage', {
          value: {
              getItem: jest.fn(() => 'mockToken'),
              setItem: jest.fn(),
              removeItem: jest.fn(),
              clear: jest.fn(),
          },
          writable: true,
      });

      // Mock specific document.getElementById calls used in your application
      const mockForm = { addEventListener: jest.fn(), submit: jest.fn() };
      document.getElementById = jest.fn((id) => {
          if (id === 'accountCreationForm' || id === 'modifySpaceForm') return mockForm;
          if (id.endsWith('Base64') || id === 'floorPlanName' || id === 'modifyFloorPlanName') return { value: 'mockValue' };
          if (id === 'spacesContainer' || id === 'spacesModificationContainer') return { getElementsByClassName: () => [] };
          return null;
      });

      // Load your script after mocking
      require('../SpaceManager/pages/SpaceManager.js');
  });

  afterEach(() => {
      jest.clearAllMocks();
  });

  it('should alert success on createSpace API call', async () => {
      // Simulate form submission for create space
      const form = document.getElementById('accountCreationForm');
      form.addEventListener.mock.calls[0][1]({ preventDefault: jest.fn() }); // Simulate submit

      expect(global.fetch).toHaveBeenCalledWith(expect.any(String), expect.objectContaining({
          method: 'POST',
          headers: expect.objectContaining({
              'Authorization': 'Bearer mockToken',
          }),
          body: expect.any(String),
      }));
  });

  it('should alert success on modifySpace API call', async () => {
      // Simulate form submission for modify space
      const form = document.getElementById('modifySpaceForm');
      form.addEventListener.mock.calls[0][1]({ preventDefault: jest.fn() }); // Simulate submit

      expect(global.fetch).toHaveBeenCalledWith(expect.any(String), expect.objectContaining({
          method: 'POST',
          headers: expect.objectContaining({
              'Authorization': 'Bearer mockToken',
          }),
          body: expect.any(String),
      }));
  });
});
