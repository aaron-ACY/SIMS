import React from 'react';
import { Filter } from 'lucide-react';

const AssignmentFilter = ({ currentFilter, onFilterChange, options }) => {
  return (
    <div className="flex items-center gap-3">
      <div className="flex items-center gap-2 px-3 py-1.5 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-lg">
        <Filter size={14} className="text-[var(--theme-textMuted)]" />
        <span className="text-xs font-bold uppercase tracking-widest text-[var(--theme-textMuted)]">Filter</span>
      </div>
      <div className="relative">
        <select 
          value={currentFilter}
          onChange={(e) => onFilterChange(e.target.value)}
          className="appearance-none pl-4 pr-10 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors cursor-pointer"
        >
          <option value="all">My Classes (All)</option>
          {options.map((opt, idx) => (
            <option key={idx} value={opt.value}>{opt.label}</option>
          ))}
        </select>
        <div className="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none">
          <svg className="w-4 h-4 text-[var(--theme-textMuted)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
          </svg>
        </div>
      </div>
    </div>
  );
};

export default AssignmentFilter;
