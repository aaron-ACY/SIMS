import React from 'react';
import Modal from './Modal';
import { AlertTriangle, Loader2 } from 'lucide-react';
import { motion } from 'framer-motion';

const DeleteConfirmModal = ({ isOpen, onClose, onConfirm, isDeleting, title = "Confirm Deletion", message = "This action cannot be undone. This will permanently delete the user account and all associated data." }) => {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title} maxWidth="max-w-md">
      <div className="relative flex flex-col items-center text-center px-4 py-8 overflow-hidden">
        {/* Soft background glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 w-48 h-48 bg-red-500/10 blur-[50px] rounded-full pointer-events-none" />
        
        <motion.div 
          initial={{ scale: 0.5, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          transition={{ type: "spring", bounce: 0.5, duration: 0.6 }}
          className="relative w-20 h-20 bg-gradient-to-br from-red-100 to-red-50 dark:from-red-900/40 dark:to-red-900/10 rounded-full flex items-center justify-center mb-6 shadow-inner ring-4 ring-red-50 dark:ring-red-900/20"
        >
          <div className="absolute inset-0 rounded-full border border-red-200 dark:border-red-800/50" />
          <motion.div
            animate={{ scale: [1, 1.1, 1] }}
            transition={{ duration: 2, repeat: Infinity, ease: "easeInOut" }}
          >
            <AlertTriangle className="text-red-600 dark:text-red-500" size={36} strokeWidth={2.5} />
          </motion.div>
        </motion.div>

        <motion.h3 
          initial={{ y: 10, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.1 }}
          className="text-2xl font-black text-[var(--theme-text)] tracking-tight mb-2 z-10"
        >
          Are you absolutely sure?
        </motion.h3>

        <motion.p 
          initial={{ y: 10, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.2 }}
          className="text-[var(--theme-textMuted)] text-[15px] max-w-[320px] leading-relaxed mb-8 z-10"
        >
          {message}
        </motion.p>
        
        <motion.div 
          initial={{ y: 10, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.3 }}
          className="flex w-full gap-3 z-10"
        >
          <button 
            type="button"
            onClick={onClose}
            disabled={isDeleting}
            className="flex-1 px-5 py-3 text-sm font-bold text-[var(--theme-text)] bg-transparent border-2 border-[var(--theme-border)] rounded-xl hover:bg-[var(--theme-hover)] hover:border-[var(--theme-textMuted)] transition-all disabled:opacity-50"
          >
            Cancel
          </button>
          <button 
            type="button"
            onClick={onConfirm}
            disabled={isDeleting}
            className="flex-1 flex justify-center items-center gap-2 px-5 py-3 text-sm font-bold text-white bg-gradient-to-r from-red-500 to-rose-600 rounded-xl hover:from-red-600 hover:to-rose-700 transition-all shadow-lg shadow-red-500/25 hover:shadow-red-500/40 transform hover:-translate-y-0.5 active:translate-y-0 disabled:opacity-50 disabled:transform-none"
          >
            {isDeleting ? <Loader2 size={18} className="animate-spin" /> : 'Yes, Delete'}
          </button>
        </motion.div>
      </div>
    </Modal>
  );
};

export default DeleteConfirmModal;
