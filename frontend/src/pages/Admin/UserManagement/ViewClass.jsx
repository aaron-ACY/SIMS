import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  School, 
  ArrowLeft,
  Search,
  GraduationCap,
  Sparkles
} from 'lucide-react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { classService } from '../../../api/services';

const ViewClass = () => {
  const navigate = useNavigate();
  const { id } = useParams(); // Note: id is currently classCode from ClassList

  const [isLoading, setIsLoading] = useState(true);
  const [classInfo, setClassInfo] = useState(null);
  const [students, setStudents] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        // Fetch class details from the class list
        const classRes = await classService.getClasses();
        if (classRes.success && classRes.result) {
          const foundClass = classRes.result.find(c => c.classCode === id);
          if (foundClass) {
            setClassInfo(foundClass);

            // Use the integer ID (not classCode) as the backend route expects {classId:int}
            const enrollRes = await classService.getEnrollments(foundClass.id);
            if (enrollRes.success && enrollRes.result) {
              // Backend returns { enrollments: [...] }, not { students: [...] }
              setStudents(enrollRes.result.enrollments || []);
            }
          }
        }
      } catch (err) {
        console.error("Error fetching class data", err);
      } finally {
        setIsLoading(false);
      }
    };

    if (id) {
      fetchData();
    }
  }, [id]);

  const filteredStudents = students.filter(s => {
    const query = searchQuery.toLowerCase();
    // Each item: { enrollmentId, student: { studentCode, fullName, gender }, status, enrolledAt }
    return (s.student?.fullName?.toLowerCase().includes(query) ||
            s.student?.studentCode?.toLowerCase().includes(query));
  });

  if (isLoading) {
    return (
      <div className="p-10 flex flex-col items-center justify-center">
        <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
        <p className="text-[var(--theme-textMuted)] font-medium">Loading class information...</p>
      </div>
    );
  }

  return (
    <motion.div 
      initial={{ opacity: 0, y: 15 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] pb-10"
    >
      {/* Breadcrumb Navigation */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <button 
            onClick={() => navigate('/admin/classes')}
            className="flex items-center gap-2 px-4 py-2 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] text-[var(--theme-text)]/40 hover:text-[var(--theme-primary)] hover:bg-[var(--theme-hover)] transition-all shadow-none group"
          >
            <ArrowLeft size={16} className="group-hover:-translate-x-1 transition-transform" />
            <span className="text-[10px] font-black uppercase tracking-widest">Back</span>
          </button>
          <div>
            <div className="flex items-center gap-2 text-[10px] font-black uppercase tracking-widest text-[var(--theme-text)]/40 mb-1">
              <Link to="/admin/dashboard" className="hover:text-[var(--theme-primary)] hover:no-underline transition-colors no-underline">admin site</Link>
              <span>/</span>
              <Link to="/admin/classes" className="hover:text-[var(--theme-primary)] hover:no-underline transition-colors no-underline">Class Manage</Link>
              <span>/</span>
              <span className="text-[var(--theme-primary)]">View Class</span>
            </div>
            <h2 className="text-2xl font-black tracking-tight">Class Administration Portal</h2>
          </div>
        </div>
      </div>

      {/* Class Profile Overview */}
      <div className="relative rounded-[8px] overflow-hidden bg-gradient-to-r from-[var(--theme-primary)] to-[var(--theme-primaryDark)] p-6 sm:p-8 text-white shadow-none flex flex-col sm:flex-row sm:items-center justify-between gap-6">
        <div className="relative z-10 flex items-center gap-4">
          <div className="w-14 h-14 bg-white/10 rounded-[8px] flex items-center justify-center text-white backdrop-blur-md">
            <School size={28} />
          </div>
          <div>
            <div className="flex items-center gap-3">
              <span className="text-[10px] font-black uppercase tracking-widest px-2 py-0.5 bg-white/20 rounded-[8px]">Class Profile</span>
            </div>
            <h3 className="text-2xl font-black mt-1">Class: {classInfo?.classCode || id}  |  Subject: {classInfo?.subjectName || 'Unknown'} </h3>
          </div>
        </div>

        <div className="relative z-10 flex items-center gap-2 text-white/70 font-semibold text-xs bg-black/10 px-4 py-3 rounded-[8px] border border-white/5 shrink-0">
          <Sparkles size={14} className="text-yellow-300" />
          <span>Advisor: <strong className="text-white font-black">{classInfo?.instructorName || 'Unassigned'}</strong></span>
        </div>
      </div>

      {/* Tab & Action Controls Row */}
      <div className="bg-[var(--theme-sidebarBg)] rounded-[8px] border border-[var(--theme-border)] shadow-none overflow-hidden flex flex-col">
        
        <div className="px-6 py-5 border-b border-[var(--theme-border)] flex flex-col md:flex-row md:items-center justify-between gap-4 bg-[var(--theme-hover)]/10">
          <div className="flex items-center gap-2 bg-[var(--theme-hover)]/25 p-1 rounded-[8px] w-fit">
            <button 
              className="px-5 py-2.5 rounded-[8px] text-xs font-black uppercase tracking-wider transition-all bg-[var(--theme-sidebarBg)] text-[var(--theme-primary)] shadow-none"
            >
              Enrolled Students ({students.length})
            </button>
          </div>

          <div className="flex flex-col sm:flex-row items-center gap-3 w-full md:w-auto">
            <div className="relative w-full sm:w-64">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--theme-text)]/30" size={16} />
              <input 
                type="text" 
                placeholder="Search students..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-9 pr-4 py-2 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] outline-none focus:ring-2 focus:ring-[var(--theme-primary)]/20 font-bold text-xs transition-all"
              />
            </div>
          </div>
        </div>

        {/* Student List */}
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="text-[var(--theme-text)]/30 text-[10px] font-black uppercase tracking-widest border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30">
                <th className="py-5 px-6">Student Code</th>
                <th className="py-5 px-6">Full Name</th>
                <th className="py-5 px-6">Gender</th>
                <th className="py-5 px-6">Email</th>
                <th className="py-5 px-6">Major</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-[var(--theme-border)]">
              {filteredStudents.map((enrollment, idx) => (
                <tr key={enrollment.enrollmentId ?? idx} className="group hover:bg-[var(--theme-hover)]/20 transition-all duration-300 text-xs hover:no-underline">
                  <td className="py-5 px-6 font-mono text-[var(--theme-text)]/70 font-normal">{enrollment.student?.studentCode}</td>
                  <td className="py-5 px-6 font-normal group-hover:text-[var(--theme-primary)] transition-colors">
                    <span className="text-[var(--theme-text)] group-hover:text-[var(--theme-primary)] font-bold">{enrollment.student?.fullName}</span>
                  </td>
                  <td className="py-5 px-6 font-normal">
                    <span className={`px-2.5 py-0.5 rounded-[8px] text-[10px] font-black uppercase ${
                      enrollment.student?.gender === 'Male' ? 'bg-blue-50 text-blue-600' :
                      enrollment.student?.gender === 'Female' ? 'bg-pink-50 text-pink-600' : 'bg-gray-100 text-gray-600'
                    }`}>{enrollment.student?.gender || 'Unknown'}</span>
                  </td>
                  <td className="py-5 px-6 text-[var(--theme-text)]/70 font-normal">-</td>
                  <td className="py-5 px-6 text-[var(--theme-text)]/70 font-normal">-</td>
                </tr>
              ))}
            </tbody>
          </table>

          {filteredStudents.length === 0 && (
            <div className="py-20 flex flex-col items-center justify-center text-[var(--theme-text)]/30">
              <GraduationCap size={48} strokeWidth={1} className="mb-4 opacity-20" />
              <p className="font-black text-lg uppercase tracking-widest">No Students Found</p>
            </div>
          )}
        </div>
      </div>
    </motion.div>
  );
};

export default ViewClass;
