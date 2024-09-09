namespace Shared;

public class CartaoMessage
{
    public string? Cvv { get; set; }
    public string? Number { get; set; }
}

public class CredicardMessage : CartaoMessage
{
    public string? Title { get; set; }
}

public class VisaMessage : CartaoMessage
{
    public string? Description { get; set; }
}