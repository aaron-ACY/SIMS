import React from 'react';
import EmptyState from '../../Shared/EmptyState';
import { BookX } from 'lucide-react';

const EmptyClasses = () => {
  return (
    <div className="py-12">
      <EmptyState 
        icon={BookX}
        title="No Enrolled Classes"
        description="You are not enrolled in any classes for the current semester. Please check with your academic advisor."
      />
    </div>
  );
};

export default EmptyClasses;
