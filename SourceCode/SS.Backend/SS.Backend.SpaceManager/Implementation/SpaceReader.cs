using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data;

namespace SS.Backend.SpaceManager
{

    public class SpaceReader : ISpaceReader
    {
        private readonly ISpaceManagerDao _spaceManagerDao;

        public SpaceReader(ISpaceManagerDao spaceManagerDao)
        {
            _spaceManagerDao = spaceManagerDao;
        }

        public async Task<IEnumerable<CompanyInfoWithID>> GetCompanyInfoAsync()
        {
            List<CompanyInfoWithID> companyInfos = new List<CompanyInfoWithID>();
            var commandBuilder = new CustomSqlCommandBuilder();
            var command = commandBuilder.BeginStoredProcedure("GetCompanyInfoPROD").Build();

            Response response = new Response();
            try
            {
                response = await _spaceManagerDao.ExecuteReadCompanyTables(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
                return companyInfos;
            }

            if (!response.HasError && response.ValuesRead != null)
            {

                companyInfos = ConvertToCompanyInfoList(response.ValuesRead);
            }
            else
            {
                Console.WriteLine("No data found or error occurred.");
            }

            return companyInfos;
        }

        public async Task<IEnumerable<CompanyInfoWithID>> GetAvailableCompaniesForUser(int? employeeCompanyID)
        {
            var companies = await GetAllFacilities();

            if (employeeCompanyID.HasValue)
            {
                Console.WriteLine("This is an employee");
                Console.WriteLine(employeeCompanyID.Value);
                var employeeCompany = await GetEmployeeCompany(employeeCompanyID.Value);
                companies.InsertRange(0, employeeCompany);
            }

            return companies;
        }

        public async Task<List<CompanyInfoWithID>> GetAllFacilities()
        {
            var companyInfos = new List<CompanyInfoWithID>();
            var commandBuilder = new CustomSqlCommandBuilder();
            var command = commandBuilder.BeginStoredProcedure("GetFacilitiesInfoPROD").Build();

            try
            {
                var response = await _spaceManagerDao.ExecuteReadCompanyTables(command);

                if (!response.HasError && response.ValuesRead != null)
                {
                    companyInfos = ConvertToCompanyInfoList(response.ValuesRead);
                }
                else
                {
                    Console.WriteLine("No data found or error occurred.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
            }

            return companyInfos;
        }

        public async Task<List<CompanyInfoWithID>> GetEmployeeCompany(int companyID)
        {
            var companyInfos = new List<CompanyInfoWithID>();
            var commandBuilder = new CustomSqlCommandBuilder();

            var parameters = new Dictionary<string, object>
            {
                {"CompanyID", companyID}
            };


            var command = commandBuilder.BeginStoredProcedure("GetEmployeeCompanyInfoPROD")
                                        .AddParameters(parameters)
                                        .Build();

            try
            {
                var response = await _spaceManagerDao.ExecuteReadCompanyTables(command);

                if (!response.HasError && response.ValuesRead != null)
                {
                    companyInfos = ConvertToCompanyInfoList(response.ValuesRead);
                }
                else
                {
                    Console.WriteLine("No data found or error occurred.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing stored procedure: {ex.Message}");
            }

            return companyInfos;
        }

        private List<CompanyInfoWithID> ConvertToCompanyInfoList(DataTable dataTable)
        {
            var companyInfos = new List<CompanyInfoWithID>();

            foreach (DataRow row in dataTable.Rows)
            {
                var companyInfo = new CompanyInfoWithID
                {
                    CompanyID = Convert.ToInt32(row["companyID"]),
                    CompanyName = Convert.ToString(row["companyName"]).Trim(),
                    Address = Convert.ToString(row["address"]).Trim(),
                    OpeningHours = TimeSpan.Parse(Convert.ToString(row["openingHours"])),
                    ClosingHours = TimeSpan.Parse(Convert.ToString(row["closingHours"])),
                    DaysOpen = Convert.ToString(row["daysOpen"]).Trim()
                };
                companyInfos.Add(companyInfo);
            }

            return companyInfos;
        }


        public async Task<IEnumerable<CompanyFloorStrImage>> GetCompanyFloorsAsync(int companyId)
        {
            var floors = new Dictionary<int, CompanyFloorStrImage>();

            var commandBuilder = new CustomSqlCommandBuilder();
            var command = commandBuilder.BeginStoredProcedure("GetCompanyFloorPROD")
                                        .AddParameters(new Dictionary<string, object> { { "companyId", companyId } })
                                        .Build();

            Console.WriteLine("Command: " + command.CommandText);

            Response response = await _spaceManagerDao.ExecuteReadCompanyTables(command);

            Console.WriteLine(response.ErrorMessage);
            if (!response.HasError && response.ValuesRead != null)
            {
                foreach (DataRow row in response.ValuesRead.Rows)
                {
                    int floorPlanID = row["floorPlanID"] as int? ?? default(int);
                    string? floorPlanName = row["floorPlanName"]?.ToString().Trim();
                    byte[]? floorPlanImage = row["floorPlanImage"] as byte[];
                    string? spaceID = row["spaceID"]?.ToString().Trim();
                    int timeLimit = row["timeLimit"] as int? ?? default;

                    CompanyFloorStrImage? floor;
                    if (!floors.TryGetValue(floorPlanID, out floor))
                    {
                        floor = new CompanyFloorStrImage
                        {
                            FloorPlanID = floorPlanID,
                            FloorPlanName = floorPlanName ?? string.Empty,
                            FloorPlanImageBase64 = floorPlanImage != null ? Convert.ToBase64String(floorPlanImage) : null,
                        };
                        floors.Add(floorPlanID, floor);
                    }

                    floor.FloorSpaces[spaceID] = timeLimit;
                }
            }
            else
            {
                Console.WriteLine("No data found or error occurred.");
            }

            Console.WriteLine("Returning floors");

            return floors.Values;
        }


        public async Task<Response> InsertIntoCompanyFloorPlansAsync(int companyID, string floorPlanName, string floorPlanPath)
        {
            Response response = new Response();

            byte[] imageBytes = ConvertImageToByteArray(floorPlanPath);

            var commandBuilder = new CustomSqlCommandBuilder();
            var command = commandBuilder.BeginStoredProcedure("InsertCompanyFloor")
                                        .AddParameters(new Dictionary<string, object> { { "companyId", companyID }, { "floorPlanName", floorPlanName }, { "floorPlanImage", imageBytes } })
                                        .Build();
            response = await _spaceManagerDao.ExecuteWriteCompanyTables(command);
            if (!response.HasError)
            {
                response.HasError = false;
                response.ErrorMessage += "Floor plan inserted successfully.";
            }
            else
            {
                response.HasError = true;
                response.ErrorMessage += "Failed to insert the floor plan.";
            }
            return response;
        }




        private byte[] ConvertImageToByteArray(string imagePath)
        {
            byte[] imageBytes;
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    imageBytes = br.ReadBytes((int)fs.Length);
                }
            }
            return imageBytes;
        }
    }
}
