import React from 'react';

const FeedbackInput = () => {
  return (
    <div>
      <label className="block text-sm font-bold text-[var(--theme-text)] mb-2">
        Feedback (Optional)
      </label>
      <textarea 
        rows="4"
        placeholder="Enter constructive feedback for the student..."
        className="w-full p-4 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors resize-none custom-scrollbar"
      ></textarea>
    </div>
  );
};

export default FeedbackInput;
