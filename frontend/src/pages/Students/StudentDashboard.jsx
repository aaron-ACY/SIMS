import React, { useState, useEffect } from 'react';
import { BookOpen, GraduationCap, Clock, Award } from 'lucide-react';
import { motion } from 'framer-motion';

import ScheduleCard from '../../components/dashboard/ScheduleCard';
import AnnouncementCard from '../../components/dashboard/AnnouncementCard';
import AssignmentSummary from '../../components/student/dashboard/AssignmentSummary';
import OverviewCard from '../../components/dashboard/OverviewCard';
import { userService, studentService, gradeService } from '../../api/services';

const StudentDashboard = () => {
  // API States
  const [isLoading, setIsLoading] = useState(false);
  
  // Dashboard Data
  const [studentInfo, setStudentInfo] = useState({
    name: null,
    semester: null,
    academicYear: null
  });

  const [stats, setStats] = useState({
    enrolledClasses: null,
    totalCredits: null,
    currentGPA: null
  });

  const [assignments, setAssignments] = useState([]);
  const [scheduleData, setScheduleData] = useState([]);
  const [announcements, setAnnouncements] = useState([]);

  useEffect(() => {
    const fetchDashboardData = async () => {
      setIsLoading(true);
      try {
        const userRes = await userService.getMe();
        if (!userRes.success || !userRes.result?.studentCode) return;
        const user = userRes.result;
        
        setStudentInfo({
          name: `${user.firstName} ${user.lastName}`.trim(),
          semester: 1, // Placeholder
          academicYear: '2026-2027' // Placeholder
        });

        const [classesRes, gradesRes] = await Promise.all([
          studentService.getMyClasses(),
          gradeService.getStudentGrades(user.studentCode)
        ]);

        const classes = (classesRes.success ? classesRes.result : []) || [];
        const gradesData = (gradesRes.success && gradesRes.result ? gradesRes.result.classes : []) || [];

        // Calculate GPA
        let gpa = 0;
        const allGrades = gradesData.flatMap(c => c.grades);
        if (allGrades.length > 0) {
          const sum = allGrades.reduce((acc, curr) => acc + curr.scores, 0);
          const avg = sum / allGrades.length;
          gpa = ((avg / 10) * 4).toFixed(2);
        }

        setStats({
          enrolledClasses: classes.length,
          totalCredits: classes.length * 3, // Assuming 3 credits per class
          currentGPA: gpa
        });

        // Format assignments
        const recentAssignments = classes.map(cls => {
          let status = 'Pending';
          const classGradeGroup = gradesData.find(g => g.classCode === cls.classCode);
          if (classGradeGroup && classGradeGroup.grades && classGradeGroup.grades.length > 0) {
            status = (classGradeGroup.grades[0].scores > 0 || classGradeGroup.grades[0].rating) ? 'Graded' : 'Submitted';
          }
          return {
            id: cls.enrollmentId,
            title: `${cls.subjectName} Assignment`,
            class: cls.classCode,
            status: status,
            dueDate: 'End of Semester'
          };
        });
        setAssignments(recentAssignments.slice(0, 5));

        // Format Schedule
        const sched = classes.map(cls => ({
          id: cls.classId,
          title: cls.subjectName,
          time: cls.schedule || 'TBA',
          room: cls.room || 'TBA',
          type: 'Lecture'
        }));
        setScheduleData(sched);

      } catch (err) {
        console.error('Failed to fetch dashboard data', err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchDashboardData();
  }, []);

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      {/* Welcome Section */}
      <div className="bg-gradient-to-r from-[var(--theme-primary)] to-[var(--theme-primaryDark)] text-white p-6 sm:p-8 rounded-2xl flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 shadow-sm">
        <div>
          <h2 className="text-2xl sm:text-3xl font-black tracking-tight mb-2">
            Welcome back, {studentInfo.name || 'Student'}!
          </h2>
          <p className="text-sm font-medium text-white/80">
            {studentInfo.semester ? `Semester ${studentInfo.semester}` : '--'} | Academic Year {studentInfo.academicYear || '--'}
          </p>
        </div>
        <div className="px-4 py-2 bg-white/10 backdrop-blur-md rounded-xl text-xs font-bold uppercase tracking-widest border border-white/20">
          Student Account
        </div>
      </div>

      {/* My Classes Summary */}
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <OverviewCard 
          title="Enrolled Classes" 
          value={stats.enrolledClasses} 
          icon={BookOpen} 
          isLoading={isLoading} 
          colorClass="text-blue-500" 
          bgClass="bg-blue-500/10" 
        />
        <OverviewCard 
          title="Total Credits" 
          value={stats.totalCredits} 
          icon={Clock} 
          isLoading={isLoading} 
          colorClass="text-purple-500" 
          bgClass="bg-purple-500/10" 
        />
        <OverviewCard 
          title="Current GPA" 
          value={stats.currentGPA} 
          icon={Award} 
          isLoading={isLoading} 
          colorClass="text-amber-500" 
          bgClass="bg-amber-500/10" 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 flex flex-col gap-6">
          <AssignmentSummary assignments={assignments} isLoading={isLoading} />
          <AnnouncementCard announcements={announcements} isLoading={isLoading} />
        </div>
        
        <div className="flex flex-col gap-6">
          <ScheduleCard scheduleData={scheduleData} isLoading={isLoading} />
        </div>
      </div>
    </motion.div>
  );
};

export default StudentDashboard;