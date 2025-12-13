namespace NimbleMock.Tests.Fixtures;

public interface IUserRepository
{
    User GetById(int id);
    Task<bool> SaveAsync(User user);
    Task<List<User>> GetAllAsync();
    void Delete(int id);
}

public interface IEmailService
{
    Task<bool> SendAsync(string to, string subject, string body);
    Task<bool> SendBulkAsync(List<string> recipients, string subject);
}

public interface IExternalApi
{
    Task<ApiResponse> FetchDataAsync(string endpoint);
    void HealthCheck();
}

public interface IAsyncRepository
{
    ValueTask<User> GetAsync(int id);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class ApiResponse
{
    public string Data { get; set; } = "";
    public int StatusCode { get; set; }
}

