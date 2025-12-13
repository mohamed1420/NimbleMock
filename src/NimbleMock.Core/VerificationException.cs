using System;

namespace NimbleMock;

public sealed class VerificationException : Exception
{
    public VerificationException(string message) : base(message) { }
}

