namespace UserService.Application;

public interface IAppDbContext
{
    public Task<int> SaveChangesAsync(CancellationToken token);
}