import React, { useState, useEffect, useRef } from 'react';
import { NavLink, useNavigate, useLocation } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  GraduationCap,
  Bell,
  ChevronDown,
  User,
  Settings,
  LogOut,
  Menu,
  X,
  LayoutDashboard,
  Users,
  Building2,
  BookOpen,
  School,
  Check,
  FileText
} from 'lucide-react';
import { useAuth } from '../../context/AuthContext';
import { useTheme, themes } from '../../context/ThemeContext';

const Navbar = () => {
  const { user, logout } = useAuth();
  const { currentTheme, setCurrentTheme } = useTheme();
  const navigate = useNavigate();
  const location = useLocation();

  const [isScrolled, setIsScrolled] = useState(false);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [isNotificationsOpen, setIsNotificationsOpen] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const [isMoreMenuOpen, setIsMoreMenuOpen] = useState(false);
  const [showThemePicker, setShowThemePicker] = useState(false);

  const profileRef = useRef(null);
  const notificationsRef = useRef(null);
  const moreMenuRef = useRef(null);

  // Detect scroll to show subtle shadow
  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > 10) {
        setIsScrolled(true);
      } else {
        setIsScrolled(false);
      }
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  // Close popups on click outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (profileRef.current && !profileRef.current.contains(event.target)) {
        setIsProfileOpen(false);
        setShowThemePicker(false);
      }
      if (notificationsRef.current && !notificationsRef.current.contains(event.target)) {
        setIsNotificationsOpen(false);
      }
      if (moreMenuRef.current && !moreMenuRef.current.contains(event.target)) {
        setIsMoreMenuOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Close mobile menu on route change
  useEffect(() => {
    setIsMobileMenuOpen(false);
    setIsMoreMenuOpen(false);
    setIsProfileOpen(false);
  }, [location.pathname]);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  // Define role-specific navigation menus
  const adminMenus = [
    { name: 'Dashboard', path: '/admin/dashboard', icon: LayoutDashboard },
    { name: 'Students', path: '/admin/students', icon: GraduationCap },
    { name: 'Instructors', path: '/admin/instructors', icon: Users },
    { name: 'Subjects', path: '/admin/subjects', icon: BookOpen },
    { name: 'Classes', path: '/admin/classes', icon: School },
    { name: 'Courses', path: '/admin/courses', icon: BookOpen },
    { name: 'Reports', path: '/admin/reports', icon: FileText },
  ];

  const studentMenus = [
    { name: 'Dashboard', path: '/student/dashboard', icon: LayoutDashboard },
    { name: 'My Class', path: '/student/class', icon: School },
    { name: 'Course Material', path: '/student/materials', icon: BookOpen },
    { name: 'Assignments', path: '/student/assignments', icon: GraduationCap },
  ];

  const lecturerMenus = [
    { name: 'Dashboard', path: '/lecturer/dashboard', icon: LayoutDashboard },
    { name: 'My Class', path: '/lecturer/class', icon: School },
  ];

  const navItems = user?.role === 'STUDENT'
    ? studentMenus
    : user?.role === 'LECTURER'
      ? lecturerMenus
      : adminMenus;

  // Split menus for tablet view (show first 5, collapse rest in "More")
  const primaryNavItems = navItems.slice(0, 5);
  const secondaryNavItems = navItems.slice(5);

  const notificationsList = [
    { id: 1, title: 'New Assignment Posted', time: '10m ago', unread: true },
    { id: 2, title: 'Grade updated for CS101', time: '1h ago', unread: true },
    { id: 3, title: 'System Maintenance Scheduled', time: '1d ago', unread: false },
  ];

  return (
    <header
      className={`sticky top-0 z-40 w-full bg-[var(--theme-sidebarBg)] border-b border-[var(--theme-border)] transition-all duration-300 ${isScrolled ? 'shadow-sm shadow-black/5 backdrop-blur-md bg-[var(--theme-sidebarBg)]/95' : ''
        }`}
    >
      {/* TOP TIER HEADER ROW: Logo on Left, Notifications & User Icon on Right ONLY */}
      <div className="h-14 px-4 md:px-8 flex items-center justify-between border-b border-[var(--theme-border)]/60">

        {/* TOP LEFT: SIMS Logo & System Name */}
        <div className="flex items-center gap-3">
          {/* Mobile Hamburger Toggle */}
          <button
            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            className="lg:hidden p-2 text-[var(--theme-text)]/70 hover:text-[var(--theme-primary)] hover:bg-[var(--theme-hover)] rounded-lg transition-colors cursor-pointer"
            aria-label="Toggle menu"
          >
            {isMobileMenuOpen ? <X size={22} /> : <Menu size={22} />}
          </button>

          {/* Logo Brand Mark */}
          <NavLink
            to={user?.role === 'STUDENT' ? '/student/dashboard' : user?.role === 'LECTURER' ? '/lecturer/dashboard' : '/admin/dashboard'}
            className="flex items-center gap-3 no-underline hover:no-underline group"
          >
            <div className="w-9 h-9 bg-[var(--theme-primary)] text-white rounded-xl flex items-center justify-center shadow-sm group-hover:scale-105 transition-transform duration-200 flex-shrink-0">
              <GraduationCap size={20} strokeWidth={2.5} />
            </div>
            <div className="flex flex-col">
              <div className="flex items-center gap-1.5 leading-none">
                <span className="text-lg font-black tracking-tight text-[var(--theme-text)]">
                  SIMS
                </span>
                <span className="px-1.5 py-0.5 text-[10px] font-extrabold uppercase bg-[var(--theme-hover)] text-[var(--theme-primary)] rounded-md tracking-wider">
                  {user?.role === 'STUDENT' ? 'Student' : user?.role === 'LECTURER' ? 'Lecturer' : 'Admin'}
                </span>
              </div>
              <span className="text-[10px] text-[var(--theme-textMuted)] font-medium tracking-tight hidden sm:inline-block mt-0.5">
                Student Information Management System
              </span>
            </div>
          </NavLink>
        </div>

        {/* TOP RIGHT: Notification Button & Profile Dropdown ONLY */}
        <div className="flex items-center gap-2 sm:gap-3">

          {/* NOTIFICATION BUTTON */}
          <div className="relative" ref={notificationsRef}>
            <button
              onClick={() => setIsNotificationsOpen(!isNotificationsOpen)}
              className="w-9 h-9 rounded-full flex items-center justify-center text-[var(--theme-text)]/70 hover:text-[var(--theme-primary)] hover:bg-[var(--theme-hover)] transition-all duration-200 relative cursor-pointer group"
              aria-label="Notifications"
            >
              <Bell size={19} className="transition-transform duration-200 group-hover:rotate-12" />
              {/* Unread badge indicator */}
              <span className="absolute top-1.5 right-1.5 w-2.5 h-2.5 bg-emerald-500 rounded-full ring-2 ring-[var(--theme-sidebarBg)] animate-pulse" />
            </button>

            {/* Notifications Dropdown Panel */}
            <AnimatePresence>
              {isNotificationsOpen && (
                <motion.div
                  initial={{ opacity: 0, scale: 0.95, y: 8 }}
                  animate={{ opacity: 1, scale: 1, y: 0 }}
                  exit={{ opacity: 0, scale: 0.95, y: 8 }}
                  transition={{ duration: 0.2, ease: 'easeOut' }}
                  className="absolute right-0 mt-2 w-80 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-xl shadow-black/5 p-4 z-50"
                >
                  <div className="flex items-center justify-between pb-3 border-b border-[var(--theme-border)]">
                    <h4 className="font-bold text-sm text-[var(--theme-text)]">Notifications</h4>
                    <span className="text-[11px] font-semibold px-2 py-0.5 bg-[var(--theme-hover)] text-[var(--theme-primary)] rounded-full">
                      2 Unread
                    </span>
                  </div>
                  <div className="py-2 space-y-1 max-h-64 overflow-y-auto">
                    {notificationsList.map((notif) => (
                      <div
                        key={notif.id}
                        className={`p-2.5 rounded-xl transition-colors text-xs flex items-start gap-2.5 cursor-pointer ${notif.unread ? 'bg-[var(--theme-hover)]/40 hover:bg-[var(--theme-hover)]' : 'hover:bg-[var(--theme-hover)]/30'
                          }`}
                      >
                        <div className={`w-2 h-2 rounded-full mt-1.5 flex-shrink-0 ${notif.unread ? 'bg-[var(--theme-primary)]' : 'bg-gray-300'}`} />
                        <div className="flex-1">
                          <p className={`text-[var(--theme-text)] ${notif.unread ? 'font-bold' : 'font-medium'}`}>{notif.title}</p>
                          <span className="text-[10px] text-[var(--theme-textMuted)]">{notif.time}</span>
                        </div>
                      </div>
                    ))}
                  </div>
                </motion.div>
              )}
            </AnimatePresence>
          </div>

          {/* USER PROFILE DROPDOWN */}
          <div className="relative" ref={profileRef}>
            <button
              onClick={() => setIsProfileOpen(!isProfileOpen)}
              className="flex items-center gap-2 p-1 rounded-full hover:bg-[var(--theme-hover)]/60 transition-all duration-200 cursor-pointer group"
            >
              {/* User Avatar */}
              <div className="w-8 h-8 rounded-full bg-gradient-to-tr from-[var(--theme-primary)] to-[var(--theme-primaryDark)] p-[2px] shadow-sm flex-shrink-0">
                <div className="w-full h-full bg-[var(--theme-sidebarBg)] rounded-full flex items-center justify-center text-[var(--theme-primary)] font-black text-xs overflow-hidden">
                  {user?.image ? (
                    <img src={user.image} alt={user?.name || 'User'} className="w-full h-full object-cover" />
                  ) : (
                    user?.name?.[0]?.toUpperCase() || 'A'
                  )}
                </div>
              </div>

              {/* Username & Dropdown Arrow */}
              <span className="hidden sm:block text-sm font-bold text-[var(--theme-text)] group-hover:text-[var(--theme-primary)] transition-colors max-w-[120px] truncate">
                {user?.name || 'Admin User'}
              </span>
              <ChevronDown
                size={15}
                className={`text-[var(--theme-text)]/50 group-hover:text-[var(--theme-primary)] transition-transform duration-200 ${isProfileOpen ? 'rotate-180' : ''
                  }`}
              />
            </button>

            {/* PROFILE DROPDOWN CONTENT (Fade + Scale) */}
            <AnimatePresence>
              {isProfileOpen && (
                <motion.div
                  initial={{ opacity: 0, scale: 0.95, y: 8 }}
                  animate={{ opacity: 1, scale: 1, y: 0 }}
                  exit={{ opacity: 0, scale: 0.95, y: 8 }}
                  transition={{ duration: 0.2, ease: 'easeOut' }}
                  className="absolute right-0 mt-2 w-64 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-2xl shadow-xl shadow-black/5 p-2 z-50 overflow-hidden"
                >
                  {/* Top Area: Avatar, Username, Email */}
                  <div className="p-3 flex items-center gap-3 bg-[var(--theme-hover)]/30 rounded-xl mb-1">
                    <div className="w-11 h-11 rounded-full bg-[var(--theme-primary)] text-white font-bold flex items-center justify-center text-lg flex-shrink-0 shadow-sm">
                      {user?.image ? (
                        <img src={user.image} alt="User" className="w-full h-full rounded-full object-cover" />
                      ) : (
                        user?.name?.[0]?.toUpperCase() || 'A'
                      )}
                    </div>
                    <div className="flex-1 min-w-0">
                      <h4 className="text-sm font-bold text-[var(--theme-text)] truncate leading-tight">
                        {user?.name || 'Admin User'}
                      </h4>
                      <p className="text-xs text-[var(--theme-textMuted)] truncate mt-0.5">
                        {user?.email || (user?.role === 'STUDENT' ? 'student@sims.edu.vn' : 'admin@sims.edu.vn')}
                      </p>
                    </div>
                  </div>

                  <div className="h-[1px] bg-[var(--theme-border)] my-1.5" />

                  {/* Navigation Items inside Profile Dropdown */}
                  <div className="space-y-0.5">
                    {user?.role !== 'ADMIN' && (
                      <button
                        onClick={() => {
                          setIsProfileOpen(false);
                          const profilePath = user?.role === 'STUDENT' ? '/student/profile' : '/lecturer/profile';
                          navigate(profilePath);
                        }}
                        className="w-full flex items-center gap-2.5 px-3 py-2 text-sm font-medium text-[var(--theme-text)]/80 hover:text-[var(--theme-primary)] hover:bg-[var(--theme-hover)]/60 rounded-xl transition-colors cursor-pointer"
                      >
                        <User size={18} className="text-[var(--theme-primary)]" />
                        <span>Profile</span>
                      </button>
                    )}

                    <button
                      onClick={() => setShowThemePicker(!showThemePicker)}
                      className="w-full flex items-center justify-between px-3 py-2 text-sm font-medium text-[var(--theme-text)]/80 hover:text-[var(--theme-primary)] hover:bg-[var(--theme-hover)]/60 rounded-xl transition-colors cursor-pointer"
                    >
                      <div className="flex items-center gap-2.5">
                        <Settings size={18} className="text-[var(--theme-primary)]" />
                        <span>Settings & Theme</span>
                      </div>
                      <ChevronDown size={14} className={`transition-transform duration-200 ${showThemePicker ? 'rotate-180' : ''}`} />
                    </button>

                    {/* Inline Theme Switcher */}
                    <AnimatePresence>
                      {showThemePicker && (
                        <motion.div
                          initial={{ opacity: 0, height: 0 }}
                          animate={{ opacity: 1, height: 'auto' }}
                          exit={{ opacity: 0, height: 0 }}
                          className="pl-4 pr-1 py-1 space-y-1 overflow-hidden"
                        >
                          {Object.entries(themes).map(([key, theme]) => (
                            <button
                              key={key}
                              onClick={() => setCurrentTheme(key)}
                              className={`w-full flex items-center justify-between p-2 rounded-lg text-xs font-semibold transition-colors cursor-pointer ${currentTheme === key
                                  ? 'bg-[var(--theme-hover)] text-[var(--theme-primary)]'
                                  : 'text-[var(--theme-text)]/70 hover:bg-[var(--theme-hover)]/40'
                                }`}
                            >
                              <div className="flex items-center gap-2">
                                <span
                                  className="w-3.5 h-3.5 rounded-full border border-white"
                                  style={{ backgroundColor: theme.colors.primary }}
                                />
                                <span>{theme.name}</span>
                              </div>
                              {currentTheme === key && <Check size={14} className="text-[var(--theme-primary)]" />}
                            </button>
                          ))}
                        </motion.div>
                      )}
                    </AnimatePresence>
                  </div>

                  <div className="h-[1px] bg-[var(--theme-border)] my-1.5" />

                  {/* Logout Button with subtle red text */}
                  <button
                    onClick={handleLogout}
                    className="w-full flex items-center gap-2.5 px-3 py-2 text-sm font-semibold text-red-500 hover:bg-red-50 dark:hover:bg-red-950/30 rounded-xl transition-colors cursor-pointer"
                  >
                    <LogOut size={18} />
                    <span>Logout</span>
                  </button>
                </motion.div>
              )}
            </AnimatePresence>
          </div>
        </div>
      </div>

      {/* SECOND TIER ROW: Primary Navigation Bar Below the Logo */}
      <div className="h-11 px-4 md:px-8 hidden lg:flex items-center gap-1 xl:gap-2 overflow-x-auto custom-scrollbar">
        {primaryNavItems.map((item) => {
          const isActive = location.pathname.startsWith(item.path);
          const ItemIcon = item.icon;

          return (
            <NavLink
              key={item.path}
              to={item.path}
              className="relative px-3 py-1.5 rounded-lg text-sm font-semibold transition-all duration-200 flex items-center gap-2 group no-underline hover:no-underline hover:-translate-y-0.5 flex-shrink-0"
            >
              <ItemIcon
                size={17}
                className={`transition-colors duration-200 ${isActive ? 'text-[var(--theme-primary)]' : 'text-[var(--theme-text)]/50 group-hover:text-[var(--theme-primary)]'
                  }`}
              />
              <span className={isActive ? 'text-[var(--theme-primary)] font-bold' : 'text-[var(--theme-text)]/75 group-hover:text-[var(--theme-text)]'}>
                {item.name}
              </span>

              {/* Hover Background */}
              <div className="absolute inset-0 rounded-lg bg-[var(--theme-hover)]/60 opacity-0 group-hover:opacity-100 transition-opacity duration-200 -z-10" />

              {/* Animated Underline Indicator */}
              {isActive && (
                <motion.div
                  layoutId="activeNavbarUnderline"
                  className="absolute bottom-0 left-1.5 right-1.5 h-[2.5px] bg-[var(--theme-primary)] rounded-full"
                  transition={{ type: 'spring', stiffness: 380, damping: 30 }}
                />
              )}
            </NavLink>
          );
        })}

        {/* Secondary Navigation Items for XL view */}
        <div className="hidden xl:flex items-center gap-1 xl:gap-2">
          {secondaryNavItems.map((item) => {
            const isActive = location.pathname.startsWith(item.path);
            const ItemIcon = item.icon;

            return (
              <NavLink
                key={item.path}
                to={item.path}
                className="relative px-3 py-1.5 rounded-lg text-sm font-semibold transition-all duration-200 flex items-center gap-2 group no-underline hover:no-underline hover:-translate-y-0.5 flex-shrink-0"
              >
                <ItemIcon
                  size={17}
                  className={`transition-colors duration-200 ${isActive ? 'text-[var(--theme-primary)]' : 'text-[var(--theme-text)]/50 group-hover:text-[var(--theme-primary)]'
                    }`}
                />
                <span className={isActive ? 'text-[var(--theme-primary)] font-bold' : 'text-[var(--theme-text)]/75 group-hover:text-[var(--theme-text)]'}>
                  {item.name}
                </span>

                <div className="absolute inset-0 rounded-lg bg-[var(--theme-hover)]/60 opacity-0 group-hover:opacity-100 transition-opacity duration-200 -z-10" />

                {isActive && (
                  <motion.div
                    layoutId="activeNavbarUnderline"
                    className="absolute bottom-0 left-1.5 right-1.5 h-[2.5px] bg-[var(--theme-primary)] rounded-full"
                    transition={{ type: 'spring', stiffness: 380, damping: 30 }}
                  />
                )}
              </NavLink>
            );
          })}
        </div>

        {/* Tablet "More" Dropdown Menu when items overflow on LG screens */}
        {secondaryNavItems.length > 0 && (
          <div className="relative xl:hidden" ref={moreMenuRef}>
            <button
              onClick={() => setIsMoreMenuOpen(!isMoreMenuOpen)}
              className={`px-3 py-1.5 rounded-lg text-sm font-semibold flex items-center gap-1.5 transition-all duration-200 cursor-pointer ${secondaryNavItems.some(item => location.pathname.startsWith(item.path))
                  ? 'text-[var(--theme-primary)] font-bold bg-[var(--theme-hover)]/50'
                  : 'text-[var(--theme-text)]/75 hover:text-[var(--theme-text)] hover:bg-[var(--theme-hover)]/60'
                }`}
            >
              <span>More</span>
              <ChevronDown size={14} className={`transition-transform duration-200 ${isMoreMenuOpen ? 'rotate-180' : ''}`} />
            </button>

            <AnimatePresence>
              {isMoreMenuOpen && (
                <motion.div
                  initial={{ opacity: 0, scale: 0.95, y: 8 }}
                  animate={{ opacity: 1, scale: 1, y: 0 }}
                  exit={{ opacity: 0, scale: 0.95, y: 8 }}
                  transition={{ duration: 0.2, ease: 'easeOut' }}
                  className="absolute left-0 mt-2 w-52 bg-[var(--theme-sidebarBg)] border border-[var(--theme-border)] rounded-xl shadow-xl shadow-black/5 p-1.5 z-50"
                >
                  {secondaryNavItems.map((item) => {
                    const isActive = location.pathname.startsWith(item.path);
                    const ItemIcon = item.icon;

                    return (
                      <NavLink
                        key={item.path}
                        to={item.path}
                        onClick={() => setIsMoreMenuOpen(false)}
                        className={`flex items-center gap-2.5 px-3 py-2 rounded-lg text-sm font-medium transition-colors no-underline ${isActive
                            ? 'bg-[var(--theme-hover)] text-[var(--theme-primary)] font-bold'
                            : 'text-[var(--theme-text)]/80 hover:bg-[var(--theme-hover)]/50 hover:text-[var(--theme-text)]'
                          }`}
                      >
                        <ItemIcon size={16} className={isActive ? 'text-[var(--theme-primary)]' : 'text-[var(--theme-text)]/50'} />
                        <span>{item.name}</span>
                      </NavLink>
                    );
                  })}
                </motion.div>
              )}
            </AnimatePresence>
          </div>
        )}
      </div>

      {/* MOBILE RESPONSIVE DRAWER OVERLAY */}
      <AnimatePresence>
        {isMobileMenuOpen && (
          <motion.div
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: 'auto' }}
            exit={{ opacity: 0, height: 0 }}
            transition={{ duration: 0.25, ease: 'easeInOut' }}
            className="absolute top-[56px] left-0 w-full bg-[var(--theme-sidebarBg)] border-b border-[var(--theme-border)] shadow-xl lg:hidden overflow-hidden z-30"
          >
            <div className="p-4 space-y-1.5 max-h-[calc(100vh-80px)] overflow-y-auto">
              <div className="px-3 py-1.5 text-[10px] font-bold text-[var(--theme-textMuted)] uppercase tracking-wider">
                Navigation
              </div>
              {navItems.map((item) => {
                const isActive = location.pathname.startsWith(item.path);
                const ItemIcon = item.icon;

                return (
                  <NavLink
                    key={item.path}
                    to={item.path}
                    onClick={() => setIsMobileMenuOpen(false)}
                    className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-semibold transition-colors no-underline ${isActive
                        ? 'bg-[var(--theme-hover)] text-[var(--theme-primary)] font-bold'
                        : 'text-[var(--theme-text)]/75 hover:bg-[var(--theme-hover)]/50 hover:text-[var(--theme-text)]'
                      }`}
                  >
                    <ItemIcon size={20} className={isActive ? 'text-[var(--theme-primary)]' : 'text-[var(--theme-text)]/50'} />
                    <span>{item.name}</span>
                  </NavLink>
                );
              })}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
};

export default Navbar;