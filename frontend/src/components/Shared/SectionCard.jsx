import React from 'react';
import { motion } from 'framer-motion';

const SectionCard = ({ children, className = '' }) => {
  return (
    <motion.div
      initial={{ opacity: 0, y: 15 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4 }}
      className={`bg-[var(--theme-cardBg)] border border-[var(--theme-border)] rounded-2xl shadow-sm overflow-hidden ${className}`}
    >
      {children}
    </motion.div>
  );
};

export default SectionCard;
