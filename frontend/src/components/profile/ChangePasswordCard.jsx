import React from 'react';
import ProfileCard from './ProfileCard';
import { KeyRound, Save } from 'lucide-react';

const ChangePasswordCard = () => {
  return (
    <ProfileCard title="Change Password" icon={KeyRound}>
      <div className="space-y-4">
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Current Password</label>
          <input 
            type="password" 
            placeholder="Enter current password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">New Password</label>
          <input 
            type="password" 
            placeholder="Enter new password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Confirm New Password</label>
          <input 
            type="password" 
            placeholder="Confirm new password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>

        <div className="bg-amber-500/10 border border-amber-500/20 p-3 rounded-lg text-xs font-semibold text-amber-600">
          Password must be at least 8 characters long and contain numbers and symbols.
        </div>

        <div className="pt-2">
          <button className="w-full px-4 py-2.5 text-sm font-bold text-white bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] rounded-xl transition-all shadow-sm flex items-center justify-center gap-2 cursor-pointer">
            <Save size={16} />
            Update Password
          </button>
        </div>
      </div>
    </ProfileCard>
  );
};

export default ChangePasswordCard;
