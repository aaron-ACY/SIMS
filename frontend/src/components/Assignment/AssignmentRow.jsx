import React from 'react';
import { Edit3, FileCheck } from 'lucide-react';

const AssignmentRow = ({ submission, onGradeClick }) => {
  const getStatusStyle = (status) => {
    switch (status) {
      case 'Submitted':
        return 'bg-emerald-500/10 text-emerald-500';
      case 'Late':
        return 'bg-amber-500/10 text-amber-500';
      case 'Missing':
        return 'bg-red-500/10 text-red-500';
      default:
        return 'bg-[var(--theme-hover)] text-[var(--theme-textMuted)]';
    }
  };

  return (
    <tr className="group hover:bg-[var(--theme-hover)]/30 transition-colors">
      <td className="py-4 px-6 font-mono font-bold text-sm text-[var(--theme-primary)]">
        {submission.StudentCode}
      </td>
      <td className="py-4 px-6 font-bold text-sm text-[var(--theme-text)]">
        {submission.StudentName}
      </td>
      <td className="py-4 px-6 text-center">
        <span className={`px-2.5 py-1 rounded-lg text-[10px] font-black tracking-wider uppercase ${getStatusStyle(submission.SubmissionStatus)}`}>
          {submission.SubmissionStatus}
        </span>
      </td>
      <td className="py-4 px-6 font-mono text-sm text-[var(--theme-textMuted)]">
        {submission.SubmittedAt || 'N/A'}
      </td>
      <td className="py-4 px-6 font-mono text-sm text-center font-bold text-[var(--theme-text)]">
        {submission.Score !== null ? `${submission.Score}/100` : '-'}
      </td>
      <td className="py-4 px-6 text-sm text-[var(--theme-textMuted)] truncate max-w-[150px]" title={submission.Feedback}>
        {submission.Feedback || 'No feedback'}
      </td>
      <td className="py-4 px-6 text-right">
        <button 
          onClick={onGradeClick}
          className={`p-2 rounded-lg transition-colors cursor-pointer inline-flex items-center gap-1.5 ${
            submission.Score !== null 
              ? 'bg-[var(--theme-hover)] hover:bg-amber-500/10 text-[var(--theme-textMuted)] hover:text-amber-500' 
              : 'bg-[var(--theme-primary)]/10 hover:bg-[var(--theme-primary)]/20 text-[var(--theme-primary)]'
          }`}
          title={submission.Score !== null ? "Edit Grade" : "Enter Grade"}
        >
          {submission.Score !== null ? <Edit3 size={16} /> : <FileCheck size={16} />}
        </button>
      </td>
    </tr>
  );
};

export default AssignmentRow;
