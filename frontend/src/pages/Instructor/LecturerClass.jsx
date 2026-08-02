import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { School, Users, Eye, BookOpen, Clock, Edit } from 'lucide-react';
import { motion } from 'framer-motion';
import PageHeader from '../../components/Shared/PageHeader';
import EmptyState from '../../components/Shared/EmptyState';

const LecturerClass = () => {
  const navigate = useNavigate();

  const [classesList, setClassesList] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  // Future API integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchClasses().then(data => setClassesList(data)).finally(() => setIsLoading(false));
  }, []);

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      <PageHeader 
        title="My Classes"
        description="Manage class lists, schedule courses, and grade assignments."
      />

      {isLoading ? (
        <div className="py-20 flex flex-col items-center justify-center">
           <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
           <p className="text-[var(--theme-textMuted)] font-medium">Loading classes...</p>
        </div>
      ) : classesList.length > 0 ? (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {classesList.map((item, idx) => (
            <div key={item.id || idx} className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 flex flex-col justify-between hover:border-[var(--theme-primary)]/40 transition-all duration-300 shadow-sm">
              <div>
                <div className="flex justify-between items-start mb-4">
                  <span className="px-2.5 py-1 bg-[var(--theme-hover)] text-[var(--theme-text)]/70 text-xs font-black rounded-lg font-mono">
                    {item.code}
                  </span>
                  <span className="text-xs font-black uppercase text-[var(--theme-primary)] tracking-wider">
                    Room: {item.room}
                  </span>
                </div>
                
                <h3 className="text-xl font-bold text-[var(--theme-text)] mb-1">{item.name}</h3>
                <p className="text-xs text-[var(--theme-text)]/50 font-bold mb-6 flex items-center gap-1">
                  <BookOpen size={12} />
                  Subject Code: {item.subject}
                </p>

                <div className="space-y-2 border-t border-[var(--theme-border)] pt-4 mb-6">
                  <div className="flex justify-between items-center text-sm font-semibold">
                    <span className="text-[var(--theme-text)]/60 flex items-center gap-1.5 font-bold"><Users size={16} /> Students Enrolled</span>
                    <span>{item.students} students</span>
                  </div>
                  <div className="flex justify-between items-center text-sm font-semibold">
                    <span className="text-[var(--theme-text)]/60 flex items-center gap-1.5 font-bold"><Clock size={16} /> Weekly Schedule</span>
                    <span className="text-right">{item.schedule}</span>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-3 mt-auto">
                <button 
                  onClick={() => navigate(`/lecturer/class/view/${item.code}`)}
                  className="w-full py-2.5 bg-[var(--theme-bg)] text-[var(--theme-text)] border border-[var(--theme-border)] hover:bg-[var(--theme-hover)] font-bold text-xs uppercase tracking-widest rounded-xl flex items-center justify-center gap-2 transition-all active:scale-[0.99]"
                >
                  <Eye size={14} />
                  View Details
                </button>
                <button 
                  onClick={() => navigate(`/lecturer/class/grading/${item.code}`)}
                  className="w-full py-2.5 bg-[var(--theme-primary)] text-white hover:bg-[var(--theme-primaryDark)] font-bold text-xs uppercase tracking-widest rounded-xl flex items-center justify-center gap-2 transition-all active:scale-[0.99] shadow-sm hover:shadow"
                >
                  <Edit size={14} />
                  Grade Assignment
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <EmptyState 
          icon={School}
          title="No Classes Assigned"
          description="You are not currently assigned to any classes for this semester."
        />
      )}
    </motion.div>
  );
};

export default LecturerClass;
