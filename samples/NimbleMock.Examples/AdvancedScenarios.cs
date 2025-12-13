using NimbleMock;
using NimbleMock.Exceptions;

namespace NimbleMock.Examples;

public class AdvancedScenarios
{
    public static void PartialMockExample()
    {
        // Partial mock - only mock specific methods
        var mock = Mock.Partial<IUserRepository>()
            .Only(x => x.GetById(1), new User { Id = 1 })
            .Build();

        var user = mock.Object.GetById(1); // Works
        // mock.Object.Delete(1); // Would throw NotImplementedException
    }

    public static void ExceptionHandlingExample()
    {
        var mock = Mock.Of<IExternalApi>()
            .Throws(x => x.HealthCheck(), new TimeoutException("API down"))
            .Build();

        try
        {
            mock.Object.HealthCheck();
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Caught: {ex.Message}");
        }
    }

    public static void ArgumentVerificationExample()
    {
        var mock = Mock.Of<IEmailService>()
            .SetupAsync(x => x.SendAsync(default!, default!, default!), true)
            .Build();

        mock.Object.SendAsync("user@test.com", "Hello", "World").Wait();

        mock.Verify(x => x.SendAsync(default!, default!, default!))
            .WithArg<string>(0)
            .Matching(email => email.Contains("@"));
    }
}

public interface IExternalApi
{
    void HealthCheck();
}

public interface IEmailService
{
    Task<bool> SendAsync(string to, string subject, string body);
}

