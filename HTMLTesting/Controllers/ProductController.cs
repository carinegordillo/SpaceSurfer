//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using SS.Backend.DataAccess;
//using System.Data;

//namespace HTMLTesting.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly SqlDAO _sqlDAO;

//        public ProductController(SqlDAO sqlDAO)
//        {
//            _sqlDAO = sqlDAO;
//        }

//        public async Task<IActionResult> Index()
//        {
//            // Example SQL command to retrieve products from a table named Products
//            var sqlCommand = new SqlCommand("SELECT Id, Name, Price FROM Products");

//            // Execute the SQL command and get the response
//            var response = await _sqlDAO.ReadSqlResult(sqlCommand);

//            if (!response.HasError)
//            {
//                // If no error, convert the DataTable to a list of Product objects
//                var products = new List<SS.Backend.DataAccess.Product>();
//                foreach (DataRow row in response.ValuesRead.Rows)
//                {
//                    products.Add(new SS.Backend.DataAccess.Product
//                    {
//                        Id = Convert.ToInt32(row["Id"]),
//                        Name = row["Name"].ToString(),
//                        Price = Convert.ToDecimal(row["Price"])
//                    });
//                }

//                // Pass the list of products to the view
//                return View(products);
//            }
//            else
//            {
//                // Handle error
//                return View("Error");
//            }
//        }
//    }
//}
