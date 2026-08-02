import React from 'react';
import SkeletonCard from './SkeletonCard';
import EmptyState from '../Shared/EmptyState';
import { Clock } from 'lucide-react';

const DeadlineCard = ({ deadlines, isLoading }) => {
  if (isLoading) {
    return <SkeletonCard type="list" />;
  }

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm flex flex-col h-full">
      <h3 className="text-lg font-black text-[var(--theme-text)] flex items-center gap-2 mb-4">
        <Clock size={18} className="text-amber-500" /> 
        Upcoming Deadlines
      </h3>
      
      <div className="flex-1 flex flex-col justify-center">
        {deadlines && deadlines.length > 0 ? (
          <div className="space-y-4">
            {/* Future mapping */}
          </div>
        ) : (
          <EmptyState 
            icon={Clock}
            title="No upcoming deadlines."
            description="There are currently no assignments waiting for grading or upcoming submissions."
          />
        )}
      </div>
    </div>
  );
};

export default DeadlineCard;
