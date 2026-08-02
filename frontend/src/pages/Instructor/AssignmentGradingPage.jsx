import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Calendar, FileText } from 'lucide-react';
import { motion } from 'framer-motion';

import SectionCard from '../../components/Shared/SectionCard';
import AssignmentTable from '../../components/Assignment/AssignmentTable';
import EmptyAssignment from '../../components/Assignment/EmptyAssignment';
import GradeModal from '../../components/Assignment/GradeModal';

const AssignmentGradingPage = () => {
  const { classId } = useParams();
  const navigate = useNavigate();

  // API Ready States
  const [isLoading, setIsLoading] = useState(false);
  const [assignment, setAssignment] = useState(null); // null means no assignment exists
  const [submissions, setSubmissions] = useState([]);
  
  // Modal State
  const [isGradeModalOpen, setIsGradeModalOpen] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState(null);

  // Future API Integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchAssignmentForClass(classId).then(data => {
    //   if (data) {
    //     setAssignment(data.assignmentInfo);
    //     setSubmissions(data.submissions);
    //   }
    // }).finally(() => setIsLoading(false));
  }, [classId]);

  const handleGradeClick = (submission) => {
    setSelectedStudent(submission);
    setIsGradeModalOpen(true);
  };

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      {/* Top action row */}
      <div className="flex items-center gap-4">
        <button 
          onClick={() => navigate('/lecturer/class')}
          className="p-2.5 bg-[var(--theme-sidebarBg)] hover:bg-[var(--theme-hover)] text-[var(--theme-text)]/75 rounded-xl transition-colors border border-[var(--theme-border)] shadow-sm cursor-pointer"
        >
          <ArrowLeft size={18} />
        </button>
        <div>
          <h2 className="text-3xl font-black tracking-tight">Assignment Grading</h2>
          <p className="text-sm text-[var(--theme-textMuted)] font-bold">Manage and grade submissions for your class.</p>
        </div>
      </div>

      {isLoading ? (
        <div className="py-20 flex flex-col items-center justify-center">
           <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
           <p className="text-[var(--theme-textMuted)] font-medium">Loading grading data...</p>
        </div>
      ) : !assignment ? (
        <SectionCard className="p-8">
          <EmptyAssignment 
            type="no_assignment" 
            onAction={() => alert('Create Assignment functionality coming soon!')}
          />
        </SectionCard>
      ) : (
        <>
          {/* Assignment Meta Header */}
          <SectionCard className="p-6 bg-[var(--theme-primary)]/5 border-[var(--theme-primary)]/20">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <div>
                <h3 className="text-2xl font-black text-[var(--theme-text)] mb-2">{assignment.title}</h3>
                <p className="text-sm font-semibold text-[var(--theme-textMuted)] flex items-center gap-2">
                  <span className="px-2.5 py-1 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-md font-mono text-[var(--theme-primary)]">
                    {classId}
                  </span>
                  <span>{assignment.classInfo}</span>
                </p>
              </div>
              <div className="flex items-center gap-3 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] px-4 py-2.5 rounded-xl">
                <Calendar size={18} className="text-[var(--theme-textMuted)]" />
                <div>
                  <p className="text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider">Due Date</p>
                  <p className="text-sm font-semibold text-[var(--theme-text)]">{assignment.dueDate}</p>
                </div>
              </div>
            </div>
          </SectionCard>

          {/* Submissions Table Area */}
          <SectionCard>
            <div className="p-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30 flex items-center gap-2">
              <FileText size={18} className="text-[var(--theme-textMuted)]" />
              <h4 className="font-bold text-[var(--theme-text)]">Student Submissions</h4>
            </div>

            {submissions.length > 0 ? (
              <AssignmentTable 
                submissions={submissions}
                onGradeClick={handleGradeClick}
              />
            ) : (
              <div className="p-8">
                <EmptyAssignment type="no_submissions" />
              </div>
            )}
          </SectionCard>
        </>
      )}

      {/* Grade Modal */}
      <GradeModal 
        isOpen={isGradeModalOpen}
        onClose={() => setIsGradeModalOpen(false)}
        studentName={selectedStudent?.StudentName || 'Unknown Student'}
      />
    </motion.div>
  );
};

export default AssignmentGradingPage;
