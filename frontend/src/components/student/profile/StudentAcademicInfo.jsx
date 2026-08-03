import React from 'react';
import ProfileCard from '../../profile/ProfileCard';
import { GraduationCap } from 'lucide-react';

const StudentAcademicInfo = ({ data }) => {
  return (
    <ProfileCard title="Academic Information" icon={GraduationCap}>
      <div className="space-y-4">
        <InfoRow label="Major" value={data?.major} />
        <InfoRow label="GPA" value={data?.gpa ? `${data.gpa} / 4.00` : null} />
      </div>
    </ProfileCard>
  );
};

const InfoRow = ({ label, value }) => (
  <div className="flex flex-col sm:flex-row sm:justify-between sm:items-baseline border-b border-[var(--theme-border)] pb-2 last:border-0 last:pb-0">
    <span className="text-xs font-bold text-[var(--theme-textMuted)]">{label}:</span>
    <span className="text-sm font-semibold text-[var(--theme-text)]">
      {value || <span className="text-[var(--theme-textMuted)] italic">Not provided</span>}
    </span>
  </div>
);

export default StudentAcademicInfo;
