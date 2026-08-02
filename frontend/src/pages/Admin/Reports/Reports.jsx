import React from 'react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import EmptyState from '../../../components/Shared/EmptyState';
import { FileText } from 'lucide-react';

const Reports = () => {
  return (
    <div className="p-6">
      <PageHeader 
        title="Reports" 
        description="System analytics and reports" 
      />
      <SectionCard>
        <EmptyState 
          icon={FileText}
          title="No report data available."
          description="Reports will be available once there is sufficient data."
        />
      </SectionCard>
    </div>
  );
};

export default Reports;
