import React, { useState, useEffect } from 'react';
import { BookOpen, ExternalLink, ChevronDown, FileText, Download, School } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

const StudentMaterials = () => {
  const [repositoryType, setRepositoryType] = useState('SchoolRepository'); // 'SchoolRepository' | 'InstructorResources'
  const [selectedClass, setSelectedClass] = useState('all');
  
  const [schoolMaterials, setSchoolMaterials] = useState([]);
  const [instructorMaterials, setInstructorMaterials] = useState([]);
  const [enrolledCourses, setEnrolledCourses] = useState([]);

  const classDetails = {
    'IT101': 'Software Engineering',
    'IT102': 'Database Management Systems',
    'IT103': 'Web Application Development',
    'MATH201': 'Probability and Statistics'
  };

  useEffect(() => {
    // 1. Get student's enrolled courses from localStorage
    const enrolledStr = localStorage.getItem('student_enrolled_courses');
    const enrolledList = enrolledStr ? JSON.parse(enrolledStr) : ['IT101', 'IT102', 'IT103'];
    setEnrolledCourses(enrolledList);

    // 2. Load school repository materials (sync with Admin key)
    const storedSchool = localStorage.getItem('school_repository_materials');
    if (storedSchool) {
      setSchoolMaterials(JSON.parse(storedSchool));
    } else {
      // Default fallback seed matching Admin seed
      const defaultSchool = [
        { id: 'sm1', title: 'SE Course Outline & Objectives', subjectCode: 'SE-IT101', type: 'Syllabus', driveLink: 'https://drive.google.com/drive/folders/1official_se' },
        { id: 'sm2', title: 'Agile & Scrum Framework Handbook', subjectCode: 'SE-IT101', type: 'Textbook', driveLink: 'https://drive.google.com/drive/folders/1official_scrum' },
        { id: 'sm3', title: 'Database Systems Lecture Notes', subjectCode: 'DBMS-IT102', type: 'Lecture Slide', driveLink: 'https://drive.google.com/drive/folders/1official_dbms' },
        { id: 'sm4', title: 'SQL Complex Queries Standard Reference', subjectCode: 'DBMS-IT102', type: 'Cheat Sheet', driveLink: 'https://drive.google.com/drive/folders/1official_sql' },
        { id: 'sm5', title: 'React 19 Hooks and Context Guide', subjectCode: 'WAD-IT103', type: 'Lecture Slide', driveLink: 'https://drive.google.com/drive/folders/1official_react' },
        { id: 'sm6', title: 'Probability Distributions & Regression Formulas', subjectCode: 'MATH-201', type: 'Cheat Sheet', driveLink: 'https://drive.google.com/drive/folders/1official_math' }
      ];
      localStorage.setItem('school_repository_materials', JSON.stringify(defaultSchool));
      setSchoolMaterials(defaultSchool);
    }

    // 3. Load instructor materials (sync with Lecturer key)
    const storedInstructor = localStorage.getItem('instructor_materials');
    if (storedInstructor) {
      setInstructorMaterials(JSON.parse(storedInstructor));
    } else {
      const defaultInstructor = [
        { id: 'm1', classCode: 'IT101', title: 'Software Requirement Specification (SRS) Template', description: 'Standard IEEE SRS template for Assignment 1.', type: 'Reference', fileName: 'IEEE_SRS_Template.pdf', fileSize: '1.2 MB', date: '10/05/2026' },
        { id: 'm2', classCode: 'IT101', title: 'Design Patterns Reference Manual', description: 'Brief guide on structural, creational, and behavioral design patterns.', type: 'Book', fileName: 'Design_Patterns_Guide.pdf', fileSize: '2.5 MB', date: '11/05/2026' },
        { id: 'm3', classCode: 'IT102', title: 'Relational Algebra & Normalization Exercises', description: 'Practice exercises for 1NF, 2NF, 3NF, and BCNF normalization.', type: 'Reference', fileName: 'Normalization_Practice.docx', fileSize: '850 KB', date: '04/05/2026' },
        { id: 'm4', classCode: 'IT103', title: 'React Hook Cheatsheet', description: 'React hooks cheat sheet including useState, useEffect, useContext.', type: 'Slide', fileName: 'React_Hooks_CheatSheet.png', fileSize: '1.8 MB', date: '14/05/2026' },
      ];
      localStorage.setItem('instructor_materials', JSON.stringify(defaultInstructor));
      setInstructorMaterials(defaultInstructor);
    }
  }, []);

  // Filter lists based on student's enrolled courses and selected dropdown filter
  const getFilteredMaterials = () => {
    if (repositoryType === 'SchoolRepository') {
      return schoolMaterials.filter(mat => {
        // Find if this subjectCode matches any enrolled courses
        const isEnrolled = enrolledCourses.some(code => mat.subjectCode.toLowerCase().includes(code.toLowerCase()));
        if (!isEnrolled) return false;

        // Filter by class dropdown selection
        if (selectedClass !== 'all') {
          return mat.subjectCode.toLowerCase().includes(selectedClass.toLowerCase());
        }
        return true;
      });
    } else {
      return instructorMaterials.filter(mat => {
        // Check if instructor class code is in enrolled list
        const isEnrolled = enrolledCourses.includes(mat.classCode);
        if (!isEnrolled) return false;

        // Filter by class dropdown selection
        if (selectedClass !== 'all') {
          return mat.classCode === selectedClass;
        }
        return true;
      });
    }
  };

  const activeMaterials = getFilteredMaterials();

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 text-[var(--theme-text)]"
    >
      {/* Header Row */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 className="text-3xl font-black tracking-tight">Course Materials</h2>
          <p className="text-sm text-[var(--theme-text)]/50 font-bold">
            Access official school syllabus guides or review handouts and slides uploaded by your class lecturers.
          </p>
        </div>

        {/* Tab switch */}
        <div className="flex bg-[var(--theme-hover)]/30 p-1 rounded-[8px] border border-[var(--theme-border)] w-full sm:w-auto">
          <button 
            onClick={() => setRepositoryType('SchoolRepository')}
            className={`flex-1 sm:flex-initial px-4 py-2 text-xs font-black uppercase tracking-wider rounded-[6px] transition-all flex items-center justify-center gap-1.5 ${
              repositoryType === 'SchoolRepository' 
                ? 'bg-[var(--theme-primary)] text-white' 
                : 'text-[var(--theme-text)]/65 hover:text-[var(--theme-text)]'
            }`}
          >
            <School size={13} />
            School Repository
          </button>
          <button 
            onClick={() => setRepositoryType('InstructorResources')}
            className={`flex-1 sm:flex-initial px-4 py-2 text-xs font-black uppercase tracking-wider rounded-[6px] transition-all flex items-center justify-center gap-1.5 ${
              repositoryType === 'InstructorResources' 
                ? 'bg-[var(--theme-primary)] text-white' 
                : 'text-[var(--theme-text)]/65 hover:text-[var(--theme-text)]'
            }`}
          >
            <BookOpen size={13} />
            Instructor Resources
          </button>
        </div>
      </div>

      {/* Dropdown Class Filter */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 bg-[var(--theme-sidebarBg)] p-4 border border-[var(--theme-border)] rounded-[8px]">
        <div className="relative w-full sm:w-64">
          <select 
            value={selectedClass} 
            onChange={(e) => setSelectedClass(e.target.value)}
            className="w-full pl-4 pr-10 py-2.5 bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-[8px] outline-none font-bold text-sm text-[var(--theme-text)] cursor-pointer appearance-none"
          >
            <option value="all">All Enrolled Classes</option>
            {enrolledCourses.map(code => (
              <option key={code} value={code}>
                {code} - {classDetails[code] || 'Course Class'}
              </option>
            ))}
          </select>
          <ChevronDown size={16} className="absolute right-3 top-1/2 -translate-y-1/2 text-[var(--theme-text)]/40 pointer-events-none" />
        </div>

        <div className="text-xs text-[var(--theme-text)]/40 font-bold uppercase tracking-wider">
          Total Resources: {activeMaterials.length} items
        </div>
      </div>

      {/* Materials List Table / Cards */}
      <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] overflow-hidden">
        <div className="px-8 py-4 border-b border-[var(--theme-border)] bg-[var(--theme-hover)]/10">
          <h3 className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]/70">
            {repositoryType === 'SchoolRepository' ? 'Academic Affairs Official drive docs' : 'Instructor Handouts & Practice Sheets'}
          </h3>
        </div>

        <div className="divide-y divide-[var(--theme-border)]">
          {activeMaterials.length > 0 ? (
            activeMaterials.map((item, idx) => (
              <div key={idx} className="p-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 hover:bg-[var(--theme-hover)]/10 transition-all">
                <div className="flex gap-4 items-start min-w-0">
                  <div className="p-3 bg-[var(--theme-primary)]/10 text-[var(--theme-primary)] rounded-[8px] shrink-0">
                    <FileText size={24} />
                  </div>
                  <div className="min-w-0">
                    <div className="flex items-center gap-2 mb-1 flex-wrap">
                      <span className="px-2 py-0.5 bg-[var(--theme-hover)] text-[var(--theme-text)]/70 text-[10px] font-black rounded font-mono shrink-0">
                        {repositoryType === 'SchoolRepository' ? item.subjectCode : item.classCode}
                      </span>
                      <span className="px-2 py-0.5 bg-[var(--theme-primary)]/10 text-[var(--theme-primary)] text-[10px] font-black rounded uppercase tracking-wider shrink-0">
                        {item.type}
                      </span>
                      <h4 className="font-bold text-base text-[var(--theme-text)] truncate">{item.title}</h4>
                    </div>
                    {item.description && (
                      <p className="text-xs text-[var(--theme-text)]/60 font-medium mb-1.5">{item.description}</p>
                    )}
                    <div className="flex items-center gap-3 text-[10px] text-[var(--theme-text)]/40 font-bold uppercase tracking-wide">
                      {item.fileName && <span>File: {item.fileName}</span>}
                      {item.fileName && <span>•</span>}
                      {item.fileSize && <span>Size: {item.fileSize}</span>}
                      {item.fileSize && <span>•</span>}
                      <span>Published: {item.date || 'Syllabus Standard'}</span>
                    </div>
                  </div>
                </div>

                <div className="shrink-0 self-end sm:self-center">
                  {repositoryType === 'SchoolRepository' ? (
                    <a 
                      href={item.driveLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="px-4 py-2 bg-[var(--theme-primary)] hover:opacity-90 text-white rounded-[8px] font-bold text-xs uppercase tracking-wider flex items-center gap-1.5 transition-all no-underline hover:no-underline"
                    >
                      <ExternalLink size={14} />
                      View on Drive
                    </a>
                  ) : (
                    <button 
                      onClick={() => alert(`Downloading reference material: ${item.fileName}`)}
                      className="px-4 py-2 bg-[var(--theme-primary)] hover:opacity-90 text-white rounded-[8px] font-bold text-xs uppercase tracking-wider flex items-center gap-1.5 transition-all"
                    >
                      <Download size={14} />
                      Download
                    </button>
                  )}
                </div>
              </div>
            ))
          ) : (
            <div className="py-20 flex flex-col items-center justify-center text-[var(--theme-text)]/30">
              <BookOpen size={48} strokeWidth={1} className="mb-4 opacity-20" />
              <p className="font-black text-sm uppercase tracking-widest">No materials published yet</p>
              <p className="text-xs font-bold mt-1">Please select another class or tab folder.</p>
            </div>
          )}
        </div>
      </div>
    </motion.div>
  );
};

export default StudentMaterials;
