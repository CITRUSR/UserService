namespace UserService.API.Mappers;

public interface IMapper<TFrom, TOut>
{
    public TOut Map(TFrom from);
}
