import React from 'react';
import { Camera } from 'lucide-react';

const AvatarUploader = ({ avatarUrl, fullName, role = "User" }) => {
  const getInitials = (name) => {
    if (!name) return '?';
    return name.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  };

  return (
    <div className="flex flex-col items-center justify-center p-6 space-y-4">
      <div className="relative group">
        <div className="w-32 h-32 rounded-full overflow-hidden border-4 border-[var(--theme-sidebarBg)] shadow-xl bg-gradient-to-br from-[var(--theme-primary)] to-[var(--theme-primaryDark)] flex items-center justify-center text-4xl font-black text-white">
          {avatarUrl ? (
            <img src={avatarUrl} alt={fullName} className="w-full h-full object-cover" />
          ) : (
            getInitials(fullName)
          )}
        </div>
        
        {/* Upload Overlay */}
        <button className="absolute inset-0 bg-black/50 rounded-full opacity-0 group-hover:opacity-100 transition-opacity flex flex-col items-center justify-center text-white cursor-pointer">
          <Camera size={24} className="mb-1" />
          <span className="text-xs font-bold uppercase tracking-wider">Change</span>
        </button>
      </div>
      
      <div className="text-center">
        <h2 className="text-xl font-black text-[var(--theme-text)]">{fullName || 'Unknown User'}</h2>
        <p className="text-sm font-semibold text-[var(--theme-primary)]">{role}</p>
      </div>
    </div>
  );
};

export default AvatarUploader;
