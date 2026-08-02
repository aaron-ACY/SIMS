# SIMS API — Request Bodies

> Base URL: `https://localhost:7096/api`
> Tất cả các endpoint (trừ login, register, refresh, logout) đều yêu cầu header:
> `Authorization: Bearer <access_token>`

---

## 🔐 Auth

### POST `/auth/login`
**Permission:** Public
```json
{
  "username": "admin",
  "password": "Admin123"
}
```

---

### POST `/auth/register`
**Permission:** Public — email phải đã được admin import trước
```json
{
  "email": "student@example.com",
  "username": "student01",
  "password": "Password123"
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `email` | ✅ | valid email |
| `username` | ✅ | min 6 ký tự |
| `password` | ✅ | min 8 ký tự |

---

### POST `/auth/refresh`
**Permission:** Public
```json
{
  "accessToken": "<jwt_token_hiện_tại>"
}
```

---

### POST `/auth/logout`
**Permission:** Authenticated (gửi token trong header Authorization)
> Không có body. Token được đọc từ header.

---

## 👤 Users

### GET `/users/me`
> Không có body.

### PUT `/users/me`
**Permission:** `EDIT_PROFILE`
```json
{
  "email": "newemail@example.com",
  "firstName": "Nguyen",
  "lastName": "Van A"
}
```

---

### POST `/users/student`
**Permission:** `CREATE_USER` — Tạo tài khoản + hồ sơ sinh viên cùng lúc
```json
{
  "username": "sinhvien01",
  "email": "sinhvien01@example.com",
  "password": "Password123",
  "firstName": "Van A",
  "lastName": "Nguyen",
  "dateOfBirth": "2003-05-15T00:00:00Z",
  "gender": "Male",
  "phone": "0901234567",
  "address": "123 Nguyen Hue, HCM",
  "major": "Information Technology",
  "enrollmentYear": 2023,
  "status": "Active"
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `username` | ✅ | min 6 ký tự |
| `email` | ✅ | valid email |
| `password` | ✅ | min 8 ký tự |
| `firstName` | ✅ | |
| `lastName` | ✅ | |
| `dateOfBirth` | ✅ | ISO 8601 |
| `gender` | ✅ | max 10 ký tự |
| `phone` | ✅ | định dạng số điện thoại |
| `address` | ✅ | max 200 ký tự |
| `major` | ✅ | max 100 ký tự |
| `enrollmentYear` | ✅ | 1900–2100 |
| `status` | ❌ | mặc định `"Active"`, max 50 ký tự |

---

### POST `/users/instructor`
**Permission:** `CREATE_USER` — Tạo tài khoản + hồ sơ giảng viên cùng lúc
```json
{
  "username": "giangvien01",
  "email": "giangvien01@example.com",
  "password": "Password123",
  "firstName": "Thi B",
  "lastName": "Le",
  "department": "Faculty of IT",
  "degree": "PhD",
  "phone": "0912345678"
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `username` | ✅ | min 6 ký tự |
| `email` | ✅ | valid email |
| `password` | ✅ | min 8 ký tự |
| `firstName` | ✅ | |
| `lastName` | ✅ | |
| `department` | ✅ | max 100 ký tự |
| `degree` | ✅ | max 50 ký tự |
| `phone` | ✅ | định dạng số điện thoại |

---

## 🎓 Students

### POST `/students`
**Permission:** `CREATE_STUDENT` — Tạo hồ sơ sinh viên cho user đã tồn tại
```json
{
  "userId": 5,
  "studentCode": "SV2023001",
  "dateOfBirth": "2003-05-15T00:00:00Z",
  "gender": "Female",
  "phone": "0901234567",
  "address": "456 Le Loi, HCM",
  "major": "Information Technology",
  "enrollmentYear": 2023,
  "status": "Active"
}
```

---

### PUT `/students/{id}`
**Permission:** `EDIT_STUDENT` — Tất cả fields đều optional (partial update)
```json
{
  "studentCode": "SV2023001",
  "dateOfBirth": "2003-05-15T00:00:00Z",
  "gender": "Female",
  "phone": "0901234567",
  "address": "456 Le Loi, HCM",
  "major": "Computer Science",
  "enrollmentYear": 2023,
  "status": "Active"
}
```
> Chỉ gửi các field muốn cập nhật. Field null/omit sẽ không thay đổi.

---

### POST `/students/import`
**Permission:** `IMPORT_STUDENTS` — Multipart form data
```
Content-Type: multipart/form-data
file: <file.csv>
```

---

## 👨‍🏫 Instructors

### POST `/instructors`
**Permission:** `CREATE_INSTRUCTOR` — Tạo hồ sơ giảng viên cho user đã tồn tại
```json
{
  "userId": 3,
  "instructorCode": "GV001",
  "department": "Faculty of IT",
  "degree": "PhD",
  "phone": "0912345678"
}
```

---

### PUT `/instructors/{id}`
**Permission:** `EDIT_INSTRUCTOR` — Tất cả fields đều optional
```json
{
  "instructorCode": "GV001",
  "department": "Faculty of Computer Science",
  "degree": "Associate Professor",
  "phone": "0912345678"
}
```

---

### POST `/instructors/import`
**Permission:** `IMPORT_INSTRUCTORS` — Multipart form data
```
Content-Type: multipart/form-data
file: <file.csv>
```

---

## 🏫 Classes

### POST `/classes`
**Permission:** `CREATE_CLASS`
```json
{
  "classCode": "IT101-03",
  "subjectId": 1,
  "instructorId": 1,
  "semester": 1,
  "academicYear": "2026-2027",
  "room": "A101",
  "schedule": "Mon/Wed 08:00-09:30",
  "maxEnrollment": 40
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `classCode` | ✅ | string |
| `subjectId` | ✅ | int ≥ 1 (Subject.Id từ subjects.csv) |
| `instructorId` | ✅ | int ≥ 1 (Instructor.Id từ instructors.csv, **không phải** User.Id) |
| `semester` | ✅ | 1, 2 hoặc 3 |
| `academicYear` | ✅ | vd: `"2026-2027"` |
| `room` | ❌ | |
| `schedule` | ❌ | |
| `maxEnrollment` | ✅ | 1–500 |

---

### POST `/classes/{classId}/enrollments`
**Permission:** `ENROLLMENTS`
```json
{
  "studentId": 3
}
```
> `studentId` là `Student.Id` từ students.csv, **không phải** User.Id.

---

### DELETE `/classes/{classId}/enrollments/{studentId}`
**Permission:** `GETOUT`
> Không có body. `classId` và `studentId` đều là path params (integer).

---

## 📝 Grades

### POST `/grades`
**Permission:** `ENTER_GRADE`
```json
{
  "enrollmentId": 1,
  "score": 8.5
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `enrollmentId` | ✅ | int ≥ 1 (Enrollment.Id từ enrollments.csv) |
| `score` | ✅ | 0.0 – 10.0 |

---

### PUT `/grades/{id}`
**Permission:** `EDIT_GRADE`
```json
{
  "score": 9.0
}
```

---

### POST `/grades/{enrollmentId}/submit`
**Permission:** `SUBMITTED` — Sinh viên nộp bài tập
```
Content-Type: multipart/form-data
file: <file>
```

---

## 📚 Subjects

### POST `/subjects`
**Permission:** `CREATE_SUB`
```json
{
  "subjectCode": "IT101",
  "name": "Introduction to Programming",
  "description": "Basic programming concepts",
  "credits": 3,
  "department": "Faculty of IT",
  "major": "Information Technology",
  "academicYear": "2026-2027",
  "semester": 1,
  "isRequired": true
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `subjectCode` | ✅ | |
| `name` | ✅ | |
| `description` | ❌ | |
| `credits` | ✅ | 1–10 |
| `department` | ✅ | |
| `major` | ✅ | |
| `academicYear` | ✅ | |
| `semester` | ✅ | 1, 2 hoặc 3 |
| `isRequired` | ❌ | boolean, mặc định `false` |

---

## 🎓 Majors

### POST `/majors`
**Permission:** `CREATE_MAJOR`
```json
{
  "majorCode": "CNTT",
  "name": "Information Technology",
  "description": "Bachelor of IT",
  "department": "Faculty of IT",
  "totalCredits": 130
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `majorCode` | ✅ | |
| `name` | ✅ | |
| `description` | ❌ | |
| `department` | ✅ | |
| `totalCredits` | ✅ | 1–300 |

---

## 📖 Courses

### POST `/courses`
**Permission:** `CREATE_COURSE`
```json
{
  "courseCode": "CS501",
  "name": "Advanced Algorithms",
  "description": "Advanced algorithm design",
  "credits": 3,
  "isRequired": false
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `courseCode` | ✅ | |
| `name` | ✅ | |
| `description` | ❌ | |
| `credits` | ✅ | 1–10 |
| `isRequired` | ❌ | boolean, mặc định `false` |

---

## 🔑 Permissions

### POST `/permissions`
**Permission:** `CREATE_PERMISSION`
```json
{
  "name": "VIEW_REPORT",
  "description": "View system reports"
}
```
| Field | Bắt buộc | Ràng buộc |
|-------|----------|-----------|
| `name` | ✅ | max 64 ký tự |
| `description` | ✅ | max 256 ký tự |

---

### PUT `/permissions/{id}`
**Permission:** `EDIT_PERMISSION` — Cả hai field đều optional
```json
{
  "name": "VIEW_REPORT_V2",
  "description": "View all system reports"
}
```

---

## 🛡️ Roles

### POST `/roles/{roleId}/permissions`
**Permission:** `GET_PERMISSION` — Gán permission cho role
```json
{
  "permissionId": 28
}
```
> `permissionId` là `Permission.Id` từ permissions.csv.

---

## 📊 Tổng hợp endpoints có body

| Method | Endpoint | Body / Notes |
|--------|----------|------|
| POST | `/auth/login` | `username`, `password` |
| POST | `/auth/register` | `email`, `username`, `password` |
| POST | `/auth/refresh` | `accessToken` |
| POST | `/auth/logout` | _(no body)_ |
| PUT | `/users/me` | `email`, `firstName`, `lastName` |
| POST | `/users/student` | credentials + student profile |
| POST | `/users/instructor` | credentials + instructor profile |
| POST | `/students` | `userId` + student profile |
| PUT | `/students/{id}` | partial student profile |
| POST | `/students/import` | `multipart/form-data` file |
| POST | `/instructors` | `userId` + instructor profile |
| PUT | `/instructors/{id}` | partial instructor profile |
| POST | `/instructors/import` | `multipart/form-data` file |
| POST | `/classes` | class info |
| POST | `/classes/{classId}/enrollments` | `studentId` |
| DELETE | `/classes/{classId}/enrollments/{studentId}` | _(no body)_ |
| POST | `/grades` | `enrollmentId`, `score` |
| PUT | `/grades/{id}` | `score` |
| POST | `/grades/{enrollmentId}/submit` | `multipart/form-data` file |
| POST | `/subjects` | subject info |
| POST | `/majors` | major info |
| POST | `/courses` | course info |
| POST | `/permissions` | `name`, `description` |
| PUT | `/permissions/{id}` | `name?`, `description?` |
| POST | `/roles/{roleId}/permissions` | `permissionId` |
