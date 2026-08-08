# SIMS.Tests — Hướng dẫn chạy test

Project test dùng **xUnit** + **FluentAssertions** + **Moq**, target `net8.0`.
Có 3 loại test nằm trong 3 thư mục tách biệt:

| Loại | Thư mục | Namespace | Số test | Cần gì để chạy |
|---|---|---|---|---|
| Unit | `Unit/` | `SIMS.Tests.Unit.*` | 28 | Không cần gì thêm |
| Integration | `Integration/` | `SIMS.Tests.Integration.*` | 2 | Không cần gì thêm (ghi CSV vào thư mục tạm) |
| UI (Selenium) | `Selenium/` | `SIMS.Tests.Selenium.*` | 3 | Chrome/Edge/Firefox + có mạng |
| | | **Tổng** | **33** | |

Chạy được bằng cả CLI (`dotnet test`, mục 2) và Test Explorer của Visual Studio (mục 3).

Toàn bộ lệnh CLI dưới đây chạy từ thư mục gốc repo (`d:\project\sims`).
Nếu bạn đang ở trong `SIMS-v2/`, thay đường dẫn thành `SIMS.Tests/SIMS.Tests.csproj`.

---

## 1. Chuẩn bị

```bash
dotnet restore SIMS-v2/SIMS-BackEnd.slnx
dotnet build   SIMS-v2/SIMS.Tests/SIMS.Tests.csproj
```

`dotnet test` tự restore và build nên bước này không bắt buộc, chỉ hữu ích khi
muốn tách lỗi biên dịch ra khỏi lỗi test.

---

## 2. Chạy test

### Chạy tất cả (kể cả UI — sẽ mở browser)

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj
```

### Chạy tất cả TRỪ UI — dùng thường ngày và cho CI

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category!=UI"
```

→ 30 test (28 unit + 2 integration). Không cần browser, không cần mạng, chạy dưới 1 giây.
Đây là lệnh nên dùng mặc định khi đang code.

### Chỉ chạy Unit test

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "FullyQualifiedName~SIMS.Tests.Unit"
```

→ 28 test.

> **Lưu ý quan trọng:** đừng dùng `--filter "Category=Unit"` để chạy unit test.
> Hiện chỉ có class `StudentRepositoryUnitTests` được gắn `[Trait("Category", "Unit")]`,
> nên filter đó chỉ ra **3 test** chứ không phải 28 — hai class `GradeServiceTests`
> và `StudentCodeAttributeTests` chưa có trait. Lọc theo namespace như trên mới đúng.
> Xem [mục 7](#7-quy-ước-khi-thêm-test-mới) nếu muốn sửa cho nhất quán.

### Chỉ chạy Integration test

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category=Integration"
```

→ 2 test. Đi qua đường dây thật `StudentService → StudentRepository → file .csv`,
không mock tầng nào. An toàn với dữ liệu thật: `CsvDataStoreFixture` tạo một thư mục
tạm riêng cho từng test (`%TEMP%/sims-itest-<guid>`) và ghi đè `DataStore:BasePath`
vào đó, `Dispose()` xoá sạch sau khi chạy. File trong `SIMS-BackEnd/Data` không bị chạm.

### Chỉ chạy UI test (Selenium)

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category=UI"
```

→ 3 test. Xem [mục 5](#5-cấu-hình-cho-ui-test-selenium) để đổi browser hoặc bật chế độ xem được.

### Chạy một class hoặc một test cụ thể

```bash
# Cả class
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj \
  --filter "FullyQualifiedName~StudentRepositoryUnitTests"

# Một test
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj \
  --filter "FullyQualifiedName~GradeServiceTests.Classify_WhenScoreIsAtOrAbove9_ShouldReturnDistinction"

# Nhiều điều kiện: dùng | cho OR, & cho AND
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj \
  --filter "Category=Unit|Category=Integration"
```

`~` là "chứa", `=` là "khớp chính xác", `!=` là "khác".

### Xem danh sách test mà không chạy

```bash
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --list-tests
```

---

## 3. Chạy bằng Test Explorer của Visual Studio

Không bắt buộc dùng CLI. Test Explorer chạy được toàn bộ 33 test, kể cả Selenium.

**Mở và chạy:**

1. Mở solution `SIMS-v2/SIMS-BackEnd.slnx` (đã có sẵn `SIMS.Tests` trong đó).
2. `Test` → `Test Explorer`.
3. Build solution một lần (`Ctrl+Shift+B`). Test chỉ hiện ra sau khi biên dịch xong.
4. Bấm `Run All`, hoặc chọn từng node rồi `Run`.

Project đã có sẵn `Microsoft.NET.Test.Sdk` và `xunit.runner.visualstudio` trong
`SIMS.Tests.csproj` — đây là hai package bắt buộc để Test Explorer khám phá được test.
Không cần cài extension gì thêm.

**Nên đổi Group By sang `Namespace`.**

Dropdown `Group By` trên thanh công cụ Test Explorer mặc định nhóm theo Class. Đổi sang
`Namespace` thì được đúng 3 nhóm khớp 3 loại test, rất tiện để chạy riêng từng loại:

```
SIMS.Tests.Unit.Grades              (22)
SIMS.Tests.Unit.Students            (3)
SIMS.Tests.Unit.Validation          (3)
SIMS.Tests.Integration.Students     (2)
SIMS.Tests.Selenium.Auth            (3)
```

Chọn node namespace → `Run` là chạy cả nhóm.

> Đừng dùng `Group By` → `Traits` để phân loại. Vì hiện chỉ `StudentRepositoryUnitTests`
> có trait, bạn sẽ thấy 30 test bị dồn vào nhóm `No Traits` và chỉ 3 test nằm dưới
> `Category [Unit]`. Đây cùng là vấn đề với `--filter "Category=Unit"` ở mục 2.
> Nhóm theo `Namespace` không bị ảnh hưởng vì cấu trúc thư mục đã đúng.

**Cẩn thận với `Run All`:** nó chạy cả 3 UI test, tức mở và đóng browser 3 lần và cần
mạng. Nếu chỉ muốn kiểm tra logic trong lúc code, đừng bấm `Run All`. Hai cách gọn hơn:

- Chọn các node `SIMS.Tests.Unit.*` và `SIMS.Tests.Integration.*` → `Run`.
- Hoặc tạo Playlist một lần rồi dùng lại: chọn các node đó → chuột phải →
  `Add to Playlist` → `New Playlist`, lưu lại thành file `.playlist`. Sau này chọn
  playlist từ dropdown là chạy đúng 30 test không-UI.

Ô search của Test Explorer cũng lọc được theo trait, ví dụ `Trait:"Category=UI"` để
khoanh riêng nhóm UI. Nhưng với tình trạng trait hiện tại thì lọc theo namespace
đáng tin hơn.

**Debug một test:** chuột phải vào test → `Debug`. Đặt breakpoint trong test hoặc trong
code nghiệp vụ đều dừng được. Đây là điểm Test Explorer hơn CLI rõ nhất.

Chạy trong IDE hay CLI đều dùng cùng một test runner nên kết quả giống nhau; CLI tiện
cho CI và cho việc lọc phức tạp, IDE tiện cho debug và chạy lặp lại một test.

---

## 4. Xem kết quả chi tiết hơn

```bash
# In tên từng test kèm kết quả
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category!=UI" --logger "console;verbosity=detailed"

# Xuất file .trx (dùng cho CI)
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --logger "trx;LogFileName=test-results.trx"

# Đo code coverage (coverlet.collector đã có sẵn trong csproj)
dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category!=UI" --collect:"XPlat Code Coverage"
```

File coverage `.cobertura.xml` nằm trong `TestResults/<guid>/`.

---

## 5. Cấu hình cho UI test (Selenium)

Cấu hình đọc từ `Selenium/selenium.settings.json` (được copy ra thư mục output khi build):

```json
{
  "Ui": {
    "BaseUrl": "https://practicetestautomation.com",
    "LoginPath": "/practice-test-login/",
    "Browser": "chrome",
    "Headless": true,
    "TimeoutSeconds": 15,
    "ValidUsername": "student",
    "ValidPassword": "Password123",
    "SlowMoMs": 0,
    "TypingDelayMs": 0
  }
}
```

Mọi khoá đều ghi đè được bằng biến môi trường theo dạng `Ui__<Tên>` (hai dấu gạch dưới),
tiện khi không muốn sửa file:

```bash
# Xem browser chạy thật thay vì headless, chậm lại để quan sát
Ui__Headless=false Ui__SlowMoMs=500 Ui__TypingDelayMs=80 \
  dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category=UI"

# Đổi sang Edge hoặc Firefox
Ui__Browser=edge dotnet test SIMS-v2/SIMS.Tests/SIMS.Tests.csproj --filter "Category=UI"
```

Trên PowerShell dùng `$env:Ui__Headless="false"` trước khi gọi `dotnet test`.

Browser hỗ trợ: `chrome`, `edge`, `firefox`. Không cần tải driver thủ công —
Selenium Manager (có từ Selenium 4.6+) tự lo. Nhưng browser tương ứng phải được
cài trên máy.

**Hai điều cần biết về bộ UI test này:**

1. Nó bắn vào site ngoài `practicetestautomation.com`, không phải vào app SIMS.
   Nên nó phụ thuộc mạng và phụ thuộc việc site đó còn sống — không hermetic.
   Đừng để nó chặn CI; luôn tách bằng `Category!=UI`.
2. xUnit tạo instance mới cho mỗi `[Fact]`, mà constructor của `LoginUiTests` mở
   một WebDriver. Nên chạy 3 test = mở và đóng browser 3 lần.

---

## 6. Cấu trúc thư mục

```
SIMS.Tests/
├── Unit/                                  # Test thuần, mock hết dependency
│   ├── Grades/GradeClassifyTests.cs           # GradeService: nộp bài, nhập/sửa điểm, xếp loại (22 test)
│   ├── Students/StudentRepositoryUnitTests.cs # TC4/TC5/TC6 trên in-memory repo (3 test)
│   ├── Support/InMemoryStudentRepository.cs   # Test double, không phải test
│   └── Validation/StudentCodeAttributeTests.cs# StudentCodeAttribute (3 test)
├── Integration/                           # Chạy thật qua DI container + file CSV
│   ├── CsvDataStoreFixture.cs                 # Fixture thư mục tạm, không phải test
│   └── Students/StudentCsvPersistenceTests.cs # TC7/TC8 ghi–xoá CSV vật lý (2 test)
└── Selenium/                              # End-to-end qua browser
    ├── Auth/LoginUiTests.cs                   # 3 kịch bản login (3 test)
    ├── Pages/LoginPage.cs                     # Page Object
    ├── Support/                               # UiTestConfig, WebDriverFactory
    └── selenium.settings.json
```

---

## 7. Quy ước khi thêm test mới

- Đặt file vào đúng một trong ba thư mục `Unit/`, `Integration/`, `Selenium/`,
  và cho namespace khớp đường dẫn (`SIMS.Tests.Unit.Students`, ...).
- Gắn trait cho class để filter hoạt động đúng:

  ```csharp
  [Trait("Category", "Unit")]          // hoặc "Integration", hoặc "UI"
  public class MyTests { }
  ```

- Nếu muốn `--filter "Category=Unit"` chạy đủ 28 test, thêm dòng trait trên vào
  `GradeServiceTests` và `StudentCodeAttributeTests` — hiện hai class đó còn thiếu.
  Sau khi thêm thì `Category=Unit` và filter theo namespace sẽ cho cùng kết quả.
- Test cần đọc/ghi file thì dùng `CsvDataStoreFixture` thay vì trỏ vào
  `SIMS-BackEnd/Data`, để không làm bẩn dữ liệu thật và để các test độc lập với nhau.

---

## 8. Xử lý sự cố

| Hiện tượng | Nguyên nhân thường gặp |
|---|---|
| `Category=Unit` chỉ ra 3 test | Thiếu trait ở 2 class, xem mục 2, 3 và 7 |
| Test Explorer không hiện test nào | Chưa build solution; build xong test mới được khám phá |
| Test Explorer hiện 30 test trong nhóm `No Traits` | Đúng như hiện trạng, không phải lỗi — nhóm theo `Namespace` thay vì `Traits`, xem mục 3 |
| `Run All` trong IDE bỗng mở browser | Đang chạy cả 3 UI test; dùng playlist hoặc chọn riêng node `Unit`/`Integration`, xem mục 3 |
| UI test lỗi `NoSuchDriverException` / không mở được browser | Chưa cài browser tương ứng, hoặc máy không ra được internet để Selenium Manager tải driver |
| UI test timeout | Site ngoài chậm hoặc không vào được; thử tăng `Ui__TimeoutSeconds=30` |
| Integration test lỗi ghi file | Không có quyền ghi vào `%TEMP%` |
| Build lại không thấy đổi `selenium.settings.json` | File được copy theo `PreserveNewest`; chạy `dotnet build` lại hoặc `dotnet clean` |
| Muốn biết vì sao một test fail | CLI: thêm `--logger "console;verbosity=detailed"`. IDE: bấm test rồi xem ô chi tiết bên phải |
