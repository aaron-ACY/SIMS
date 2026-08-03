import React from 'react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import EmptyState from '../../../components/Shared/EmptyState';
import { FileText } from 'lucide-react';

const Reports = () => {
  return (
    <div className="p-6">
      <SectionCard>
        <EmptyState 
          icon={FileText}
          title="System under development"
          description="This feature is currently under development and will be available in future updates."
        />
      </SectionCard>
    </div>
  );
};

export default Reports;
