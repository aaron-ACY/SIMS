import React from 'react';
import { motion } from 'framer-motion';

const PageHeader = ({ title, description, actions }) => {
  return (
    <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
      <motion.div
        initial={{ opacity: 0, x: -10 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.3 }}
      >
        <h1 className="text-2xl font-black text-[var(--theme-text)] tracking-tight">
          {title}
        </h1>
        {description && (
          <p className="text-sm font-medium text-[var(--theme-textMuted)] mt-1">
            {description}
          </p>
        )}
      </motion.div>
      {actions && (
        <motion.div
          initial={{ opacity: 0, x: 10 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3 }}
          className="flex items-center gap-3"
        >
          {actions}
        </motion.div>
      )}
    </div>
  );
};

export default PageHeader;
