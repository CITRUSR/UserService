using UserService.Application.Abstraction;

namespace UserService.Application.CQRS;

public class HandlerBase
{
    protected IAppDbContext DbContext;

    public HandlerBase(IAppDbContext dbContext)
    {
        DbContext = dbContext;
    }
}