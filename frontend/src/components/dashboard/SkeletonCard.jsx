import React from 'react';

const SkeletonCard = ({ type = 'overview' }) => {
  if (type === 'overview') {
    return (
      <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm animate-pulse">
        <div className="flex items-center gap-4">
          <div className="p-4 rounded-xl bg-[var(--theme-hover)] w-14 h-14"></div>
          <div className="space-y-2 flex-1">
            <div className="h-4 bg-[var(--theme-hover)] rounded w-1/2"></div>
            <div className="h-8 bg-[var(--theme-hover)] rounded w-1/3"></div>
          </div>
        </div>
      </div>
    );
  }

  if (type === 'list') {
    return (
      <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm animate-pulse h-full">
        <div className="h-6 bg-[var(--theme-hover)] rounded w-1/3 mb-6"></div>
        <div className="space-y-4">
          <div className="h-16 bg-[var(--theme-hover)] rounded-xl w-full"></div>
          <div className="h-16 bg-[var(--theme-hover)] rounded-xl w-full"></div>
          <div className="h-16 bg-[var(--theme-hover)] rounded-xl w-full"></div>
        </div>
      </div>
    );
  }

  return null;
};

export default SkeletonCard;
