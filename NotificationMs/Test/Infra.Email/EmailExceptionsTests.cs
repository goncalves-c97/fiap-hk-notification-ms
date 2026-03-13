using Infra.Email.Exceptions;

namespace Test.Infra.Email;

public class EmailExceptionsTests
{
    [Fact]
    public void EmailException_Constructors_ShouldPreserveMessageAndInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var empty = new EmailException();
        var withMessage = new EmailException("message");
        var withInner = new EmailException("message", inner);

        Assert.NotNull(empty);
        Assert.Equal("message", withMessage.Message);
        Assert.Equal("message", withInner.Message);
        Assert.Same(inner, withInner.InnerException);
    }

    [Fact]
    public void EmailBuildException_Constructors_ShouldPreserveMessageAndInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var empty = new EmailBuildException();
        var withMessage = new EmailBuildException("message");
        var withInner = new EmailBuildException("message", inner);

        Assert.IsAssignableFrom<EmailException>(empty);
        Assert.Equal("message", withMessage.Message);
        Assert.Equal("message", withInner.Message);
        Assert.Same(inner, withInner.InnerException);
    }

    [Fact]
    public void EmailFailureException_Constructors_ShouldPreserveMessageAndInnerException()
    {
        var inner = new InvalidOperationException("inner");

        var empty = new EmailFailureException();
        var withMessage = new EmailFailureException("message");
        var withInner = new EmailFailureException("message", inner);

        Assert.IsAssignableFrom<EmailException>(empty);
        Assert.Equal("message", withMessage.Message);
        Assert.Equal("message", withInner.Message);
        Assert.Same(inner, withInner.InnerException);
    }
}
