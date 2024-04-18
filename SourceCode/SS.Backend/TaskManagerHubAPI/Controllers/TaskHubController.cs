using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using SS.Backend.SharedNamespace;
using SS.Backend.TaskManagerHub;
// using SS.Backend.ReservationManagers;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using System.Text.Json;

namespace TaskManagerHubAPI.Controllers;

[ApiController]
[Route("api/v1/taskManagerHub")]
public class TaskManagerHubController : ControllerBase
{
    // private readonly IReservationCreationManager _reservationCreationManager;
    private readonly ITaskManagerHubManager _taskManagerHubManager;
    private readonly SSAuthService _authService;
    private readonly IConfiguration _config;


   public TaskManagerHubController(ITaskManagerHubManager taskManagerHubManager,
                                 SSAuthService authService, IConfiguration config)

    {
        _taskManagerHubManager = taskManagerHubManager;
        _authService = authService;
        _config = config;
    }

    [HttpGet("ListTasksNoAuth")]
    public async Task<IActionResult> ListTasksNoAuth(string userName)
    {
        var response = await _taskManagerHubManager.ListTasks(userName);
        if (response.HasError)
        {
            return StatusCode(500, response.ErrorMessage);
        }
        return Ok(response.Values);  // Return only the list for cleaner JSON output
    }


    [HttpGet("ListTasks")]
    public async Task<IActionResult> ListTasks(string userName)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
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
                            var tasks = await _taskManagerHubManager.ListTasks(userName);
                            return Ok(new { tasks.Values, newToken });
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
                            var tasks = await _taskManagerHubManager.ListTasks(userName);
                            return Ok(tasks.Values);
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
    [HttpGet("ListTasksByPriority")]
    public async Task<IActionResult> ListTasksByPriority(string userName, string priority)
    {
        string? accessToken = HttpContext.Request.Headers["Authorization"];
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
                            var tasks = await _taskManagerHubManager.ListTasks(userName);
                            return Ok(new { tasks, newToken });
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
                            var tasks = await _taskManagerHubManager.ListTasks(userName);
                            return Ok(tasks);
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


    private bool TryValidateToken(string accessToken, out Dictionary<string, string> claims, out string userName)
    {
        claims = null;
        userName = null;
        var claimsJson = _authService.ExtractClaimsFromToken(accessToken);
        if (claimsJson != null)
        {
            claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);
            if (claims.TryGetValue("userName", out userName)) {
                return true;
            }
        }
        return false;
    }

    [HttpPost("CreateTask")]
    public async Task<IActionResult> CreateTask([FromBody] TaskHub task)
    {
        // var response = await _taskManagerHubManager.CreateNewTask(userName, task);
        // return Ok(response);
        string accessToken = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
        if (TryValidateToken(accessToken, out var claims, out var userName))
        {
            if (claims.TryGetValue("Role", out var role) && new[] {"1", "2", "3", "4", "5"}.Contains(role))
            {
                console.WriteLine("this is the username:::  ", userName)
                var response = await _taskManagerHubManager.CreateNewTask(userName, task);
                return Ok(response);
            }
            return BadRequest("Unauthorized role.");
        }
        return BadRequest("Invalid token.");
    }



    [HttpPost("CreateMultipleTasks")]
    public async Task<IActionResult> CreateMultipleTasks(string userName, List<TaskHub> tasks)
    {
        var response = await _taskManagerHubManager.CreateMultipleNewTasks(userName, tasks);
        return Ok(response);


        // string accessToken = HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
        // if (TryValidateToken(accessToken, out var claims, out var userName))
        // {
        //     if (claims.TryGetValue("Role", out var role) && new[] {"1", "2", "3", "4", "5"}.Contains(role))
        //     {
        //         var response = await _taskManagerHubManager.CreateMultipleNewTasks(userName, tasks);
        //         return Ok(response);
        //     }
        //     return BadRequest("Unauthorized role.");
        // }
        // return BadRequest("Invalid token.");
    }
}
