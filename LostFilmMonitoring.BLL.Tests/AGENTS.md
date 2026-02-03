# Business Logic Tests

## Testing Framework

- NUnit + Moq + FluentAssertions
- All test classes marked `[ExcludeFromCodeCoverage]`

## Test Structure

```csharp
[ExcludeFromCodeCoverage]
internal class MyCommandTests
{
    private Mock<IDal> dal;
    private Mock<ILogger> logger;
    
    [SetUp]
    public void Setup()
    {
        dal = new Mock<IDal>();
        logger = new Mock<ILogger>();
        logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
    }
    
    [Test]
    public async Task ExecuteAsync_should_doSomething_when_condition()
    {
        // Arrange
        var command = new MyCommand(dal.Object, logger.Object);
        
        // Act
        var result = await command.ExecuteAsync(request);
        
        // Assert
        result.Should().NotBeNull();
        dal.Verify(x => x.Method(...), Times.Once);
    }
}
```

## Naming Convention

- Test method: `MethodName_should_expectedBehavior_when_condition`
- Always test constructor null checks
- Mock logger scope: `logger.Setup(l => l.CreateScope(...)).Returns(logger.Object)`

## Common Patterns

```csharp
// Verify DAO calls
dal.Verify(x => x.SaveAsync(It.IsAny<User>()), Times.Once);

// FluentAssertions
result.Should().NotBeNull();
result.UserId.Should().NotBeNullOrEmpty();
result.IsSuccess.Should().BeTrue();
```
