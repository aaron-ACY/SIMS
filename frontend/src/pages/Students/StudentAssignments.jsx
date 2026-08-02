import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { BookOpen } from 'lucide-react';
import { motion } from 'framer-motion';

import AssignmentFilter from '../../components/student/assignments/AssignmentFilter';
import AssignmentTable from '../../components/student/assignments/AssignmentTable';
import EmptyAssignments from '../../components/student/assignments/EmptyAssignments';
import PageHeader from '../../components/Shared/PageHeader';

const StudentAssignments = () => {
  const navigate = useNavigate();

  const [isLoading, setIsLoading] = useState(false);
  const [filter, setFilter] = useState('all');
  const [assignments, setAssignments] = useState([]);
  const [filterOptions, setFilterOptions] = useState([]);

  // Future API Integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchStudentAssignments().then(data => {
    //   setAssignments(data.assignments);
    //   setFilterOptions(data.availableClasses.map(c => ({ value: c.id, label: c.name })));
    // }).finally(() => setIsLoading(false));
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
              assignments={assignments} 
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
