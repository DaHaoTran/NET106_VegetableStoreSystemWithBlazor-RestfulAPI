namespace API.Services.Interfaces
{
    public interface IReadable<Type>
    {
        Task<IEnumerable<Type>> ReadDatas();
    }
    public interface IAddable<Type>
    {
        Task<Type> AddNewData(Type entity);
    }
    public interface IEditable<Type>
    {
        Task<Type> EditData(Type entity);
    }
    public interface IDeletable<Type, Entity>
    {
        Task<string> DeleteData(Type key);
    }
}
