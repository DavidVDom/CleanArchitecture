namespace CleanArchitecture.Domain.Reviews
{
    // strong type id, como en UserId
    public record ReviewId(Guid Value)
    {
        public static ReviewId New() => new(Guid.NewGuid());
    }
}
