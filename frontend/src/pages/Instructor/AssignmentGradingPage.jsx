import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Calendar, FileText } from 'lucide-react';
import { motion } from 'framer-motion';
import toast from 'react-hot-toast';

import SectionCard from '../../components/Shared/SectionCard';
import AssignmentTable from '../../components/Assignment/AssignmentTable';
import EmptyAssignment from '../../components/Assignment/EmptyAssignment';
import GradeModal from '../../components/Assignment/GradeModal';
import { classService, gradeService } from '../../api/services';

const AssignmentGradingPage = () => {
  const { classId } = useParams();
  const navigate = useNavigate();

  // API Ready States
  const [isLoading, setIsLoading] = useState(false);
  const [classInfo, setClassInfo] = useState(null); 
  const [submissions, setSubmissions] = useState([]);
  
  // Modal State
  const [isGradeModalOpen, setIsGradeModalOpen] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState(null);

  const fetchGradingData = async () => {
    try {
      setIsLoading(true);
      // 1. Get class enrollments (students)
      const enrollRes = await classService.getEnrollments(classId);
      if (!enrollRes.success) throw new Error("Failed to load class enrollments");
      
      const classData = enrollRes.result;
      setClassInfo({
        title: classData.subjectName || 'Course Assignment',
        classInfo: classData.classCode,
        dueDate: 'End of Semester' // Placeholder
      });

      const enrollments = classData.enrollments || [];

      // 2. Get class grades
      const gradesRes = await gradeService.getClassGrades(classId);
      const grades = gradesRes.success ? (gradesRes.result || []) : [];

      // 3. Merge data
      const mergedSubmissions = enrollments.map(enr => {
        const gradeInfo = grades.find(g => g.enrollmentId === enr.enrollmentId);
        
        return {
          Id: enr.enrollmentId,
          StudentCode: enr.student.studentCode,
          StudentName: enr.student.fullName,
          SubmissionStatus: gradeInfo?.submissionPath ? 'Submitted' : 'Missing',
          SubmittedAt: gradeInfo?.submissionPath ? 'File uploaded' : 'N/A',
          Score: gradeInfo?.score !== undefined ? gradeInfo.score : null,
          Feedback: gradeInfo?.classification || '',
          SubmissionPath: gradeInfo?.submissionPath,
          GradeId: gradeInfo?.id, // ID of the grade record if it exists
          EnrollmentId: enr.enrollmentId
        };
      });

      setSubmissions(mergedSubmissions);
    } catch (err) {
      console.error(err);
      toast.error("Error loading grading data");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchGradingData();
  }, [classId]);

  const handleGradeClick = (submission) => {
    setSelectedStudent(submission);
    setIsGradeModalOpen(true);
  };

  const handleGradeSubmit = async (score) => {
    if (!selectedStudent) return;
    
    try {
      if (selectedStudent.GradeId) {
        // Update existing grade
        const res = await gradeService.updateGrade(selectedStudent.GradeId, {
          enrollmentId: selectedStudent.EnrollmentId,
          score: score
        });
        if (res.success) toast.success("Grade updated!");
      } else {
        // Enter new grade
        const res = await gradeService.enterGrade({
          enrollmentId: selectedStudent.EnrollmentId,
          score: score
        });
        if (res.success) toast.success("Grade submitted!");
      }
      setIsGradeModalOpen(false);
      fetchGradingData(); // Refresh list
    } catch (err) {
      console.error(err);
      toast.error(err.response?.data?.message || "Failed to submit grade. Student might not have submitted.");
    }
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
      ) : !classInfo ? (
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
                <h3 className="text-2xl font-black text-[var(--theme-text)] mb-2">{classInfo.title}</h3>
                <p className="text-sm font-semibold text-[var(--theme-textMuted)] flex items-center gap-2">
                  <span className="px-2.5 py-1 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-md font-mono text-[var(--theme-primary)]">
                    {classId}
                  </span>
                  <span>{classInfo.classInfo}</span>
                </p>
              </div>
              <div className="flex items-center gap-3 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] px-4 py-2.5 rounded-xl">
                <Calendar size={18} className="text-[var(--theme-textMuted)]" />
                <div>
                  <p className="text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider">Due Date</p>
                  <p className="text-sm font-semibold text-[var(--theme-text)]">{classInfo.dueDate}</p>
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
        onSubmitGrade={handleGradeSubmit}
        initialScore={selectedStudent?.Score}
      />
    </motion.div>
  );
};

export default AssignmentGradingPage;
