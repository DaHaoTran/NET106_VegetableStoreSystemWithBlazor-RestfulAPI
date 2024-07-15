namespace API.Services.Interfaces
{
    public interface ILookupSvc<Key, Result>
    {
        Task<Result> GetDataByKey(Key key);
    }
}
