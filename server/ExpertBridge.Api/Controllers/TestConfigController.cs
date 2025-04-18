using ExpertBridge.Api.Configurations;
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

    public TestConfigController(
        IOptions<ConnectionStrings> connectionStrings,
        IOptions<AwsConfigurations> awsConfig,
        IOptions<AiSettings> aiSettings)
    {
        _connectionStrings = connectionStrings.Value;
        _awsConfig = awsConfig.Value;
        _aiSettings = aiSettings.Value;
    }

    [HttpGet]
    public IActionResult TestConfigurations()
    {
        var configTest = new
        {
            DbConnectionExists = !string.IsNullOrEmpty(_connectionStrings.Postgresql),
            AwsConfigExists = !string.IsNullOrEmpty(_awsConfig.BucketName),
            AiConfigExists = !string.IsNullOrEmpty(_aiSettings.PostCategorizationUrl)
        };

        return Ok(configTest);
    }

    // should return true ( hopefully :)) )
}
