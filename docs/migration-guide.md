# Migration Guide from Moq

This guide helps you migrate from Moq to NimbleMock.

## Basic Setup

### Moq
```csharp
var mock = new Mock<IUserRepository>();
mock.Setup(x => x.GetById(1)).Returns(user);
var repo = mock.Object;
```

### NimbleMock
```csharp
var mock = Mock.Of<IUserRepository>()
    .Setup(x => x.GetById(1), user)
    .Build();
var repo = mock.Object;
```

## Async Methods

### Moq
```csharp
var mock = new Mock<IUserRepository>();
mock.Setup(x => x.SaveAsync(It.IsAny<User>())).ReturnsAsync(true);
var repo = mock.Object;
```

### NimbleMock
```csharp
var mock = Mock.Of<IUserRepository>()
    .SetupAsync(x => x.SaveAsync(default!), true)
    .Build();
var repo = mock.Object;
```

## Verification

### Moq
```csharp
mock.Verify(x => x.GetById(1), Times.Once);
mock.Verify(x => x.Delete(2), Times.Never);
```

### NimbleMock
```csharp
mock.Verify(x => x.GetById(1)).Once();
mock.Verify(x => x.Delete(2)).Never();
```

## Exceptions

### Moq
```csharp
var mock = new Mock<IService>();
mock.Setup(x => x.HealthCheck()).Throws(new TimeoutException());
var service = mock.Object;
```

### NimbleMock
```csharp
var mock = Mock.Of<IService>()
    .Throws(x => x.HealthCheck(), new TimeoutException("API down"))
    .Build();
var service = mock.Object;
```

## Benefits

- **Faster**: 34x faster than Moq in mock creation, 3x faster in verification
- **Less memory**: Zero allocations for typical scenarios
- **Better API**: Fluent, chainable syntax
- **Compile-time checks**: Analyzers catch errors early

