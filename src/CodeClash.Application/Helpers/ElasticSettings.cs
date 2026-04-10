namespace CodeClash.Application.Helpers;
public sealed class ElasticSettings
{
    public string Url { get; set; }
    public string DefaultIndex { get; set; }
    public string SecondaryIndex { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
