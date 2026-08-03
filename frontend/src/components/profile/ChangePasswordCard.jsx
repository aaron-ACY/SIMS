import React, { useState } from 'react';
import ProfileCard from './ProfileCard';
import { KeyRound, Save, Loader2 } from 'lucide-react';
import { userService } from '../../api/services';
import toast from 'react-hot-toast';

const ChangePasswordCard = () => {
  const [formData, setFormData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  });
  const [isLoading, setIsLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleUpdatePassword = async () => {
    if (!formData.currentPassword || !formData.newPassword || !formData.confirmNewPassword) {
      toast.error('Please fill in all password fields');
      return;
    }

    if (formData.newPassword !== formData.confirmNewPassword) {
      toast.error('New password and confirm password do not match');
      return;
    }

    try {
      setIsLoading(true);
      const token = localStorage.getItem('access_token');
      await userService.changePassword({
        currentPassword: formData.currentPassword,
        newPassword: formData.newPassword
      }, token);
      toast.success('Password updated successfully');
      setFormData({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: ''
      });
    } catch (error) {
      toast.error(error.response?.data?.message || 'Failed to update password');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <ProfileCard title="Change Password" icon={KeyRound}>
      <div className="space-y-4">
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Current Password</label>
          <input 
            type="password" 
            name="currentPassword"
            value={formData.currentPassword}
            onChange={handleChange}
            placeholder="Enter current password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">New Password</label>
          <input 
            type="password" 
            name="newPassword"
            value={formData.newPassword}
            onChange={handleChange}
            placeholder="Enter new password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>
        <div>
          <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Confirm New Password</label>
          <input 
            type="password" 
            name="confirmNewPassword"
            value={formData.confirmNewPassword}
            onChange={handleChange}
            placeholder="Confirm new password"
            className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" 
          />
        </div>

        <div className="bg-amber-500/10 border border-amber-500/20 p-3 rounded-lg text-xs font-semibold text-amber-600">
          Password must be at least 8 characters long and contain numbers and symbols.
        </div>

        <div className="pt-2">
          <button 
            onClick={handleUpdatePassword}
            disabled={isLoading}
            className="w-full px-4 py-2.5 text-sm font-bold text-white bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] disabled:opacity-50 disabled:cursor-not-allowed rounded-xl transition-all shadow-sm flex items-center justify-center gap-2 cursor-pointer"
          >
            {isLoading ? <Loader2 size={16} className="animate-spin" /> : <Save size={16} />}
            {isLoading ? 'Updating...' : 'Update Password'}
          </button>
        </div>
      </div>
    </ProfileCard>
  );
};

export default ChangePasswordCard;
