# Báo Cáo Đánh Giá Toàn Diện — SIMS-BackEnd

> **Người đánh giá:** Senior Software Architect · Principal .NET Engineer · Security Reviewer
> **Ngày đánh giá:** 31/07/2026
> **Nhánh được đánh giá:** `main`
> **Commit gần nhất:** `92ce248`

---

## Tổng Quan Dự Án

| Thông tin | Giá trị |
|---|---|
| Framework | .NET 8, ASP.NET Core |
| Kiến trúc | Clean Architecture (5 layer + tests) |
| Lưu trữ dữ liệu | CSV flat files qua CsvHelper (không có database) |
| Xác thực | JWT HS256, mật khẩu PBKDF2-SHA256, thu hồi token theo JTI |
| CQRS | Chưa triển khai — sử dụng service layer truyền thống |
| DI Container | Microsoft.Extensions.DependencyInjection |
| Bộ kiểm thử | xUnit + Moq + FluentAssertions |

### Sơ đồ phụ thuộc dự án

```
SIMS.API → SIMS.Application → SIMS.Domain ← SIMS.Shared
SIMS.API → SIMS.Infrastructure → SIMS.Application
                               → SIMS.Domain
                               → SIMS.Shared
SIMS.Tests → SIMS.Application → SIMS.Domain → SIMS.Shared
```

---

## 1. Đánh Giá Kiến Trúc

### 1.1 Tuân thủ Clean Architecture

Quy tắc phụ thuộc (dependency rule) được tuân thủ đúng: các lớp bên trong không phụ thuộc vào lớp bên ngoài. Infrastructure và API không được Domain biết đến. Đây là điểm tốt nhất của dự án.

### 1.2 Vi phạm kiến trúc phát hiện được

**[Vi phạm 1] — Application layer phụ thuộc vào `Microsoft.Extensions.Options`**

`AuthService` nhận `IOptions<TokenPolicy>` qua constructor injection. Điều này tạo ra sự phụ thuộc vào hosting framework ngay trong Application layer — vi phạm nguyên tắc Clean Architecture.

```csharp
// Hiện tại (vi phạm)
public AuthService(IOptions<TokenPolicy> tokenPolicy, ...)

// Đúng hơn — inject record trực tiếp, giải quyết tại composition root
public AuthService(TokenPolicy tokenPolicy, ...)
```

**[Vi phạm 2] — Không có Unit of Work**

Các thao tác ghi đụng chạm nhiều repository (ví dụ: ghi enrollment + cập nhật số lượng lớp) không được bọc trong transaction. Không có `IUnitOfWork` nào được định nghĩa.

### 1.3 CQRS

CQRS chưa được triển khai. Dự án dùng service layer truyền thống. MediatR vắng mặt. Nếu dự án có kế hoạch mở rộng, đây là khu vực cần cải thiện đầu tiên.

### 1.4 Repository Pattern

Mỗi aggregate có một repository interface riêng trong Application và một implementation trong Infrastructure. `CsvRepositoryBase<T>` cung cấp logic I/O dùng chung. Pattern nhất quán, rõ ràng.

### 1.5 Dependency Injection

- Repositories: **Singleton** — đúng (chia sẻ semaphore, file handle nhất quán).
- Services: **Scoped** — đúng.
- `RolePermissionRepository` được đăng ký cả kiểu cụ thể lẫn interface trỏ về cùng instance — đảm bảo đúng invariant single-semaphore.

### 1.6 Cấu trúc thư mục

| Layer | Nhận xét |
|---|---|
| `SIMS.Application/Services/` | Services nằm phẳng, không có thư mục con theo domain |
| `SIMS.Application/DTOs/` | Tốt — có thư mục con theo domain |
| `SIMS.API/Controllers/` | Phẳng, chấp nhận được ở quy mô này |
| `SIMS.Infrastructure/Persistence/` | Có `Base/`, rõ ràng |
| `SIMS.Domain/Entities/` | Phẳng, chấp nhận được |

### 1.7 Tính nhất quán đặt tên

Nhìn chung nhất quán: `PascalCase` cho kiểu, `_camelCase` cho trường private, hậu tố `Async` trên mọi phương thức bất đồng bộ. Một điểm không nhất quán: file `GradeClassifyTests.cs` nhưng class bên trong là `GradeServiceTests`.

### 1.8 SOLID

| Nguyên tắc | Trạng thái | Vấn đề |
|---|---|---|
| SRP | ⚠️ | `AuthService` xử lý đăng nhập, refresh, logout, quản lý revocation |
| OCP | ✅ | Service được giao tiếp qua interface |
| LSP | ✅ | Không có hierarchy kế thừa vi phạm |
| ISP | ⚠️ | Các interface lớn nhưng hợp lý |
| DIP | ✅ | Mọi phụ thuộc đều qua interface |

---

## 2. Đánh Giá Domain

### 2.1 Entities

Tất cả entities trong `SIMS.Domain/Entities/` là POCO thuần, có `int Id` làm primary key, không có navigation properties. Phù hợp với kho lưu trữ CSV.

**Thiếu đóng gói:** Mọi property đều là `public { get; set; }`. Trong DDD nghiêm ngặt, ít nhất các điểm mutation nên được kiểm soát qua phương thức. Đây là điểm có thể cải thiện về sau.

`User.FullName` là computed property (`$"{FirstName} {LastName}"`) được đặt đúng chỗ trong entity — không rò rỉ ra service.

### 2.2 Value Objects

**Không có Value Object nào.** `StudentCode`, `InstructorCode`, email, điểm số đều là primitive string/decimal. Đây là ứng viên tự nhiên cho Value Object trong DDD nghiêm ngặt.

Ví dụ cải thiện:
```csharp
public sealed record StudentCode
{
    private static readonly Regex Format = new(@"^BD\d{5}$", RegexOptions.Compiled);
    public string Value { get; }
    public StudentCode(string value)
    {
        if (!Format.IsMatch(value ?? ""))
            throw new AppException(ErrorCode.INVALID_STUDENT_CODE);
        Value = value;
    }
}
```

### 2.3 Domain Events

Không có cơ sở hạ tầng Domain Event. Điều này giới hạn khả năng audit và mở rộng.

### 2.4 Business Logic rò rỉ ra ngoài Domain

| Quy tắc nghiệp vụ | Vị trí hiện tại | Nên ở |
|---|---|---|
| Định dạng mã sinh viên | DataAnnotation trong Application | Domain Value Object |
| Phân loại điểm (Classify) | `GradeService` (Application) | Domain entity/service |
| Kiểm tra sức chứa lớp học | `ClassService` (Application) | Domain entity method |
| Tính duy nhất mã code | Service (Application) | Repository hoặc Application |

`GradeService.Classify()` là pure function không có dependency — thuộc về entity `Grade` hoặc Domain Service, không phải Application Service.

### 2.5 Enums

Không có enum nào được sử dụng. Roles và permissions là string constant — đây là lựa chọn có chủ đích (CSV storage làm enum bất tiện) nhưng đánh đổi type-safety tại compile time.

---

## 3. Đánh Giá Application Layer

### 3.1 Services

Tất cả services theo cùng một cấu trúc: constructor injection các repository interface, phương thức async trả DTO, ném exception qua `AppException`. Nhất quán và dễ đọc.

**SRP vi phạm — `AuthService` làm quá nhiều việc:**

```
AuthService
  ├── LoginAsync         — xác thực thông tin đăng nhập
  ├── RefreshTokenAsync  — luân chuyển token
  ├── LogoutAsync        — quản lý revocation list
  ├── BuildAccessToken   — xây dựng JWT (delegate đến ITokenService ✅)
  └── RevokeTokenAsync   — ghi revocation
```

Nên tách `TokenRevocationService` riêng để xử lý logout và revocation.

**Logic mapping trùng lặp** — mỗi service có một method `Map()` private static riêng:
```csharp
// StudentService.cs
private static StudentResponse Map(Student student, ...) { ... }
// InstructorService.cs
private static InstructorResponse Map(Instructor instructor, ...) { ... }
```
Vì Mapster đã có trong `.csproj` (dù chưa dùng), đây là cơ hội thay thế bằng mapping cấu hình.

### 3.2 DTOs

DTOs được tổ chức tốt theo domain và chiều Request/Response. Không có DTO nào bị dùng chung sai ngữ cảnh.

`ExpiredTokenPrincipal` record là thiết kế tốt — cố tình chỉ chứa `UserId`, `Jti`, `ExpiresAt` để buộc đường refresh re-read permissions từ store.

**Thiếu:** `LoginResponse` không có `ExpiresAt`. Client phải tự decode JWT để biết thời hạn token.

### 3.3 Validators

Validation dùng `DataAnnotations` với custom attributes `StudentCodeAttribute` và `InstructorCodeAttribute`. Regex được viết đúng.

**Lỗ hổng validation:** `UpdateStudentRequest.StudentCode` là `string?` không có `[Required]`. `StudentCodeAttribute` deferTo `[Required]` khi null/empty nhưng `[Required]` không được khai báo, nên gửi `""` vẫn pass validation.

```csharp
// Thiếu [Required]
[StudentCode]
public string? StudentCode { get; set; }

// Đúng
[Required]
[StudentCode]
public string StudentCode { get; set; } = string.Empty;
```

### 3.4 Interfaces

Repository interfaces trong `SIMS.Application/Interfaces/Repositories/` được tách đúng theo từng aggregate. Service interfaces phản chiếu implementation 1:1 — đúng.

**Dead code:** `IInstructorRepository` chỉ có `GetAllAsync` nhưng DTOs `CreateInstructorRequest`/`UpdateInstructorRequest` tồn tại không được dùng đến.

### 3.5 Settings

`TokenPolicy` trong `SIMS.Application/Settings/` là cách tách biệt tốt — Application layer có view riêng về JWT config, không phụ thuộc trực tiếp vào `JwtSettings` của Infrastructure.

---

## 4. Đánh Giá Infrastructure

### 4.1 CsvRepositoryBase — Lỗi đồng thời nghiêm trọng

**Vấn đề:** Semaphore được giải phóng giữa lần đọc và ghi:

```csharp
// Pattern hiện tại trong mọi repository
public async Task<T> AddAsync(T entity)
{
    var records = await ReadAllAsync();   // giữ + giải phóng semaphore
    // ← tại đây thread khác có thể đọc cùng dữ liệu cũ
    records.Add(entity);
    await WriteAllAsync(records);         // giữ + giải phóng semaphore
    return entity;
}
```

Kết quả: hai request đồng thời có thể ghi đè lên nhau, **mất dữ liệu im lặng**.

**Giải pháp — bọc trong một lần giữ semaphore:**

```csharp
protected async Task<T> ReadModifyWriteAsync(Func<List<T>, T> mutate)
{
    await _semaphore.WaitAsync();
    try
    {
        var records = ReadFromDisk();   // sync
        var result  = mutate(records);
        WriteToDisk(records);           // sync
        return result;
    }
    finally
    {
        _semaphore.Release();
    }
}
```

### 4.2 ID Assignment — Lỗi với danh sách rỗng

```csharp
// Ném InvalidOperationException nếu records rỗng
int nextId = records.Max(r => r.Id) + 1;

// Đúng
int nextId = records.Count == 0 ? 1 : records.Max(r => r.Id) + 1;
```

### 4.3 JwtTokenService

`ValidateIgnoringLifetime` chính xác:
- Tắt lifetime validation
- Validate issuer, audience, chữ ký
- Chặn `alg:none` bằng kiểm tra string

```csharp
if (jwt.Header.Alg != SecurityAlgorithms.HmacSha256)
    throw new SecurityTokenException("Invalid algorithm.");
```

Đây là best practice tốt.

**Vấn đề:** Xóa `typ` header là không chuẩn (RFC 7515 recommends `"JWT"`). Một số thư viện JWT bên thứ ba có thể từ chối token không có `typ`.

### 4.4 Pbkdf2PasswordHasher

100.000 iterations (tuân thủ NIST), 16-byte salt ngẫu nhiên, 32-byte derived key, SHA-256, so sánh constant-time. **Được triển khai đúng.**

### 4.5 DependencyInjection.cs

Giải quyết `BasePath` tương đối so với `contentRootPath` — ghi dữ liệu vào `SIMS-BackEnd/Data/` thay vì `bin/Debug/`. Đây là thiết kế đúng đắn.

**Fallback unreachable:** `DataStoreSettings.ResolvePath()` có nhánh fallback dùng `AppContext.BaseDirectory` nhưng nhánh này không bao giờ được thực thi vì `AddInfrastructure` luôn resolve path thành tuyệt đối trước đó. Comment trong code mô tả sai hành vi thực tế.

### 4.6 EnrollmentRepository — Soft Delete

`EnrollmentRepository` thực hiện soft-delete (`IsActive = false`) đúng cách — phù hợp với yêu cầu theo dõi lịch sử ghi danh.

### 4.7 Orphaned Data khi xóa

`UserService.DeleteAsync` xóa cứng bản ghi user nhưng không xóa `Student` hay `Instructor` liên quan (còn `UserId` treo). `SubjectRepository.DeleteAsync` và `CourseRepository.DeleteAsync` xóa bản ghi nhưng không kiểm tra các `Class` row đang tham chiếu đến chúng. Kết quả: dữ liệu mồ côi với trường rỗng trong các lần đọc sau.

---

## 5. Đánh Giá API

### 5.1 Controllers

Tất cả controllers theo pattern nhất quán: inject service interface, gọi method, bọc kết quả trong `ApiResponse.Success(...)` hoặc ném `AppException`. Sạch và dễ đọc.

**Vấn đề nghiêm trọng — Không có `[Authorize]` mặc định ở class level:**

Hiện tại authorization chỉ được áp dụng per-action. Bất kỳ action mới nào được thêm vào mà quên `[Authorize(Policy = ...)]` sẽ là public endpoint.

```csharp
// Hiện tại — không an toàn
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase { ... }

// Đúng — deny-by-default
[ApiController]
[Route("api/[controller]")]
[Authorize]  // ← thêm vào mọi controller
public class StudentsController : ControllerBase { ... }
```

Sau đó áp dụng `[AllowAnonymous]` tường minh trên ba action public trong `AuthController`.

### 5.2 REST API Design

HTTP semantics nhìn chung đúng:

| Endpoint | Method | Mã trạng thái | Đánh giá |
|---|---|---|---|
| `POST /api/auth/login` | POST | 200 | ✅ |
| `GET /api/students` | GET | 200 | ✅ |
| `POST /api/students` | POST | 200 | ⚠️ Nên trả 201 Created |
| `PUT /api/students/{id}` | PUT | 200 | ✅ |
| `DELETE /api/students/{id}` | DELETE | 200 | ⚠️ Nên trả 204 No Content |
| `POST /api/classes/{id}/enrollments` | POST | 200 | ⚠️ Nên trả 201 Created |

### 5.3 Versioning

Không có API versioning. Khi API mở rộng, thêm prefix `v1` sớm tránh breaking changes.

```csharp
// Thêm Asp.Versioning.Http vào csproj
builder.Services.AddApiVersioning(o => {
    o.DefaultApiVersion = new ApiVersion(1);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});
```

### 5.4 Response Consistency

`ApiResponse<T>` được dùng nhất quán trên tất cả controllers, error paths, `InvalidModelStateResponseFactory`, và `GlobalExceptionHandler`. Đây là **điểm mạnh nhất** của dự án.

### 5.5 Swagger

Swagger được cấu hình với JWT bearer scheme. Security requirement được áp dụng đúng. Cho phép test authenticated endpoints trực tiếp từ Swagger UI.

### 5.6 Exception Handling

`GlobalExceptionHandler.cs` xử lý `AppException` → structured error response, mọi exception khác → 500 với `UNCATEGORIZED_EXCEPTION`. Stack trace không bị lộ ra client. Thiết kế đúng.

---

## 6. Đánh Giá Bảo Mật

### 🔴 CRITICAL

#### SEC-01 — JWT Secret Key lưu trong source control

**File:** `SIMS-BackEnd/appsettings.json` dòng 10

```json
"SecretKey": "3hdxZVEZaR8jtZSFqJuP4As/2DKBbm2wAGe8VNtYEdCtT39Sm5O+d5z/pX0NEPCS"
```

**Tại sao nguy hiểm:** Bất kỳ ai đọc repository này đều có thể giả mạo JWT hợp lệ cho bất kỳ user nào với bất kỳ role/permission nào. Key đã bị burn vào git history ngay cả sau khi xóa.

**Giải pháp:**
1. Rotate key ngay lập tức.
2. Dùng `dotnet user-secrets` cho development.
3. Dùng environment variables hoặc Azure Key Vault / AWS Secrets Manager cho production.
4. Thêm `appsettings.json` vào `.gitignore`, cung cấp `appsettings.example.json` với giá trị placeholder.

```bash
dotnet user-secrets set "Jwt:SecretKey" "<new-key>"
```

#### SEC-02 — Seed data người dùng chia sẻ cùng password hash

**File:** `SIMS-BackEnd/Data/users.csv`

Users 3–17 có `PasswordHash` và `Salt` giống hệt nhau — tất cả dùng cùng mật khẩu. Nếu dữ liệu này được deploy lên môi trường non-development, một credential bị lộ sẽ ảnh hưởng nhiều tài khoản.

**Giải pháp:** Mỗi seed user phải có salt và hash riêng biệt. Generate fresh hashes bằng `Pbkdf2PasswordHasher` của dự án. Đánh dấu file này là dev-only.

---

### 🟠 HIGH

#### SEC-03 — Controllers không có `[Authorize]` mặc định ở class level

**File:** Tất cả controllers

Không có deny-by-default. Action mới không khai báo `[Authorize]` sẽ là public endpoint.

**Giải pháp:** Thêm `[Authorize]` ở class level cho mọi controller. Xem chi tiết ở mục 5.1.

#### SEC-04 — `GetJtiFromToken` / `GetExpiryFromToken` không validate chữ ký

**File:** `SIMS.Infrastructure/Security/JwtTokenService.cs`

```csharp
public string? GetJtiFromToken(string token)
{
    var jwt = _handler.ReadJwtToken(token);   // KHÔNG validate chữ ký
    return jwt.Id;
}
```

Dùng trong `LogoutAsync` — client có thể gửi token giả mạo với `jti` của nạn nhân để force-logout người khác.

**Giải pháp:**
```csharp
// AuthService.cs — LogoutAsync
var principal = _tokenService.ValidateIgnoringLifetime(token);
if (principal is null) throw new AppException(ErrorCode.INVALID_TOKEN);
var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
```

#### SEC-05 — Revocation check đọc toàn bộ file trên mỗi request

**File:** `SIMS-BackEnd/Program.cs` — `OnTokenValidated`

Mỗi request xác thực đọc toàn bộ `revoked_tokens.csv`. Vừa là bottleneck hiệu suất vừa là áp lực lên semaphore.

**Giải pháp ngắn hạn:** Cache revocation list trong `IMemoryCache` với TTL ngắn (30–60 giây).

---

### 🟡 MEDIUM

#### SEC-06 — Không có Rate Limiting trên Auth endpoints

**File:** `SIMS-BackEnd/Controllers/AuthController.cs`

`/api/auth/login` và `/api/auth/refresh` không có rate limit. Kẻ tấn công có thể brute-force không giới hạn.

**Giải pháp:**
```csharp
builder.Services.AddRateLimiter(o => o
    .AddFixedWindowLimiter("auth", opts => {
        opts.PermitLimit = 5;
        opts.Window = TimeSpan.FromMinutes(1);
        opts.QueueLimit = 0;
    }));
app.UseRateLimiter();
// Áp dụng [EnableRateLimiting("auth")] lên AuthController
```

#### SEC-07 — `UpdateStudentRequest.StudentCode` chấp nhận chuỗi rỗng

**File:** `SIMS.Application/DTOs/Students/UpdateStudentRequest.cs`

`StudentCodeAttribute` defer null/empty cho `[Required]` nhưng `[Required]` không được khai báo. Gửi `""` vượt qua validation.

#### SEC-08 — Không có CORS Policy

`Program.cs` không cấu hình CORS policy rõ ràng. Nếu API được gọi từ browser, cross-origin requests sẽ hoạt động tùy thuộc vào hosting defaults.

---

### 🟢 LOW

#### SEC-09 — Xóa `typ` header từ JWT

Non-standard theo RFC 7515. Một số security scanner và thư viện JWT bên thứ ba từ chối token không có `typ`.

#### SEC-10 — `ExpiryMinutes = 2` quá ngắn

Token 2 phút buộc client refresh rất thường xuyên, tăng tải server. Cân nhắc 15–30 phút là cân bằng hợp lý.

---

## 7. Đánh Giá Hiệu Suất

### 🔴 N+1 CSV Reads — GradeService

**File:** `SIMS.Application/Services/GradeService.cs`

```csharp
foreach (var grade in grades)
{
    // Đọc toàn bộ classes.csv mỗi vòng lặp
    var schoolClass = await _classRepository.GetByIdAsync(grade.ClassId);
    // Đọc toàn bộ subjects.csv mỗi vòng lặp
    var subject = await _subjectRepository.GetByIdAsync(schoolClass.SubjectId);
}
```

Với N grades: **2N lần đọc file đầy đủ**. 100 grades = 200 lần đọc CSV để phục vụ 1 request.

**Giải pháp:**
```csharp
// Load một lần, tra cứu bằng Dictionary
var allClasses  = (await _classRepository.GetAllAsync())
    .ToDictionary(c => c.Id);
var allSubjects = (await _subjectRepository.GetAllAsync())
    .ToDictionary(s => s.Id);

foreach (var grade in grades)
{
    allClasses.TryGetValue(grade.ClassId, out var schoolClass);
    allSubjects.TryGetValue(schoolClass?.SubjectId ?? 0, out var subject);
    // ...
}
```

### 🟡 MEDIUM

| Vấn đề | File | Mô tả |
|---|---|---|
| GetByIdAsync load toàn bộ | `StudentService.cs` | `GetAllAsync()` rồi build Dictionary chỉ để lấy 1 user — dùng `GetByIdAsync` thay |
| Revocation list không cache | `Program.cs` | Đọc CSV mỗi request |
| Không có pagination | Tất cả list endpoints | Trả toàn bộ CSV, không giới hạn |
| Revoked tokens không được dọn | `revoked_tokens.csv` | File lớn dần vô hạn — cần cleanup theo `ExpiresAt` |

### 🟢 LOW

**Không có Cancellation Token:** Mọi async method đều không nhận `CancellationToken`. Khi client disconnect, server tiếp tục xử lý vô ích.

```csharp
// Thêm vào tất cả async methods
public async Task<StudentResponse?> GetByIdAsync(int id, CancellationToken ct = default)
{
    var students = await _studentRepository.GetAllAsync(ct);
    // ...
}
```

---

## 8. Đánh Giá Chất Lượng Code

### 8.1 Code Smells

| Code Smell | File | Mức độ |
|---|---|---|
| Magic strings (`"sub"`, `"jti"`, `"role"`) | `Program.cs` | Thấp |
| Manual mapping lặp lại ở mọi service | Tất cả services | Trung bình |
| AuthService quá nhiều trách nhiệm | `AuthService.cs` | Trung bình |
| Dead DTOs (CreateInstructorRequest, UpdateInstructorRequest) | `DTOs/Instructors/` | Thấp |
| Dead packages (Mapster, FluentValidation) | `SIMS.Application.csproj` | Thấp |

### 8.2 Đặt tên

Nhìn chung tốt. Một số vấn đề:

- `INSTRUCTOR_NOT_EXISTED`, `SUBJECT_NOT_EXISTED`, `CLASS_NOT_EXISTED`, `COURSE_NOT_EXISTED` trong `ErrorCode.cs` — dùng "existed" thay "found" là không tự nhiên. Nên đổi thành `_NOT_FOUND`.
- File `GradeClassifyTests.cs` nhưng class là `GradeServiceTests` — tên file và tên class không khớp.
- `EDIT_INFO` vs `EDIT_PROFILE` — hai permission constant cho cùng mục đích, gây nhầm lẫn.

### 8.3 Khả năng đọc

Service methods ngắn gọn và tập trung. `GradeService.Classify()` là pure function sạch, dễ đọc. `CsvRepositoryBase` được comment đầy đủ, đặc biệt phần xử lý DateTime UTC round-trip. Nhìn chung khả năng đọc tốt.

### 8.4 Complexity

Không có method nào với cyclomatic complexity cao. Method phức tạp nhất là `AuthService.RefreshTokenAsync` (phân nhánh expired token, revocation check, re-read claims) — nhưng vẫn đọc được và comment rõ ràng.

### 8.5 Dead Code

- `CreateInstructorRequest.cs` — không được dùng
- `UpdateInstructorRequest.cs` — không được dùng
- Package Mapster — khai báo nhưng không dùng
- Package FluentValidation.DependencyInjectionExtensions — khai báo nhưng không dùng
- `EDIT_INFO` permission constant — có trong `Permissions.All` nhưng không có endpoint nào require nó
- Nhánh fallback trong `DataStoreSettings.ResolvePath()` — không bao giờ được thực thi

---

## 9. Đánh Giá Tests

### 9.1 Tổng quan coverage

| File test | Số test | Nội dung kiểm thử |
|---|---|---|
| `AuthServiceTests.cs` | 3 | Đăng nhập hợp lệ, username không tồn tại, sai mật khẩu |
| `GradeClassifyTests.cs` | 8 | CRUD grade + tất cả 4 band phân loại |
| `StudentServiceTests.cs` | 4 | Tạo thành công, trùng mã, 2 test chỉ test mock |
| `StudentCodeAttributeTests.cs` | 2 | Mã hợp lệ, null/empty |

**Tổng cộng: 17 tests** cho một ứng dụng 12 services — coverage rất thấp.

### 9.2 Vấn đề chất lượng test

**Hai test trong `StudentServiceTests` chỉ test mock, không test service:**

```csharp
// Chỉ verify Moq trả về thứ bạn đã cấu hình — không có giá trị
[Fact]
public async Task GetByStudentCode_WhenCodeExists_ShouldReturnMatchingStudent()
{
    _studentRepo.Setup(r => r.GetByStudentCodeAsync("BD00777"))
        .ReturnsAsync(student);

    // Gọi trực tiếp mock object, không phải SUT
    var result = await _studentRepo.Object.GetByStudentCodeAsync("BD00777");
    result.Should().NotBeNull();
}
```

Những test này nên gọi `_studentService.GetByStudentCodeAsync(...)` hoặc bị xóa.

### 9.3 Scenarios còn thiếu

| Test cần thêm | Ưu tiên |
|---|---|
| `AuthService.RefreshTokenAsync` — refresh hợp lệ, hết refresh window, token đã revoke | 🔴 Cao |
| `AuthService.LogoutAsync` — thành công, token không hợp lệ | 🔴 Cao |
| `ClassService.EnrollStudentAsync` — thành công, hết sức chứa, trùng enrollment, lớp inactive | 🔴 Cao |
| `JwtTokenService` — tạo token, validate, chặn alg:none | 🔴 Cao |
| `Pbkdf2PasswordHasher` — hash/verify round-trip, phát hiện hash bị sửa | 🟡 Trung bình |
| `StudentCodeAttribute` — định dạng sai (sai prefix, quá ngắn, quá dài, ký tự không phải số) | 🟡 Trung bình |
| `StudentService.UpdateAsync` — thành công, trùng mã, không tìm thấy | 🟡 Trung bình |
| `GradeService.GetScoresByStudentCodeAsync` — không có grade, nhiều grade | 🟡 Trung bình |

### 9.4 Integration Tests

Không có integration test nào. Với CSV-backed architecture, integration test với temp CSV files sẽ rất đơn giản và có giá trị cao.

```csharp
// Ví dụ integration test pattern
public class StudentRepositoryTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public StudentRepositoryTests()
    {
        Directory.CreateDirectory(_tempDir);
        File.WriteAllText(Path.Combine(_tempDir, "students.csv"), "Id,StudentCode,...\n");
    }

    [Fact]
    public async Task AddAsync_ShouldPersistToCsv()
    {
        var settings = Options.Create(new DataStoreSettings { BasePath = _tempDir });
        var repo = new StudentRepository(settings);
        // ...
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);
}
```

---

## 10. Sẵn Sàng Production

| Mục | Trạng thái | Ghi chú |
|---|---|---|
| Structured logging | ⚠️ Một phần | `GlobalExceptionHandler` log đúng; repositories không log |
| Health checks | ❌ Thiếu | Không có endpoint `/health` |
| Monitoring / Metrics | ❌ Thiếu | Không có telemetry |
| Retry policy | ❌ Thiếu | Không có Polly hay retry trên file I/O |
| Configuration validation | ⚠️ Một phần | `JwtSettings` bind nhưng không validate at startup |
| Docker | ❌ Thiếu | Không có `Dockerfile` hay `docker-compose.yml` |
| CI/CD | ❌ Thiếu | `.github/workflows/` tồn tại nhưng rỗng |
| Secrets management | ❌ Vi phạm | Secret trong `appsettings.json` |
| Environment config | ✅ | `appsettings.Development.json` tồn tại |
| Pagination | ❌ Thiếu | Mọi endpoint trả unbounded list |
| Cancellation Tokens | ❌ Thiếu | Không được truyền vào bất kỳ async method nào |

### Cấu hình Health Check (quick win):

```csharp
// Program.cs
builder.Services.AddHealthChecks();
app.MapHealthChecks("/health");
```

### Validate config at startup:

```csharp
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();  // fail fast nếu config thiếu/sai
```

---

## 11. So Sánh Best Practices

| Best Practice | Trạng thái | Ghi chú |
|---|---|---|
| Clean Architecture dependency rule | ✅ Đúng | Phụ thuộc từ ngoài vào trong |
| Domain entities không phụ thuộc framework | ✅ Đúng | Pure POCO |
| Application định nghĩa repository interfaces | ✅ Đúng | |
| IOptions trong Application layer | ⚠️ | Nên inject record trực tiếp |
| Async all the way | ✅ Đúng | |
| Cancellation tokens | ❌ | Không có |
| Constant-time password comparison | ✅ Đúng | `CryptographicOperations.FixedTimeEquals` |
| Error envelope nhất quán | ✅ Xuất sắc | `ApiResponse<T>` mọi nơi |
| Secrets trong config | ❌ Vi phạm nghiêm trọng | |
| OWASP JWT best practices | ⚠️ | Thiếu rate limit, typ header, logout validation |
| alg:none attack prevention | ✅ Đúng | Explicit algorithm check |
| PBKDF2 iterations đủ | ✅ Đúng | 100.000 iterations |
| Repository pattern | ✅ Đúng | |
| Unit of Work | ❌ | Không có |
| Pagination | ❌ | Không có |
| Structured logging | ⚠️ Một phần | |

---

## 12. Đề Xuất Tái Cấu Trúc

### Quick Wins (< 1 ngày/item)

1. **Thêm `[Authorize]` ở class level** cho mọi controller. Xóa logic `[AllowAnonymous]` injection trong `Program.cs` và áp dụng tường minh trên 3 action public của `AuthController`.

2. **Fix validation lỗ hổng:** Thêm `[Required]` vào `UpdateStudentRequest.StudentCode` và audit tất cả update DTOs khác.

3. **Fix N+1 trong `GradeService`:** Load tất cả classes và subjects một lần trước vòng lặp.

4. **Xóa dead packages:** Loại bỏ `Mapster`, `FluentValidation.DependencyInjectionExtensions` khỏi `SIMS.Application.csproj`.

5. **Xóa dead DTOs:** `CreateInstructorRequest`, `UpdateInstructorRequest` — hoặc implement CRUD còn thiếu.

6. **Fix tên error codes:** `NOT_EXISTED` → `NOT_FOUND` trong `ErrorCode.cs`.

7. **Thêm `ExpiresAt` vào `LoginResponse`.**

8. **Fix hai mock-only tests** trong `StudentServiceTests`.

9. **Thêm Health Check endpoint** (`/health`).

10. **Thêm `.ValidateOnStart()`** cho JwtSettings.

### Medium (1–3 ngày/item)

1. **Atomic Read-Modify-Write:** Tái cấu trúc `CsvRepositoryBase` để bọc read + modify + write trong một lần giữ semaphore.

2. **Fix ID assignment:** Xử lý trường hợp `records.Count == 0` tránh `InvalidOperationException`.

3. **Cache revocation list:** Dùng `IMemoryCache` với TTL 30–60 giây cho revocation check.

4. **Fix logout validation:** Dùng `ValidateIgnoringLifetime` thay `ReadJwtToken` trong `LogoutAsync`.

5. **Rate limiting:** Thêm `Microsoft.AspNetCore.RateLimiting` cho auth endpoints.

6. **Pagination:** Thêm `page`/`pageSize` query params cho tất cả list endpoints.

7. **Cleanup revoked tokens:** Job hoặc cleanup-on-read xóa entries đã hết hạn.

8. **Thêm Cancellation Tokens** vào mọi async method.

9. **Tạo CI workflow:** `.github/workflows/build.yml` chạy `dotnet build` và `dotnet test`.

10. **Bổ sung unit tests** cho các scenarios còn thiếu (đặc biệt Auth, Class, JWT).

### Major Refactoring (1–2 tuần)

1. **Unit of Work:** Thiết kế và implement `IUnitOfWork` (hoặc application-level lock) cho cross-repository operations.

2. **Tách `AuthService`:** Tách `TokenRevocationService` riêng để xử lý logout và revocation management.

3. **Domain Value Objects:** Implement `StudentCode`, `InstructorCode` là Value Objects trong Domain để mang validation vào đúng chỗ.

4. **Di chuyển `Classify()` vào Domain:** Chuyển logic phân loại điểm vào entity `Grade` hoặc Domain Service.

5. **Cascade deletes:** Implement kiểm tra dependent records trước khi xóa (user → student/instructor, subject → class, course → class).

### Long-term Architecture Improvements

1. **CQRS + MediatR:** Tách Commands/Queries/Handlers theo từng use case. Giúp mở rộng, test, và maintain từng tính năng độc lập.

2. **Chuyển sang database thực:** SQLite (minimal change) hoặc PostgreSQL/SQL Server với EF Core. Giải quyết toàn bộ vấn đề concurrency, transaction, pagination, indexing.

3. **Domain Events:** Implement event bus nội bộ để decouple side effects (audit log, notification).

4. **Background service:** Cleanup expired revoked tokens định kỳ bằng `IHostedService`.

5. **Observability:** Tích hợp OpenTelemetry cho traces, metrics, logs.

---

## 13. Báo Cáo Ưu Tiên

Bảng đầy đủ mọi vấn đề tìm thấy:

| # | Mức độ | File | Class/Method | Vấn đề | Giải pháp |
|---|---|---|---|---|---|
| 1 | 🔴 Critical | `appsettings.json` | — | JWT secret key commit lên source control | Rotate key, dùng user-secrets/env var |
| 2 | 🔴 Critical | `Data/users.csv` | — | 15 user chia sẻ cùng password hash | Generate hash riêng cho mỗi user |
| 3 | 🟠 High | `CsvRepositoryBase.cs` | `AddAsync`/`UpdateAsync` | Read-modify-write không atomic — mất dữ liệu khi concurrent | `ReadModifyWriteAsync` giữ semaphore suốt |
| 4 | 🟠 High | `ClassService.cs` | `EnrollStudentAsync` | Race condition capacity check + enrollment | Lock application-level hoặc verify trong write |
| 5 | 🟠 High | `UserService.cs` | `DeleteAsync` | Xóa user để lại Student/Instructor orphaned | Cascade delete hoặc kiểm tra dependent |
| 6 | 🟠 High | Tất cả controllers | — | Không có `[Authorize]` mặc định ở class level | Thêm `[Authorize]` ở class level |
| 7 | 🟠 High | `JwtTokenService.cs` | `GetJtiFromToken` | Logout không validate chữ ký JWT | Dùng `ValidateIgnoringLifetime` |
| 8 | 🟡 Medium | `GradeService.cs` | `GetScoresByStudentCodeAsync` | N+1 CSV reads (2N reads cho N grades) | Load all + Dictionary lookup |
| 9 | 🟡 Medium | `SIMS.Application.csproj` | — | Dead packages (Mapster, FluentValidation) | Xóa khỏi csproj |
| 10 | 🟡 Medium | `DTOs/Instructors/` | — | Dead DTOs không được dùng | Xóa hoặc implement CRUD |
| 11 | 🟡 Medium | `DataStoreSettings.cs` | `ResolvePath` | Fallback unreachable, comment sai | Xóa dead branch, fix comment |
| 12 | 🟡 Medium | `ErrorCode.cs` | — | `INSTRUCTOR_NOT_EXISTED` → trả 400 thay 404 | Đổi `HttpStatusCode.NotFound` |
| 13 | 🟡 Medium | `Permissions.cs` | — | `EDIT_INFO` là orphan permission | Xóa hoặc áp dụng cho endpoint |
| 14 | 🟡 Medium | `Program.cs` | — | Permission tạo qua API không dùng được không restart | Load policies động từ CSV |
| 15 | 🟡 Medium | `AuthController.cs` | `Login`/`Refresh` | Không có rate limiting | Thêm `RateLimiter` middleware |
| 16 | 🟡 Medium | `UpdateStudentRequest.cs` | — | `StudentCode` thiếu `[Required]`, chấp nhận `""` | Thêm `[Required]` |
| 17 | 🟡 Medium | `Program.cs` | `OnTokenValidated` | Revocation check đọc file mỗi request | Cache với `IMemoryCache` |
| 18 | 🟢 Low | `JwtTokenService.cs` | `GenerateToken` | Xóa `typ` header — non-standard | Bỏ `header.Remove("typ")` |
| 19 | 🟢 Low | `StudentService.cs` | `GetByIdAsync` | Load toàn bộ users chỉ để lấy 1 record | Dùng `GetByIdAsync` thay `GetAllAsync` |
| 20 | 🟢 Low | `StudentServiceTests.cs` | 2 test cuối | Test gọi mock trực tiếp, không test SUT | Fix để gọi `_studentService` |
| 21 | 🟢 Low | `StudentCodeAttributeTests.cs` | — | Không có test case âm (sai format) | Thêm `[Theory][InlineData]` invalid cases |
| 22 | 🟢 Low | Tất cả list endpoints | — | Không có pagination | Thêm `page`/`pageSize` |
| 23 | 🟢 Low | `LoginResponse.cs` | — | Không có `ExpiresAt` | Thêm `ExpiresAt` từ `TokenResult` |
| 24 | 🟢 Low | `.github/workflows/` | — | Không có CI workflow | Tạo `build.yml` |
| 25 | 🟢 Low | Tất cả async methods | — | Không có `CancellationToken` | Thêm `ct = default` parameter |
| 26 | 🟢 Low | `revoked_tokens.csv` | — | File lớn dần không có cleanup | Job cleanup entries đã hết hạn |
| 27 | 🟢 Low | `AuthService.cs` | — | Vi phạm SRP — quá nhiều trách nhiệm | Tách `TokenRevocationService` |
| 28 | 🟢 Low | `Program.cs` | — | Magic strings cho claim types | Dùng `JwtRegisteredClaimNames` constants |

---

## 14. Điểm Tổng Thể

### Thang điểm: 0–10

| Tiêu chí | Điểm | Nhận xét |
|---|---|---|
| **Kiến trúc** | **7.0/10** | Dependency rule đúng, Clean Architecture nhìn chung tốt. Trừ điểm: thiếu UoW, CQRS chưa triển khai, IOptions trong Application |
| **Khả năng bảo trì** | **6.5/10** | Code nhất quán, dễ đọc. Trừ điểm: dead code, SRP vi phạm, manual mapping lặp lại |
| **Khả năng mở rộng** | **4.0/10** | Không có pagination, CSV không scale, concurrency bugs, thiếu caching |
| **Hiệu suất** | **4.5/10** | N+1 reads, không có caching, revocation check mỗi request, không có pagination |
| **Bảo mật** | **3.5/10** | Secret trong source control là Critical. Logout không validate chữ ký, thiếu rate limit, thiếu CORS |
| **Khả năng đọc** | **7.5/10** | Code sạch, đặt tên nhất quán, method ngắn. `CsvRepositoryBase` comment tốt |
| **Khả năng kiểm thử** | **4.0/10** | 17 tests cho 12 services, 2 test vô giá trị, thiếu integration tests |
| **Sẵn sàng Production** | **2.5/10** | Thiếu Docker, CI/CD, health check, monitoring, secret management |

### 🏆 Điểm Tổng: **5.0 / 10**

---

### Phân tích điểm mạnh & điểm yếu

**Điểm mạnh:**
- Clean Architecture dependency rule được tuân thủ đúng
- `ApiResponse<T>` envelope nhất quán xuất sắc trên toàn API
- `Pbkdf2PasswordHasher` implement chuẩn bảo mật đúng
- `CsvRepositoryBase` có thiết kế tốt (semaphore, UTC DateTime, whitespace trimming)
- `ExpiredTokenPrincipal` là thiết kế bảo mật thông minh
- Error enumeration prevention trong `AuthService.LoginAsync` (thông báo lỗi không phân biệt user/password)
- `alg:none` attack được chặn tường minh

**Điểm yếu:**
- JWT secret commit lên source control — lỗi bảo mật nghiêm trọng nhất
- Read-modify-write không atomic — bug dữ liệu tiềm ẩn
- 17 tests cho toàn bộ ứng dụng là hoàn toàn không đủ
- Không có CI/CD, Docker, health check — không sẵn sàng production
- N+1 CSV reads trong `GradeService`
- Không có rate limiting, CORS, hoặc cancellation support

---

## 15. Lộ Trình Ưu Tiên

### Sprint 1 — Bảo mật & Ổn định (tuần 1)
1. Rotate JWT secret, cấu hình env vars
2. Fix atomic read-modify-write trong `CsvRepositoryBase`
3. Fix logout validation (dùng `ValidateIgnoringLifetime`)
4. Thêm `[Authorize]` class-level cho mọi controller
5. Fix ID assignment khi danh sách rỗng

### Sprint 2 — Chất lượng & Hiệu suất (tuần 2)
1. Fix N+1 trong `GradeService`
2. Cache revocation list
3. Thêm rate limiting cho auth endpoints
4. Fix validation lỗ hổng (`[Required]` + `StudentCode`)
5. Cleanup dead code (packages, DTOs, constants)
6. Fix tên error codes (`NOT_EXISTED` → `NOT_FOUND`)

### Sprint 3 — Tests & CI (tuần 3)
1. Fix mock-only tests
2. Viết tests cho Auth (refresh, logout, revocation)
3. Viết tests cho ClassService (enrollment scenarios)
4. Viết tests cho JwtTokenService
5. Tạo CI workflow
6. Thêm health check endpoint

### Sprint 4 — Production Readiness (tuần 4)
1. Thêm pagination cho tất cả list endpoints
2. Thêm Cancellation Tokens
3. Thêm `ExpiresAt` vào `LoginResponse`
4. Cấu hình CORS policy
5. Tạo Dockerfile
6. Cleanup expired revoked tokens

### Long-term
- Xem xét chuyển sang database (SQLite/PostgreSQL)
- CQRS + MediatR
- Domain Value Objects
- Domain Events

---

*Báo cáo này được tạo tự động từ quá trình phân tích toàn bộ source code của repository SIMS-BackEnd.*
*Tổng số file được đánh giá: 70+ file .cs, .json, .csv, .csproj*


