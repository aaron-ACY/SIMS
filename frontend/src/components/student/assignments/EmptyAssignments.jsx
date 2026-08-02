import React from 'react';
import EmptyState from '../../Shared/EmptyState';
import { FileQuestion } from 'lucide-react';

const EmptyAssignments = () => {
  return (
    <div className="py-12">
      <EmptyState 
        icon={FileQuestion}
        title="No Assignments Found"
        description="There are currently no assignments matching your criteria."
      />
    </div>
  );
};

export default EmptyAssignments;
