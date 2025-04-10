
namespace MyApp.Namespace;
public interface IUserInterface
{
    public User GetEmpById(int userId);
    // Task<User> GetEmpByIdAsync(int loggedInEmpId);
    Task<bool> UpdatePasswordAsync(User user);
    void UpdateUser(User user);
}