import React, { useState } from 'react';
import { UploadCloud, File, X, CheckCircle, AlertCircle } from 'lucide-react';
import SectionCard from './SectionCard';
import { motion, AnimatePresence } from 'framer-motion';

const CSVUploader = ({ title = "Import CSV", description = "Upload data using CSV files.", onImport }) => {
  const [dragActive, setDragActive] = useState(false);
  const [selectedFiles, setSelectedFiles] = useState([]);
  const [uploadStatus, setUploadStatus] = useState('idle'); // idle, uploading, success, error
  const [uploadProgress, setUploadProgress] = useState(0);
  const [successMessage, setSuccessMessage] = useState('');

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
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      handleFilesSelect(Array.from(e.dataTransfer.files));
    }
  };

  const handleChange = (e) => {
    e.preventDefault();
    if (e.target.files && e.target.files.length > 0) {
      handleFilesSelect(Array.from(e.target.files));
    }
  };

  const handleFilesSelect = (files) => {
    const validFiles = files.filter(file => file.type === "text/csv" || file.name.endsWith('.csv'));
    
    if (validFiles.length === 0) {
      setUploadStatus('error');
      return;
    }

    // Append new valid files
    setSelectedFiles(prev => {
      const newFiles = [...prev];
      validFiles.forEach(vf => {
        if (!newFiles.find(f => f.name === vf.name)) {
          newFiles.push(vf);
        }
      });
      return newFiles;
    });
    setUploadStatus('idle');
    setUploadProgress(0);
  };

  const removeFile = (indexToRemove) => {
    setSelectedFiles(prev => prev.filter((_, idx) => idx !== indexToRemove));
  };

  const handleImport = async () => {
    if (selectedFiles.length === 0) return;
    
    setUploadStatus('uploading');
    
    try {
      if (onImport) {
        // Upload sequentially and calculate progress
        let processed = 0;
        let messages = [];
        for (const file of selectedFiles) {
          const resMessage = await onImport(file);
          if (resMessage) messages.push(resMessage);
          processed++;
          setUploadProgress(Math.round((processed / selectedFiles.length) * 100));
        }
        if (messages.length > 0) setSuccessMessage(messages.join(' '));
      } else {
        // Fallback simulation if no onImport provided
        let progress = 0;
        const interval = setInterval(() => {
          progress += 20;
          setUploadProgress(progress);
          if (progress >= 100) clearInterval(interval);
        }, 300);
        await new Promise(resolve => setTimeout(resolve, 1500));
      }
      setUploadStatus('success');
    } catch (e) {
      console.error("Import failed", e);
      setUploadStatus('error');
    }
  };

  const handleCancel = () => {
    setSelectedFiles([]);
    setUploadStatus('idle');
    setUploadProgress(0);
    setSuccessMessage('');
  };

  return (
    <SectionCard className="p-6">
      <div className="mb-6">
        <h3 className="text-lg font-bold text-[var(--theme-text)]">{title}</h3>
        <p className="text-sm text-[var(--theme-textMuted)]">{description}</p>
      </div>

      <div 
        className={`relative border-2 border-dashed rounded-xl p-8 text-center transition-colors duration-200 ${
          dragActive 
            ? 'border-[var(--theme-primary)] bg-[var(--theme-hover)]/50' 
            : 'border-[var(--theme-border)] bg-[var(--theme-sidebarBg)] hover:bg-[var(--theme-hover)]/30'
        }`}
        onDragEnter={handleDrag}
        onDragLeave={handleDrag}
        onDragOver={handleDrag}
        onDrop={handleDrop}
      >
        <input
          type="file"
          accept=".csv"
          multiple
          onChange={handleChange}
          className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
          disabled={uploadStatus === 'uploading' || uploadStatus === 'success'}
        />
        
        <div className="flex flex-col items-center gap-3">
          <div className={`w-12 h-12 rounded-full flex items-center justify-center ${
            dragActive ? 'bg-[var(--theme-primary)] text-white' : 'bg-[var(--theme-hover)] text-[var(--theme-primary)]'
          }`}>
            <UploadCloud size={24} />
          </div>
          <div>
            <p className="text-sm font-semibold text-[var(--theme-text)] mb-1">
              <span className="text-[var(--theme-primary)]">Select CSV files</span> or Drag & Drop here
            </p>
            <p className="text-xs text-[var(--theme-textMuted)]">Supported format: .csv</p>
          </div>
        </div>
      </div>

      <AnimatePresence mode="wait">
        {selectedFiles.length > 0 && uploadStatus !== 'success' && (
          <motion.div
            initial={{ opacity: 0, height: 0, marginTop: 0 }}
            animate={{ opacity: 1, height: 'auto', marginTop: 16 }}
            exit={{ opacity: 0, height: 0, marginTop: 0 }}
            className="bg-[var(--theme-hover)]/50 rounded-xl p-4 overflow-hidden"
          >
            <div className="space-y-2 mb-4 max-h-40 overflow-y-auto custom-scrollbar pr-2">
              {selectedFiles.map((file, idx) => (
                <div key={idx} className="flex items-center justify-between bg-[var(--theme-bg)] border border-[var(--theme-border)] rounded-lg p-2.5">
                  <div className="flex items-center gap-3 overflow-hidden">
                    <div className="w-8 h-8 rounded-md bg-[var(--theme-primary)]/10 flex items-center justify-center text-[var(--theme-primary)] flex-shrink-0">
                      <File size={14} />
                    </div>
                    <div className="min-w-0">
                      <p className="text-sm font-semibold text-[var(--theme-text)] truncate">
                        {file.name}
                      </p>
                      <p className="text-xs text-[var(--theme-textMuted)]">
                        {(file.size / 1024).toFixed(1)} KB
                      </p>
                    </div>
                  </div>
                  {uploadStatus !== 'uploading' && (
                    <button 
                      onClick={() => removeFile(idx)}
                      className="text-[var(--theme-textMuted)] hover:text-red-500 hover:bg-red-500/10 rounded-md p-1.5 transition-colors flex-shrink-0 cursor-pointer"
                    >
                      <X size={14} />
                    </button>
                  )}
                </div>
              ))}
            </div>

            {uploadStatus === 'uploading' && (
              <div className="mb-4">
                <div className="flex justify-between text-xs font-semibold mb-1 text-[var(--theme-text)]">
                  <span>Uploading {selectedFiles.length} file{selectedFiles.length !== 1 ? 's' : ''}...</span>
                  <span>{uploadProgress}%</span>
                </div>
                <div className="w-full bg-[var(--theme-border)] rounded-full h-1.5 overflow-hidden">
                  <motion.div 
                    className="bg-[var(--theme-primary)] h-full rounded-full"
                    initial={{ width: 0 }}
                    animate={{ width: `${uploadProgress}%` }}
                  />
                </div>
              </div>
            )}

            <div className="flex gap-2 border-t border-[var(--theme-border)] pt-4">
              <button 
                onClick={handleCancel}
                disabled={uploadStatus === 'uploading'}
                className="flex-1 px-4 py-2 text-sm font-semibold text-[var(--theme-text)] bg-transparent border border-[var(--theme-border)] rounded-xl hover:bg-[var(--theme-hover)] transition-colors disabled:opacity-50 cursor-pointer"
              >
                Clear All
              </button>
              <button 
                onClick={handleImport}
                disabled={uploadStatus === 'uploading'}
                className="flex-1 px-4 py-2 text-sm font-semibold text-white bg-[var(--theme-primary)] hover:bg-[var(--theme-primaryDark)] rounded-xl transition-colors shadow-sm disabled:opacity-50 flex items-center justify-center gap-2 cursor-pointer"
              >
                {uploadStatus === 'uploading' ? (
                  <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                ) : `Import ${selectedFiles.length} File${selectedFiles.length !== 1 ? 's' : ''}`}
              </button>
            </div>
          </motion.div>
        )}

        {uploadStatus === 'success' && (
          <motion.div
            initial={{ opacity: 0, height: 0, marginTop: 0 }}
            animate={{ opacity: 1, height: 'auto', marginTop: 16 }}
            className="bg-emerald-50 dark:bg-emerald-950/30 border border-emerald-200 dark:border-emerald-900/50 rounded-xl p-4 flex items-start gap-3 overflow-hidden"
          >
            <CheckCircle className="text-emerald-500 shrink-0 mt-0.5" size={20} />
            <div className="flex-1">
              <h4 className="text-sm font-bold text-emerald-800 dark:text-emerald-400">Import Successful</h4>
              <p className="text-xs text-emerald-600 dark:text-emerald-500/80 mt-1">
                {successMessage || `Data from ${selectedFiles.length} file${selectedFiles.length !== 1 ? 's' : ''} has been processed.`}
              </p>
              <button 
                onClick={handleCancel}
                className="mt-3 text-xs font-bold text-emerald-700 hover:text-emerald-800 dark:text-emerald-400 dark:hover:text-emerald-300 cursor-pointer"
              >
                Upload more files
              </button>
            </div>
          </motion.div>
        )}

        {uploadStatus === 'error' && (
          <motion.div
            initial={{ opacity: 0, height: 0, marginTop: 0 }}
            animate={{ opacity: 1, height: 'auto', marginTop: 16 }}
            className="bg-red-50 dark:bg-red-950/30 border border-red-200 dark:border-red-900/50 rounded-xl p-4 flex items-start gap-3 overflow-hidden"
          >
            <AlertCircle className="text-red-500 shrink-0 mt-0.5" size={20} />
            <div>
              <h4 className="text-sm font-bold text-red-800 dark:text-red-400">Invalid File Type</h4>
              <p className="text-xs text-red-600 dark:text-red-500/80 mt-1">Please ensure all selected files are valid .csv files.</p>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </SectionCard>
  );
};

export default CSVUploader;
