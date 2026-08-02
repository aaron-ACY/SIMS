import React from 'react';
import { Eye } from 'lucide-react';

const AssignmentRow = ({ assignment, onViewClick }) => {
  const getStatusBadge = (status) => {
    switch (status) {
      case 'Submitted':
        return <span className="px-2.5 py-1 bg-emerald-500/10 text-emerald-500 rounded-lg text-[10px] font-black tracking-wider uppercase">Submitted</span>;
      case 'Graded':
        return <span className="px-2.5 py-1 bg-blue-500/10 text-blue-500 rounded-lg text-[10px] font-black tracking-wider uppercase">Graded</span>;
      case 'Pending':
        return <span className="px-2.5 py-1 bg-amber-500/10 text-amber-500 rounded-lg text-[10px] font-black tracking-wider uppercase">Pending</span>;
      case 'Late':
        return <span className="px-2.5 py-1 bg-red-500/10 text-red-500 rounded-lg text-[10px] font-black tracking-wider uppercase">Late</span>;
      default:
        return <span className="px-2.5 py-1 bg-[var(--theme-hover)] text-[var(--theme-textMuted)] rounded-lg text-[10px] font-black tracking-wider uppercase">{status || 'Unknown'}</span>;
    }
  };

  return (
    <tr className="group hover:bg-[var(--theme-hover)]/30 transition-colors">
      <td className="py-5 px-6 font-bold text-sm text-[var(--theme-text)]">{assignment.title}</td>
      <td className="py-5 px-6 font-semibold text-sm text-[var(--theme-textMuted)]">{assignment.class}</td>
      <td className="py-5 px-6 font-mono text-sm text-[var(--theme-textMuted)]">{assignment.dueDate}</td>
      <td className="py-5 px-6 text-center">{getStatusBadge(assignment.status)}</td>
      <td className="py-5 px-6 text-center font-mono font-bold text-[var(--theme-text)]">{assignment.score ? `${assignment.score}/100` : '-'}</td>
      <td className="py-5 px-6 text-right">
        <button 
          onClick={onViewClick}
          className="p-2 bg-[var(--theme-hover)] hover:bg-[var(--theme-primary)]/10 text-[var(--theme-textMuted)] hover:text-[var(--theme-primary)] rounded-lg transition-colors cursor-pointer inline-flex items-center gap-1.5"
          title="View Details"
        >
          <Eye size={16} />
        </button>
      </td>
    </tr>
  );
};

export default AssignmentRow;
