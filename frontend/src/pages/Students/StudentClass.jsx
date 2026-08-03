import React, { useState, useEffect } from 'react';
import { School, User, Mail, Calendar } from 'lucide-react';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import EmptyClasses from '../../components/student/classes/EmptyClasses';
import { studentService } from '../../api/services';
const StudentClass = () => {
  const navigate = useNavigate();
  const [isLoading, setIsLoading] = useState(false);
  const [classList, setClassList] = useState([]);

  useEffect(() => {
    const fetchClasses = async () => {
      setIsLoading(true);
      try {
        const res = await studentService.getMyClasses();
        if (res.success && res.result) {
          setClassList(res.result);
        }
      } catch (err) {
        console.error('Failed to fetch student classes', err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchClasses();
  }, []);

  const handleRowClick = (classId) => {
    navigate(`/student/class/view/${classId}`);
  };

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      <div>
        <h2 className="text-3xl font-black tracking-tight">My Classes</h2>
        <p className="text-sm text-[var(--theme-textMuted)] font-bold">List of all your active classes for the current semester.</p>
      </div>

      {/* Class List Section */}
      <div className="bg-[var(--theme-sidebarBg)] rounded-2xl border border-[var(--theme-border)] shadow-sm overflow-hidden transition-all duration-500">
        <div className="px-8 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30">
          <h3 className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]">Class List</h3>
        </div>

        <div className="overflow-x-auto">
          {isLoading ? (
             <div className="py-20 flex flex-col items-center justify-center">
               <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
               <p className="text-[var(--theme-textMuted)] font-medium">Loading classes...</p>
             </div>
          ) : classList.length > 0 ? (
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-[var(--theme-hover)]/30 text-[var(--theme-textMuted)] text-[11px] font-black uppercase tracking-widest border-b border-[var(--theme-border)]">
                  <th className="py-5 px-6">Class ID</th>
                  <th className="py-5 px-6">Class Name</th>
                  <th className="py-5 px-6">Subject</th>
                  <th className="py-5 px-6">Lecturer</th>
                  <th className="py-5 px-6">Semester</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-[var(--theme-border)]">
                {classList.map((cls, idx) => (
                  <tr 
                    key={cls.classId || idx} 
                    onClick={() => handleRowClick(cls.classId)}
                    className="group hover:bg-[var(--theme-hover)]/30 transition-colors cursor-pointer"
                  >
                    <td className="py-5 px-6 font-mono text-sm group-hover:no-underline">{cls.classId}</td>
                    <td className="py-5 px-6 font-bold text-sm text-[var(--theme-text)] group-hover:text-[var(--theme-primary)] transition-colors group-hover:no-underline">{cls.classCode}</td>
                    <td className="py-5 px-6 font-semibold text-sm text-[var(--theme-textMuted)] group-hover:no-underline">{cls.subjectName}</td>
                    <td className="py-5 px-6 font-semibold text-sm text-[var(--theme-textMuted)] group-hover:no-underline">{cls.instructorName}</td>
                    <td className="py-5 px-6 font-semibold text-sm text-[var(--theme-textMuted)] group-hover:no-underline">{cls.semester}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <EmptyClasses />
          )}
        </div>
      </div>
    </motion.div>
  );
};

export default StudentClass;
