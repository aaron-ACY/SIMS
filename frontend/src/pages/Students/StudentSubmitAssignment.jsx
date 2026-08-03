import React, { useState, useRef, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { UploadCloud, ArrowLeft, FileText, X } from 'lucide-react';
import { motion } from 'framer-motion';
import { userService, studentService, gradeService } from '../../api/services';

const StudentSubmitAssignment = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const fileInputRef = useRef(null);

  const [assignment, setAssignment] = useState(null);
  const [selectedFile, setSelectedFile] = useState(null);
  const [dragActive, setDragActive] = useState(false);

  useEffect(() => {
    const fetchAssignmentData = async () => {
      try {
        const userRes = await userService.getMe();
        if (!userRes.success || !userRes.result?.studentCode) return;
        const studentCode = userRes.result.studentCode;

        const [classesRes, gradesRes] = await Promise.all([
          studentService.getMyClasses(),
          gradeService.getStudentGrades(studentCode)
        ]);

        const classes = (classesRes.success ? classesRes.result : []) || [];
        const gradesData = (gradesRes.success && gradesRes.result ? gradesRes.result.classes : []) || [];

        // Find the specific class matching the enrollment ID
        const cls = classes.find(c => String(c.enrollmentId) === String(id) || String(c.classId) === String(id));
        if (cls) {
          let score = null;
          let status = 'Pending';
          let rating = '';
          
          const classGradeGroup = gradesData.find(g => g.classCode === cls.classCode);
          if (classGradeGroup && classGradeGroup.grades && classGradeGroup.grades.length > 0) {
            score = classGradeGroup.grades[0].scores;
            rating = classGradeGroup.grades[0].rating;
            if (score > 0 || rating) {
              status = 'Graded';
            } else {
              status = 'Submitted';
            }
          }

          setAssignment({
            id: cls.enrollmentId,
            title: `${cls.subjectName} Assignment`,
            subject: cls.subjectName,
            status: status,
            grade: score !== null ? `${score}/10` : '--', // assuming /10 scale based on 85/100 -> actually SIMS uses 10 point scale
            rating: rating
          });

          if (status === 'Submitted' || status === 'Graded') {
             // Mock file selection since backend doesn't return file info
             setSelectedFile({
               name: 'submitted_assignment.pdf',
               size: '1.2 MB',
               type: 'pdf'
             });
          }
        }
      } catch (err) {
        console.error('Failed to load assignment', err);
      }
    };
    fetchAssignmentData();
  }, [id]);

  if (!assignment) {
    return (
      <div className="py-20 text-center text-[var(--theme-text)]/40 font-bold">
        Loading assignment details...
      </div>
    );
  }

  const isGraded = assignment.status === 'Graded'; 
  const isSubmitted = assignment.status === 'Submitted' || assignment.status === 'Graded'; 

  // Get raw grade score (e.g. "85/100" -> "85")
  const getRawGrade = (gradeStr) => {
    if (!gradeStr || gradeStr === '--') return null;
    return gradeStr.split('/')[0];
  };

  const handleDrag = (e) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === "dragenter" || e.type === "dragover") {
      setDragActive(true);
    } else if (e.type === "dragleave") {
      setDragActive(false);
    }
  };

  const handleDrop = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);

    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      const file = e.dataTransfer.files[0];
      setSelectedFile(file);
    }
  };

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setSelectedFile(file);
    }
  };

  const removeFile = () => {
    setSelectedFile(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const triggerFileInput = () => {
    fileInputRef.current.click();
  };

  const handleSubmitAssignment = async () => {
    if (!selectedFile) return;

    // Use the enrollmentId (id) to submit
    try {
      const res = await gradeService.submitAssignment(assignment.id, selectedFile);
      if (res.success) {
        setAssignment({
          ...assignment,
          status: 'Submitted',
          grade: '--'
        });
        alert('Assignment submitted successfully!');
        navigate('/student/assignments');
      } else {
        alert(res.errors?.join('\n') || 'Failed to submit assignment');
      }
    } catch (err) {
      console.error('Submission failed', err);
      alert('An error occurred while submitting.');
    }
  };

  return (
    <motion.div 
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="text-[var(--theme-text)] max-w-4xl mx-auto space-y-6"
    >
      {/* Top bar with back button */}
      <div className="flex items-center gap-4">
        <button 
          onClick={() => navigate('/student/assignments')}
          className="p-2.5 bg-[var(--theme-sidebarBg)] hover:bg-[var(--theme-hover)] text-[var(--theme-text)]/75 rounded-[8px] transition-colors border border-[var(--theme-border)]"
        >
          <ArrowLeft size={18} />
        </button>
        <div>
          <h2 className="text-2xl font-black tracking-tight">{assignment.title}</h2>
          <p className="text-xs text-[var(--theme-text)]/50 font-bold">{assignment.subject}</p>
        </div>
      </div>

      {/* Box 1: Status Box (Theme Compatible) */}
      <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] p-6 shadow-none">
        <h3 className="font-black text-[var(--theme-text)] text-lg mb-4">Status</h3>
        
        <div className="space-y-3">
          {/* Grading status */}
          <div className="flex items-baseline">
            <span className="w-36 text-[var(--theme-text)]/70 text-sm font-semibold">Grading status:</span>
            <span className={`text-sm font-bold ${
              isGraded ? 'text-green-600' : isSubmitted ? 'text-blue-600' : 'text-[var(--theme-text)]'
            }`}>
              {isGraded ? 'Graded' : isSubmitted ? 'Submitted' : 'not Graded'}
            </span>
          </div>

          {/* Deadline */}
          <div className="flex items-baseline">
            <span className="w-36 text-[var(--theme-text)]/70 text-sm font-semibold">Deadline:</span>
            <span className="text-[var(--theme-primary)] hover:underline cursor-pointer text-sm font-bold">
              Tuesday, 17 March 2028
            </span>
          </div>

          {/* Time Remaining */}
          <div className="flex items-baseline">
            <span className="w-36 text-[var(--theme-text)]/70 text-sm font-semibold">Time Remaining:</span>
            <span className="text-[var(--theme-text)] text-sm font-bold">
              {isSubmitted ? 'Submitted 1 day 15 hours early' : '7 days 5 hours'}
            </span>
          </div>

          {/* Last modified */}
          <div className="flex items-baseline">
            <span className="w-36 text-[var(--theme-text)]/70 text-sm font-semibold">Last modified:</span>
            <span className="text-[var(--theme-text)] text-sm font-bold">
              {isSubmitted ? 'Tuesday, 17 March 2028' : '--'}
            </span>
          </div>
        </div>
      </div>

      {/* DETAIL INFORMATION Label */}
      <p className="text-[10px] font-black text-[var(--theme-text)]/40 uppercase tracking-widest ml-1">
        Detail information
      </p>

      {/* Box 2: Submission Area (Theme Compatible Grid Layout) */}
      <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] p-6 shadow-none">
        
        {/* Hidden File Input */}
        <input 
          type="file" 
          ref={fileInputRef}
          onChange={handleFileChange}
          disabled={isGraded}
          className="hidden" 
        />

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 items-stretch">
          
          {/* Left Column: Upload Zone & Submit button */}
          <div className="flex flex-col justify-between min-h-[220px]">
            {/* Upload Zone */}
            <div 
              onDragEnter={handleDrag}
              onDragOver={handleDrag}
              onDragLeave={handleDrag}
              onDrop={handleDrop}
              onClick={!isGraded ? triggerFileInput : undefined}
              className={`border-2 border-dashed border-[var(--theme-primary)]/40 rounded-[8px] bg-[var(--theme-primary)]/5 p-6 flex flex-col items-center justify-center transition-colors w-full h-44 ${
                !isGraded ? 'cursor-pointer hover:bg-[var(--theme-primary)]/10 hover:border-[var(--theme-primary)]/80' : 'cursor-not-allowed'
              }`}
            >
              <UploadCloud size={38} className="text-[var(--theme-primary)] mb-2" />
              <p className="font-bold text-[var(--theme-primary)] text-sm mb-1">Upload Zone</p>
              <p className="text-[11px] text-[var(--theme-text)]/60 font-bold text-center">
                Drag and Drop your file or <span className="text-[var(--theme-primary)] underline">browse</span>
              </p>
            </div>

            {/* Submit Button */}
            <button 
              onClick={handleSubmitAssignment}
              disabled={!selectedFile || isGraded}
              className={`w-full py-2.5 font-bold text-sm rounded-[8px] transition-colors mt-4 ${
                selectedFile && !isGraded
                  ? 'bg-[var(--theme-primary)] hover:opacity-90 text-white active:scale-[0.99]' 
                  : 'bg-[var(--theme-hover)]/50 text-[var(--theme-text)]/30 cursor-not-allowed'
              }`}
            >
              Submit
            </button>
          </div>

          {/* Right Column: Submitted file & Edit Submission button */}
          <div className="flex flex-col justify-between min-h-[220px]">
            {/* Submitted File content */}
            <div className="space-y-2">
              <p className="text-[10px] font-black text-[var(--theme-text)]/40 uppercase tracking-widest">
                Submitted File
              </p>
              
              {selectedFile ? (
                <div className="flex items-center justify-between p-3 bg-[var(--theme-hover)]/30 border border-[var(--theme-border)] rounded-[8px] transition-all">
                  <div className="flex items-center gap-2 min-w-0">
                    <FileText size={20} className="text-[var(--theme-primary)] shrink-0" />
                    <div className="min-w-0">
                      <p className="font-bold text-xs text-[var(--theme-text)] truncate">{selectedFile.name}</p>
                      <p className="text-[9px] text-[var(--theme-text)]/40 font-black uppercase">
                        {typeof selectedFile.size === 'number' 
                          ? `${(selectedFile.size / (1024 * 1024)).toFixed(1)} MB` 
                          : selectedFile.size || 'unknown size'}
                      </p>
                    </div>
                  </div>
                  {!isGraded && (
                    <button 
                      onClick={removeFile}
                      className="p-1.5 text-red-500 hover:bg-red-500/10 rounded-full transition-colors shrink-0"
                      title="Remove file"
                    >
                      <X size={15} />
                    </button>
                  )}
                </div>
              ) : (
                <p className="text-sm font-medium text-[var(--theme-text)]/40 italic">
                  No files submitted yet.
                </p>
              )}
            </div>

            {/* Edit Submission Button */}
            <button 
              onClick={!isGraded ? triggerFileInput : undefined}
              disabled={isGraded}
              className={`w-full py-2.5 font-bold text-sm rounded-[8px] transition-all mt-4 ${
                !isGraded
                  ? 'bg-[var(--theme-hover)] hover:opacity-90 text-[var(--theme-text)] active:scale-[0.99]'
                  : 'bg-[var(--theme-hover)]/30 text-[var(--theme-text)]/30 cursor-not-allowed'
              }`}
            >
              Edit Submission
            </button>
          </div>

        </div>

      </div>

      {/* Graded Details / Comments Section */}
      {isGraded && (
        <div className="bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-[8px] overflow-hidden shadow-none flex flex-col md:flex-row items-stretch">
          <div className="p-6 md:w-32 bg-[var(--theme-hover)]/10 flex flex-col items-center justify-center shrink-0 border-b md:border-b-0 md:border-r border-[var(--theme-border)]">
            <p className="font-black text-[10px] uppercase tracking-widest text-[var(--theme-text)]/40 mb-1">GRADE</p>
            <p className="text-5xl font-black text-[var(--theme-primary)]">{getRawGrade(assignment.grade)}</p>
          </div>
          <div className="p-6 flex-1">
            <p className="font-black text-xs uppercase tracking-widest text-[var(--theme-text)]/45 mb-2">Teacher comment</p>
            <p className="text-sm font-medium leading-relaxed text-[var(--theme-text)]/75">
              Lorem ipsum dolor sit amet consectetur. Porta id viverra nec proin volutpat pulvinar. Nunc sociis mauris suspendisse scelerisque et tellus varius. Massa proin ultrices tortor nulla arcu parturient scelerisque adipiscing congue. Feugiat sed vestibulum tortor proin cursus lacus ipsum cursus scelerisque.
            </p>
          </div>
        </div>
      )}
    </motion.div>
  );
};

export default StudentSubmitAssignment;
