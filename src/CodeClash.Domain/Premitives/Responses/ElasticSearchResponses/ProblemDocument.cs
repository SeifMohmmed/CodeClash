namespace CodeClash.Domain.Premitives.Responses.ElasticSearchResponses;
public sealed class ProblemDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Difficulty Difficulty { get; set; }
    public List<Guid> Topics { get; set; }  // TopicIds from ProblemTopics
}
