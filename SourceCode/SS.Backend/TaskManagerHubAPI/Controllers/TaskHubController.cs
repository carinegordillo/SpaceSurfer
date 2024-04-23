using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using SS.Backend.SharedNamespace;
using SS.Backend.TaskManagerHub;
// using SS.Backend.ReservationManagers;
using SS.Backend.DataAccess;
using SS.Backend.Security;
using System.Text.Json;
// using Newtonsoft.Json;
using System.Collections.Generic;

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

    [HttpGet("ScoreTasks")]
    public async Task<IActionResult> ScoreTasks(string userName)
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
                            var tasks = await _taskManagerHubManager.ScoreTasks(userName);
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
                            var tasks = await _taskManagerHubManager.ScoreTasks(userName);
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
        try
        {
            var claimsJson = _authService.ExtractClaimsFromToken(accessToken);
            if (!string.IsNullOrEmpty(claimsJson))
            {
                claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);
                return claims.TryGetValue("sub", out userName); // commonly 'sub' is used for subject which represents the user
            }
        }
        catch (JsonException ex)
        {
            // Log or handle the error as needed
            Console.WriteLine($"Error deserializing token claims: {ex.Message}");
        }
        return false;
    }

    [HttpPost("CreateTask")]
    // public async Task<IActionResult> CreateTask([FromBody] TaskHub task)
    public async Task<IActionResult> CreateTask([FromBody] TaskHub task)    {
        // userName = HttpUtility.UrlDecode(userName);
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
                            var tasks = await _taskManagerHubManager.CreateNewTask(task);
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
                            var tasks = await _taskManagerHubManager.CreateNewTask(task);
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

    [HttpPost("DeleteTask")]
    public async Task<IActionResult> DeleteTask([FromBody] TaskHub task)    {
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
                            var tasks = await _taskManagerHubManager.DeleteTask(task);
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
                            var tasks = await _taskManagerHubManager.DeleteTask(task);
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

    [HttpPost("ModifyTask")]
    public async Task<IActionResult> ModifyTask([FromBody] ModifyTaskRequest request)
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
                            var fieldsToUpdateJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.FieldsToUpdateJson);
                            var fieldsToUpdate = fieldsToUpdateJson.ToDictionary(
                                kvp => kvp.Key,
                                kvp => ConvertJsonElement(kvp.Value));

                            var task = new TaskHub { hashedUsername = request.UserName, title = request.TaskTitle };
                            var response = await _taskManagerHubManager.ModifyTasks(task, fieldsToUpdate);

                            return Ok(new { response });
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
                            var fieldsToUpdateJson = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.FieldsToUpdateJson);
                            var fieldsToUpdate = fieldsToUpdateJson.ToDictionary(
                                kvp => kvp.Key,
                                kvp => ConvertJsonElement(kvp.Value));

                            var task = new TaskHub { hashedUsername = request.UserName, title = request.TaskTitle };
                            var response = await _taskManagerHubManager.ModifyTasks(task, fieldsToUpdate);

                            return Ok(new { response });
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

    public class ModifyTaskRequest
    {
        public string UserName { get; set; }
        public string TaskTitle { get; set; }
        public string FieldsToUpdateJson { get; set; } 
    }
    private object ConvertJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString();
            case JsonValueKind.Number:
                return element.TryGetInt64(out long l) ? l : (object)element.GetDouble();
            case JsonValueKind.True:
            case JsonValueKind.False:
                return element.GetBoolean();
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                return null;
            default:
                throw new InvalidOperationException("Unsupported JsonValueKind: " + element.ValueKind);
        }
    }
}
