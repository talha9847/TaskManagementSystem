namespace MyApp.Namespace;

public interface ILoginInterface{
    User GetUserByEmail(string email, string password);
    Task<int> GetAdmin(string email, string password);
}