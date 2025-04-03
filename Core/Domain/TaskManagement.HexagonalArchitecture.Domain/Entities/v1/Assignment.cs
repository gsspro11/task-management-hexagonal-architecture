using TaskManagement.HexagonalArchitecture.Domain.Enums;

namespace TaskManagement.HexagonalArchitecture.Domain.Entities.v1
{
    public class Assignment
    {
        public Assignment(string title, string description, DateTime dueDate, int priority, AssignmentStatus status)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = status;
            CreatedDate = DateTime.Now;
        }
        
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime DueDate { get; private set; }
        public int Priority { get; private set; }
        public AssignmentStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }

        public List<User> Users { get; private set; }
        public List<Comment> Comments { get; private set; }
        
        public void Update(string title, string description, DateTime dueDate, int priority, AssignmentStatus status)
        {
            Title = title;
            Description = description;
            DueDate = dueDate;
            Priority = priority;
            Status = status;

            UpdatedDate = DateTime.Now;
        }
    }
}