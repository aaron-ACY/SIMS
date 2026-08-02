import React, { useState, useEffect } from 'react';
import { School, Users, GraduationCap, CheckCircle } from 'lucide-react';
import { motion } from 'framer-motion';

import OverviewCard from '../../components/dashboard/OverviewCard';
import ScheduleCard from '../../components/dashboard/ScheduleCard';
import ActivityCard from '../../components/dashboard/ActivityCard';
import AnnouncementCard from '../../components/dashboard/AnnouncementCard';
import DeadlineCard from '../../components/dashboard/DeadlineCard';

const LecturerDashboard = () => {
  // API States
  const [isLoading, setIsLoading] = useState(false);
  
  // Dashboard Data
  const [stats, setStats] = useState({
    classesCount: null,
    studentsCount: null,
    assignmentsCount: null,
    pendingGrading: null
  });
  
  const [scheduleData, setScheduleData] = useState([]);
  const [activities, setActivities] = useState([]);
  const [announcements, setAnnouncements] = useState([]);
  const [deadlines, setDeadlines] = useState([]);

  // Future API Integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchDashboardData().then(data => {
    //   setStats(data.stats);
    //   setScheduleData(data.schedule);
    //   setActivities(data.activities);
    //   setAnnouncements(data.announcements);
    //   setDeadlines(data.deadlines);
    // }).finally(() => setIsLoading(false));
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
        <div className="px-4 py-2 bg-white/10 backdrop-blur-md rounded-xl text-xs font-bold uppercase tracking-widest border border-white/20">
          Instructor Account
        </div>
      </div>

      {/* Stats Widgets */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
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
        <OverviewCard 
          title="Assignments" 
          value={stats.assignmentsCount} 
          icon={GraduationCap} 
          isLoading={isLoading} 
          colorClass="text-amber-500" 
          bgClass="bg-amber-500/10" 
        />
        <OverviewCard 
          title="To Grade" 
          value={stats.pendingGrading} 
          icon={CheckCircle} 
          isLoading={isLoading} 
          colorClass="text-purple-500" 
          bgClass="bg-purple-500/10" 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 flex flex-col gap-6">
          <ActivityCard activities={activities} isLoading={isLoading} />
          <AnnouncementCard announcements={announcements} isLoading={isLoading} />
        </div>
        
        <div className="flex flex-col gap-6">
          <ScheduleCard scheduleData={scheduleData} isLoading={isLoading} />
          <DeadlineCard deadlines={deadlines} isLoading={isLoading} />
        </div>
      </div>
    </motion.div>
  );
};

export default LecturerDashboard;