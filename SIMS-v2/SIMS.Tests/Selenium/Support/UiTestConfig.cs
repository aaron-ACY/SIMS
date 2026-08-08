using Microsoft.Extensions.Configuration;

namespace SIMS.Tests.Selenium.Support;

/// <summary>
/// Cấu hình cho bộ Selenium test. Đọc từ selenium.settings.json,
/// cho phép ghi đè bằng biến môi trường dạng Ui__BaseUrl, Ui__Headless...
/// </summary>
public sealed class UiTestConfig
{
    public string BaseUrl { get; init; } = "https://practicetestautomation.com";
    public string LoginPath { get; init; } = "/practice-test-login/";
    public string Browser { get; init; } = "chrome";
    public bool Headless { get; init; } = true;
    public int TimeoutSeconds { get; init; } = 15;
    public string ValidUsername { get; init; } = "student";
    public string ValidPassword { get; init; } = "Password123";

    /// <summary>Delay (ms) sau mỗi hành động, để quan sát bằng mắt. 0 = chạy full speed.</summary>
    public int SlowMoMs { get; init; }

    /// <summary>Delay (ms) giữa mỗi ký tự khi nhập text. 0 = nhập một lần.</summary>
    public int TypingDelayMs { get; init; }

    public string LoginUrl => $"{BaseUrl.TrimEnd('/')}/{LoginPath.TrimStart('/')}";

    public static UiTestConfig Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("selenium.settings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        return configuration.GetSection("Ui").Get<UiTestConfig>() ?? new UiTestConfig();
    }
}
