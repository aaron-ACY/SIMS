import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import PrivateRoute from "./routes/PrivateRoute";

import Home from "./pages/Home/Home";
import Login from "./pages/Auth/Login";
import Register from "./pages/Auth/Register";
import StudentDashboard from "./pages/Students/StudentDashboard";
import StudentClass from "./pages/Students/StudentClass";
import StudentViewClass from "./pages/Students/StudentViewClass";
import StudentMaterials from "./pages/Students/StudentMaterials";
import StudentAssignments from "./pages/Students/StudentAssignments";
import StudentSubmitAssignment from "./pages/Students/StudentSubmitAssignment";
import StudentProfile from "./pages/Students/StudentProfile";

import LecturerDashboard from "./pages/Instructor/LecturerDashboard";
import LecturerClass from "./pages/Instructor/LecturerClass";
import LecturerViewClass from "./pages/Instructor/LecturerViewClass";
import LecturerProfile from "./pages/Instructor/LecturerProfile";
import AssignmentGradingPage from "./pages/Instructor/AssignmentGradingPage";

import AdminLayout from "./components/Layout/AdminLayout";
import StudentLayout from "./components/Layout/StudentLayout";
import LecturerLayout from "./components/Layout/LecturerLayout";
import Dashboard from "./pages/Admin/Dashboard/Dashboard";
import StudentList from "./pages/Admin/UserManagement/StudentList";
import InstructorList from "./pages/Admin/UserManagement/InstructorList";
import SubjectList from "./pages/Admin/UserManagement/SubjectList";
import ClassList from "./pages/Admin/UserManagement/ClassList";
import ViewClass from "./pages/Admin/UserManagement/ViewClass";
import CourseList from "./pages/Admin/CourseManager/CourseList";
import Reports from "./pages/Admin/Reports/Reports";

import { ThemeProvider } from "./context/ThemeContext";

function App() {
  return (
    <ThemeProvider>
      <Router>
        <AuthProvider>
          <Routes>
            {/* Public Routes */}
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* Admin Routes */}
            <Route
              path="/admin"
              element={
                <PrivateRoute allowedRoles={["ADMIN"]}>
                  <AdminLayout />
                </PrivateRoute>
              }
            >
              <Route index element={<Navigate to="/admin/dashboard" replace />} />
              <Route path="dashboard" element={<Dashboard />} />
              <Route path="students" element={<StudentList />} />
              <Route path="instructors" element={<InstructorList />} />
              <Route path="subjects" element={<SubjectList />} />
              <Route path="classes" element={<ClassList />} />
              <Route path="classes/view/:id" element={<ViewClass />} />
              <Route path="courses" element={<CourseList />} />
              <Route path="reports" element={<Reports />} />
            </Route>

            {/* Student Routes */}
            <Route
              path="/student"
              element={
                <PrivateRoute allowedRoles={["STUDENT"]}>
                  <StudentLayout />
                </PrivateRoute>
              }
            >
              <Route index element={<Navigate to="/student/dashboard" replace />} />
              <Route path="dashboard" element={<StudentDashboard />} />
              <Route path="class" element={<StudentClass />} />
              <Route path="class/view/:id" element={<StudentViewClass />} />
              <Route path="materials" element={<StudentMaterials />} />
              <Route path="assignments" element={<StudentAssignments />} />
              <Route path="assignments/view/:id" element={<StudentSubmitAssignment />} />
              <Route path="profile" element={<StudentProfile />} />
            </Route>

            {/* Lecturer Routes */}
            <Route
              path="/lecturer"
              element={
                <PrivateRoute allowedRoles={["LECTURER"]}>
                  <LecturerLayout />
                </PrivateRoute>
              }
            >
              <Route index element={<Navigate to="/lecturer/dashboard" replace />} />
              <Route path="dashboard" element={<LecturerDashboard />} />
              <Route path="class" element={<LecturerClass />} />
              <Route path="class/view/:id" element={<LecturerViewClass />} />
              <Route path="class/grading/:classId" element={<AssignmentGradingPage />} />
              <Route path="profile" element={<LecturerProfile />} />
            </Route>
          </Routes>
        </AuthProvider>
      </Router>
    </ThemeProvider>
  );
}

export default App;
