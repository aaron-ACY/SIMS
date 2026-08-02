import React from 'react';

const ScoreInput = () => {
  return (
    <div>
      <label className="block text-sm font-bold text-[var(--theme-text)] mb-2">
        Score (0 - 100)
      </label>
      <input 
        type="number" 
        min="0" 
        max="100" 
        placeholder="Enter score"
        className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-bold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors appearance-none"
      />
    </div>
  );
};

export default ScoreInput;
