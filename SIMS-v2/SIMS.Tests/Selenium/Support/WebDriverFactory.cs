using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace SIMS.Tests.Selenium.Support;

/// <summary>
/// Tạo WebDriver theo cấu hình. Selenium Manager (đi kèm Selenium 4.6+)
/// tự tải driver tương ứng nên không cần khai báo đường dẫn driver.
/// </summary>
public static class WebDriverFactory
{
    public static IWebDriver Create(UiTestConfig config)
    {
        IWebDriver driver = config.Browser.ToLowerInvariant() switch
        {
            "chrome"  => new ChromeDriver(BuildChromeOptions(config.Headless)),
            "edge"    => new EdgeDriver(BuildEdgeOptions(config.Headless)),
            "firefox" => new FirefoxDriver(BuildFirefoxOptions(config.Headless)),
            _ => throw new NotSupportedException(
                     $"Browser '{config.Browser}' chưa được hỗ trợ. Dùng chrome, edge hoặc firefox.")
        };

        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(config.TimeoutSeconds * 2);
        return driver;
    }

    private static ChromeOptions BuildChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        if (headless) options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        return options;
    }

    private static EdgeOptions BuildEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();
        if (headless) options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1920,1080");
        return options;
    }

    private static FirefoxOptions BuildFirefoxOptions(bool headless)
    {
        var options = new FirefoxOptions();
        if (headless) options.AddArgument("-headless");
        return options;
    }
}
