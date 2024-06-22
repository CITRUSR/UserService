namespace UserService.Application.Common.Errors;

public class ValidationError
{
    public string Service { get; private set; } = "Users";
    public IEnumerable<string> Erorrs { get; set; }
}