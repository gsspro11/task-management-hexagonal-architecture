using System.ComponentModel;

namespace TaskManagement.HexagonalArchitecture.Domain.Enums
{
    public enum AssignmentStatus
    {
        [Description("NEW")]
        N = 'N',
        [Description("ACTIVE")]
        A = 'A',
        [Description("RESOLVED")]
        R = 'R',
        [Description("CLOSED")]
        C = 'C'
    }
}
