import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { ArrowLeft } from 'lucide-react';

const StudentViewClass = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  // Mock data based on ID
  const classInfo = {
    id: id || 'CLA001',
    name: 'IT01 - Computer Science',
    advisor: 'Dr. Johnathan',
    advisorEmail: 'johnathan@sims.edu',
    room: 'Lab Room 302',
    schedule: 'Mon, Wed, Fri (08:00 - 10:00)'
  };

  const classmates = [
    { id: 'STD001', name: 'David Vu', gender: 'Male', email: 'davidvu@gmail.com' },
    { id: 'STD002', name: 'Nguyễn Văn A', gender: 'Male', email: 'nva@sims.edu' },
    { id: 'STD003', name: 'Trần Thị B', gender: 'Female', email: 'ttb@sims.edu' },
    { id: 'STD004', name: 'Lê Văn C', gender: 'Male', email: 'lvc@sims.edu' },
  ];

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)]"
    >
      <div className="flex items-center gap-4">
        <button 
          onClick={() => navigate('/student/class')}
          className="p-2 bg-[var(--theme-hover)]/50 hover:bg-[var(--theme-hover)] text-[var(--theme-text)]/70 hover:text-[var(--theme-text)] rounded-[8px] transition-colors"
        >
          <ArrowLeft size={20} />
        </button>
        <div>
          <h2 className="text-3xl font-black tracking-tight">Class Details</h2>
          <p className="text-sm text-[var(--theme-text)]/50 font-bold">View information about {classInfo.name}.</p>
        </div>
      </div>

      {/* Class Details Card */}
      <div className="bg-[var(--theme-sidebarBg)] p-6 rounded-[8px] border border-[var(--theme-border)] shadow-none flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
        <div>
          <span className="text-[10px] font-black uppercase tracking-widest px-2 py-0.5 bg-[var(--theme-hover)] text-[var(--theme-text)]/70 rounded-[8px]">Class Profile</span>
          <h3 className="text-2xl font-black mt-2">Class: {classInfo.name}</h3>
          <p className="text-sm text-[var(--theme-text)]/60 font-bold mt-1">Advisor: {classInfo.advisor} ({classInfo.advisorEmail})</p>
        </div>
      </div>

      {/* Classmate List Section */}
      <div className="bg-[var(--theme-sidebarBg)] rounded-[8px] border border-[var(--theme-border)] shadow-none overflow-hidden transition-all duration-500">
        <div className="px-8 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/10">
          <h3 className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]/70">Classmates ({classmates.length})</h3>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-[var(--theme-hover)]/30 text-[var(--theme-text)]/40 text-[11px] font-black uppercase tracking-widest border-b border-[var(--theme-border)]">
                <th className="py-5 px-6">Student ID</th>
                <th className="py-5 px-6">Full Name</th>
                <th className="py-5 px-6">Gender</th>
                <th className="py-5 px-6">Email Address</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-[var(--theme-border)]">
              {classmates.map((student, idx) => (
                <tr key={idx} className="group hover:bg-[var(--theme-hover)]/20 transition-colors hover:no-underline">
                  <td className="py-6 px-6 font-normal text-sm group-hover:no-underline">{student.id}</td>
                  <td className="py-6 px-6 font-normal text-sm text-[var(--theme-text)] group-hover:text-[var(--theme-primary)] transition-colors group-hover:no-underline">{student.name}</td>
                  <td className="py-6 px-6 font-normal text-sm text-[var(--theme-text)]/70 group-hover:no-underline">{student.gender}</td>
                  <td className="py-6 px-6 font-normal text-sm text-[var(--theme-text)]/70 group-hover:no-underline">{student.email}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </motion.div>
  );
};

export default StudentViewClass;
