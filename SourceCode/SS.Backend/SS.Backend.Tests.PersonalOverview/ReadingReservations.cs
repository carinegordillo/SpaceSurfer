using SS.Backend.DataAccess;
using SS.Backend.ReservationManagement;
using SS.Backend.Services.PersonalOverviewService;
using SS.Backend.SharedNamespace;

namespace SS.Backend.Tests.PersonalOverviewService
{
    [TestClass]
    public class ReadingReservations
    {
        private Response response;
        private CustomSqlCommandBuilder commandBuilder;
        private SqlDAO sqlDAO;
        private PersonalOverview _personalOverview;
        private PersonalOverviewDAO _personalOverviewDAO;
        private ReservationManagementRepository _reservationManagementRepository;


        [TestMethod]
        public void TestInitialize()
        {
        }
    }
}