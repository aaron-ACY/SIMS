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
                <div key={idx}>{/* Future activity implementation */}</div>
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
