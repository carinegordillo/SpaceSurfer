using System;
using Microsoft.AspNetCore.Mvc;
using SS.Backend.Security;
using SS.Backend.Services.EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;
using System.Threading.Tasks;
using SS.Backend.UserDataProtection;
using SS.Backend.Services.DeletingService;

namespace UserDataProtectionAPI.Controllers
{
    [ApiController]
    [Route("api/userDataProtection")]
    public class UserDataProtectionController : Controller
    {
        private readonly SSAuthService _authService;
        private readonly UserDataProtection _userDataProtection;
        private readonly IAccountDeletion _accountDeletion;

        public UserDataProtectionController(SSAuthService authService, UserDataProtection userDataProtection, IAccountDeletion accountDeletionService)
        {
            _authService = authService;
            _userDataProtection = userDataProtection;
            _accountDeletion = accountDeletionService;
        }

        [HttpPost("accessData")]
        public async Task<IActionResult> accessData([FromBody] string userHash)
        {
            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

                if (claimsJson != null)
                {
                    var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                    if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3"))
                    {
                        bool closeToExpTime = _authService.CheckExpTime(accessToken);
                        if (closeToExpTime)
                        {
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                            principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                            var newToken = _authService.CreateJwt(Request, principal);

                            try
                            {
                                Console.WriteLine("Attempting to access data.");
                                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
                                var userData = await _userDataProtection.accessData_Manager(userHash);
                                Console.WriteLine("Successfully accessed data.");
                                await _userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
                                Console.WriteLine("Successfully wrote to file.");
                                await _userDataProtection.sendAccessEmail(userData, outputPath);
                                Console.WriteLine("Successfully sent email.");

                                return Ok( newToken );
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
                                var userData = await _userDataProtection.accessData_Manager(userHash);
                                Console.WriteLine("Successfully accessed data.");
                                await _userDataProtection.WriteToFile_Manager(userData, outputPath);
                                Console.WriteLine("Successfully wrote to file.");
                                await _userDataProtection.sendAccessEmail(userData, outputPath);
                                Console.WriteLine("Successfully sent email.");
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }

                        }
                    }
                    
                    else if (role == "4" || role == "5")
                    {
                        bool closeToExpTime = _authService.CheckExpTime(accessToken);
                        if (closeToExpTime)
                        {
                            SSPrincipal principal = new SSPrincipal();
                            principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
                            principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
                            var newToken = _authService.CreateJwt(Request, principal);

                            try
                            {
                                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
                                var userData = await _userDataProtection.accessData_GeneralUser(userHash);
                                Console.WriteLine("Successfully accessed data.");
                                await _userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
                                Console.WriteLine("Successfully wrote to file.");
                                await _userDataProtection.sendAccessEmail(userData, outputPath);
                                Console.WriteLine("Successfully sent email.");

                                return Ok(newToken);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
                                var userData = await _userDataProtection.accessData_GeneralUser(userHash);
                                Console.WriteLine("Successfully accessed data.");
                                await _userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
                                Console.WriteLine("Successfully wrote to file.");
                                await _userDataProtection.sendAccessEmail(userData, outputPath);
                                Console.WriteLine("Successfully sent email.");
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, ex.Message);
                            }

                        }
                    }
                    else
                    {
                        return BadRequest("Unauthorized role.");
                    }

                }
                else
                {
                    return BadRequest("Invalid token.");
                }
            }
            else
            {
                return BadRequest("Unauthorized. Access token is missing or invalid.");
            }
        }

        //[HttpPost("deleteData")]
        //public async Task<IActionResult> deleteData([FromBody] string userHash)
        //{
        //    string? accessToken = HttpContext.Request.Headers["Authorization"];
        //    if (accessToken != null && accessToken.StartsWith("Bearer "))
        //    {
        //        accessToken = accessToken.Substring("Bearer ".Length).Trim();
        //        var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

        //        if (claimsJson != null)
        //        {
        //            var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

        //            if (claims.TryGetValue("Role", out var role) && (role == "1" || role == "2" || role == "3"))
        //            {
        //                bool closeToExpTime = _authService.CheckExpTime(accessToken);
        //                if (closeToExpTime)
        //                {
        //                    SSPrincipal principal = new SSPrincipal();
        //                    principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
        //                    principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
        //                    var newToken = _authService.CreateJwt(Request, principal);

        //                    try
        //                    {
        //                        Console.WriteLine("--- Begin account deletion endpoint ---");
        //                        Console.WriteLine("Attempting to access data.");
        //                        string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
        //                        var userData = await _userDataProtection.accessData_Manager(userHash);
        //                        Console.WriteLine("Successfully accessed data.");
        //                        await _userDataProtection.WriteToFile_Manager(userData, outputPath);
        //                        Console.WriteLine("Successfully wrote to file.");
        //                        await _userDataProtection.sendAccessEmail(userData, outputPath);
        //                        Console.WriteLine("Successfully sent email.");
        //                        Console.WriteLine("Deleting data.");
        //                        var response = await _accountDeletion.DeleteAccount(userHash);

        //                        return Ok(newToken);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return StatusCode(500, ex.Message);
        //                    }
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        Console.WriteLine("--- Begin account deletion endpoint ---");
        //                        Console.WriteLine("Attempting to access data.");
        //                        string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
        //                        var userData = await _userDataProtection.accessData_Manager(userHash);
        //                        Console.WriteLine("Successfully accessed data.");
        //                        await _userDataProtection.WriteToFile_Manager(userData, outputPath);
        //                        Console.WriteLine("Successfully wrote to file.");
        //                        await _userDataProtection.sendAccessEmail(userData, outputPath);
        //                        Console.WriteLine("Successfully sent email.");
        //                        Console.WriteLine("Deleting data.");
        //                        var response = await _accountDeletion.DeleteAccount(userHash);
        //                        return Ok();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return StatusCode(500, ex.Message);
        //                    }

        //                }
        //            }

        //            else if (role == "4" || role == "5")
        //            {
        //                bool closeToExpTime = _authService.CheckExpTime(accessToken);
        //                if (closeToExpTime)
        //                {
        //                    SSPrincipal principal = new SSPrincipal();
        //                    principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
        //                    principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
        //                    var newToken = _authService.CreateJwt(Request, principal);

        //                    try
        //                    {
        //                        Console.WriteLine("--- Begin account deletion endpoint ---");
        //                        string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
        //                        var userData = await _userDataProtection.accessData_GeneralUser(userHash);
        //                        Console.WriteLine("Successfully accessed data.");
        //                        await _userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
        //                        Console.WriteLine("Successfully wrote to file.");
        //                        await _userDataProtection.sendAccessEmail(userData, outputPath);
        //                        Console.WriteLine("Successfully sent email.");
        //                        Console.WriteLine("Deleting data.");
        //                        var response = await _accountDeletion.DeleteAccount(userHash);

        //                        return Ok(newToken);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return StatusCode(500, ex.Message);
        //                    }
        //                }
        //                else
        //                {
        //                    try
        //                    {
        //                        Console.WriteLine("--- Begin account deletion endpoint ---");
        //                        string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SpaceSurfers_UserData");
        //                        var userData = await _userDataProtection.accessData_GeneralUser(userHash);
        //                        Console.WriteLine("Successfully accessed data.");
        //                        await _userDataProtection.WriteToFile_GeneralUser(userData, outputPath);
        //                        Console.WriteLine("Successfully wrote to file.");
        //                        await _userDataProtection.sendAccessEmail(userData, outputPath);
        //                        Console.WriteLine("Successfully sent email.");
        //                        Console.WriteLine("Deleting data.");
        //                        var response = await _accountDeletion.DeleteAccount(userHash);
        //                        return Ok();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return StatusCode(500, ex.Message);
        //                    }

        //                }
        //            }
        //            else
        //            {
        //                return BadRequest("Unauthorized role.");
        //            }

        //        }
        //        else
        //        {
        //            return BadRequest("Invalid token.");
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest("Unauthorized. Access token is missing or invalid.");
        //    }
        //}

        [HttpGet("checkTokenExp")]
        public IActionResult checkTokenExp()
        {

            string? accessToken = HttpContext.Request.Headers["Authorization"];
            if (accessToken != null && accessToken.StartsWith("Bearer "))
            {
                accessToken = accessToken.Substring("Bearer ".Length).Trim();
                bool tokenExpired = _authService.IsTokenExpired(accessToken);
                if (tokenExpired)
                {
                    return Ok(true);
                }
                else
                {
                    return Ok(false);
                }
            }
            else
            {
                return BadRequest("Unauthorized. Access token is missing or invalid.");
            }
        }

    }
}

