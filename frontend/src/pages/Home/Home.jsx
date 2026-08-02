import React from 'react';
import Navbar from '../../components/home/Navbar';
import HeroSection from '../../components/home/HeroSection';
import AboutSection from '../../components/home/AboutSection';
import StatsSection from '../../components/home/StatsSection';
import ArrowDecoration from '../../components/home/ArrowDecoration';
import CampusGallery from '../../components/home/CampusGallery';
import FeaturesSection from '../../components/home/FeaturesSection';
import ContactSection from '../../components/home/ContactSection';
import Footer from '../../components/home/Footer';

const Home = () => {
  return (
    <div className="landing-page min-h-screen bg-white text-gray-800 font-sans selection:bg-emerald-100 selection:text-emerald-900 overflow-x-hidden antialiased">
      {/* 1. Navbar */}
      <Navbar />

      <main>
        {/* 2. Hero Section */}
        <HeroSection />

        {/* 3. About Section */}
        <AboutSection />

        {/* 4. Statistics Section */}
        <StatsSection />

        {/* 5. Arrow Decoration (between Stats & Campus Gallery) */}
        <ArrowDecoration />

        {/* 6. Campus Gallery */}
        <CampusGallery />

        {/* 8. Contact Section */}
        <ContactSection />
      </main>

      {/* 9. Footer */}
      <Footer />
    </div>
  );
};

export default Home;