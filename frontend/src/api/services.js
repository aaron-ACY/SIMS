import api from './axiosClient';

export const authService = {
  login: (username, password) => api.post('/auth/login', { username, password }),
  register: (username, password, email) => api.post('/auth/register', { username, password, email }),
  refresh: (token) => api.post('/auth/refresh', { token }),
  logout: () => api.post('/auth/logout'),
};

export const userService = {
  getMe: () => api.get('/users/me'),
  updateMe: (data) => api.put('/users/me', data),
  getUsers: () => api.get('/users'),
  createUser: (data) => api.post('/users', data),
  createStudent: (data) => api.post('/users/student', data),
  createInstructor: (data) => api.post('/users/instructor', data),
  deleteUser: (id) => api.delete(`/users/${id}`),
};

export const studentService = {
  getStudents: () => api.get('/students'),
  getStudentById: (id) => api.get(`/students/${id}`),
  createStudent: (data) => api.post('/students', data),
  updateStudent: (id, data) => api.put(`/students/${id}`, data),
  deleteStudent: (id) => api.delete(`/students/${id}`),
  importStudents: (file) => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/students/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};

export const instructorService = {
  getInstructors: () => api.get('/instructors'),
  getInstructorById: (id) => api.get(`/instructors/${id}`),
  createInstructor: (data) => api.post('/instructors', data),
  updateInstructor: (id, data) => api.put(`/instructors/${id}`, data),
  deleteInstructor: (id) => api.delete(`/instructors/${id}`),
  importInstructors: (file) => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post('/instructors/import', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};

export const courseService = {
  getCourses: () => api.get('/courses'),
  createCourse: (data) => api.post('/courses', data),
  deleteCourse: (id) => api.delete(`/courses/${id}`),
};

export const subjectService = {
  getSubjects: () => api.get('/subjects'),
  createSubject: (data) => api.post('/subjects', data),
  deleteSubject: (id) => api.delete(`/subjects/${id}`),
};

export const classService = {
  getClasses: () => api.get('/classes'),
  createClass: (data) => api.post('/classes', data),
  getEnrollments: (classId) => api.get(`/classes/${classId}/enrollments`),
  enrollStudent: (classId, data) => api.post(`/classes/${classId}/enrollments`, data),
  removeStudent: (classId, studentId) => api.delete(`/classes/${classId}/enrollments/${studentId}`),
};

export const gradeService = {
  submitAssignment: (enrollmentId, file) => {
    const formData = new FormData();
    formData.append('file', file);
    return api.post(`/grades/${enrollmentId}/submit`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
  enterGrade: (data) => api.post('/grades', data),
  updateGrade: (id, data) => api.put(`/grades/${id}`, data),
  getStudentGrades: (studentCode) => api.get(`/grades/student/${studentCode}`),
};

export const permissionService = {
  getPermissions: () => api.get('/permissions'),
  createPermission: (data) => api.post('/permissions', data),
  updatePermission: (id, data) => api.put(`/permissions/${id}`, data),
};

export const roleService = {
  assignPermissions: (roleId, data) => api.post(`/roles/${roleId}/permissions`, data),
};
