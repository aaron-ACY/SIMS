import React from 'react';
import { motion } from 'framer-motion';
import { Award, BookOpen, Users, ShieldCheck, CheckCircle } from 'lucide-react';
import { themes } from '../../context/ThemeContext';

const AboutSection = () => {
  const colors = themes.jungle.colors;

  const pillars = [
    {
      icon: Award,
      title: 'Academic Excellence',
      desc: 'Delivering world-class curriculum frameworks backed by modern digital evaluation systems.'
    },
    {
      icon: Users,
      title: 'Student-Centric Focus',
      desc: 'Empowering students with instant access to academic records, schedules, and progress insights.'
    },
    {
      icon: ShieldCheck,
      title: 'Institutional Trust',
      desc: 'Ensuring strict data protection, role-based access control, and compliant records management.'
    }
  ];

  return (
    <section id="about" className="py-20 md:py-28 bg-white border-t border-gray-100" style={{ paddingTop: "20px" }} >
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 items-center">
          {/* Left Column: Heading & Content */}
          <motion.div
            initial={{ opacity: 0, x: -30 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6 }}
            className="lg:col-span-6 space-y-6"
          >
            <div className="inline-block text-xs font-bold uppercase tracking-widest px-3 py-1 rounded-md"
                 style={{ backgroundColor: colors.hover, color: colors.primaryDark }}>
              Institutional Overview
            </div>

            <h2 className="text-3xl sm:text-4xl font-extrabold text-gray-900 tracking-tight leading-tight">
              About Our Institution
            </h2>

            <p className="text-base sm:text-lg text-gray-600 leading-relaxed font-normal">
              Founded on the principles of academic integrity and digital innovation, our institution provides an integrated educational ecosystem. The Student Information Management System (SIMS) serves as the digital backbone, bridging administration, faculty, and students on a single secure platform.
            </p>

            <div className="space-y-3 pt-2">
              {[
                'Standardized digital academic record keeping',
                'Seamless integration between courses, grades, and attendance',
                'Transparent reporting for faculty and administrative oversight'
              ].map((item, index) => (
                <div key={index} className="flex items-start gap-3">
                  <CheckCircle className="w-5 h-5 flex-shrink-0 mt-0.5" style={{ color: colors.primary }} />
                  <span className="text-sm font-medium text-gray-700">{item}</span>
                </div>
              ))}
            </div>
          </motion.div>

          {/* Right Column: Pillars Grid */}
          <motion.div
            initial={{ opacity: 0, x: 30 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="lg:col-span-6 grid grid-cols-1 sm:grid-cols-1 gap-5"
          >
            {pillars.map((pillar, idx) => {
              const Icon = pillar.icon;
              return (
                <div
                  key={idx}
                  className="p-6 rounded-2xl border border-gray-100 bg-gray-50/50 hover:bg-white hover:border-gray-200 transition-all duration-300 shadow-xs hover:shadow-md flex items-start gap-5"
                >
                  <div
                    className="p-3.5 rounded-xl flex-shrink-0 text-white"
                    style={{ backgroundColor: colors.primary }}
                  >
                    <Icon className="w-6 h-6" />
                  </div>
                  <div>
                    <h3 className="text-lg font-bold text-gray-900 mb-1.5">{pillar.title}</h3>
                    <p className="text-sm text-gray-600 leading-relaxed">{pillar.desc}</p>
                  </div>
                </div>
              );
            })}
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default AboutSection;
