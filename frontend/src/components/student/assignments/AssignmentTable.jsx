import React from 'react';
import AssignmentRow from './AssignmentRow';

const AssignmentTable = ({ assignments, onViewClick }) => {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left border-collapse bg-white">
        <thead>
          <tr className="bg-[var(--theme-hover)]/30 text-[var(--theme-textMuted)] text-[11px] font-black uppercase tracking-widest border-b border-[var(--theme-border)]">
            <th className="py-5 px-6">Assignment Title</th>
            <th className="py-5 px-6">Class</th>
            <th className="py-5 px-6">Due Date</th>
            <th className="py-5 px-6 text-center">Submission Status</th>
            <th className="py-5 px-6 text-center">Score</th>
            <th className="py-5 px-6 text-right">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-[var(--theme-border)]">
          {assignments.map((assignment, idx) => (
            <AssignmentRow 
              key={assignment.id || idx} 
              assignment={assignment} 
              onViewClick={() => onViewClick(assignment.id)} 
            />
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AssignmentTable;

