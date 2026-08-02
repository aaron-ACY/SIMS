import React from 'react';
import { motion } from 'framer-motion';
import { ArrowRight, Sparkles, ShieldCheck, Users, BookOpen, BarChart3, CheckCircle2 } from 'lucide-react';
import { themes } from '../../context/ThemeContext';

const HeroSection = () => {
  const colors = themes.jungle.colors;

  return (
    <section className="relative pt-32 pb-20 md:pt-40 md:pb-28 overflow-hidden bg-gradient-to-b from-emerald-50/40 via-white to-white">
      {/* Background Soft Glow */}
      <div
        className="absolute top-1/4 left-1/2 -translate-x-1/2 w-[600px] h-[350px] rounded-full blur-[120px] pointer-events-none opacity-30"
        style={{ backgroundColor: colors.hover }}
      />

      <div className="max-w-7xl mx-auto px-6 sm:px-8 relative z-10">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-8 items-center">
          {/* Left Column - Copy & CTA */}
          <motion.div
            initial={{ opacity: 0, y: 30 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.6, ease: 'easeOut' }}
            className="lg:col-span-7 space-y-6 text-center lg:text-left"
          >
            {/* Main Heading */}
            <h1 className="text-4xl sm:text-5xl lg:text-6xl font-extrabold text-gray-900 tracking-tight leading-[1.15]">
              Student Information <br />
              <span className="relative inline-block" style={{ color: colors.primary }}>
                Management System
              </span>
            </h1>

            {/* Subtitle */}
            <p className="text-lg sm:text-xl text-gray-600 max-w-2xl mx-auto lg:mx-0 leading-relaxed font-normal">
              Modern Education Starts with Smart Management. A unified, cloud-first platform standardizing student records, academic progress, and institutional workflows seamlessly.
            </p>

            {/* CTA Buttons */}
            <div className="flex flex-col sm:flex-row items-center justify-center lg:justify-start gap-4 pt-2">
              <a
                href="#features"
                className="w-full sm:w-auto px-8 py-4 text-sm font-semibold text-white rounded-xl shadow-md hover:shadow-lg transition-all duration-200 flex items-center justify-center gap-2 group cursor-pointer"
                style={{ backgroundColor: colors.primary }}
                onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = colors.primaryDark)}
                onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = colors.primary)}
              >
                <span>Explore Now</span>
                <ArrowRight className="w-4 h-4 transition-transform group-hover:translate-x-1" />
              </a>

              <a
                href="#contact"
                className="w-full sm:w-auto px-8 py-4 text-sm font-semibold text-gray-700 bg-white border border-gray-200 rounded-xl hover:bg-gray-50 hover:text-gray-900 transition-all duration-200 flex items-center justify-center cursor-pointer"
              >
                Contact
              </a>
            </div>

            {/* Trust Markers */}
            <div className="pt-6 flex items-center justify-center lg:justify-start gap-6 text-xs text-gray-500 font-medium">
              <span className="flex items-center gap-1.5">
                <CheckCircle2 className="w-4 h-4" style={{ color: colors.primary }} /> Verified Platform
              </span>
              <span className="flex items-center gap-1.5">
                <ShieldCheck className="w-4 h-4" style={{ color: colors.primary }} /> Enterprise Security
              </span>
            </div>
          </motion.div>

          {/* Right Column - Dashboard Placeholder Illustration */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 0.8, delay: 0.2 }}
            className="lg:col-span-5 relative flex justify-center"
          >
            <motion.div
              whileHover={{ y: -6 }}
              transition={{ duration: 0.3 }}
              className="w-full max-w-md bg-white rounded-2xl border border-gray-200/80 p-6 shadow-xl relative overflow-hidden"
              style={{ boxShadow: '0 20px 40px -15px rgba(47, 160, 132, 0.15)' }}
            >
              {/* Header Bar Mock */}
              <div className="flex items-center justify-between border-b border-gray-100 pb-4 mb-5">
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full bg-red-400" />
                  <div className="w-3 h-3 rounded-full bg-yellow-400" />
                  <div className="w-3 h-3 rounded-full bg-emerald-400" />
                </div>
                <div className="text-xs font-semibold text-gray-400 uppercase tracking-wider">SIMS Portal</div>
              </div>

              {/* Stat Cards Row Mock */}
              <div className="grid grid-cols-2 gap-3 mb-5">
                <div className="p-3.5 rounded-xl border border-emerald-100" style={{ backgroundColor: `${colors.hover}40` }}>
                  <div className="flex items-center justify-between text-xs text-emerald-800 font-semibold mb-1">
                    <span>Students</span>
                    <Users className="w-3.5 h-3.5" style={{ color: colors.primary }} />
                  </div>
                  <div className="text-xl font-bold text-gray-900">15,420</div>
                  <div className="text-[10px] text-emerald-600 font-medium">99.8% Active Enrolled</div>
                </div>

                <div className="p-3.5 rounded-xl bg-gray-50 border border-gray-100">
                  <div className="flex items-center justify-between text-xs text-gray-600 font-semibold mb-1">
                    <span>Courses</span>
                    <BookOpen className="w-3.5 h-3.5 text-gray-500" />
                  </div>
                  <div className="text-xl font-bold text-gray-900">1,280</div>
                  <div className="text-[10px] text-gray-500 font-medium">Spring Semester</div>
                </div>
              </div>

              {/* Progress Graph Mock */}
              <div className="p-4 rounded-xl border border-gray-100 bg-gray-50/60 mb-4">
                <div className="flex items-center justify-between text-xs font-semibold text-gray-700 mb-3">
                  <span>Academic Overview</span>
                  <BarChart3 className="w-4 h-4 text-gray-400" />
                </div>
                <div className="space-y-2">
                  <div className="flex items-center gap-2">
                    <span className="text-[11px] font-medium text-gray-500 w-16">Grade Avg</span>
                    <div className="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                      <div className="h-full rounded-full transition-all duration-1000" style={{ width: '88%', backgroundColor: colors.primary }} />
                    </div>
                    <span className="text-[11px] font-bold text-gray-700">3.85</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-[11px] font-medium text-gray-500 w-16">Attendance</span>
                    <div className="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                      <div className="h-full rounded-full transition-all duration-1000" style={{ width: '96%', backgroundColor: colors.primaryDark }} />
                    </div>
                    <span className="text-[11px] font-bold text-gray-700">96%</span>
                  </div>
                </div>
              </div>

              {/* Floating Badge overlay */}
              <div className="absolute -bottom-2 -right-2 px-3 py-1.5 bg-white border border-gray-200 rounded-xl shadow-lg flex items-center gap-2 text-xs font-semibold text-gray-800">
                <div className="w-2 h-2 rounded-full animate-ping" style={{ backgroundColor: colors.primary }} />
                <span>Live System Sync</span>
              </div>
            </motion.div>
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
