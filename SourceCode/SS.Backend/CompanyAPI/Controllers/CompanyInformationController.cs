using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using SS.Backend.DataAccess;
using System.Text.Json;

using Microsoft.AspNetCore.Http.HttpResults;



namespace CompanyAPI.Controllers;

[ApiController]
[Route("api/v1/spaceBookingCenter/companies")]
public class CompanyInfoController : ControllerBase
{
    private readonly ISpaceReader _spaceReader;
    private readonly SSAuthService _authService;
    private readonly IConfiguration _config;

    public CompanyInfoController(ISpaceReader spaceReader, SSAuthService authService, IConfiguration config)
    {
        _spaceReader = spaceReader;
        _authService = authService;
        _config = config;
    }

    [HttpGet("ListCompanies")]
    public async Task<IActionResult> ListCompanies()
    {
        string accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" || role == "2" || role == "3" || role == "4" || role == "5")
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
                           var companies = await _spaceReader.GetCompanyInfoAsync();
                            return Ok(new { companies, newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, "Internal server error: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            var companies = await _spaceReader.GetCompanyInfoAsync();
                            foreach (var company in companies)
                            {
                                Console.WriteLine(company.CompanyName);
                                company.CompanyName = company.CompanyName.Trim();
                            }
                            return Ok(companies);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, "Internal server error: " + ex.Message);
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


    [HttpGet("FloorPlans/{companyId}")]
    public async Task<IActionResult> GetCompanyFloorPlans(int companyId)
    {
        string accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

            if (claimsJson != null)
            {
                var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

                if (claims.TryGetValue("Role", out var role) && role == "1" || role == "2" || role == "3" || role == "4" || role == "5")
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
                            var floorPlans = await _spaceReader.GetCompanyFloorsAsync(companyId);
                            foreach (var floorplan in floorPlans)
                            {
                                Console.WriteLine(floorplan);
                            }

                            return Ok(new { floorPlans, newToken });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, "Internal server error: " + ex.Message);
                        }
                    }
                    else
                    {
                        try
                            {
                                var floorPlans = await _spaceReader.GetCompanyFloorsAsync(companyId);
                                if (floorPlans == null || !floorPlans.Any())
                                {
                                    return NotFound("Floor plans not found for the given company ID.");
                                }


                                return Ok(floorPlans);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, "Internal server error: " + ex.Message);
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


    [HttpGet("checkTokenExp")]
    public async Task<IActionResult> checkTokenExp()
    {

        string accessToken = HttpContext.Request.Headers["Authorization"];
        if (accessToken != null && accessToken.StartsWith("Bearer "))
        {
            accessToken = accessToken.Substring("Bearer ".Length).Trim();
            bool tokenExpired =  _authService.IsTokenExpired(accessToken);
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
