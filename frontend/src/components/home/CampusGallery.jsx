import React from 'react';
import { motion } from 'framer-motion';
import { themes } from '../../context/ThemeContext';

import campus1Img from '../../assets/Campus1.jpg';
import insideCampusImg from '../../assets/Inside Campus.jpg';
import libraryImg from '../../assets/Library.jpg';
import modernCampusImg from '../../assets/modernCampus.jpg';
import overviewImg from '../../assets/Overview.jpg';

const CampusGallery = () => {
  const colors = themes.jungle.colors;

  return (
    <section id="campus" className="py-20 md:py-28 bg-white border-t border-gray-100" style={{ paddingTop: "20px" }} >
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        {/* Section Header */}
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
            Campus Life & Infrastructure
          </div>
          <h2 className="text-3xl sm:text-4xl font-extrabold text-gray-900 tracking-tight">
            Our Campus Environment
          </h2>
          <p className="text-base text-gray-600 font-normal">
            Designed for collaboration, research, and holistic student growth with state-of-the-art academic facilities.
          </p>
        </motion.div>

        {/* Asymmetrical Gallery Grid */}
        <div className="space-y-6">
          {/* Top Row: Large Featured Image Left + Two Stacked Images Right */}
          <div className="grid grid-cols-1 md:grid-cols-12 gap-6 items-stretch">
            {/* 1. Large Main Featured Image (Overview.jpg) */}
            <motion.div
              initial={{ opacity: 0, scale: 0.98 }}
              whileInView={{ opacity: 1, scale: 1 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5 }}
              className="md:col-span-7 group relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer min-h-[320px] md:min-h-[440px]"
            >
              <img
                src={overviewImg}
                alt="Campus Overview"
                className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition-opacity duration-300" />
              <div className="absolute bottom-6 left-6 right-6 text-white">
                <span className="text-xs uppercase font-semibold tracking-wider opacity-80">Aerial View</span>
                <h3 className="text-xl font-bold">University Campus Overview</h3>
              </div>
            </motion.div>

            {/* Right Stacked Column */}
            <div className="md:col-span-5 grid grid-cols-1 gap-6">
              {/* 2. Library.jpg */}
              <motion.div
                initial={{ opacity: 0, scale: 0.98 }}
                whileInView={{ opacity: 1, scale: 1 }}
                viewport={{ once: true }}
                transition={{ duration: 0.5, delay: 0.1 }}
                className="group relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer h-[200px] md:h-[208px]"
              >
                <img
                  src={libraryImg}
                  alt="Central Library"
                  className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition-opacity duration-300" />
                <div className="absolute bottom-4 left-4 right-4 text-white">
                  <span className="text-[10px] uppercase font-semibold tracking-wider opacity-80">Research</span>
                  <h3 className="text-base font-bold">Central Digital Library</h3>
                </div>
              </motion.div>

              {/* 3. Inside Campus.jpg */}
              <motion.div
                initial={{ opacity: 0, scale: 0.98 }}
                whileInView={{ opacity: 1, scale: 1 }}
                viewport={{ once: true }}
                transition={{ duration: 0.5, delay: 0.2 }}
                className="group relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer h-[200px] md:h-[208px]"
              >
                <img
                  src={insideCampusImg}
                  alt="Inside Campus Commons"
                  className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition-opacity duration-300" />
                <div className="absolute bottom-4 left-4 right-4 text-white">
                  <span className="text-[10px] uppercase font-semibold tracking-wider opacity-80">Learning Spaces</span>
                  <h3 className="text-base font-bold">Academic Commons & Atrium</h3>
                </div>
              </motion.div>
            </div>
          </div>

          {/* Bottom Row: Two Wider Images */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* 4. Campus1.jpg */}
            <motion.div
              initial={{ opacity: 0, scale: 0.98 }}
              whileInView={{ opacity: 1, scale: 1 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5, delay: 0.3 }}
              className="group relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer h-[240px] md:h-[280px]"
            >
              <img
                src={campus1Img}
                alt="Main Academic Building"
                className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition-opacity duration-300" />
              <div className="absolute bottom-5 left-5 right-5 text-white">
                <span className="text-xs uppercase font-semibold tracking-wider opacity-80">Main Quad</span>
                <h3 className="text-lg font-bold">Historic Academic Hall</h3>
              </div>
            </motion.div>

            {/* 5. modernCampus.jpg */}
            <motion.div
              initial={{ opacity: 0, scale: 0.98 }}
              whileInView={{ opacity: 1, scale: 1 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5, delay: 0.4 }}
              className="group relative rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 cursor-pointer h-[240px] md:h-[280px]"
            >
              <img
                src={modernCampusImg}
                alt="Modern Innovation Complex"
                className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-80 group-hover:opacity-90 transition-opacity duration-300" />
              <div className="absolute bottom-5 left-5 right-5 text-white">
                <span className="text-xs uppercase font-semibold tracking-wider opacity-80">Innovation Hub</span>
                <h3 className="text-lg font-bold">Technology & Science Center</h3>
              </div>
            </motion.div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default CampusGallery;
