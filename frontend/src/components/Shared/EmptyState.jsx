import React from 'react';
import { motion } from 'framer-motion';

const EmptyState = ({ 
  icon: Icon, 
  title, 
  description, 
  actionLabel, 
  onAction 
}) => {
  return (
    <div className="flex flex-col items-center justify-center py-16 px-4 text-center">
      <motion.div 
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.3 }}
        className="w-20 h-20 bg-[var(--theme-hover)] text-[var(--theme-primary)] rounded-full flex items-center justify-center mb-6 shadow-sm"
      >
        {Icon && <Icon size={40} strokeWidth={1.5} />}
      </motion.div>
      <motion.h3 
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3, delay: 0.1 }}
        className="text-xl font-bold text-[var(--theme-text)] mb-2"
      >
        {title}
      </motion.h3>
      <motion.p 
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3, delay: 0.2 }}
        className="text-[var(--theme-textMuted)] max-w-sm mb-8"
      >
        {description}
      </motion.p>
      {actionLabel && onAction && (
        <motion.button
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.3, delay: 0.3 }}
          onClick={onAction}
          className="px-6 py-2.5 bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] text-white font-semibold rounded-xl transition-all shadow-sm hover:shadow-md cursor-pointer"
        >
          {actionLabel}
        </motion.button>
      )}
    </div>
  );
};

export default EmptyState;
