// string? accessToken = HttpContext.Request.Headers["Authorization"];
//         if (accessToken != null && accessToken.StartsWith("Bearer "))
//         {
//             accessToken = accessToken.Substring("Bearer ".Length).Trim();
//             var claimsJson = _authService.ExtractClaimsFromToken(accessToken);

//             if (claimsJson != null)
//             {
//                 var claims = JsonSerializer.Deserialize<Dictionary<string, string>>(claimsJson);

//                 if (claims.TryGetValue("Role", out var role) && role == "1" || role == "2" || role == "3" || role == "4" || role == "5")
//                 {
//                     bool closeToExpTime = _authService.CheckExpTime(accessToken);
//                     if (closeToExpTime)
//                     {
//                         SSPrincipal principal = new SSPrincipal();
//                         principal.UserIdentity = _authService.ExtractSubjectFromToken(accessToken);
//                         principal.Claims = _authService.ExtractClaimsFromToken_Dictionary(accessToken);
//                         var newToken = _authService.CreateJwt(Request, principal);
//                         try
//                         {
//                             var response = await _profileModifier.ModifyProfile(userProfile);
            
//                                 if (response == null)
//                                 {
//                                     Console.WriteLine("ModifyProfile returned null");
//                                     return StatusCode(500, "Internal server error: response is null");
//                                 }

//                                 Console.WriteLine($"Response: HasError={response.HasError}, ErrorMessage={response.ErrorMessage}");

//                             return Ok(new { response, newToken });
//                         }
//                         catch (Exception ex)
//                         {
//                             return StatusCode(500, "Internal server error: " + ex.Message);
//                         }
//                     }
//                     else
//                     {
//                         try
//                             {
//                                 var response = await _profileModifier.ModifyProfile(userProfile);
            
//                                 if (response == null)
//                                 {
//                                     Console.WriteLine("ModifyProfile returned null");
//                                     return StatusCode(500, "Internal server error: response is null");
//                                 }

//                                 Console.WriteLine($"Response: HasError={response.HasError}, ErrorMessage={response.ErrorMessage}");

//                                 return Ok(response);
//                         }
//                         catch (Exception ex)
//                         {
//                             return StatusCode(500, "Internal server error: " + ex.Message);
//                         }
//                     }
//                 }
//                 else
//                 {
//                     return BadRequest("Unauthorized role.");
//                 }
//             }
//             else
//             {
//                 return BadRequest("Invalid token.");
//             }
//         }
//         else
//         {
//             return BadRequest("Unauthorized. Access token is missing or invalid.");
//         }