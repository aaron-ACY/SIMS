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
| POST | `/api/auth/register` | Public | Đăng ký tài khoản mới (email phải có sẵn trong hệ thống) |
| POST | `/api/auth/refresh` | Public | Làm mới access token từ token cũ |
| POST | `/api/auth/logout` | Public | Thu hồi token hiện tại |

> **Rate limiting**: login, register, refresh bị giới hạn **5 lần/phút/IP**.

**Body login:**
```json
{ "username": "string", "password": "string" }
```

**Body register:**
```json
{ "username": "string", "password": "string", "email": "string" }
```

**Body refresh:**
```json
{ "token": "<access_token_cũ>" }
```

---

### 3.2 Users — `/api/users`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/users/me` | Authenticated (bất kỳ role) | Xem profile của chính mình |
| PUT | `/api/users/me` | `EDIT_PROFILE` | Cập nhật thông tin cá nhân |
| GET | `/api/users` | `VIEW_USERS` | Lấy danh sách tất cả user |
| POST | `/api/users` | `CREATE_USER` | Tạo user thuần (không có profile) |
| POST | `/api/users/student` | `CREATE_USER` | Tạo user + student profile cùng lúc |
| POST | `/api/users/instructor` | `CREATE_USER` | Tạo user + instructor profile cùng lúc |
| DELETE | `/api/users/{id}` | `DELETE_USER` | Xoá tài khoản (không thể xoá chính mình) |

---

### 3.3 Students — `/api/students`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/students` | `VIEW_STUDENTS` | Lấy danh sách sinh viên |
| GET | `/api/students/{id}` | `VIEW_STUDENTS` | Lấy thông tin 1 sinh viên |
| POST | `/api/students` | `CREATE_STUDENT` | Tạo mới sinh viên |
| PUT | `/api/students/{id}` | `EDIT_STUDENT` | Cập nhật thông tin sinh viên |
| DELETE | `/api/students/{id}` | `DELETE_STUDENT` | Xoá sinh viên |
| POST | `/api/students/import` | `IMPORT_STUDENTS` | Import hàng loạt từ CSV (`multipart/form-data`) |

**Import CSV — các cột (không cần header):**
```
StudentCode, FirstName, LastName, DateOfBirth, Gender, Phone, City, Country, Email, Major
```

---

### 3.4 Instructors — `/api/instructors`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/instructors` | `VIEW_INSTRUCTORS` | Lấy danh sách giảng viên |
| GET | `/api/instructors/{id}` | `VIEW_INSTRUCTORS` | Lấy thông tin 1 giảng viên |
| POST | `/api/instructors` | `CREATE_INSTRUCTOR` | Tạo mới giảng viên |
| PUT | `/api/instructors/{id}` | `EDIT_INSTRUCTOR` | Cập nhật thông tin giảng viên |
| DELETE | `/api/instructors/{id}` | `DELETE_INSTRUCTOR` | Xoá giảng viên (lỗi 409 nếu còn lớp đang dạy) |
| POST | `/api/instructors/import` | `IMPORT_INSTRUCTORS` | Import hàng loạt từ CSV (`multipart/form-data`) |

**Import CSV — các cột (không cần header):**
```
InstructorCode, FirstName, LastName, DateOfBirth, Gender, Phone, City, Country, Email, Department, Degree
```

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

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/subjects` | `VIEW_SUB` | Lấy danh sách môn học |
| POST | `/api/subjects` | `CREATE_SUB` | Tạo môn học mới |
| DELETE | `/api/subjects/{id}` | `DELETE_SUB` | Xoá môn học |

---

### 3.8 Classes — `/api/classes`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/classes` | `VIEW_CLA` | Lấy danh sách lớp học |
| POST | `/api/classes` | `CREATE_CLASS` | Tạo lớp mới |
| GET | `/api/classes/{classId}/enrollments` | `LIST_STU` | Xem danh sách sinh viên trong lớp |
| POST | `/api/classes/{classId}/enrollments` | `ENROLLMENTS` | Đăng ký sinh viên vào lớp |
| DELETE | `/api/classes/{classId}/enrollments/{studentId}` | `GETOUT` | Xoá sinh viên khỏi lớp |

---

### 3.9 Grades — `/api/grades`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| POST | `/api/grades/{enrollmentId}/submit` | `SUBMITTED` | Nộp bài tập (`multipart/form-data`) |
| POST | `/api/grades` | `ENTER_GRADE` | Nhập điểm cho sinh viên |
| PUT | `/api/grades/{id}` | `EDIT_GRADE` | Sửa điểm |
| GET | `/api/grades/student/{studentCode}` | `VIEW_SCORE` | Xem bảng điểm của sinh viên |

---

### 3.10 Permissions — `/api/permissions`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| GET | `/api/permissions` | `VIEW_PERMISSIONS` | Lấy danh sách tất cả permission |
| POST | `/api/permissions` | `CREATE_PERMISSION` | Tạo permission mới |
| PUT | `/api/permissions/{id}` | `EDIT_PERMISSION` | Chỉnh sửa permission |

---

### 3.11 Roles — `/api/roles`

| Method | Endpoint | Permission yêu cầu | Mô tả |
|---|---|---|---|
| POST | `/api/roles/{roleId}/permissions` | `GET_PERMISSION` | Gán permission cho role |

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
          token: oldToken,
        });

        const newToken = data.data.token;
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
  localStorage.setItem('access_token', data.data.token);
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
| `SUBMITTED` | Nộp bài tập |
| `VIEW_PERMISSIONS` | Xem danh sách permission |
| `CREATE_PERMISSION` | Tạo permission |
| `EDIT_PERMISSION` | Sửa permission |
| `GET_PERMISSION` | Gán permission cho role |
