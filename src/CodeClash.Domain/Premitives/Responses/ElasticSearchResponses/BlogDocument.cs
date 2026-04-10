namespace CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
public class BlogDocument
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public List<string> Topics { get; set; }
}
