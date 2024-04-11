//using SS.Backend.DataAccess;
//using SS.Backend.SharedNamespace;
//using SS.Backend.SpaceManager;
//using System.Diagnostics;
//using Microsoft.Data.SqlClient;


//namespace SS.Backend.Tests.SpaceManagerReadCompanies;

//[TestClass]
//public class ReadCompanyData
//{

//    private SpaceCreation? _spaceCreation;
//    private ISpaceManagerDao? _spaceManagerDao; 
//    private SqlDAO? _sqlDao;
//    private ConfigService? _configService;
//    private SpaceReader? _spaceReader;


//    [TestInitialize]
//    public void Setup()
//    {
//        var baseDirectory = AppContext.BaseDirectory;
//        var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//        var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//        _configService = new ConfigService(configFilePath);
//        _sqlDao = new SqlDAO(_configService);
//        _spaceManagerDao = new SpaceManagerDao(_sqlDao);
//        _spaceCreation = new SpaceCreation(_spaceManagerDao);
//        _spaceReader = new SpaceReader(_spaceManagerDao);

//    }
//    [TestMethod]
//    public async Task GetCompanyInfoAsync_ReturnsCorrectData()
//    {


//        var companyInfos = await _spaceReader.GetCompanyInfoAsync();

//        Console.WriteLine("CompanyInfos: " + companyInfos.Count());
//        Console.WriteLine("CompanyInfos: " + companyInfos);

//        Assert.IsNotNull(companyInfos);
//        Assert.IsTrue(companyInfos.Any()); 

//    }

//    [TestMethod]
//    public async Task GetCompanyInfoAsyncList_ReturnsCorrectData()
//    {

//        var companyInfos = await _spaceReader.GetCompanyInfoAsync();


//        foreach (var info in companyInfos)
//        {
//            Console.WriteLine($"CompanyID: {info.CompanyID}, Name: {info.CompanyName}");
//        }

//        Assert.IsNotNull(companyInfos, "The result of GetCompanyInfoAsync is null.");
//        Assert.IsTrue(companyInfos.Any(), "No company information was returned.");


//    }

//    [TestMethod]
//    public async Task GetCompanyFloorsAsync_ReturnsFloorsForSpecificCompany()
//    {
//        int companyId = 1; 
//        var floors = await _spaceReader.GetCompanyFloorsAsync(companyId);

//        foreach (var floor in floors)
//        {
//            Console.WriteLine($"FloorPlanID: {floor.FloorPlanID}");
//            Console.WriteLine($"FloorPlanName: {floor.FloorPlanName}");
//            Console.WriteLine($"FloorPlanImageBase64: {floor.FloorPlanImageBase64?.Substring(0, Math.Min(floor.FloorPlanImageBase64.Length, 100))}..."); 

//            Console.WriteLine("FloorSpaces:");
//            foreach (var space in floor.FloorSpaces)
//            {
//                Console.WriteLine($"  SpaceID: {space.Key}, TimeLimit: {space.Value}");
//            }
//            Console.WriteLine("----------");
//        }
//        Assert.IsNotNull(floors);
//        Assert.IsTrue(floors.Any()); 

//    }
//}

