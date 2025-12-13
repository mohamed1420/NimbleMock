using NimbleMock;

namespace NimbleMock.Examples;

public interface IUserRepository
{
    User GetById(int id);
    Task<bool> SaveAsync(User user);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class BasicUsage
{
    public static void Example()
    {
        // Create a mock
        var mock = Mock.Of<IUserRepository>()
            .Setup(x => x.GetById(1), new User { Id = 1, Name = "Alice" })
            .SetupAsync(x => x.SaveAsync(default!), true)
            .Build();

        // Use the mock
        var user = mock.Object.GetById(1);
        Console.WriteLine($"User: {user.Name}");

        // Verify calls
        mock.Verify(x => x.GetById(1)).Once();
    }
}

