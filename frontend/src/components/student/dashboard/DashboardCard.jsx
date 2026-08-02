import React from 'react';

const DashboardCard = ({ title, icon: Icon, children }) => {
  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-sm overflow-hidden flex flex-col h-full">
      <div className="px-6 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30 flex items-center gap-2">
        {Icon && <Icon size={18} className="text-[var(--theme-primary)]" />}
        <h3 className="font-black text-sm uppercase tracking-widest text-[var(--theme-text)]">
          {title}
        </h3>
      </div>
      <div className="p-6 flex-1 flex flex-col">
        {children}
      </div>
    </div>
  );
};

export default DashboardCard;
