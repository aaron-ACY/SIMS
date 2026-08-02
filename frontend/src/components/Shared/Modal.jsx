import React, { useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X } from 'lucide-react';

const Modal = ({ isOpen, onClose, title, children, maxWidth = 'max-w-md' }) => {
  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'unset';
    }
    return () => {
      document.body.style.overflow = 'unset';
    };
  }, [isOpen]);

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/60 backdrop-blur-sm z-40"
          />
          <div className="fixed inset-0 flex items-center justify-center p-4 z-50 pointer-events-none">
            <motion.div
              initial={{ opacity: 0, scale: 0.95, y: 20 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.95, y: 20 }}
              transition={{ type: "spring", duration: 0.5, bounce: 0.3 }}
              className={`w-full ${maxWidth} bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-2xl shadow-2xl overflow-hidden pointer-events-auto flex flex-col max-h-[90vh]`}
            >
              <div className="flex items-center justify-between p-4 sm:p-6 border-b border-[var(--theme-border)] bg-[var(--theme-sidebarBg)]">
                <h2 className="text-xl font-bold text-[var(--theme-text)]">{title}</h2>
                <button 
                  onClick={onClose}
                  className="p-2 hover:bg-[var(--theme-hover)] text-[var(--theme-textMuted)] hover:text-[var(--theme-text)] rounded-xl transition-colors"
                >
                  <X size={20} />
                </button>
              </div>
              <div className="p-4 sm:p-6 overflow-y-auto">
                {children}
              </div>
            </motion.div>
          </div>
        </>
      )}
    </AnimatePresence>
  );
};

export default Modal;
