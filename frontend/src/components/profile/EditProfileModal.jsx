import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Save } from 'lucide-react';
import { userService } from '../../api/services';

const EditProfileModal = ({ isOpen, onClose, data, onProfileUpdated }) => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (data) {
      setFormData({
        firstName: data.firstName || '',
        lastName: data.lastName || '',
        email: data.email || ''
      });
    }
  }, [data, isOpen]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    try {
      setIsSubmitting(true);
      const res = await userService.updateMe(formData);
      if (res.success || res.result) {
        if (onProfileUpdated) onProfileUpdated();
        onClose();
      } else {
        alert(res.message || 'Failed to update profile');
      }
    } catch (error) {
      console.error(error);
      alert('Error updating profile');
    } finally {
      setIsSubmitting(false);
    }
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
            className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-xl w-full max-w-lg overflow-hidden flex flex-col max-h-[90vh]"
          >
            <div className="flex items-center justify-between p-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30">
              <h3 className="text-lg font-black text-[var(--theme-text)] uppercase tracking-wider">
                Edit Profile
              </h3>
              <button 
                onClick={onClose}
                className="p-1.5 hover:bg-[var(--theme-hover)] rounded-lg text-[var(--theme-textMuted)] transition-colors cursor-pointer"
              >
                <X size={18} />
              </button>
            </div>
            
            <div className="p-6 space-y-6 overflow-y-auto custom-scrollbar">
              <div className="space-y-4">
                <div>
                  <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">First Name</label>
                  <input type="text" name="firstName" value={formData.firstName} onChange={handleChange} className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" />
                </div>
                <div>
                  <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Last Name</label>
                  <input type="text" name="lastName" value={formData.lastName} onChange={handleChange} className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" />
                </div>
                <div>
                  <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Email Address</label>
                  <input type="email" name="email" value={formData.email} onChange={handleChange} className="w-full px-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm font-semibold text-[var(--theme-text)] focus:outline-none focus:border-[var(--theme-primary)] transition-colors" />
                </div>
                <div>
                  <label className="block text-xs font-bold text-[var(--theme-textMuted)] uppercase tracking-wider mb-2">Avatar Upload</label>
                  <div className="border-2 border-dashed border-[var(--theme-border)] rounded-xl p-6 text-center hover:bg-[var(--theme-hover)]/30 transition-colors cursor-pointer">
                    <p className="text-sm font-semibold text-[var(--theme-primary)]">Click to upload or drag and drop</p>
                    <p className="text-xs text-[var(--theme-textMuted)] mt-1">SVG, PNG, JPG or GIF (max. 2MB)</p>
                  </div>
                </div>
              </div>
            </div>

            <div className="flex gap-3 p-4 border-t border-[var(--theme-border)] bg-[var(--theme-hover)]/30">
              <button 
                onClick={onClose}
                className="flex-1 px-4 py-2.5 text-sm font-bold text-[var(--theme-text)] bg-transparent border border-[var(--theme-border)] rounded-xl hover:bg-[var(--theme-hover)] transition-colors cursor-pointer"
              >
                Cancel
              </button>
              <button 
                onClick={handleSave}
                disabled={isSubmitting}
                className="flex-1 px-4 py-2.5 text-sm font-bold text-white bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] rounded-xl transition-all shadow-sm flex items-center justify-center gap-2 cursor-pointer disabled:opacity-50"
              >
                <Save size={16} />
                {isSubmitting ? 'Saving...' : 'Save Changes'}
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </AnimatePresence>
  );
};

export default EditProfileModal;
