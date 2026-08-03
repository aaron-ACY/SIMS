import React from 'react';
import ProfileCard from './ProfileCard';
import { Shield } from 'lucide-react';

const AccountInformation = ({ data }) => {
  return (
    <ProfileCard title="Account Information" icon={Shield} isDropdown={false}>
      <div className="space-y-4">
        <InfoRow label="Username" value={data?.username} />
        <InfoRow label="Role" value={data?.role} />
        <InfoRow 
          label="Account Status" 
          value={
            <span className={`px-2 py-1 rounded-md text-xs font-bold uppercase tracking-wider ${
              data?.status === 'Active' ? 'bg-emerald-500/10 text-emerald-500' : 'bg-red-500/10 text-red-500'
            }`}>
              {data?.status || 'Unknown'}
            </span>
          } 
        />

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

export default AccountInformation;
