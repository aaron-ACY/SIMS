import React from 'react';
import ProfileCard from '../../profile/ProfileCard';
import { User } from 'lucide-react';

const StudentPersonalInfo = ({ data }) => {
  return (
    <ProfileCard title="Personal Information" icon={User}>
      <div className="space-y-4">
        <InfoRow label="Full Name" value={data?.fullName} />
        <InfoRow label="Student Code" value={data?.code} />
        <InfoRow label="National ID" value={data?.nationalId} />
        <InfoRow label="Email" value={data?.email} />
        <InfoRow label="Phone Number" value={data?.phone} />
        <InfoRow label="Gender" value={data?.gender} />
        <InfoRow label="Date of Birth" value={data?.dob} />
        <InfoRow label="Address" value={data?.address} />
        <InfoRow label="Emergency Contact" value={data?.emergencyContact} />
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

export default StudentPersonalInfo;
