import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { ArrowLeft, Search, FileSpreadsheet, School, BookOpen, Award, BarChart3 } from 'lucide-react';
import PageHeader from '../../components/Shared/PageHeader';
import EmptyState from '../../components/Shared/EmptyState';

const LecturerViewClass = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [searchQuery, setSearchQuery] = useState('');
  
  const [classInfo, setClassInfo] = useState(null);
  const [studentsList, setStudentsList] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  // Future API Integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchClassDetails(id).then(data => {
    //   setClassInfo(data);
    //   setStudentsList(data.studentsList);
    // }).finally(() => setIsLoading(false));
  }, [id]);

  const filteredStudents = studentsList.filter(student => 
    student.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    student.code?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      {/* Top action row */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <button 
            onClick={() => navigate('/lecturer/class')}
            className="p-2.5 bg-[var(--theme-sidebarBg)] hover:bg-[var(--theme-hover)] text-[var(--theme-text)]/75 rounded-xl transition-colors border border-[var(--theme-border)] shadow-sm cursor-pointer"
          >
            <ArrowLeft size={18} />
          </button>
          <div>
            <h2 className="text-3xl font-black tracking-tight">Class Details: {id}</h2>
            <p className="text-sm text-[var(--theme-text)]/50 font-bold">{classInfo?.name || 'Loading...'}</p>
          </div>
        </div>

        <button 
          onClick={() => {
            alert('Roster list exported successfully!');
          }}
          className="flex items-center gap-1.5 px-4 py-2 bg-[var(--theme-hover)] hover:bg-[var(--theme-border)] text-[var(--theme-text)] border border-[var(--theme-border)] rounded-xl font-bold text-xs uppercase tracking-widest transition-all shadow-sm cursor-pointer"
        >
          <FileSpreadsheet size={14} />
          Export Roster
        </button>
      </div>

      {isLoading ? (
        <div className="py-20 flex flex-col items-center justify-center">
           <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
           <p className="text-[var(--theme-textMuted)] font-medium">Loading class details...</p>
        </div>
      ) : classInfo ? (
        <>
          {/* Class Profile Metric Grid */}
          <div className="grid grid-cols-1 gap-6">
            {/* Class Details Card */}
            <div className="bg-[var(--theme-sidebarBg)] p-6 rounded-2xl border border-[var(--theme-border)] shadow-sm space-y-4">
              <h4 className="text-xs font-black uppercase tracking-widest text-[var(--theme-text)]/40 flex items-center gap-1.5">
                <School size={14} /> Course Profile
              </h4>
              <div className="space-y-2.5">
                <div className="flex justify-between items-baseline text-sm font-semibold">
                  <span className="text-[var(--theme-text)]/50 font-bold">Subject:</span>
                  <span>{classInfo.name}</span>
                </div>
                <div className="flex justify-between items-baseline text-sm font-semibold">
                  <span className="text-[var(--theme-text)]/50 font-bold">Subject Code:</span>
                  <span className="font-mono text-xs px-2 py-0.5 bg-[var(--theme-hover)] rounded-md">{classInfo.subject}</span>
                </div>
                <div className="flex justify-between items-baseline text-sm font-semibold">
                  <span className="text-[var(--theme-text)]/50 font-bold">Classroom:</span>
                  <span>{classInfo.room}</span>
                </div>
                <div className="flex justify-between items-baseline text-sm font-semibold">
                  <span className="text-[var(--theme-text)]/50 font-bold">Schedule:</span>
                  <span>{classInfo.schedule}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Search & Roster list */}
          <div className="bg-[var(--theme-sidebarBg)] rounded-2xl border border-[var(--theme-border)] shadow-sm overflow-hidden">
            
            {/* Roster header with search bar */}
            <div className="px-8 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
              <div>
                <h3 className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]/70">
                  Student Roster ({filteredStudents.length} / {studentsList.length})
                </h3>
              </div>

              <div className="relative w-full sm:w-64">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--theme-text)]/40" size={14} />
                <input 
                  type="text" 
                  placeholder="Search student code or name..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  className="w-full pl-9 pr-4 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl outline-none text-xs font-bold text-[var(--theme-text)] focus:border-[var(--theme-primary)] transition-colors"
                />
              </div>
            </div>

            <div className="overflow-x-auto">
              {filteredStudents.length > 0 ? (
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-[var(--theme-hover)]/30 text-[var(--theme-textMuted)] text-[10px] font-black uppercase tracking-widest border-b border-[var(--theme-border)]">
                      <th className="py-4 px-6">Student ID</th>
                      <th className="py-4 px-6">Student Name</th>
                      <th className="py-4 px-6">Email Address</th>
                      <th className="py-4 px-6 font-mono">Phone Number</th>
                      <th className="py-4 px-6 text-center">Grade Avg</th>
                      <th className="py-4 px-6 text-center">Status</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-[var(--theme-border)]">
                    {filteredStudents.map((student, idx) => (
                      <tr key={idx} className="group hover:bg-[var(--theme-hover)]/30 transition-colors">
                        <td className="py-4 px-6 font-mono font-bold text-sm text-[var(--theme-primary)]">{student.code}</td>
                        <td className="py-4 px-6 font-bold text-sm text-[var(--theme-text)]">{student.name}</td>
                        <td className="py-4 px-6 font-normal text-sm text-[var(--theme-textMuted)]">{student.email}</td>
                        <td className="py-4 px-6 font-mono text-sm text-[var(--theme-textMuted)]">{student.phone}</td>
                        <td className="py-4 px-6 font-mono text-sm text-center font-bold text-[var(--theme-text)]">{student.avg}/100</td>
                        <td className="py-4 px-6 text-center">
                          <span className={`px-2.5 py-1 rounded-lg text-[10px] font-black tracking-wider uppercase ${
                            student.status === 'Excellent'
                              ? 'bg-green-500/10 text-green-500'
                              : student.status === 'On Track'
                                ? 'bg-blue-500/10 text-blue-500'
                                : 'bg-red-500/10 text-red-500'
                          }`}>
                            {student.status}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <div className="py-12 text-center text-[var(--theme-textMuted)] font-semibold text-sm">
                  No students found matching your search.
                </div>
              )}
            </div>
          </div>
        </>
      ) : (
        <EmptyState 
          icon={School}
          title="Class Not Found"
          description="The class details you are looking for do not exist or you don't have permission to view them."
        />
      )}
    </motion.div>
  );
};

export default LecturerViewClass;
