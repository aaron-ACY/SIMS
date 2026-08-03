import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  Search, 
  Filter, 
  Plus, 
  MoreVertical,
  School
} from 'lucide-react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import EmptyState from '../../../components/Shared/EmptyState';
import Modal from '../../../components/Shared/Modal';
import { classService, instructorService, subjectService } from '../../../api/services';
import { useNavigate } from 'react-router-dom';

const ClassList = () => {
  const navigate = useNavigate();
  const [classes, setClasses] = useState([]);
  const [instructors, setInstructors] = useState([]);
  const [subjects, setSubjects] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    classCode: '', subjectId: '', instructorId: '', semester: 1, 
    academicYear: '', room: '', schedule: '', maxEnrollment: 40
  });
  
  const fetchClasses = async () => {
    try {
      setIsLoading(true);
      const res = await classService.getClasses();
      if (res.success) {
        setClasses(res.result || []);
      }
    } catch (error) {
      console.error('Failed to fetch classes:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchDropdownData = async () => {
    try {
      const [instRes, subRes] = await Promise.all([
        instructorService.getInstructors(),
        subjectService.getSubjects()
      ]);
      if (instRes.success) setInstructors(instRes.result || []);
      if (subRes.success) setSubjects(subRes.result || []);
    } catch (error) {
      console.error('Failed to fetch dropdown data:', error);
    }
  };

  useEffect(() => {
    fetchClasses();
    fetchDropdownData();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleAddClass = async (e) => {
    e.preventDefault();
    try {
      setIsSubmitting(true);
      const res = await classService.createClass({
        ...formData,
        subjectId: parseInt(formData.subjectId) || 1,
        instructorId: parseInt(formData.instructorId) || 1,
        semester: parseInt(formData.semester) || 1,
        maxEnrollment: parseInt(formData.maxEnrollment) || 40,
        room: formData.room || "",
        schedule: formData.schedule || "",
        academicYear: formData.academicYear || ""
      });
      if (res.success || res.result) {
        setIsAddModalOpen(false);
        setFormData({
          classCode: '', subjectId: '', instructorId: '', semester: 1, 
          academicYear: '', room: '', schedule: '', maxEnrollment: 40
        });
        fetchClasses();
      } else {
        alert(res.message || 'Failed to create class');
      }
    } catch (error) {
      console.error(error);
      alert(error.response?.data?.message || 'Error creating class');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="p-6 text-[var(--theme-text)] space-y-6">
      <PageHeader 
        title="Classes Directory"
        description="Manage classes, schedules, and room assignments."
        actions={
          <button 
            onClick={() => setIsAddModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 bg-[var(--theme-primary)] text-white rounded-xl text-sm font-bold hover:bg-[var(--theme-primary)]/90 transition-colors shadow-lg shadow-[var(--theme-primary)]/20"
          >
            <Plus size={18} />
            Add Class
          </button>
        }
      />

      <SectionCard>
        {/* Filters and Search Bar */}
        <div className="p-4 border-b border-[var(--theme-border)] flex flex-col sm:flex-row gap-4 justify-between items-center bg-[var(--theme-sidebarBg)]">
          <div className="relative w-full sm:w-80">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--theme-textMuted)]" size={18} />
            <input 
              type="text" 
              placeholder="Search by class code..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm focus:outline-none focus:border-[var(--theme-primary)] transition-colors"
            />
          </div>
          
          <button className="flex items-center gap-2 px-4 py-2.5 bg-[var(--theme-hover)] text-[var(--theme-text)] rounded-xl text-sm font-semibold hover:bg-[var(--theme-border)] transition-colors w-full sm:w-auto">
            <Filter size={16} />
            Filters
          </button>
        </div>

        {/* Table Area */}
        <div className="overflow-x-auto">
          {isLoading ? (
            <div className="py-20 flex flex-col items-center justify-center">
               <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin mb-4" />
               <p className="text-[var(--theme-textMuted)] font-medium">Loading classes...</p>
            </div>
          ) : classes.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6 p-2">
              {classes.map((cls) => (
                <div 
                  key={cls.classCode} 
                  onClick={() => navigate(`/admin/classes/view/${cls.classCode}`)}
                  className="bg-white border border-[var(--theme-border)] rounded-2xl p-5 hover:border-[var(--theme-primary)]/50 hover:shadow-lg transition-all duration-300 group flex flex-col h-full cursor-pointer"
                >
                  <div className="flex justify-between items-start mb-4">
                    <span className="px-3 py-1 bg-[var(--theme-primary)]/10 text-[var(--theme-primary)] rounded-lg text-sm font-bold">
                      {cls.classCode}
                    </span>
                    <span className={`px-2.5 py-1 rounded-full text-xs font-bold ${
                      cls.isActive 
                        ? 'bg-emerald-500/10 text-emerald-600 dark:text-emerald-400' 
                        : 'bg-red-500/10 text-red-600 dark:text-red-400'
                    }`}>
                      {cls.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                  
                  <div className="flex-1">
                    <h3 className="text-lg font-bold text-[var(--theme-text)] mb-2 group-hover:text-[var(--theme-primary)] transition-colors">
                      {cls.subjectName || 'Unknown Subject'}
                    </h3>
                    <p className="text-sm font-medium text-[var(--theme-textMuted)] flex items-center gap-2 mb-4">
                      <span className="w-6 h-6 rounded-full bg-[var(--theme-hover)] flex items-center justify-center text-xs">
                        {(cls.instructorName?.[0] || 'I').toUpperCase()}
                      </span>
                      {cls.instructorName || 'Unassigned Instructor'}
                    </p>
                  </div>

                  <div className="pt-4 border-t border-[var(--theme-border)] mt-4">
                    <div className="flex justify-between items-center text-xs mb-2">
                      <span className="text-[var(--theme-textMuted)] font-medium">Enrollment</span>
                      <span className="font-bold text-[var(--theme-text)]">
                        {cls.currentEnrollment || 0} / {cls.maxEnrollment || 0}
                      </span>
                    </div>
                    <div className="w-full h-2 bg-[var(--theme-hover)] rounded-full overflow-hidden">
                      <div 
                        className="h-full bg-[var(--theme-primary)] transition-all duration-500" 
                        style={{ width: `${Math.min(100, ((cls.currentEnrollment || 0) / (cls.maxEnrollment || 1)) * 100)}%` }}
                      />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <EmptyState 
              icon={School}
              title="No classes available."
              description="There are currently no classes scheduled in the system. Create a new class to get started."
            />
          )}
        </div>
      </SectionCard>

      <Modal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        title="Add New Class"
      >
        <form onSubmit={handleAddClass} className="space-y-4">
          <p className="text-[var(--theme-textMuted)] text-sm mb-4">Fill in the details to create a new class.</p>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Class Code *</label>
              <input type="text" name="classCode" required value={formData.classCode} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Subject *</label>
              <select name="subjectId" required value={formData.subjectId} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]">
                <option value="">Select a Subject...</option>
                {subjects.map(sub => (
                  <option key={sub.id} value={sub.id}>{sub.subjectCode} - {sub.name}</option>
                ))}
              </select>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Instructor *</label>
              <select name="instructorId" required value={formData.instructorId} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]">
                <option value="">Select an Instructor...</option>
                {instructors.map(inst => (
                  <option key={inst.id} value={inst.id}>{inst.instructorCode} - {inst.fullName}</option>
                ))}
              </select>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Semester *</label>
              <select name="semester" required value={formData.semester} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]">
                <option value={1}>1</option>
                <option value={2}>2</option>
                <option value={3}>3</option>
              </select>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Academic Year *</label>
              <input type="text" name="academicYear" required value={formData.academicYear} onChange={handleInputChange} placeholder="e.g. 2026-2027" className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Max Enrollment *</label>
              <input type="number" name="maxEnrollment" required min={1} max={500} value={formData.maxEnrollment} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Room</label>
              <input type="text" name="room" value={formData.room} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Schedule</label>
              <input type="text" name="schedule" value={formData.schedule} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
          </div>

          <div className="flex justify-end gap-3 mt-6 pt-4 border-t border-[var(--theme-border)]">
            <button 
              type="button"
              onClick={() => setIsAddModalOpen(false)}
              className="px-4 py-2 text-sm font-semibold text-[var(--theme-text)] bg-[var(--theme-hover)] rounded-xl hover:bg-[var(--theme-border)] transition-colors"
            >
              Cancel
            </button>
            <button 
              type="submit" 
              disabled={isSubmitting}
              className="px-4 py-2 text-sm font-semibold text-white bg-[var(--theme-primary)] rounded-xl hover:bg-[var(--theme-primary)]/90 transition-colors shadow-lg shadow-[var(--theme-primary)]/20 disabled:opacity-50"
            >
              {isSubmitting ? 'Saving...' : 'Save Class'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default ClassList;

