import React from 'react';
import EmptyState from '../Shared/EmptyState';
import { FileText, Users } from 'lucide-react';

const EmptyAssignment = ({ type = 'no_assignment', onAction }) => {
  if (type === 'no_assignment') {
    return (
      <EmptyState 
        icon={FileText}
        title="No Assignments Found"
        description="This class does not have any assignments yet."
        actionLabel="Create Assignment"
        onAction={onAction}
      />
    );
  }

  if (type === 'no_submissions') {
    return (
      <EmptyState 
        icon={Users}
        title="No Student Submissions"
        description="Student submissions will appear here after the assignment deadline."
      />
    );
  }

  return null;
};

export default EmptyAssignment;
