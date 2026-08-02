import React from 'react';
import SkeletonCard from './SkeletonCard';
import EmptyState from '../Shared/EmptyState';
import { Calendar } from 'lucide-react';

const ScheduleCard = ({ scheduleData, isLoading }) => {
  if (isLoading) {
    return <SkeletonCard type="list" />;
  }

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm flex flex-col h-full">
      <h3 className="text-lg font-black text-[var(--theme-text)] flex items-center gap-2 mb-4">
        <Calendar size={18} className="text-[var(--theme-primary)]" /> 
        Today's Schedule
      </h3>
      
      <div className="flex-1 flex flex-col justify-center">
        {scheduleData && scheduleData.length > 0 ? (
          <div className="space-y-4">
            {scheduleData.map((item, idx) => (
              <div key={idx} className="p-4 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl">
                <p className="font-bold text-[var(--theme-text)]">{item.time}</p>
                <p className="text-sm font-semibold text-[var(--theme-textMuted)]">{item.course} - {item.room}</p>
              </div>
            ))}
          </div>
        ) : (
          <EmptyState 
            icon={Calendar}
            title="No scheduled classes today."
            description="Take a break or review upcoming assignments."
          />
        )}
      </div>
    </div>
  );
};

export default ScheduleCard;
