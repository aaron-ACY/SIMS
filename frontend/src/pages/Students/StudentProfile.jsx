import React, { useState, useEffect } from 'react';
import { Edit } from 'lucide-react';
import { motion } from 'framer-motion';

import AvatarUploader from '../../components/profile/AvatarUploader';
import AccountInformation from '../../components/profile/AccountInformation';
import ChangePasswordCard from '../../components/profile/ChangePasswordCard';
import EditProfileModal from '../../components/profile/EditProfileModal';
import SkeletonCard from '../../components/dashboard/SkeletonCard';

import StudentPersonalInfo from '../../components/student/profile/StudentPersonalInfo';
import StudentAcademicInfo from '../../components/student/profile/StudentAcademicInfo';

const StudentProfile = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [profileData, setProfileData] = useState(null); // null when no data
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  // Future API Integration
  useEffect(() => {
    // setIsLoading(true);
    // fetchStudentProfile().then(data => setProfileData(data)).finally(() => setIsLoading(false));
  }, []);

  if (isLoading) {
    return (
      <div className="p-6 space-y-6">
        <SkeletonCard type="overview" />
        <SkeletonCard type="list" />
      </div>
    );
  }

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)] p-6"
    >
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 className="text-3xl font-black tracking-tight">Student Profile</h2>
          <p className="text-sm font-bold text-[var(--theme-textMuted)]">Manage your personal information and account settings.</p>
        </div>
        <button 
          onClick={() => setIsEditModalOpen(true)}
          className="flex items-center gap-2 px-4 py-2 bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] text-white font-bold text-xs uppercase tracking-widest rounded-xl transition-colors shadow-sm cursor-pointer"
        >
          <Edit size={16} />
          Edit Profile
        </button>
      </div>

      {!profileData ? (
        <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-12 text-center shadow-sm">
          <p className="text-[var(--theme-textMuted)] font-bold mb-2">Unable to load profile.</p>
          <p className="text-sm text-[var(--theme-textMuted)]">Please try again later.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
          {/* Left Column - Avatar & Account */}
          <div className="space-y-6">
            <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-sm">
              <AvatarUploader 
                avatarUrl={profileData?.avatarUrl} 
                fullName={profileData?.personalInfo?.fullName} 
                role="Student"
              />
            </div>
            
            <AccountInformation data={profileData?.accountInfo} />
          </div>

          {/* Right Column - Info & Password */}
          <div className="lg:col-span-2 space-y-6">
            <StudentPersonalInfo data={profileData?.personalInfo} />
            <StudentAcademicInfo data={profileData?.academicInfo} />
            <ChangePasswordCard />
          </div>
        </div>
      )}

      {/* Edit Modal */}
      <EditProfileModal 
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        data={profileData?.personalInfo}
      />
    </motion.div>
  );
};

export default StudentProfile;
