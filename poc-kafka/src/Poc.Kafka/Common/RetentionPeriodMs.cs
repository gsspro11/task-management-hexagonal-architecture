namespace Poc.Kafka.Common;


/// <summary>
/// Represents retention periods in milliseconds.
/// </summary>
public enum RetentionPeriodMs : long
{
    /// <summary>
    /// Indicates that there is no retention period enforced, potentially defaulting to broker settings or implying infinite retention.
    /// </summary>
    NoRetention = -1,
    /// <summary>
    /// Represents twelve hours.
    /// </summary>
    TwelveHours = 43200000,
    /// <summary>
    /// Represents one day.
    /// </summary>
    OneDay = 86400000,
    /// <summary>
    /// Represents two days.
    /// </summary>
    TwoDays = 172800000,
    /// <summary>
    /// Represents seven days.
    /// </summary>
    SevenDays = 604800000,
    /// <summary>
    /// Represents four weeks.
    /// </summary>
    FourWeeks = 2419200000
}