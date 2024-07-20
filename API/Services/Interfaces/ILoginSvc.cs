namespace API.Services.Interfaces
{
    public interface ILoginSvc<T>
    {
        Task<bool> Login(T entity);
        Task<bool> Logout(string email);
    }
}
