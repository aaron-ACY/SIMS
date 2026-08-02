import React from 'react';
import AssignmentRow from './AssignmentRow';

const AssignmentTable = ({ submissions, onGradeClick }) => {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left border-collapse">
        <thead>
          <tr className="bg-[var(--theme-hover)]/30 text-[var(--theme-textMuted)] text-xs uppercase tracking-wider border-b border-[var(--theme-border)]">
            <th className="py-4 px-6 font-bold">Student Code</th>
            <th className="py-4 px-6 font-bold">Student Name</th>
            <th className="py-4 px-6 font-bold text-center">Submission Status</th>
            <th className="py-4 px-6 font-bold">Submitted Time</th>
            <th className="py-4 px-6 font-bold text-center">Grade</th>
            <th className="py-4 px-6 font-bold">Feedback</th>
            <th className="py-4 px-6 font-bold text-right">Action</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-[var(--theme-border)]">
          {submissions.map((sub, idx) => (
            <AssignmentRow 
              key={sub.Id || idx} 
              submission={sub} 
              onGradeClick={() => onGradeClick(sub)}
            />
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AssignmentTable;
