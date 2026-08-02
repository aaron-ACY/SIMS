import React from 'react';
import SkeletonCard from './SkeletonCard';
import EmptyState from '../Shared/EmptyState';
import { Bell } from 'lucide-react';

const AnnouncementCard = ({ announcements, isLoading }) => {
  if (isLoading) {
    return <SkeletonCard type="list" />;
  }

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-6 shadow-sm flex flex-col h-full">
      <h3 className="text-lg font-black text-[var(--theme-text)] flex items-center gap-2 mb-4">
        <Bell size={18} className="text-[var(--theme-primary)]" /> 
        Announcements
      </h3>
      
      <div className="flex-1 flex flex-col justify-center">
        {announcements && announcements.length > 0 ? (
          <div className="space-y-4">
            {/* Future list mapping */}
          </div>
        ) : (
          <EmptyState 
            icon={Bell}
            title="No announcements available."
            description="Important system and department announcements will appear here."
          />
        )}
      </div>
    </div>
  );
};

export default AnnouncementCard;
