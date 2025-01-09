namespace UserService.Application.Common.Errors;

public class Error
{
    public string Service { get; private set; } = "Users";
    public IEnumerable<string> Errors { get; set; }
}
