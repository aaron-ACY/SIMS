import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X } from 'lucide-react';

const GradeModal = ({ isOpen, onClose, studentName, onSubmitGrade, initialScore }) => {
  const [score, setScore] = useState('');

  useEffect(() => {
    if (isOpen) {
      setScore(initialScore !== undefined && initialScore !== null ? initialScore.toString() : '');
    }
  }, [isOpen, initialScore]);

  const handleSave = () => {
    const numScore = parseFloat(score);
    if (isNaN(numScore) || numScore < 0 || numScore > 100) {
      alert("Please enter a valid score between 0 and 100.");
      return;
    }
    onSubmitGrade(numScore);
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
          <motion.div 
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.2 }}
            className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-xl w-full max-w-md overflow-hidden flex flex-col"
          >
            <div className="flex items-center justify-between p-4 border-b border-[var(--theme-border)]">
              <h3 className="text-lg font-bold text-[var(--theme-text)]">
                Grade: {studentName}
              </h3>
              <button 
                onClick={onClose}
                className="p-1.5 hover:bg-[var(--theme-hover)] rounded-lg text-[var(--theme-textMuted)] transition-colors cursor-pointer"
              >
                <X size={18} />
              </button>
            </div>
            
            <div className="p-6 space-y-5">
              <div>
                <label className="block text-sm font-bold text-[var(--theme-text)] mb-2">
                  Score (0 - 100)
                </label>
                <input 
                  type="number" 
                  min="0" 
                  max="100" 
                  value={score}
                  onChange={(e) => setScore(e.target.value)}
                  placeholder="Enter score"
                  className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-bold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors appearance-none"
                />
              </div>
              
              <div className="bg-[var(--theme-hover)]/30 p-3 rounded-lg text-xs font-medium text-[var(--theme-textMuted)]">
                * Note: Grades cannot be altered once finalized for the semester.
              </div>
            </div>

            <div className="flex gap-3 p-4 border-t border-[var(--theme-border)] bg-[var(--theme-hover)]/10">
              <button 
                onClick={onClose}
                className="flex-1 px-4 py-2.5 text-sm font-semibold text-[var(--theme-text)] bg-transparent border border-[var(--theme-border)] rounded-xl hover:bg-[var(--theme-hover)] transition-colors cursor-pointer"
              >
                Cancel
              </button>
              <button 
                onClick={handleSave}
                className="flex-1 px-4 py-2.5 text-sm font-semibold text-white bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] rounded-xl transition-all shadow-sm hover:shadow cursor-pointer"
              >
                Save Grade
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </AnimatePresence>
  );
};

export default GradeModal;
