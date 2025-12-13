using System;

namespace NimbleMock.Exceptions;

/// <summary>
/// Exception thrown when mock verification fails.
/// </summary>
public sealed class VerificationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerificationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public VerificationException(string message) : base(message) { }
}

