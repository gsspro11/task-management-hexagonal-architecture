using System.ComponentModel;

namespace TaskManagement.HexagonalArchitecture.Domain.Enums
{
    public enum AssignmentStatus
    {
        [Description("AGUARDANDO")]
        A = 'A',
        [Description("PAGO")]
        P = 'P',
        [Description("AGUARDANDO REMOVIDO")]
        O = 'O',
        [Description("REMOVIDO")]
        E = 'R'
    }
}
