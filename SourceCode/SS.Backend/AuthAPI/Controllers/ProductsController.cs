using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SS.Backend.DataAccess;
using SS.Backend.SharedNamespace;
using System.Data;

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
        private readonly SqlDAO _sqldao;

        public ProductsController(SqlDAO dao)
        {
            _sqldao = dao ?? throw new ArgumentNullException(nameof(dao));
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM Products");
                Response response = await _sqldao.ReadSqlResult(cmd);

                if (response.HasError)
                {
                    return StatusCode(500, $"An error occurred: {response.ErrorMessage}");
                }

                List<Product> products = new List<Product>();

                if (response.ValuesRead != null)
                {
                    foreach (DataRow row in response.ValuesRead.Rows)
                    {
                        products.Add(new Product
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Price = Convert.ToDecimal(row["Price"])
                        });
                    }
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}
