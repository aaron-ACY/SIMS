using FluentAssertions;
using OpenQA.Selenium;
using SIMS.Tests.Selenium.Pages;
using SIMS.Tests.Selenium.Support;
using Xunit;

namespace SIMS.Tests.Selenium.Auth;

[Trait("Category", "UI")]
public sealed class LoginUiTests : IDisposable
{
    private readonly UiTestConfig _config = UiTestConfig.Load();
    private readonly IWebDriver _driver;
    private readonly LoginPage _loginPage;

    public LoginUiTests()
    {
        _driver = WebDriverFactory.Create(_config);
        _loginPage = new LoginPage(_driver, _config);
    }

    [Fact]
    public void AUTH_LOGIN_001_ValidCredentials()
    {
        // Arrange
        _loginPage.Open();

        // Act
        _loginPage.Login(_config.ValidUsername, _config.ValidPassword);

        // Assert
        _loginPage.WaitUntilLoggedIn().Should().BeTrue(
            "After successfully logging in, the user must be redirected to the 'login successful' page.");
        _loginPage.CurrentUrl.Should().Contain("logged-in-successfully");
        _loginPage.SuccessHeaderText.Should().Contain("Logged In Successfully");
    }

    [Fact]
    public void AUTH_LOGIN_002_InvalidUsername()
    {
        // Arrange
        _loginPage.Open();

        // Act
        _loginPage.Login("abc", _config.ValidPassword);

        // Assert
        _loginPage.WaitForErrorMessage()
                  .Should().NotBeNull()
                  .And.Subject.As<string>()
                  .Should().Contain("Your username is invalid!");
        _loginPage.CurrentUrl.Should().NotContain("logged-in-successfully");
    }

    [Fact]
    public void AUTH_LOGIN_003_InvalidPassword()
    {
        // Arrange
        _loginPage.Open();

        // Act
        _loginPage.Login(_config.ValidUsername, "wrongPass9");

        // Assert
        _loginPage.WaitForErrorMessage()
                  .Should().NotBeNull()
                  .And.Subject.As<string>()
                  .Should().Contain("Your password is invalid!");
        _loginPage.CurrentUrl.Should().NotContain("logged-in-successfully");
    }

    public void Dispose()
    {
        if (_config.SlowMoMs > 0) Thread.Sleep(_config.SlowMoMs * 2);

        _driver.Quit();
        _driver.Dispose();
    }
}
