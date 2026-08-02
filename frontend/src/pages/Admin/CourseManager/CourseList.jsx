import React, { useState, useEffect } from 'react';
import { Plus, BookOpen, Search, MoreVertical, Edit, Trash2 } from 'lucide-react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import Modal from '../../../components/Shared/Modal';
import { courseService } from '../../../api/services';

const CourseList = () => {
  const [courses, setCourses] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    courseCode: '', name: '', description: '', credits: 3, isRequired: true
  });

  const fetchCourses = async () => {
    try {
      setIsLoading(true);
      const res = await courseService.getCourses();
      if (res.success) {
        setCourses(res.result || []);
      }
    } catch (error) {
      console.error('Failed to fetch courses:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchCourses();
  }, []);

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({ 
      ...prev, 
      [name]: type === 'checkbox' ? checked : value 
    }));
  };

  const handleAddCourse = async (e) => {
    e.preventDefault();
    try {
      setIsSubmitting(true);
      const res = await courseService.createCourse({
        ...formData,
        credits: parseInt(formData.credits) || 3,
      });
      if (res.success || res.result) {
        setIsAddModalOpen(false);
        setFormData({ courseCode: '', name: '', description: '', credits: 3, isRequired: true });
        fetchCourses();
      } else {
        alert(res.message || 'Failed to create course');
      }
    } catch (error) {
      console.error(error);
      alert(error.response?.data?.message || 'Error creating course');
    } finally {
      setIsSubmitting(false);
    }
  };

  const filteredCourses = courses.filter(c => 
    c.name?.toLowerCase().includes(searchTerm.toLowerCase()) || 
    c.courseCode?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="p-6 text-[var(--theme-text)] space-y-6">
      <PageHeader 
        title="Courses Directory"
        description="Manage academic courses and their details."
        actions={
          <button 
            onClick={() => setIsAddModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 bg-[var(--theme-primary)] text-white rounded-xl text-sm font-bold hover:bg-[var(--theme-primary)]/90 transition-colors shadow-lg shadow-[var(--theme-primary)]/20"
          >
            <Plus size={18} />
            Add Course
          </button>
        }
      />

      <SectionCard className="p-0">
        <div className="p-4 border-b border-[var(--theme-border)] flex flex-wrap items-center justify-between gap-4">
          <div className="relative max-w-md w-full">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--theme-textMuted)]" size={18} />
            <input 
              type="text" 
              placeholder="Search by name or code..." 
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-xl text-sm focus:outline-none focus:border-[var(--theme-primary)]"
            />
          </div>
        </div>

        <div className="overflow-x-auto">
          {isLoading ? (
            <div className="py-20 flex justify-center">
              <div className="w-8 h-8 border-4 border-[var(--theme-primary)]/30 border-t-[var(--theme-primary)] rounded-full animate-spin" />
            </div>
          ) : filteredCourses.length > 0 ? (
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="text-[var(--theme-textMuted)] text-xs uppercase tracking-wider border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/30">
                  <th className="py-4 px-6 font-bold">Course Code</th>
                  <th className="py-4 px-6 font-bold">Course Name</th>
                  <th className="py-4 px-6 font-bold">Description</th>
                  <th className="py-4 px-6 font-bold">Credits</th>
                  <th className="py-4 px-6 font-bold">Required</th>
                  <th className="py-4 px-6 font-bold">Status</th>
                  <th className="py-4 px-6 font-bold text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-[var(--theme-border)]">
                {filteredCourses.map((course) => (
                  <tr key={course.id} className="hover:bg-[var(--theme-hover)]/50 transition-colors group">
                    <td className="py-4 px-6 text-sm font-bold text-[var(--theme-primary)]">{course.courseCode}</td>
                    <td className="py-4 px-6 text-sm font-semibold text-[var(--theme-text)]">{course.name}</td>
                    <td className="py-4 px-6 text-sm text-[var(--theme-textMuted)] truncate max-w-[200px]">{course.description || '-'}</td>
                    <td className="py-4 px-6 text-sm text-[var(--theme-textMuted)]">{course.credits}</td>
                    <td className="py-4 px-6 text-sm text-[var(--theme-textMuted)]">
                      {course.isRequired ? (
                        <span className="px-2 py-1 bg-amber-500/10 text-amber-600 rounded text-xs font-semibold">Required</span>
                      ) : (
                        <span className="px-2 py-1 bg-emerald-500/10 text-emerald-600 rounded text-xs font-semibold">Optional</span>
                      )}
                    </td>
                    <td className="py-4 px-6">
                      <span className={`px-2.5 py-1 rounded-full text-xs font-bold ${
                        course.isActive 
                          ? 'bg-emerald-500/10 text-emerald-600 dark:text-emerald-400' 
                          : 'bg-red-500/10 text-red-600 dark:text-red-400'
                      }`}>
                        {course.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-4 px-6 text-right">
                      <button className="p-2 hover:bg-[var(--theme-hover)] rounded-lg transition-colors text-[var(--theme-textMuted)] hover:text-[var(--theme-text)]">
                        <MoreVertical size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="py-20 flex flex-col items-center justify-center text-center px-4">
              <div className="w-16 h-16 bg-[var(--theme-primary)]/10 text-[var(--theme-primary)] rounded-full flex items-center justify-center mb-4">
                <BookOpen size={32} />
              </div>
              <h3 className="text-lg font-bold text-[var(--theme-text)]">No courses found.</h3>
              <p className="text-[var(--theme-textMuted)] mt-2 max-w-md">There are currently no courses in the system. Add a new course to get started.</p>
            </div>
          )}
        </div>
      </SectionCard>

      <Modal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        title="Add New Course"
      >
        <form onSubmit={handleAddCourse} className="space-y-4">
          <p className="text-[var(--theme-textMuted)] text-sm mb-4">Fill in the details to add a new academic course.</p>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Course Code *</label>
              <input type="text" name="courseCode" required value={formData.courseCode} onChange={handleInputChange} className="w-full px-3 py-2 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Course Name *</label>
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
            <div className="space-y-1 flex items-center mt-6">
              <label className="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" name="isRequired" checked={formData.isRequired} onChange={handleInputChange} className="w-4 h-4 text-[var(--theme-primary)] bg-[var(--theme-bg)] border-[var(--theme-border)] rounded focus:ring-[var(--theme-primary)]" />
                <span className="text-sm font-semibold text-[var(--theme-text)]">Is Required Course</span>
              </label>
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
              {isSubmitting ? 'Saving...' : 'Save Course'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default CourseList;
