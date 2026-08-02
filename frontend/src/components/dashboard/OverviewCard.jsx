import React from 'react';
import SkeletonCard from './SkeletonCard';

const OverviewCard = ({ title, value, icon: Icon, isLoading, colorClass = "text-[var(--theme-primary)]", bgClass = "bg-[var(--theme-primary)]/10" }) => {
  if (isLoading) {
    return <SkeletonCard type="overview" />;
  }

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm hover:border-[var(--theme-primary)]/30 transition-colors">
      <div className="flex items-center gap-4">
        <div className={`p-4 rounded-xl ${bgClass} ${colorClass}`}>
          <Icon size={24} />
        </div>
        <div>
          <p className="text-sm font-bold text-[var(--theme-textMuted)]">{title}</p>
          <h3 className="text-3xl font-black text-[var(--theme-text)] mt-1">
            {value !== undefined && value !== null ? value : '--'}
          </h3>
        </div>
      </div>
    </div>
  );
};

export default OverviewCard;
