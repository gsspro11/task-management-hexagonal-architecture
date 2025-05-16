using System.ComponentModel;

namespace TaskManagement.HexagonalArchitecture.Domain.Enums
{
    public enum AssignmentStatus
    {
        [Description("TO DO")]
        T,
        [Description("IN PROGRESS")]
        I,
        [Description("DONE")]
        D,
    }
}
