import React from 'react';
import { Outlet } from 'react-router-dom';
import Navbar from './Navbar';

const LecturerLayout = () => {
  return (
    <div className="min-h-screen flex flex-col bg-[var(--theme-bg)] text-[var(--theme-text)] transition-colors duration-300">
      <Navbar /> 
      <main className="flex-1 p-4 sm:p-6 md:p-8 max-w-[1600px] w-full mx-auto">
        <Outlet />
      </main>
    </div>
  );
};

export default LecturerLayout;
