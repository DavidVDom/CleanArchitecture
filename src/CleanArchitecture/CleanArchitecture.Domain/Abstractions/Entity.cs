namespace CleanArchitecture.Domain.Abstractions;

// para el id usaremos strong type id, para que no necesariamente sea un id, así que esta clase recibirá un tipo genérico para el id de los hijos
public abstract class Entity<TEntityId> : IEntity
{
    protected Entity()
    {

    }

    private readonly List<IDomainEvent> _domainEvents = new();

    protected Entity(TEntityId id)
    {
        Id = id;
    }

    //public Guid Id { get; init; }
    public TEntityId? Id { get; init; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }


}