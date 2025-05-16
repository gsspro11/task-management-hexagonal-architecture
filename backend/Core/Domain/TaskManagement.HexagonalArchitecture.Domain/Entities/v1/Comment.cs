using System.ComponentModel.DataAnnotations.Schema;
using TaskManagement.HexagonalArchitecture.Domain.Primitives;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class Comment : AggregateRoot
    {
        public Comment(Guid id, string description, string userName) : base(id)
        {
            Description = description;
            UserName = userName;
        }

        public Comment(string description, Guid assignmentId, Guid userId) : base(Guid.NewGuid())
        {
            Description = description;
            UserId = userId;
            AssignmentId = assignmentId;
            CreatedDate = DateTime.Now;
        }

        public string Description { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public Guid UserId { get; private set; }
        public User? User { get; private set; }

        [NotMapped] public string? UserName { get; private set; }

        public Guid AssignmentId { get; private set; }
        public Assignment? Assignment { get; private set; }

        public void Update(string description)
        {
            Description = description;
        }
    }
}