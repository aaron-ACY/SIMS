import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import {
  Users,
  GraduationCap,
  BookOpen,
  TrendingUp,
  Calendar
} from 'lucide-react';
import CSVUploader from '../../../components/Shared/CSVUploader';
import PageHeader from '../../../components/Shared/PageHeader';
import { studentService, instructorService, courseService, classService } from '../../../api/services';

const Dashboard = () => {
  // Ready for API integration
  const [stats, setStats] = useState([
    { label: 'Total Students', value: '-', trend: '-', isUp: true, icon: GraduationCap },
    { label: 'Total Courses', value: '-', trend: '-', isUp: true, icon: BookOpen },
    { label: 'Total Instructors', value: '-', trend: '-', isUp: true, icon: Users },
    { label: 'Active Classes', value: '-', trend: '-', isUp: true, icon: TrendingUp },
  ]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchDashboardStats = async () => {
      setIsLoading(true);
      try {
        const [studentsRes, instructorsRes, coursesRes, classesRes] = await Promise.all([
          studentService.getStudents().catch(() => ({ result: [] })),
          instructorService.getInstructors().catch(() => ({ result: [] })),
          courseService.getCourses().catch(() => ({ result: [] })),
          classService.getClasses().catch(() => ({ result: [] }))
        ]);

        setStats([
          { label: 'Total Students', value: studentsRes.result?.length || 0, trend: '+0%', isUp: true, icon: GraduationCap },
          { label: 'Total Courses', value: coursesRes.result?.length || 0, trend: '+0%', isUp: true, icon: BookOpen },
          { label: 'Total Instructors', value: instructorsRes.result?.length || 0, trend: '+0%', isUp: true, icon: Users },
          { label: 'Active Classes', value: classesRes.result?.length || 0, trend: '+0%', isUp: true, icon: TrendingUp },
        ]);
      } catch (error) {
        console.error('Failed to fetch stats:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchDashboardStats();
  }, []);

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-8 text-[var(--theme-text)] p-6"
    >
      <PageHeader 
        title="Overview Dashboard"
        description="Welcome back! Here's what's happening today."
        actions={
          <div className="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
            <button className="flex items-center justify-center gap-2 px-4 py-3 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-xl text-sm font-bold text-[var(--theme-text)] hover:bg-[var(--theme-hover)] transition-all shadow-sm w-full sm:w-auto">
              <Calendar size={18} />
              Today
            </button>
          </div>
        }
      />

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, idx) => (
          <motion.div
            key={idx}
            whileHover={{ y: -5 }}
            className="bg-[var(--theme-sidebarBg)] p-6 rounded-2xl border border-[var(--theme-border)] shadow-sm relative overflow-hidden group transition-all duration-300"
          >
            <div className="absolute top-0 right-0 w-24 h-24 bg-[var(--theme-primary)]/5 rounded-full -mr-8 -mt-8 transition-transform group-hover:scale-150 duration-700"></div>

            <div className="flex items-start justify-between mb-4 relative z-10">
              <div className="w-12 h-12 rounded-xl flex items-center justify-center text-[var(--theme-primary)] bg-[var(--theme-hover)] transition-colors duration-300">
                <stat.icon size={24} />
              </div>
            </div>

            <div className="relative z-10">
              <p className="text-[var(--theme-textMuted)] text-xs font-bold uppercase tracking-widest">{stat.label}</p>
              <h3 className="text-3xl font-black text-[var(--theme-text)] mt-1">
                {isLoading ? '...' : stat.value}
              </h3>
            </div>
          </motion.div>
        ))}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <CSVUploader 
          title="Import Students CSV"
          description="Upload student data using CSV files to batch register."
          onImport={async (file) => {
            const res = await studentService.importStudents(file);
            if (!res.success) throw new Error(res.message);
            return res.message;
          }}
        />
        <CSVUploader 
          title="Import Instructors CSV"
          description="Upload instructor data using CSV files to batch register."
          onImport={async (file) => {
            const res = await instructorService.importInstructors(file);
            if (!res.success) throw new Error(res.message);
            return res.message;
          }}
        />
      </div>
    </motion.div>
  );
};

export default Dashboard;
