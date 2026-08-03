import React, { useState, useEffect } from 'react';
import { Edit, Camera, Plus, ChevronDown } from 'lucide-react';
import { motion } from 'framer-motion';
import { userService, instructorService } from '../../api/services';

import AvatarUploader from '../../components/profile/AvatarUploader';
import ProfileInformation from '../../components/profile/ProfileInformation';
import AcademicInformation from '../../components/profile/AcademicInformation';
import AccountInformation from '../../components/profile/AccountInformation';
import ChangePasswordCard from '../../components/profile/ChangePasswordCard';
import EditProfileModal from '../../components/profile/EditProfileModal';
import SkeletonCard from '../../components/dashboard/SkeletonCard';

const LecturerProfile = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [profileData, setProfileData] = useState(null); // null when no data
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const getInitials = (name) => {
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  };

  const fetchProfileData = async () => {
    try {
      setIsLoading(true);
      const res = await userService.getMe();
      if (res.success && res.result) {
        const user = res.result;
        
        let department = user.department?.trim() || 'Contact Admin';
        let degree = user.degree?.trim() || 'Contact Admin';
        let phone = user.phone?.trim() || '';

        setProfileData({
          avatarUrl: null,
          personalInfo: {
            firstName: user.firstName,
            lastName: user.lastName,
            fullName: (user.firstName + ' ' + user.lastName).trim(),
            email: user.email,
            phone: phone,
            code: user.instructorCode || 'N/A',
          },
          accountInfo: {
            username: user.username,
            role: user.role,
            status: 'Active'
          },
          academicInfo: {
            instructorCode: user.instructorCode || 'N/A',
            department: department,
            degree: degree,
            permissions: user.permissions || []
          }
        });
      }
    } catch (error) {
      console.error('Failed to fetch profile:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchProfileData();
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
      className="text-[var(--theme-text)] pb-12 w-full"
    >
      {!profileData ? (
        <div className="p-6 max-w-6xl mx-auto">
          <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl p-12 text-center shadow-sm">
            <p className="text-[var(--theme-textMuted)] font-bold mb-2">Unable to load profile.</p>
            <p className="text-sm text-[var(--theme-textMuted)]">Please try again later.</p>
          </div>
        </div>
      ) : (
        <div className="flex flex-col w-full">
          {/* FACEBOOK HEADER AREA */}
          <div className="bg-[var(--theme-sidebarBg)] shadow-sm border-b border-[var(--theme-border)]">
            <div className="max-w-6xl mx-auto w-full">
              {/* Cover Photo */}
              <div className="relative h-64 md:h-[350px] w-full bg-gradient-to-r from-gray-300 to-gray-400 sm:rounded-b-xl overflow-hidden group">
                <img 
                  src="https://images.unsplash.com/photo-1557683316-973673baf926?q=80&w=2029&auto=format&fit=crop" 
                  alt="Cover" 
                  className="w-full h-full object-cover" 
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/40 via-transparent to-transparent"></div>
              </div>

              {/* Profile Details Area */}
              <div className="px-4 md:px-8 pb-0">
                <div className="flex flex-col md:flex-row items-center md:items-end justify-between relative -mt-12 md:-mt-16 gap-4 md:gap-0">
                  
                  {/* Avatar & Name */}
                  <div className="flex flex-col md:flex-row items-center md:items-end gap-4 md:gap-6 w-full md:w-auto">
                    <div className="relative group z-10">
                      <div className="w-40 h-40 md:w-[168px] md:h-[168px] rounded-full border-4 border-[var(--theme-sidebarBg)] bg-gradient-to-br from-[var(--theme-primary)] to-[var(--theme-primaryDark)] flex items-center justify-center text-5xl md:text-6xl font-black text-white shadow-sm overflow-hidden relative">
                        {profileData?.avatarUrl ? (
                          <img src={profileData.avatarUrl} alt="Avatar" className="w-full h-full object-cover" />
                        ) : (
                          getInitials(profileData?.personalInfo?.fullName)
                        )}
                        {/* Hover Overlay */}
                        <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center cursor-pointer">
                          <Camera size={32} className="text-white opacity-80" />
                        </div>
                      </div>
                      <button className="absolute bottom-3 right-3 p-2 bg-gray-200 hover:bg-gray-300 text-black rounded-full border-[3px] border-[var(--theme-sidebarBg)] transition-colors cursor-pointer shadow-sm z-20">
                        <Camera size={20} />
                      </button>
                    </div>

                    <div className="text-center md:text-left md:mb-6 mt-2 md:mt-0">
                      <h1 className="text-3xl md:text-[32px] font-black tracking-tight">{profileData?.personalInfo?.fullName || 'Unknown User'}</h1>
                      <p className="text-sm font-semibold text-[var(--theme-textMuted)] mt-1">Instructor • {profileData?.academicInfo?.department}</p>
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="flex items-center gap-2 md:mb-6 w-full md:w-auto justify-center md:justify-end">
                    <button 
                      onClick={() => setIsEditModalOpen(true)}
                      className="flex-1 md:flex-none flex items-center justify-center gap-2 px-4 py-2 bg-gray-200 hover:bg-gray-300 text-black font-bold text-[15px] rounded-lg transition-colors cursor-pointer shadow-sm"
                    >
                      <Edit size={16} strokeWidth={2.5} />
                      Edit profile
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* MAIN CONTENT AREA */}
          <div className="max-w-6xl mx-auto w-full p-4 md:p-6 mt-4">
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
              {/* Left Column - Intro/About */}
              <div className="space-y-6">
                <AccountInformation data={profileData?.accountInfo} />
              </div>

              {/* Right Column - Feed / Details */}
              <div className="lg:col-span-2 space-y-4">
                <ProfileInformation data={profileData?.personalInfo} />
                <AcademicInformation data={profileData?.academicInfo} />
                <ChangePasswordCard />
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Edit Modal */}
      <EditProfileModal 
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        data={profileData?.personalInfo}
        onProfileUpdated={fetchProfileData}
      />
    </motion.div>
  );
};

export default LecturerProfile;
