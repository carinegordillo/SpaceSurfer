// USED TO INSERT INTO THE TABLE 
// using SS.Backend.DataAccess;
// using SS.Backend.SharedNamespace;
// using SS.Backend.SpaceManager;
// using System.Diagnostics;
// using Microsoft.Data.SqlClient;
 

// namespace SS.Backend.Tests.SpaceManagerReadCompanies;

// [TestClass]
// public class CreateFloorPlansFloor
// {

//     private SpaceCreation? _spaceCreation;
//     private ISpaceManagerDao? _spaceManagerDao; 
//     private SqlDAO? _sqlDao;
//     private ConfigService? _configService;
//     private SpaceReader? _spaceReader;


//     [TestInitialize]
//     public void Setup()
//     {
//         var baseDirectory = AppContext.BaseDirectory;
//         var projectRootDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../../"));
//         var configFilePath = Path.Combine(projectRootDirectory, "Configs", "config.local.txt");
//         _configService = new ConfigService(configFilePath);
//         _sqlDao = new SqlDAO(_configService);
//         _spaceManagerDao = new SpaceManagerDao(_sqlDao);
//         _spaceCreation = new SpaceCreation(_spaceManagerDao);
//         _spaceReader = new SpaceReader(_spaceManagerDao);
        
//     }

//     [TestMethod]
//     public async Task InsertFloorPlan1_Success()
//     {
        
//         int CompanyID = 1027;
//         string FloorPlanName = "Floor Plan Test 11";
//         string ImagePath = @"/Users/carinegordillo/CECS491/SpaceSurfer/SourceCode/SS.Backend/FloorPlanImages/FloorPlan1.png";
        

//         // Act
//         var response = await _spaceReader.InsertIntoCompanyFloorPlansAsync(CompanyID,FloorPlanName, ImagePath);

//         Console.WriteLine(response.ErrorMessage);

//         // Assert
//         Assert.IsFalse(response.HasError);
//     }
//     [TestMethod]
//     public async Task InsertFloorPlan2_Success()
//     {
        
//         int CompanyID = 1028;
//         string FloorPlanName = "Floor1";
//         string ImagePath = @"/Users/carinegordillo/CECS491/SpaceSurfer/SourceCode/SS.Backend/FloorPlanImages/FloorPlan2.png";
        

//         // Act
//         var response = await _spaceReader.InsertIntoCompanyFloorPlansAsync(CompanyID,FloorPlanName, ImagePath);

//         // Assert
//         Assert.IsFalse(response.HasError);
//     }

//     [TestMethod]
//     public async Task InsertFloorPlan4_Success()
//     {
        
//         int CompanyID = 1028;
//         string FloorPlanName = "Floor2";
//         string ImagePath = @"/Users/carinegordillo/CECS491/SpaceSurfer/SourceCode/SS.Backend/FloorPlanImages/FloorPlan3.png";
        

//         // Act
//         var response = await _spaceReader.InsertIntoCompanyFloorPlansAsync(CompanyID,FloorPlanName, ImagePath);

//         // Assert
//         Assert.IsFalse(response.HasError);
//     }
//     [TestMethod]
//     public async Task InsertFloorPlan3_Success()
//     {
        
//         int CompanyID = 1029;
//         string FloorPlanName = "Floor Plan Test 33";
//         string ImagePath = @"/Users/carinegordillo/CECS491/SpaceSurfer/SourceCode/SS.Backend/FloorPlanImages/FloorPlan3.png";
        
//         // Act
//         var response = await _spaceReader.InsertIntoCompanyFloorPlansAsync(CompanyID,FloorPlanName, ImagePath);

//         // Assert
//         Assert.IsFalse(response.HasError);
//     }
//     [TestMethod]
//     public async Task InsertFloorPlan5_Success()
//     {
        
//         int CompanyID = 1030;
//         string FloorPlanName = "Floor Plan Test 33";
//         string ImagePath = @"/Users/carinegordillo/CECS491/SpaceSurfer/SourceCode/SS.Backend/FloorPlanImages/FloorPlan3.png";
        
//         // Act
//         var response = await _spaceReader.InsertIntoCompanyFloorPlansAsync(CompanyID,FloorPlanName, ImagePath);

//         // Assert
//         Assert.IsFalse(response.HasError);
//     }

// }