import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const AdminDashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-100 flex flex-col">
      <nav className="bg-white shadow-none px-8 py-4 flex justify-between items-center border-b border-gray-200">
        <h1 className="text-2xl font-bold text-[#aa3bff]">SIMS Admin</h1>
        <div className="flex items-center gap-4">
          <span className="font-normal text-gray-700">Xin chào, {user?.name}</span>
          <button onClick={handleLogout} className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-[8px] font-medium transition-colors shadow-none">
            Đăng xuất
          </button>
        </div>
      </nav>
      
      <main className="flex-1 p-8">
        <div className="bg-white rounded-[8px] shadow-none border border-gray-200 p-8 h-full">
          <h2 className="text-3xl font-bold text-gray-800 mb-4">Khu vực Quản trị Hệ thống</h2>
          <p className="text-gray-500 text-lg">Chào mừng {user?.name} trở lại. Tại đây bạn có thể quản lý Sinh viên, Khóa học và Giảng viên.</p>
        </div>
      </main>
    </div>
  );
};

export default AdminDashboard;