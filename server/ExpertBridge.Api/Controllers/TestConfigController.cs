using ExpertBridge.Api.Configurations;
using ExpertBridge.Api.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestConfigController : ControllerBase
{
    private readonly ConnectionStrings _connectionStrings;
    private readonly AwsConfigurations _awsConfig;
    private readonly AiSettings _aiSettings;
    private readonly FirebaseCredentials _firebaseConfig;
    private readonly FirebaseAuthSettings _firebaseAuthSettings;

    public TestConfigController(
        IOptions<ConnectionStrings> connectionStrings,
        IOptions<AwsConfigurations> awsConfig,
        IOptions<AiSettings> aiSettings,
        IOptions<FirebaseCredentials> firebaseConfig,
        IOptions<FirebaseAuthSettings> firebaseAuthSettings)
    {
        _connectionStrings = connectionStrings.Value;
        _awsConfig = awsConfig.Value;
        _aiSettings = aiSettings.Value;
        _firebaseConfig = firebaseConfig.Value;
        _firebaseAuthSettings = firebaseAuthSettings.Value;
    }

    [HttpGet]
    public IActionResult TestConfigurations()
    {
        var configTest = new
        {
            DbConnectionExists = !string.IsNullOrEmpty(_connectionStrings.Postgresql),
            AwsConfigExists = !string.IsNullOrEmpty(_awsConfig.BucketName),
            AiConfigExists = !string.IsNullOrEmpty(_aiSettings.PostCategorizationUrl),
            FirebaseConfigExists = new
            {
                ProjectId = !string.IsNullOrEmpty(_firebaseConfig.ProjectId),
                PrivateKey = !string.IsNullOrEmpty(_firebaseConfig.PrivateKey),
                ClientEmail = !string.IsNullOrEmpty(_firebaseConfig.ClientEmail)
            },
            FirebaseAuthExists = new
            {
                Issuer = !string.IsNullOrEmpty(_firebaseAuthSettings.Issuer),
                Audience = !string.IsNullOrEmpty(_firebaseAuthSettings.Audience),
                TokenUri = !string.IsNullOrEmpty(_firebaseAuthSettings.TokenUri)
            }
        };

        return Ok(configTest); // hopefully true :))))))
    }

    [HttpPost("firebase")]
    public async Task<IActionResult> TestFirebaseAuth(
        [FromServices] IFirebaseAuthService firebaseAuth,
        [FromBody] LoginRequest request)
    {
        try
        {
            var token = await firebaseAuth.LoginAsync(request.Email, request.Password);
            var verifiedToken = await firebaseAuth.VerifyIdTokenAsync(token);

            return Ok(new
            {
                LoginSuccess = !string.IsNullOrEmpty(token),
                TokenVerified = verifiedToken != null,
                UserId = verifiedToken?.Uid
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
