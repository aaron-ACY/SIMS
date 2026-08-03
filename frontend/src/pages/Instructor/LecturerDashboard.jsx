import React, { useState, useEffect } from 'react';
import { School, Users } from 'lucide-react';
import { motion } from 'framer-motion';

import { userService, classService, gradeService } from '../../api/services';

import OverviewCard from '../../components/dashboard/OverviewCard';
import ScheduleCard from '../../components/dashboard/ScheduleCard';
import ActivityCard from '../../components/dashboard/ActivityCard';
import AnnouncementCard from '../../components/dashboard/AnnouncementCard';

const LecturerDashboard = () => {
  // API States
  const [isLoading, setIsLoading] = useState(true);
  
  // Dashboard Data
  const [stats, setStats] = useState({
    classesCount: 0,
    studentsCount: 0,
  });
  
  const [scheduleData, setScheduleData] = useState([]);
  const [activities, setActivities] = useState([]);
  const [announcements, setAnnouncements] = useState([]);

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        setIsLoading(true);
        
        // Fetch current user (instructor)
        const userRes = await userService.getMe();
        if (!userRes.success || !userRes.result) return;
        const user = userRes.result;

        // Fetch all classes and filter by this instructor's code
        const classesRes = await classService.getClasses();
        let myClasses = [];
        if (classesRes.success && classesRes.result) {
          myClasses = classesRes.result.filter(c => c.instructorCode === user.instructorCode);
        }

        const totalStudents = myClasses.reduce((acc, curr) => acc + (curr.currentEnrollment || 0), 0);

        setStats({
          classesCount: myClasses.length,
          studentsCount: totalStudents,
        });

        // Generate Mock Schedule from real classes
        const mockSchedule = myClasses.map((c, index) => {
          const hours = 8 + (index * 2);
          const time = `${hours.toString().padStart(2, '0')}:00 - ${(hours + 2).toString().padStart(2, '0')}:00`;
          return {
            time: time,
            course: c.subjectName || c.classCode,
            room: `Room ${101 + index}`
          };
        });
        setScheduleData(mockSchedule);

        // Fetch grades for activities
        let allActivities = [];
        for (const cls of myClasses) {
          try {
            const gradesRes = await gradeService.getClassGrades(cls.id);
            if (gradesRes.success && gradesRes.result) {
              const submissions = gradesRes.result.filter(g => g.submissionPath);
              submissions.forEach(sub => {
                allActivities.push({
                  studentName: sub.studentName,
                  classCode: sub.classCode || cls.classCode,
                  updatedAt: new Date(sub.updatedAt),
                  isGraded: sub.score !== null
                });
              });
            }
          } catch (e) {
            console.error(`Failed to fetch grades for class ${cls.id}`, e);
          }
        }

        // Sort activities by most recent and take top 5
        allActivities.sort((a, b) => b.updatedAt - a.updatedAt);
        setActivities(allActivities.slice(0, 5));

      } catch (error) {
        console.error("Error fetching dashboard data:", error);
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
      {/* Welcome Banner */}
      <div className="bg-gradient-to-r from-[var(--theme-primary)] to-[var(--theme-primaryDark)] text-white p-6 sm:p-8 rounded-2xl flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 shadow-sm">
        <div>
          <h2 className="text-2xl sm:text-3xl font-black tracking-tight mb-2">Instructor Dashboard</h2>
          <p className="text-sm font-medium text-white/80">Manage your classes, review schedules, and track assignments grading progress.</p>
        </div>
      </div>

      {/* Stats Widgets */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <OverviewCard 
          title="Total Classes" 
          value={stats.classesCount} 
          icon={School} 
          isLoading={isLoading} 
          colorClass="text-blue-500" 
          bgClass="bg-blue-500/10" 
        />
        <OverviewCard 
          title="Total Students" 
          value={stats.studentsCount} 
          icon={Users} 
          isLoading={isLoading} 
          colorClass="text-emerald-500" 
          bgClass="bg-emerald-500/10" 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 flex flex-col gap-6">
          <ActivityCard activities={activities} isLoading={isLoading} />
        </div>
        
        <div className="flex flex-col gap-6">
          <ScheduleCard scheduleData={scheduleData} isLoading={isLoading} />
        </div>
      </div>
    </motion.div>
  );
};

export default LecturerDashboard;