import React from 'react';
import { motion } from 'framer-motion';
import { Users, Clock, BookOpen, BarChart3, FileText, ShieldCheck } from 'lucide-react';
import { themes } from '../../context/ThemeContext';

const FeaturesSection = () => {
  const colors = themes.jungle.colors;

  const features = [
    {
      icon: Users,
      title: 'Student Management',
      desc: 'Centralized student profiles, demographic records, enrollment history, and academic standing tracking.',
    },
    {
      icon: Clock,
      title: 'Attendance Tracking',
      desc: 'Real-time digital attendance recording, automated absence logging, and session reporting for classes.',
    },
    {
      icon: BookOpen,
      title: 'Course Management',
      desc: 'Comprehensive course cataloging, curriculum mapping, syllabus distribution, and class scheduling.',
    },
    {
      icon: BarChart3,
      title: 'Grade Portal',
      desc: 'Instant grade entry, credit computation, GPA calculation, and transparent academic standing views.',
    },
    {
      icon: FileText,
      title: 'Academic Reports',
      desc: 'Standardized transcript generation, institutional analytics, performance breakdown, and audit exports.',
    },
    {
      icon: ShieldCheck,
      title: 'Secure Authentication',
      desc: 'Role-based authorization for Students, Instructors, and Administrators with strict data privacy controls.',
    },
  ];

  return (
    <section id="features" className="py-20 md:py-28 bg-gray-50/60 border-t border-gray-100">
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ duration: 0.6 }}
          className="text-center max-w-3xl mx-auto mb-16 space-y-4"
        >
          <div
            className="inline-block text-xs font-bold uppercase tracking-widest px-3 py-1 rounded-md"
            style={{ backgroundColor: colors.hover, color: colors.primaryDark }}
          >
            System Capabilities
          </div>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-gray-900 tracking-tight">
            Designed for Modern Academic Workflows
          </h2>
          <p className="text-base text-gray-600 font-normal">
            Streamlining institutional management with modern SaaS reliability and clean architecture.
          </p>
        </motion.div>

        {/* 3-Column Features Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {features.map((item, idx) => {
            const Icon = item.icon;
            return (
              <motion.div
                key={idx}
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ duration: 0.5, delay: idx * 0.08 }}
                whileHover={{ y: -5 }}
                className="bg-white p-8 rounded-2xl border border-gray-200/80 shadow-xs hover:shadow-xl transition-all duration-300 group relative flex flex-col justify-between"
              >
                <div>
                  <div
                    className="w-12 h-12 rounded-xl flex items-center justify-center mb-6 transition-colors duration-300"
                    style={{ backgroundColor: colors.hover, color: colors.primaryDark }}
                  >
                    <Icon className="w-6 h-6" style={{ color: colors.primary }} />
                  </div>
                  <h3 className="text-xl font-bold text-gray-900 mb-3 group-hover:text-emerald-800 transition-colors">
                    {item.title}
                  </h3>
                  <p className="text-sm text-gray-600 leading-relaxed font-normal">
                    {item.desc}
                  </p>
                </div>

                <div
                  className="mt-6 pt-4 border-t border-gray-100 text-xs font-semibold flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity duration-300"
                  style={{ color: colors.primary }}
                >
                  <span>Learn more in portal</span>
                  <span>&rarr;</span>
                </div>
              </motion.div>
            );
          })}
        </div>
      </div>
    </section>
  );
};

export default FeaturesSection;
