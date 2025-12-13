namespace NimbleMock.Benchmarks;

public interface IUserRepository
{
    User GetById(int id);
    Task<bool> SaveAsync(User user);
    Task<List<User>> GetAllAsync();
    void Delete(int id);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

