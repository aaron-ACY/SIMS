import React, { useState } from 'react';
import { ChevronDown, ChevronUp } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

const ProfileCard = ({ title, icon: Icon, children, defaultOpen = false, isDropdown = true }) => {
  const [isOpen, setIsOpen] = useState(defaultOpen);

  return (
    <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-sm overflow-hidden flex flex-col h-full">
      <div 
        className={`px-6 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30 flex items-center justify-between ${isDropdown ? 'cursor-pointer hover:bg-[var(--theme-hover)] transition-colors select-none' : ''}`}
        onClick={() => isDropdown && setIsOpen(!isOpen)}
      >
        <div className="flex items-center gap-2">
          {Icon && <Icon size={18} className="text-[var(--theme-primary)]" />}
          <h3 className="font-black text-sm uppercase tracking-widest text-[var(--theme-text)]">
            {title}
          </h3>
        </div>
        {isDropdown && (
          <div className="p-1 rounded-lg bg-[var(--theme-bg)] border border-[var(--theme-border)] text-[var(--theme-textMuted)] transition-colors hover:text-[var(--theme-primary)]">
            {isOpen ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
          </div>
        )}
      </div>
      
      {isDropdown ? (
        <AnimatePresence initial={false}>
          {isOpen && (
            <motion.div
              initial={{ height: 0, opacity: 0 }}
              animate={{ height: 'auto', opacity: 1 }}
              exit={{ height: 0, opacity: 0 }}
              transition={{ duration: 0.2 }}
              className="overflow-hidden"
            >
              <div className="p-6 flex-1 space-y-4">
                {children}
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      ) : (
        <div className="p-6 flex-1 space-y-4">
          {children}
        </div>
      )}
    </div>
  );
};

export default ProfileCard;
