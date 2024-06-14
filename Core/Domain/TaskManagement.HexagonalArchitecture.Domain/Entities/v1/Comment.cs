namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class Comment
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public User User { get; private set; }
    }
}