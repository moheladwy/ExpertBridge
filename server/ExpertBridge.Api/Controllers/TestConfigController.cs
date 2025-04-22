// using ExpertBridge.Api.Settings;
// using ExpertBridge.Api.Settings.Serilog;
// using ExpertBridge.Api.Core.Interfaces.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;
//
// namespace ExpertBridge.Api.Controllers;
//
// [ApiController]
// [Route("api/[controller]")]
//
// // test config using GET /api/TestConfig
// // test firebase using POST api/TestConfig/firebase?Content-Type=application/json (username, password in body)
// public class TestConfigController : ControllerBase
// {
//     private readonly ConnectionStrings _connectionStrings;
//     private readonly AwsSettings _awsConfig;
//     private readonly AiSettings _aiSettings;
//     private readonly FirebaseSettings _firebaseConfig;
//     private readonly FirebaseAuthSettings _firebaseAuthSettings;
//     private readonly SerilogSettings _serilogSettings;
//
//     public TestConfigController(
//         IOptions<ConnectionStrings> connectionStrings,
//         IOptions<AwsSettings> awsConfig,
//         IOptions<AiSettings> aiSettings,
//         IOptions<FirebaseSettings> firebaseConfig,
//         IOptions<FirebaseAuthSettings> firebaseAuthSettings,
//         IOptions<SerilogSettings> serilogSettings)
//     {
//         _connectionStrings = connectionStrings.Value;
//         _awsConfig = awsConfig.Value;
//         _aiSettings = aiSettings.Value;
//         _firebaseConfig = firebaseConfig.Value;
//         _firebaseAuthSettings = firebaseAuthSettings.Value;
//         _serilogSettings = serilogSettings.Value;
//     }
//
//     [HttpGet]
//     public IActionResult TestConfigurations()
//     {
//         var configTest = new
//         {
//             DbConnectionExists = !string.IsNullOrEmpty(_connectionStrings.Postgresql),
//             AwsConfigExists = !string.IsNullOrEmpty(_awsConfig.BucketName),
//             AiConfigExists = !string.IsNullOrEmpty(_aiSettings.PostCategorizationUrl),
//             FirebaseConfigExists = new
//             {
//                 ProjectId = !string.IsNullOrEmpty(_firebaseConfig.ProjectId),
//                 PrivateKey = !string.IsNullOrEmpty(_firebaseConfig.PrivateKey),
//                 ClientEmail = !string.IsNullOrEmpty(_firebaseConfig.ClientEmail)
//             },
//             FirebaseAuthExists = new
//             {
//                 Issuer = !string.IsNullOrEmpty(_firebaseAuthSettings.Issuer),
//                 Audience = !string.IsNullOrEmpty(_firebaseAuthSettings.Audience),
//                 TokenUri = !string.IsNullOrEmpty(_firebaseAuthSettings.TokenUri)
//             },
//             SerilogConfigExists = new
//             {
//                 UsingExists = _serilogSettings.Using.Length > 0,
//                 MinimumLevelExists = !string.IsNullOrEmpty(_serilogSettings.MinimumLevel.Default),
//                 WriteToExists = _serilogSettings.WriteTo.Length > 0,
//                 EnrichExists = _serilogSettings.Enrich.Length > 0,
//                 SeqUrlExists = _serilogSettings.WriteTo.Any(w =>
//                     w.Name == "Seq" &&
//                     !string.IsNullOrEmpty(w.Args.ServerUrl))
//             }
//         };
//
//         return Ok(configTest);
//     }
//
//     [HttpPost("firebase")]
//     public async Task<IActionResult> TestFirebaseAuth(
//         [FromServices] IFirebaseAuthService firebaseAuth,
//         [FromBody] LoginRequest request)
//     {
//         try
//         {
//             var token = await firebaseAuth.LoginAsync(request.Email, request.Password);
//             var verifiedToken = await firebaseAuth.VerifyIdTokenAsync(token);
//
//             return Ok(new
//             {
//                 LoginSuccess = !string.IsNullOrEmpty(token),
//                 TokenVerified = verifiedToken != null,
//                 UserId = verifiedToken?.Uid
//             });
//         }
//         catch (Exception ex)
//         {
//             return BadRequest(new { Error = ex.Message });
//         }
//     }
//
//     public class LoginRequest
//     {
//         public string Email { get; set; } = string.Empty;
//         public string Password { get; set; } = string.Empty;
//     }
// }
