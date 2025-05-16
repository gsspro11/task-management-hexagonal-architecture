using TaskManagement.HexagonalArchitecture.Domain.Enums;
using TaskManagement.HexagonalArchitecture.Domain.Primitives;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class Assignment : AggregateRoot
    {
        public Assignment(Guid id) : base(id)
        {

        }

        public Assignment(Guid id, string title, string description, Guid userId, DateTime dueDate, int priority,
            AssignmentStatus status) : base(id)
        {
            Title = title;
            Description = description;
            UserId = userId;
            DueDate = dueDate.Date;
            Priority = priority;
            Status = status;
            CreatedDate = DateTime.Now;
        }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public int Priority { get; private set; }
        public AssignmentStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
        public Guid? UserId { get; private set; }
        public User? User { get; private set; }
        public List<Comment>? Comments { get; private set; }

        public void Update(string title, string description, DateTime dueDate, int priority, AssignmentStatus status)
        {
            Title = title;
            Description = description;
            DueDate = dueDate.Date;
            Priority = priority;
            Status = status;

            UpdatedDate = DateTime.Now;
        }
    }
}