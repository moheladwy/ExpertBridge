namespace Core.Responses;

public class InappropriateLanguageDetectionResponse
{
    public double Toxicity { get; set; }
    public double SevereToxicity { get; set; }
    public double Obscene { get; set; }
    public double Threat { get; set; }
    public double Insult { get; set; }
    public double IdentityAttack { get; set; }
    public double SexualExplicit { get; set; }
}
