using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SIMS.Tests.Selenium.Support;

namespace SIMS.Tests.Selenium.Pages;

/// <summary>
/// Page Object cho trang login của practicetestautomation.com.
/// Locator gom hết vào đây, nếu site đổi markup chỉ cần sửa ở một chỗ.
/// </summary>
public sealed class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly UiTestConfig _config;
    private readonly WebDriverWait _wait;

    // ── Locators ───────────────────────────────────────────────────────── //
    private static readonly By UsernameInput = By.Id("username");
    private static readonly By PasswordInput = By.Id("password");
    private static readonly By SubmitButton  = By.Id("submit");
    private static readonly By ErrorMessage  = By.Id("error");
    private static readonly By SuccessHeader = By.CssSelector("h1.post-title");

    public LoginPage(IWebDriver driver, UiTestConfig config)
    {
        _driver = driver;
        _config = config;
        _wait   = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TimeoutSeconds));
    }

    public LoginPage Open()
    {
        _driver.Navigate().GoToUrl(_config.LoginUrl);
        _wait.Until(d => d.FindElement(UsernameInput).Displayed);
        Pause();
        return this;
    }

    public LoginPage Login(string username, string password)
    {
        var usernameField = _wait.Until(d => d.FindElement(UsernameInput));
        usernameField.Clear();
        Type(usernameField, username);
        Pause();

        var passwordField = _driver.FindElement(PasswordInput);
        passwordField.Clear();
        Type(passwordField, password);
        Pause();

        _driver.FindElement(SubmitButton).Click();
        Pause();
        return this;
    }

    /// <summary>Nhập text, gõ từng ký tự nếu TypingDelayMs > 0 để nhìn thấy được.</summary>
    private void Type(IWebElement field, string text)
    {
        if (_config.TypingDelayMs <= 0)
        {
            field.SendKeys(text);
            return;
        }

        foreach (var character in text)
        {
            field.SendKeys(character.ToString());
            Thread.Sleep(_config.TypingDelayMs);
        }
    }

    private void Pause()
    {
        if (_config.SlowMoMs > 0) Thread.Sleep(_config.SlowMoMs);
    }

    /// <summary>Chờ tới khi điều hướng sang trang đăng nhập thành công.</summary>
    public bool WaitUntilLoggedIn()
    {
        try
        {
            return _wait.Until(d => d.Url.Contains("logged-in-successfully", StringComparison.OrdinalIgnoreCase));
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public string SuccessHeaderText =>
        _wait.Until(d => d.FindElement(SuccessHeader)).Text;

    /// <summary>Text của khối thông báo lỗi, trả về null nếu không xuất hiện.</summary>
    public string? WaitForErrorMessage()
    {
        try
        {
            var element = _wait.Until(d =>
            {
                var found = d.FindElements(ErrorMessage).FirstOrDefault();
                return found is { Displayed: true } ? found : null;
            });
            return element?.Text.Trim();
        }
        catch (WebDriverTimeoutException)
        {
            return null;
        }
    }

    public string CurrentUrl => _driver.Url;
}
