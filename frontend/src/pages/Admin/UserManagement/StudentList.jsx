import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  Search, 
  Filter, 
  Plus, 
  MoreVertical,
  GraduationCap,
  Trash2
} from 'lucide-react';
import PageHeader from '../../../components/Shared/PageHeader';
import SectionCard from '../../../components/Shared/SectionCard';
import EmptyState from '../../../components/Shared/EmptyState';
import Modal from '../../../components/Shared/Modal';
import { studentService, userService } from '../../../api/services';

const StudentList = () => {
  const [students, setStudents] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    username: '', email: '', password: '', firstName: '', lastName: '',
    dateOfBirth: '', gender: 'Male', phone: '', address: '', major: '', enrollmentYear: new Date().getFullYear()
  });
  
  const fetchStudents = async () => {
    try {
      setIsLoading(true);
      const res = await studentService.getStudents();
      if (res.success) {
        setStudents(res.result || []);
      }
    } catch (error) {
      console.error('Failed to fetch students:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchStudents();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleAddStudent = async (e) => {
    e.preventDefault();
    try {
      setIsSubmitting(true);
      const res = await userService.createStudent({
        ...formData,
        enrollmentYear: parseInt(formData.enrollmentYear) || 2026
      });
      if (res.success || res.result) {
        setIsAddModalOpen(false);
        setFormData({
          username: '', email: '', password: '', firstName: '', lastName: '',
          dateOfBirth: '', gender: 'Male', phone: '', address: '', major: '', enrollmentYear: new Date().getFullYear()
        });
        fetchStudents();
      } else {
        alert(res.message || 'Failed to create student');
      }
    } catch (error) {
      console.error(error);
      alert(error.response?.data?.message || 'Error creating student');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteStudent = async (userId) => {
    if (!userId) {
      alert("This student doesn't have a registered user account to delete.");
      return;
    }
    
    try {
      const res = await userService.deleteUser(userId);
      if (res.success) {
        fetchStudents();
      } else {
        alert(res.message || 'Failed to delete user');
      }
    } catch (error) {
      console.error(error);
      alert(error.response?.data?.message || 'Error deleting user');
    }
  };

  return (
    <div className="p-6 text-[var(--theme-text)] space-y-6">
      <PageHeader 
        title="Students Directory"
        description="Manage all enrolled students in the system."
        actions={
          <button 
            onClick={() => setIsAddModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2.5 bg-[var(--theme-primary)] text-white rounded-xl text-sm font-bold hover:bg-[var(--theme-primary)]/90 transition-colors shadow-lg shadow-[var(--theme-primary)]/20"
          >
            <Plus size={18} />
            Add Student
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
              placeholder="Search by name or code..."
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
               <p className="text-[var(--theme-textMuted)] font-medium">Loading students...</p>
            </div>
          ) : students.length > 0 ? (
            <table className="w-full text-left border-collapse bg-white">
              <thead>
                <tr className="bg-[var(--theme-hover)]/50 text-[var(--theme-textMuted)] text-xs uppercase tracking-wider">
                  <th className="py-3 px-4 font-bold">Student Code</th>
                  <th className="py-3 px-4 font-bold">Name</th>
                  <th className="py-3 px-4 font-bold">DOB</th>
                  <th className="py-3 px-4 font-bold">Gender</th>
                  <th className="py-3 px-4 font-bold">Major</th>
                  <th className="py-3 px-4 font-bold">Enrollment Yr</th>
                  <th className="py-3 px-4 font-bold text-right">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-[var(--theme-border)]">
                {students.map((student) => (
                  <tr key={student.studentCode} className="hover:bg-[var(--theme-hover)]/30 transition-colors">
                    <td className="py-3 px-4 text-sm font-semibold text-[var(--theme-text)]">{student.studentCode}</td>
                    <td className="py-3 px-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-[var(--theme-primary)]/10 text-[var(--theme-primary)] flex items-center justify-center font-bold text-xs">
                          {student.fullName?.[0] || 'S'}
                        </div>
                        <span className="text-sm font-semibold text-[var(--theme-text)]">{student.fullName}</span>
                      </div>
                    </td>
                    <td className="py-3 px-4 text-sm text-[var(--theme-text)]">
                      {student.dateOfBirth ? new Date(student.dateOfBirth).toLocaleDateString() : ''}
                    </td>
                    <td className="py-3 px-4 text-sm text-[var(--theme-text)]">{student.gender}</td>
                    <td className="py-3 px-4">
                      <span className="px-2.5 py-1 bg-[var(--theme-hover)] text-[var(--theme-text)] rounded-lg text-xs font-semibold">
                        {student.major}
                      </span>
                    </td>
                    <td className="py-3 px-4 text-sm text-[var(--theme-text)]">{student.enrollmentYear}</td>
                    <td className="py-3 px-4 text-right">
                      <button 
                        onClick={() => handleDeleteStudent(student.userId)}
                        className="p-2 hover:bg-red-500/10 rounded-lg transition-colors text-red-500/70 hover:text-red-600 cursor-pointer"
                        title="Delete Student"
                      >
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <EmptyState 
              icon={GraduationCap}
              title="No students found."
              description="There are currently no students in the system. Add a new student or import from CSV."
            />
          )}
        </div>
      </SectionCard>

      <Modal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        title="Add New Student"
      >
        <form onSubmit={handleAddStudent} className="space-y-4">
          <p className="text-[var(--theme-textMuted)] text-sm mb-4">Create a new user account and student profile.</p>
          
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Username *</label>
              <input type="text" name="username" required value={formData.username} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Password *</label>
              <input type="password" name="password" required minLength={8} value={formData.password} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1 col-span-2">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Email *</label>
              <input type="email" name="email" required value={formData.email} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">First Name *</label>
              <input type="text" name="firstName" required value={formData.firstName} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Last Name *</label>
              <input type="text" name="lastName" required value={formData.lastName} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Date of Birth *</label>
              <input type="date" name="dateOfBirth" required value={formData.dateOfBirth} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Gender *</label>
              <select name="gender" required value={formData.gender} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]">
                <option value="Male">Male</option>
                <option value="Female">Female</option>
              </select>
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Phone *</label>
              <input type="tel" name="phone" required value={formData.phone} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Major *</label>
              <input type="text" name="major" required value={formData.major} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
            </div>
            <div className="space-y-1 col-span-2">
              <label className="text-xs font-semibold text-[var(--theme-text)]">Address *</label>
              <input type="text" name="address" required value={formData.address} onChange={handleInputChange} className="w-full px-3 py-2 bg-white border border-[var(--theme-border)] rounded-lg text-sm focus:outline-none focus:border-[var(--theme-primary)]" />
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
              {isSubmitting ? 'Saving...' : 'Save Student'}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
};

export default StudentList;


