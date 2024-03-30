using Microsoft.AspNetCore.Mvc;
using System.Data;
using SS.Backend.SpaceManager;
using SS.Backend.SharedNamespace;
using SS.Backend.Security;
using SS.Backend.DataAccess;



namespace CompanyAPI.Controllers;

[ApiController]
[Route("api/v1/spaceBookingCenter/companies")]
public class CompanyInfoController : ControllerBase
{
    private readonly ISpaceReader _spaceReader;

    public CompanyInfoController(ISpaceReader spaceReader)
    {
        _spaceReader = spaceReader;
    }

    [HttpGet("ListCompanies")]
    public async Task<IActionResult> ListCompanies()
    {
        try
        {
            var companies = await _spaceReader.GetCompanyInfoAsync();
            return Ok(companies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


    [HttpGet("FloorPlans/{companyId}")]
    public async Task<IActionResult> GetCompanyFloorPlans(int companyId)
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
