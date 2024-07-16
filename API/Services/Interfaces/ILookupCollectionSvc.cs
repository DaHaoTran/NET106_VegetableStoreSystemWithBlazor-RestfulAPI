namespace API.Services.Interfaces
{
    public interface ILookupSvc<Key, Result>
    {
        Task<Result> GetDataByKey(Key key);
    }
    public interface ILookupMoreSvc<Key, Result>
    {
        Task<IEnumerable<Result>> GetListByKey(Key key);
    }
}
