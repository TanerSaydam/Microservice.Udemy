namespace MiniETicaret.Orders.WebAPI.Dtos;

public sealed class Result<T>
{
    public Result()
    {

    }

    public Result(T data)
    {
        Data = data;
    }

    public T? Data { get; set; } = default;
}
