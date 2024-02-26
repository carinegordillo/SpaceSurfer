using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;

namespace TestAPI.Controllers
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    [ApiController]
    [Route("api/Products")]
    public class ProductsController : ControllerBase
    {
        //private readonly testdao _testdao;
        private readonly SqlDAO _sqldao;

        public ProductsController(SqlDAO dao)
        {
            //_testdao = testdao ?? throw new ArgumentNullException(nameof(testdao));
            _sqldao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                Response result = new Response();
                SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM Products");
                result = await _sqldao.ReadSqlResult(cmd);

                return Ok(result.ValuesRead);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
