import React from 'react';
import { GraduationCap } from 'lucide-react';
import { themes } from '../../context/ThemeContext';

const Footer = () => {
  const colors = themes.jungle.colors;

  return (
    <footer className="bg-gray-900 text-gray-400 py-16 border-t border-gray-800">
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        <div className="grid grid-cols-1 md:grid-cols-12 gap-10 pb-12 border-b border-gray-800">
          {/* Brand Col */}
          <div className="md:col-span-5 space-y-4">
            <div className="flex items-center gap-2.5">
              <div
                className="w-8 h-8 rounded-lg flex items-center justify-center text-white"
                style={{ backgroundColor: colors.primary }}
              >
                <GraduationCap className="w-4 h-4" />
              </div>
              <span className="text-xl font-bold tracking-tight text-white">
                SIMS<span style={{ color: colors.primary }}>.</span>
              </span>
            </div>
            <p className="text-sm text-gray-400 max-w-sm leading-relaxed font-normal">
              Student Information Management System — Delivering centralized, enterprise-grade academic record solutions for higher education institutions.
            </p>
          </div>

          {/* Navigation Links */}
          <div className="md:col-span-3 space-y-3">
            <h4 className="text-xs font-bold uppercase tracking-wider text-gray-200">Navigation</h4>
            <ul className="space-y-2 text-sm">
              <li><a href="#" className="hover:text-white transition-colors">Home</a></li>
              <li><a href="#about" className="hover:text-white transition-colors">About Institution</a></li>
              <li><a href="#features" className="hover:text-white transition-colors">Platform Features</a></li>
              <li><a href="#campus" className="hover:text-white transition-colors">Campus Environment</a></li>
              <li><a href="#contact" className="hover:text-white transition-colors">Contact & Support</a></li>
            </ul>
          </div>

          {/* Legal Links */}
          <div className="md:col-span-4 space-y-3">
            <h4 className="text-xs font-bold uppercase tracking-wider text-gray-200">Legal & Governance</h4>
            <ul className="space-y-2 text-sm">
              <li><a href="#" className="hover:text-white transition-colors">Privacy Policy</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Terms of Service</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Data Security Compliance</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Institutional Governance</a></li>
            </ul>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="pt-8 flex flex-col sm:flex-row items-center justify-between text-xs text-gray-500 gap-4">
          <p>© {new Date().getFullYear()} Student Information Management System (SIMS). All rights reserved.</p>
          <p className="text-gray-500">Academic Systems Division</p>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
