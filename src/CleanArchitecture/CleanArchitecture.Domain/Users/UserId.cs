namespace CleanArchitecture.Domain.Users
{
    // implementamos strong type id, de tal forma que genere un Id cada vez que se cree un objeto nuevo llamando a UserId.New()
    // por si a futuro no queremos que el id sea un id, sino un entero, o cualquier otro tipo de dato
    public record UserId(Guid Value)
    {
        public static UserId New() => new(Guid.NewGuid());
    }
}
