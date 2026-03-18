namespace CodeClash.Domain.Premitives;
public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }
    protected Entity()
    { }

    public Guid Id { get; protected set; } = Guid.CreateVersion7();
}
