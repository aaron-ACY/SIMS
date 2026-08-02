import React from 'react';
import DashboardCard from './DashboardCard';
import EmptyState from '../../Shared/EmptyState';
import SkeletonCard from '../../dashboard/SkeletonCard';
import { Calendar } from 'lucide-react';

const AssignmentSummary = ({ assignments, isLoading }) => {
  if (isLoading) {
    return <SkeletonCard type="list" />;
  }

  return (
    <DashboardCard title="Upcoming Assignments" icon={Calendar}>
      {assignments && assignments.length > 0 ? (
        <div className="space-y-4">
          {/* Future list implementation */}
        </div>
      ) : (
        <EmptyState 
          icon={Calendar}
          title="No upcoming assignments."
          description="You have no assignments due in the near future."
        />
      )}
    </DashboardCard>
  );
};

export default AssignmentSummary;
