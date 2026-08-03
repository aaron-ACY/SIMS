import React from 'react';
import SkeletonCard from './SkeletonCard';
import EmptyState from '../Shared/EmptyState';
import { Activity } from 'lucide-react';

const ActivityCard = ({ activities, isLoading }) => {
  if (isLoading) {
    return <SkeletonCard type="list" />;
  }

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm flex flex-col h-full">
      <h3 className="text-lg font-black text-[var(--theme-text)] flex items-center gap-2 mb-4">
        <Activity size={18} className="text-[var(--theme-primary)]" /> 
        Recent Activities
      </h3>
      
      <div className="flex-1 flex flex-col justify-center">
        {activities && activities.length > 0 ? (
          <div className="space-y-4 relative before:absolute before:inset-0 before:ml-5 before:-translate-x-px md:before:mx-auto md:before:translate-x-0 before:h-full before:w-0.5 before:bg-gradient-to-b before:from-transparent before:via-[var(--theme-border)] before:to-transparent">
             {activities.map((item, idx) => (
                <div key={idx} className="relative flex items-center justify-between md:justify-normal md:odd:flex-row-reverse group is-active">
                  <div className="flex items-center justify-center w-10 h-10 rounded-full border border-white bg-slate-200 group-[.is-active]:bg-[var(--theme-primary)] text-slate-500 group-[.is-active]:text-white shadow shrink-0 md:order-1 md:group-odd:-translate-x-1/2 md:group-even:translate-x-1/2">
                    <Activity size={16} />
                  </div>
                  <div className="w-[calc(100%-4rem)] md:w-[calc(50%-2.5rem)] p-4 rounded-xl border border-[var(--theme-border)] bg-[var(--theme-bg)] shadow-sm">
                    <div className="flex items-center justify-between mb-1">
                      <div className="font-bold text-[var(--theme-text)]">Assignment Submitted</div>
                      <time className="font-medium text-xs text-[var(--theme-textMuted)]">{item.updatedAt.toLocaleDateString()}</time>
                    </div>
                    <div className="text-sm text-[var(--theme-textMuted)] font-medium">
                      <span className="font-bold text-[var(--theme-text)]">{item.studentName}</span> submitted an assignment in <span className="font-bold text-[var(--theme-primary)]">{item.classCode}</span>.
                      {item.isGraded && <span className="text-emerald-500 ml-1">(Graded)</span>}
                    </div>
                  </div>
                </div>
             ))}
          </div>
        ) : (
          <div className="py-8">
            <EmptyState 
              icon={Activity}
              title="No recent activities."
              description="Activities will appear here after data is loaded."
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default ActivityCard;
