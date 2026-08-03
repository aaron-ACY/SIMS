import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { BookOpen } from 'lucide-react';
import { motion } from 'framer-motion';

import AssignmentFilter from '../../components/student/assignments/AssignmentFilter';
import AssignmentTable from '../../components/student/assignments/AssignmentTable';
import EmptyAssignments from '../../components/student/assignments/EmptyAssignments';
import PageHeader from '../../components/Shared/PageHeader';
import { userService, studentService, gradeService } from '../../api/services';

const StudentAssignments = () => {
  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);
  const [filter, setFilter] = useState('all');
  const [assignments, setAssignments] = useState([]);
  const [filterOptions, setFilterOptions] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const userRes = await userService.getMe();
        if (!userRes.success || !userRes.result?.studentCode) return;
        const studentCode = userRes.result.studentCode;

        const [classesRes, gradesRes] = await Promise.all([
          studentService.getMyClasses(),
          gradeService.getStudentGrades(studentCode)
        ]);

        const classes = (classesRes.success ? classesRes.result : []) || [];
        const gradesData = (gradesRes.success && gradesRes.result ? gradesRes.result.classes : []) || [];

        // Build assignments list
        const assignmentsData = classes.map(cls => {
          let score = null;
          let status = 'Pending';
          
          // Find grade for this class
          const classGradeGroup = gradesData.find(g => g.classCode === cls.classCode && g.semester === cls.semester);
          if (classGradeGroup && classGradeGroup.grades && classGradeGroup.grades.length > 0) {
            // Assuming one grade per class subject
            score = classGradeGroup.grades[0].scores;
            if (score > 0 || classGradeGroup.grades[0].rating) {
              status = 'Graded';
            } else {
              status = 'Submitted'; // Or pending if they haven't submitted, but we don't have submission status
            }
          }

          return {
            id: cls.enrollmentId,
            title: `${cls.subjectName} Assignment`,
            class: cls.classCode,
            classId: cls.classId,
            dueDate: 'End of Semester',
            status: status,
            score: score
          };
        });

        setAssignments(assignmentsData);
        
        // Extract unique classes for filter
        const uniqueClasses = Array.from(new Set(classes.map(c => c.classCode))).map(code => {
          const c = classes.find(x => x.classCode === code);
          return { value: code, label: `${c.classCode} - ${c.subjectName}` };
        });
        setFilterOptions(uniqueClasses);

      } catch (err) {
        console.error('Failed to fetch assignments', err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleViewClick = (assignmentId) => {
    navigate(`/student/assignments/view/${assignmentId}`);
  };

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      <PageHeader 
        title="My Assignments"
        description="View and submit assignments for your enrolled classes."
      />

      <div className="bg-[var(--theme-sidebarBg)] rounded-2xl border border-[var(--theme-border)] shadow-sm overflow-hidden transition-all duration-500 flex flex-col">
        {/* Header & Filter */}
        <div className="px-6 sm:px-8 py-5 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          <div className="flex items-center gap-2">
            <BookOpen size={18} className="text-[var(--theme-primary)]" />
            <h3 className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]">Assignment List</h3>
          </div>
          
          <AssignmentFilter 
            currentFilter={filter}
            onFilterChange={setFilter}
            options={filterOptions}
          />
        </div>

        {/* Content Area */}
        <div className="flex-1">
          {isLoading ? (
            <div className="py-20 flex flex-col items-center justify-center">
              <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
              <p className="text-[var(--theme-textMuted)] font-medium">Loading assignments...</p>
            </div>
          ) : assignments.length > 0 ? (
            <AssignmentTable 
              assignments={filter === 'all' ? assignments : assignments.filter(a => a.class === filter)} 
              onViewClick={handleViewClick} 
            />
          ) : (
            <EmptyAssignments />
          )}
        </div>
      </div>
    </motion.div>
  );
};

export default StudentAssignments;
