namespace TaskManagement.HexagonalArchitecture.Domain.Abstractions;

public class CustomResult<T>
{
    private readonly T? _value;
    private CustomResult(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = CustomError.None;
    }
    private CustomResult(CustomError error)
    {
        if (error == CustomError.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = false;
        Error = error;
    }

    private CustomResult(CustomError[] errors)
    {
        if (errors.Length == 0)
        {
            throw new ArgumentException("Invalid errors", nameof(errors));
        }
        IsSuccess = false;
        Errors = errors;
    }

    private bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("There is no value for failure");
            }
            return _value!;
        }
        private init => _value = value;
    }
    public CustomError? Error { get; }
    public CustomError[]? Errors { get; }
    public static CustomResult<T> Success(T value)
    {
        return new CustomResult<T>(value);
    }
    public static CustomResult<T> Failure(CustomError error)
    {
        return new CustomResult<T>(error);
    }
    public static CustomResult<T> Failure(CustomError[] errors)
    {
        return new CustomResult<T>(errors);
    }
}