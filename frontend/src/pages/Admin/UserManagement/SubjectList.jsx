import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  Search, 
  Filter, 
  Plus, 
  MoreVertical,
  BookOpen
} from 'lucide-react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import EmptyState from '../../../components/Shared/EmptyState';
import Modal from '../../../components/Shared/Modal';
import { subjectService } from '../../../api/services';

const SubjectList = () => {
  const [subjects, setSubjects] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    subjectCode: '', name: '', description: '', credits: 3, 
    department: '', major: '', academicYear: '2026-2027', semester: 1, isRequired: true
  });
  
  const fetchSubjects = async () => {
    try {
      setIsLoading(true);
      const res = await subjectService.getSubjects();
      if (res.success) {
        setSubjects(res.result || []);
      }
    } catch (error) {
      console.error('Failed to fetch subjects:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchSubjects();
  }, []);

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({ 
      ...prev, 
      [name]: type === 'checkbox' ? checked : value 
    }));
  };

  const handleAddSubject = async (e) => {
    e.preventDefault();
    try {
      setIsSubmitting(true);
      const res = await subjectService.createSubject({
        ...formData,
        credits: parseInt(formData.credits) || 3,
        semester: parseInt(formData.semester) || 1
      });
      if (res.success || res.result) {
        setIsAddModalOpen(false);
        setFormData({
          subjectCode: '', name: '', description: '', credits: 3, 
          department: '', major: '', academicYear: '2026-2027', semester: 1, isRequired: true
        });
        fetchSubjects();
      } else {
        alert(res.message || 'Failed to create subject');
      }
    } catch (error) {
      console.error(error);
      alert(error.response?.data?.message || 'Error creating subject');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="p-6 text-[var(--theme-text)] space-y-6">
      <PageHeader 
        title="Subjects Directory"
        description="Manage academic subjects, credits, and prerequisites."
        actions={
          <button 
            onClick={() => setIsAddModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 bg-[var(--theme-primary)] text-white rounded-xl text-sm font-bold hover:bg-[var(--theme-primary)]/90 transition-colors shadow-lg shadow-[var(--theme-primary)]/20"
          >
            <Plus size={18} />
            Add Subject
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
              placeholder="Search by subject code or name..."
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
               <p className="text-[var(--theme-textMuted)] font-medium">Loading subjects...</p>
            </div>
          ) : subjects.length > 0 ? (
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-[var(--theme-hover)]/50 text-[var(--theme-textMuted)] text-xs uppercase tracking-wider">
                  <th className="py-3 px-4 font-bold">Subject Code</th>
                  <th className="py-3 px-4 font-bold">Subject Name</th>
                  <th className="py-3 px-4 font-bold">Credits</th>
                  <th className="py-3 px-4 font-bold">Department</th>
                  <th className="py-3 px-4 font-bold text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-[var(--theme-border)]">
                {subjects.map((subject) => (
                  <tr key={subject.id} className="hover:bg-[var(--theme-hover)]/30 transition-colors">
                    <td className="py-3 px-4 text-sm font-bold text-[var(--theme-primary)]">{subject.subjectCode}</td>
                    <td className="py-3 px-4 text-sm font-semibold text-[var(--theme-text)]">{subject.name}</td>
                    <td className="py-3 px-4 text-sm text-[var(--theme-text)]">
                      <span className="px-2.5 py-1 bg-[var(--theme-hover)] text-[var(--theme-text)] rounded-lg text-xs font-semibold">
                        {subject.credits} Credits
                      </span>
                    </td>
                    <td className="py-3 px-4 text-sm text-[var(--theme-text)]">{subject.department || '-'}</td>
                    <td className="py-3 px-4 text-right">
                      <button className="p-2 hover:bg-[var(--theme-hover)] rounded-lg transition-colors text-[var(--theme-textMuted)] hover:text-[var(--theme-text)]">
                        <MoreVertical size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <EmptyState 
              icon={BookOpen}
              title="No subjects available."
              description="There are currently no subjects defined in the system. Add a new subject to get started."
            />
          )}
        </div>
      </SectionCard>

      <Modal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        title="Add New Subject"
      >
        <form onSubmit={handleAddSubject} className="space-y-4">
          <p className="text-[var(--theme-textMuted)] text-sm mb-4">Fill in the details to add a new subject.</p>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Subject Code *</label>
              <input type="text" name="subjectCode" required value={formData.subjectCode} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Subject Name *</label>
              <input type="text" name="name" required value={formData.name} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1 col-span-2">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Description</label>
              <input type="text" name="description" value={formData.description} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Credits *</label>
              <input type="number" name="credits" required min={1} max={10} value={formData.credits} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Department *</label>
              <input type="text" name="department" required value={formData.department} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Major *</label>
              <input type="text" name="major" required value={formData.major} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Academic Year *</label>
              <input type="text" name="academicYear" required value={formData.academicYear} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Semester *</label>
              <select name="semester" required value={formData.semester} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]">
                <option value={1}>1</option>
                <option value={2}>2</option>
                <option value={3}>3</option>
              </select>
            </div>
            <div className="col-span-2 flex items-center gap-2 mt-2">
              <input type="checkbox" name="isRequired" id="isRequired" checked={formData.isRequired} onChange={handleInputChange} className="w-4 h-4 text-[var(--theme-primary)] rounded" />
              <label htmlFor="isRequired" className="text-sm font-semibold text-[var(--theme-text)]">Is Required Subject</label>
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
              {isSubmitting ? 'Saving...' : 'Save Subject'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default SubjectList;
