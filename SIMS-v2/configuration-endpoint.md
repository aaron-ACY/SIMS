# SIMS API — Hướng dẫn cấu hình & Endpoint

## 1. Yêu cầu môi trường

| Công cụ | Phiên bản tối thiểu |
|---|---|
| .NET SDK | 8.0 |
| Node.js (frontend) | 18+ |

---

## 2. Chạy Backend (ASP.NET Core)

### 2.1 Cấu hình JWT Secret

JWT Secret **không được** lưu trong `appsettings.json`. Cần thiết lập qua **User Secrets** (dev) hoặc **biến môi trường** (prod).

**Development — dùng User Secrets (chỉ cần làm 1 lần):**
```powershell
cd SIMS-BackEnd
dotnet user-secrets set "Jwt:SecretKey" "super-secret-key-change-this-32chars!!"
```

> Key phải **tối thiểu 32 ký tự**. Thay chuỗi trên bằng bất kỳ chuỗi ngẫu nhiên nào dài hơn 32 ký tự.

**Kiểm tra Secret đã được set chưa:**
```powershell
dotnet user-secrets list
```

**Production — biến môi trường:**
```powershell
# Windows (PowerShell)
$env:Jwt__SecretKey = "super-secret-key-change-this-32chars!!"

# Linux / macOS
export Jwt__SecretKey="super-secret-key-change-this-32chars!!"
```

> Lưu ý: dùng dấu `__` (double underscore) khi đặt tên biến môi trường thay cho dấu `:`.

### 2.2 Khởi động API

```powershell
cd SIMS-BackEnd

# HTTP (port 5198)
dotnet run --launch-profile http

# HTTPS (port 7096) — khuyến nghị
dotnet run --launch-profile https
```

| Profile | URL |
|---|---|
| http | `http://localhost:5198` |
| https | `https://localhost:7096` |
| Swagger UI | `http://localhost:5198/swagger` hoặc `https://localhost:7096/swagger` |

### 2.3 Cấu hình mặc định (`appsettings.json`)

```json
{
  "Jwt": {
    "Issuer": "SIMS-API",
    "Audience": "SIMS-Client",
    "ExpiryMinutes": 30,
    "RefreshWindowMinutes": 10080
  },
  "DataStore": {
    "BasePath": "Data"
  }
}
```

- **ExpiryMinutes**: Access token hết hạn sau 30 phút.
- **RefreshWindowMinutes**: Refresh token có hiệu lực trong 7 ngày (10080 phút).

---

## 3. Danh sách Endpoint

> **Base URL**: `https://localhost:7096/api` (dev)
>
> Tất cả endpoint yêu cầu xác thực phải gửi kèm header:
> ```
> Authorization: Bearer <access_token>
> ```

### 3.1 Auth — `/api/auth`

| Method | Endpoint | Auth | Mô tả |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Đăng nhập, nhận JWT |
| POST | `/api/auth/register` | Public | Tự đăng ký tài khoản (email phải được admin import sẵn) |
| POST | `/api/auth/refresh` | Public | Làm mới access token từ token cũ |
| POST | `/api/auth/logout` | Public | Thu hồi token hiện tại (gửi kèm header `Authorization`) |

> **Rate limiting**: login, register, refresh bị giới hạn **5 lần/phút/IP**. Logout không bị giới hạn.

**Body login:**
```json
{ "username": "string", "password": "string" }
```

**Response login** (`200`):
```json
{
  "accessToken": "eyJhbGciOi...",
  "expiresAt": "2026-08-04T10:30:00Z"
}
```

**Body register:**
```json
{ "email": "string", "username": "string", "password": "string" }
```

> `username` tối thiểu **6 ký tự**, `password` tối thiểu **8 ký tự**.
> Backend tra `email` trong danh sách student/instructor đã import để xác định role và trả về **201** kèm token (đăng nhập luôn).
> Trả **422** nếu email không tồn tại trong bất kỳ profile nào, **400** nếu email/username đã có tài khoản.

**Response register** (`201`):
```json
{
  "accessToken": "eyJhbGciOi...",
  "expiresAt": "2026-08-04T10:30:00Z",
  "role": "Student"
}
```

**Body refresh:**
```json
{ "accessToken": "<access_token_cũ>" }
```

> Token cũ có thể đã hết hạn nhưng phải còn hợp lệ về signature và nằm trong `RefreshWindowMinutes`. Token xuất trình sẽ bị thu hồi sau khi refresh thành công — mỗi token chỉ refresh được **một lần**.

---

### 3.2 Users — `/api/users`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/users/profile` | Authenticated (bất kỳ role) | Xem profile của chính mình |
| PUT | `/api/users/profile` | `EDIT_PROFILE` | Cập nhật thông tin cá nhân |
| PUT | `/api/users/change-password` | `CHANGE_PASSWORD` | Đổi mật khẩu của chính mình |
| GET | `/api/users` | `VIEW_USERS` | Lấy danh sách tất cả user |
| POST | `/api/users` | `CREATE_USER` | Tạo user thuần (không có profile) |
| POST | `/api/users/student` | `CREATE_USER` | Tạo user + student profile cùng lúc |
| POST | `/api/users/instructor` | `CREATE_USER` | Tạo user + instructor profile cùng lúc |
| DELETE | `/api/users/{id}` | `DELETE_USER` | Xoá tài khoản (không thể xoá chính mình) |

**Response `GET /api/users/profile`:**
```json
{
  "username": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "role": "Student",
  "studentCode": "BD00519",
  "dateOfBirth": "2004-05-12T00:00:00",
  "gender": "Male",
  "major": "Computer Science",
  "phone": "0901234567",
  "address": "..."
}
```

> Các field theo role được **ẩn khi null**: `studentCode`, `dateOfBirth`, `gender`, `major` chỉ xuất hiện với Student; `instructorCode`, `department`, `degree` chỉ xuất hiện với Instructor. `permissions` không có trong response vì đã nằm trong JWT.

**Body `PUT /api/users/profile`** — tất cả field đều optional, field nào `null`/không gửi thì giữ nguyên:
```json
{
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "phone": "string"
}
```

> `phone` chỉ áp dụng cho profile Student và Instructor. Đổi mật khẩu dùng endpoint riêng bên dưới.

**Body `PUT /api/users/change-password`:**
```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```

> `newPassword` tối thiểu **8 ký tự**. Sai `currentPassword` trả về lỗi `WRONG_CURRENT_PASSWORD`.
> Permission `CHANGE_PASSWORD` được gán cho role **Instructor** và **Student**.

**Body `POST /api/users`** (user thuần, không có profile):
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "roleName": "Admin"
}
```

> `roleName` nhận `"Admin"`, `"Instructor"` hoặc `"Student"`.

**Body `POST /api/users/student`** (tạo account + student profile trong 1 request):
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "2004-05-12",
  "gender": "Male",
  "phone": "0901234567",
  "address": "string",
  "major": "Computer Science",
  "enrollmentYear": 2023,
  "status": "Active"
}
```

> `username` ≥ **6 ký tự**, `password` ≥ **8 ký tự**, `gender` ≤ 10 ký tự, `address` ≤ 200 ký tự, `major` ≤ 100 ký tự, `enrollmentYear` trong khoảng **1900–2100**. `status` mặc định `"Active"`.

**Body `POST /api/users/instructor`** (tạo account + instructor profile trong 1 request):
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string",
  "department": "string",
  "degree": "string",
  "phone": "0901234567"
}
```

> `department` ≤ 100 ký tự, `degree` ≤ 50 ký tự.

---

### 3.3 Students — `/api/students`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/students/me/classes` | Authenticated (Student) | Xem các lớp sinh viên đang học, kèm `enrollmentId` để nộp bài |
| GET | `/api/students` | `VIEW_STUDENTS` | Lấy danh sách sinh viên |
| GET | `/api/students/{id}` | `VIEW_STUDENTS` | Lấy thông tin 1 sinh viên |
| POST | `/api/students` | `CREATE_STUDENT` | Tạo mới sinh viên (gắn vào user đã tồn tại) |
| PUT | `/api/students/{id}` | `EDIT_STUDENT` | Cập nhật thông tin sinh viên |
| DELETE | `/api/students/{id}` | `DELETE_STUDENT` | Xoá sinh viên |
| POST | `/api/students/import` | `IMPORT_STUDENTS` | Import hàng loạt từ CSV (`multipart/form-data`) |

> `GET /api/students/me/classes` không cần permission riêng vì response chỉ chứa dữ liệu của chính người gọi.

**Response `GET /api/students/me/classes`** (mảng):
```json
[
  {
    "enrollmentId": 12,
    "classId": 3,
    "classCode": "CS101-01",
    "subjectName": "Introduction to Programming",
    "instructorName": "Nguyen Van A",
    "semester": 1,
    "academicYear": "2025-2026",
    "room": "A201",
    "schedule": "Mon 07:30-09:30",
    "isActive": true
  }
]
```

> Dùng `enrollmentId` cho `POST /api/grades/{enrollmentId}/submit` khi nộp bài.

**Body tạo mới (`POST /api/students`):**
```json
{
  "userId": 5,
  "studentCode": "BD00519",
  "dateOfBirth": "2004-05-12",
  "gender": "Male",
  "phone": "0901234567",
  "address": "string",
  "major": "Computer Science",
  "enrollmentYear": 2023,
  "status": "Active"
}
```

> `studentCode` phải theo định dạng **BD + chữ số** (ví dụ `BD00519`). `userId` là user đã tồn tại — nếu muốn tạo user + profile cùng lúc thì dùng `POST /api/users/student`.

**Body cập nhật (`PUT /api/students/{id}`)** — các field đều optional, chỉ gửi field cần đổi:
```json
{
  "studentCode": "BD00519",
  "dateOfBirth": "2004-05-12",
  "gender": "Male",
  "phone": "0901234567",
  "address": "string",
  "major": "Computer Science",
  "enrollmentYear": 2023,
  "status": "Active"
}
```

> Nếu có gửi `studentCode` thì vẫn phải đúng định dạng **BD + chữ số**.

**Import CSV — các cột (không cần header):**
```
StudentCode, FirstName, LastName, DateOfBirth, Gender, Phone, City, Country, Email, Major
```

**Response import:**
```json
{
  "totalRows": 100,
  "imported": 97,
  "skipped": 3,
  "errors": ["Row 4: duplicate student code BD00519", "..."]
}
```

> Dòng nào lỗi validation hoặc trùng code/email sẽ bị bỏ qua và ghi vào `errors`, phần còn lại vẫn được import.

---

### 3.4 Instructors — `/api/instructors`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/instructors` | `VIEW_INSTRUCTORS` | Lấy danh sách giảng viên |
| GET | `/api/instructors/{id}` | `VIEW_INSTRUCTORS` | Lấy thông tin 1 giảng viên |
| POST | `/api/instructors` | `CREATE_INSTRUCTOR` | Tạo mới giảng viên (gắn vào user đã tồn tại) |
| PUT | `/api/instructors/{id}` | `EDIT_INSTRUCTOR` | Cập nhật thông tin giảng viên |
| DELETE | `/api/instructors/{id}` | `DELETE_INSTRUCTOR` | Xoá giảng viên (lỗi 409 nếu còn lớp đang dạy) |
| POST | `/api/instructors/import` | `IMPORT_INSTRUCTORS` | Import hàng loạt từ CSV (`multipart/form-data`) |

**Body tạo mới (`POST /api/instructors`):**
```json
{
  "userId": 8,
  "instructorCode": "GV001",
  "department": "Faculty of IT",
  "degree": "Master",
  "phone": "0901234567"
}
```

> `instructorCode` phải là duy nhất. Muốn tạo user + profile cùng lúc thì dùng `POST /api/users/instructor`.

**Body cập nhật (`PUT /api/instructors/{id}`)** — các field đều optional:
```json
{
  "instructorCode": "GV001",
  "department": "Faculty of IT",
  "degree": "PhD",
  "phone": "0901234567"
}
```

**Import CSV — các cột (không cần header):**
```
InstructorCode, FirstName, LastName, DateOfBirth, Gender, Phone, City, Country, Email, Department, Degree
```

**Response import:** cấu trúc giống import sinh viên (`totalRows`, `imported`, `skipped`, `errors`).

---

### 3.5 Majors — `/api/majors`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/majors` | `VIEW_MAJOR` | Lấy danh sách tất cả chuyên ngành |
| POST | `/api/majors` | `CREATE_MAJOR` | Tạo chuyên ngành mới |
| DELETE | `/api/majors/{id}` | `DELETE_MAJOR` | Xoá chuyên ngành |

**Body tạo mới (POST):**
```json
{
  "majorCode": "string",
  "name": "string",
  "description": "string",
  "department": "string",
  "totalCredits": 120
}
```

> `totalCredits` phải nằm trong khoảng **1–300**.

**Response mẫu:**
```json
{
  "id": 1,
  "majorCode": "CS",
  "name": "Computer Science",
  "description": "...",
  "department": "Faculty of IT",
  "totalCredits": 130,
  "isActive": true
}
```

---

### 3.6 Courses — `/api/courses`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/courses` | `VIEW_COURSES` | Lấy danh sách tất cả môn học |
| POST | `/api/courses` | `CREATE_COURSE` | Tạo môn học mới |
| DELETE | `/api/courses/{id}` | `DELETE_COURSE` | Xoá môn học |

**Body tạo mới (POST):**
```json
{
  "courseCode": "string",
  "name": "string",
  "description": "string",
  "credits": 3,
  "isRequired": true
}
```

> `credits` phải nằm trong khoảng **1–10**.

**Response mẫu:**
```json
{
  "id": 1,
  "courseCode": "CS101",
  "name": "Introduction to Programming",
  "description": "...",
  "credits": 3,
  "isRequired": true,
  "isActive": true
}
```

---

### 3.7 Subjects — `/api/subjects`

Subject là môn học được mở theo học kỳ / năm học — đây là thực thể mà `POST /api/classes` tham chiếu qua `subjectId`.

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/subjects` | `VIEW_SUB` | Lấy danh sách môn học |
| POST | `/api/subjects` | `CREATE_SUB` | Tạo môn học mới |
| DELETE | `/api/subjects/{id}` | `DELETE_SUB` | Xoá môn học |

**Body tạo mới (POST):**
```json
{
  "subjectCode": "CS101",
  "name": "Introduction to Programming",
  "description": "...",
  "credits": 3,
  "department": "Faculty of IT",
  "major": "Computer Science",
  "academicYear": "2025-2026",
  "semester": 1,
  "isRequired": true
}
```

> `credits` trong khoảng **1–10**, `semester` phải là **1, 2 hoặc 3**.

---

### 3.8 Classes — `/api/classes`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/classes` | `VIEW_CLA` | Lấy danh sách lớp học |
| POST | `/api/classes` | `CREATE_CLASS` | Tạo lớp mới |
| GET | `/api/classes/{classId}/enrollments` | `LIST_STU` | Xem danh sách sinh viên trong lớp |
| POST | `/api/classes/{classId}/enrollments` | `ENROLLMENTS` | Đăng ký sinh viên vào lớp |
| DELETE | `/api/classes/{classId}/enrollments/{studentId}` | `GETOUT` | Xoá sinh viên khỏi lớp |

**Body tạo lớp (`POST /api/classes`):**
```json
{
  "classCode": "CS101-01",
  "subjectId": 1,
  "instructorId": 3,
  "semester": 1,
  "academicYear": "2025-2026",
  "room": "A101",
  "schedule": "Mon 07:00-09:00",
  "maxEnrollment": 40
}
```

> `subjectId` là `Subject.Id`, `instructorId` là `Instructor.Id` (**không phải** `User.Id`).
> `semester` phải là **1, 2 hoặc 3**; `maxEnrollment` trong khoảng **1–500**.

**Response `GET /api/classes`** — mỗi phần tử trong `data`:
```json
{
  "id": 1,
  "classCode": "CS101-01",
  "subjectName": "Introduction to Programming",
  "instructorId": 3,
  "instructorCode": "GV001",
  "instructorName": "Nguyen Van A",
  "maxEnrollment": 40,
  "currentEnrollment": 32,
  "isActive": true
}
```

> Dùng `instructorId` để lọc lớp theo giảng viên thay vì so sánh theo tên.

**Body đăng ký sinh viên (`POST /api/classes/{classId}/enrollments`):**
```json
{ "studentId": 12 }
```

> `studentId` là `Student.Id`, không phải `studentCode`.

**Response `GET /api/classes/{classId}/enrollments`:**
```json
{
  "classCode": "CS101-01",
  "schoolYear": "2025-2026",
  "totalStudents": 2,
  "enrollments": [
    {
      "enrollmentId": 5,
      "student": {
        "studentCode": "BD00519",
        "fullName": "Tran Thi B",
        "dateOfBirth": "2004-05-12T00:00:00",
        "gender": "Female"
      },
      "status": "Enrolled",
      "enrolledAt": "2026-08-01T09:15:00"
    }
  ]
}
```

---

### 3.9 Grades — `/api/grades`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| POST | `/api/grades/{enrollmentId}/submit` | `SUBMITTED` | Sinh viên nộp bài tập (`multipart/form-data`) |
| POST | `/api/grades` | `ENTER_GRADE` | Nhập điểm cho sinh viên |
| PUT | `/api/grades/{id}` | `EDIT_GRADE` | Sửa điểm |
| GET | `/api/grades/class/{classId}` | `VIEW_CLASS_GRADES` | Xem toàn bộ bài nộp + điểm của một lớp |
| GET | `/api/grades/student/{studentCode}` | `VIEW_SCORE` | Xem bảng điểm của sinh viên |

**Nộp bài (`POST /api/grades/{enrollmentId}/submit`)** — `multipart/form-data`, field `file`.
Lần nộp đầu tạo bản ghi grade **chưa có điểm**; nộp lại sẽ ghi đè file đã lưu.
`enrollmentId` lấy từ `GET /api/students/me/classes`.

**Body nhập điểm (`POST /api/grades`):**
```json
{ "enrollmentId": 5, "score": 8.5 }
```

> `score` trong khoảng **0–10**. Sinh viên phải nộp bài trước (`submissionPath` đã có giá trị), nếu không sẽ bị từ chối.

**Body sửa điểm (`PUT /api/grades/{id}`):**
```json
{ "score": 9.0 }
```

> Chỉ sửa được grade đã được nhập điểm chính thức (`gradedAt` đã có giá trị).

**Response grade (POST / PUT / `GET /api/grades/class/{classId}`):**
```json
{
  "id": 1,
  "enrollmentId": 5,
  "studentId": 12,
  "studentCode": "BD00519",
  "studentName": "Tran Thi B",
  "classId": 1,
  "classCode": "CS101-01",
  "score": 8.5,
  "classification": "Good",
  "submissionPath": "files/5_a1b2c3.pdf",
  "gradedAt": "2026-08-02T10:00:00",
  "updatedAt": "2026-08-02T10:00:00"
}
```

> `score`, `classification`, `gradedAt` là `null` cho tới khi giảng viên nhập điểm. `submissionPath` là `null` cho tới khi sinh viên nộp bài.

**Response `GET /api/grades/student/{studentCode}`** — điểm gom theo lớp:
```json
{
  "studentCode": "BD00519",
  "firstName": "Thi B",
  "lastName": "Tran",
  "classes": [
    {
      "classCode": "CS101-01",
      "semester": 1,
      "grades": [
        {
          "subjectCode": "CS101",
          "subjectName": "Introduction to Programming",
          "scores": 8.5,
          "rating": "Good"
        }
      ]
    }
  ]
}
```

> `scores` và `rating` là `null` khi giảng viên chưa nhập điểm.

---

### 3.10 Permissions — `/api/permissions`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/permissions` | `VIEW_PERMISSIONS` | Lấy danh sách tất cả permission |
| POST | `/api/permissions` | `CREATE_PERMISSION` | Tạo permission mới |
| PUT | `/api/permissions/{id}` | `EDIT_PERMISSION` | Chỉnh sửa permission |

**Body tạo mới (POST):**
```json
{ "name": "VIEW_REPORT", "description": "Xem báo cáo" }
```

> `name` được lưu **in hoa** và phải là duy nhất (tối đa 64 ký tự), `description` tối đa 256 ký tự.

**Body cập nhật (`PUT /api/permissions/{id}`)** — cả hai field đều optional, bỏ field nào thì giữ nguyên giá trị cũ:
```json
{ "name": "VIEW_REPORT", "description": "Mô tả mới" }
```

---

### 3.11 Roles — `/api/roles`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| POST | `/api/roles/{roleId}/permissions` | `GET_PERMISSION` | Gán permission cho role |

**Body:**
```json
{ "permissionId": 12 }
```

**Response** — trả về toàn bộ permission set của role sau khi gán.

> Permission được nhúng vào JWT lúc login, **không** đọc lại theo từng request. User đang giữ role đó vẫn dùng permission set cũ cho tới khi đăng nhập lại.

---

## 4. Cấu trúc Response chung

Tất cả response đều bọc trong `ApiResponse<T>`:

```json
{
  "success": true,
  "message": "...",
  "data": { ... },
  "errors": null
}
```

Khi lỗi:
```json
{
  "success": false,
  "message": "...",
  "data": null,
  "errors": ["Chi tiết lỗi 1", "Chi tiết lỗi 2"]
}
```

### HTTP Status codes

| Code | Ý nghĩa |
|---|---|
| 200 | Thành công |
| 201 | Tạo mới thành công |
| 400 | Dữ liệu đầu vào không hợp lệ |
| 401 | Chưa xác thực / token không hợp lệ |
| 403 | Không có quyền |
| 404 | Không tìm thấy |
| 409 | Conflict (ví dụ: giảng viên còn lớp đang dạy) |
| 422 | Validation thất bại |
| 429 | Quá giới hạn request (rate limit) |

---

## 5. Setup Frontend (React)

### 5.1 Cài đặt Axios

```bash
npm install axios
```

### 5.2 Tạo file cấu hình `src/api/axiosConfig.js`

```js
import axios from 'axios';

const BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7096/api';

const api = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Tự động gắn token vào mọi request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Tự động refresh token khi nhận 401
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) prom.reject(error);
    else prom.resolve(token);
  });
  failedQueue = [];
};

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return api(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const oldToken = localStorage.getItem('access_token');

      try {
        const { data } = await axios.post(`${BASE_URL}/auth/refresh`, {
          accessToken: oldToken,
        });

        const newToken = data.data.accessToken;
        localStorage.setItem('access_token', newToken);
        processQueue(null, newToken);

        originalRequest.headers.Authorization = `Bearer ${newToken}`;
        return api(originalRequest);
      } catch (err) {
        processQueue(err, null);
        localStorage.removeItem('access_token');
        window.location.href = '/login'; // redirect về trang login
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export default api;
```

### 5.3 File `.env` (Vite)

```env
VITE_API_URL=https://localhost:7096/api
```

> Với Create React App dùng `REACT_APP_API_URL` thay vì `VITE_API_URL`.

### 5.4 Ví dụ gọi API

**Đăng nhập:**
```js
import api from './api/axiosConfig';

const login = async (username, password) => {
  const { data } = await api.post('/auth/login', { username, password });
  localStorage.setItem('access_token', data.data.accessToken);
};
```

**Lấy danh sách sinh viên:**
```js
const getStudents = async () => {
  const { data } = await api.get('/students');
  return data.data; // mảng sinh viên
};
```

**Upload file CSV (import sinh viên):**
```js
const importStudents = async (file) => {
  const formData = new FormData();
  formData.append('file', file);

  const { data } = await api.post('/students/import', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return data.data;
};
```

**Nộp bài tập:**
```js
const submitAssignment = async (enrollmentId, file) => {
  const formData = new FormData();
  formData.append('file', file);

  const { data } = await api.post(`/grades/${enrollmentId}/submit`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return data.data;
};
```

**Đăng xuất:**
```js
const logout = async () => {
  await api.post('/auth/logout');
  localStorage.removeItem('access_token');
  window.location.href = '/login';
};
```

### 5.5 Xử lý lỗi tập trung

```js
// Trong component hoặc service
try {
  const { data } = await api.get('/students');
  setStudents(data.data);
} catch (error) {
  const message = error.response?.data?.message || 'Đã xảy ra lỗi';
  const errors  = error.response?.data?.errors || [];
  console.error(message, errors);
}
```

---

## 6. Bảng Permission — Tham khảo nhanh

| Permission | Mô tả |
|---|---|
| `VIEW_USERS` | Xem danh sách user |
| `CREATE_USER` | Tạo user mới |
| `DELETE_USER` | Xoá user |
| `EDIT_PROFILE` | Sửa profile cá nhân |
| `CHANGE_PASSWORD` | Đổi mật khẩu của chính mình |
| `VIEW_STUDENTS` | Xem sinh viên |
| `CREATE_STUDENT` | Tạo sinh viên |
| `EDIT_STUDENT` | Sửa sinh viên |
| `DELETE_STUDENT` | Xoá sinh viên |
| `IMPORT_STUDENTS` | Import sinh viên từ CSV |
| `VIEW_INSTRUCTORS` | Xem giảng viên |
| `CREATE_INSTRUCTOR` | Tạo giảng viên |
| `EDIT_INSTRUCTOR` | Sửa giảng viên |
| `DELETE_INSTRUCTOR` | Xoá giảng viên |
| `IMPORT_INSTRUCTORS` | Import giảng viên từ CSV |
| `VIEW_MAJOR` | Xem danh sách chuyên ngành |
| `CREATE_MAJOR` | Tạo chuyên ngành |
| `DELETE_MAJOR` | Xoá chuyên ngành |
| `VIEW_COURSES` | Xem danh sách môn học |
| `CREATE_COURSE` | Tạo môn học |
| `DELETE_COURSE` | Xoá môn học |
| `VIEW_SUB` | Xem subjects |
| `CREATE_SUB` | Tạo subject |
| `DELETE_SUB` | Xoá subject |
| `VIEW_CLA` | Xem lớp học |
| `CREATE_CLASS` | Tạo lớp học |
| `LIST_STU` | Xem sinh viên trong lớp |
| `ENROLLMENTS` | Đăng ký sinh viên vào lớp |
| `GETOUT` | Xoá sinh viên khỏi lớp |
| `ENTER_GRADE` | Nhập điểm |
| `EDIT_GRADE` | Sửa điểm |
| `VIEW_SCORE` | Xem bảng điểm |
| `VIEW_CLASS_GRADES` | Xem toàn bộ bài nộp / điểm của một lớp |
| `SUBMITTED` | Nộp bài tập |
| `VIEW_PERMISSIONS` | Xem danh sách permission |
| `CREATE_PERMISSION` | Tạo permission |
| `EDIT_PERMISSION` | Sửa permission |
| `GET_PERMISSION` | Gán permission cho role |
