namespace MyApp.Namespace;
public interface IRegisterInterface
{
    Task<int> Add(User user);
    Task<bool> CheckIfEmailExists(string email);
}