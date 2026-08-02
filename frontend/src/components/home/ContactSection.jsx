import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Mail, Phone, Send, CheckCircle2 } from 'lucide-react';
import { themes } from '../../context/ThemeContext';

const ContactSection = () => {
  const colors = themes.jungle.colors;
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = (e) => {
    e.preventDefault();
    setSubmitted(true);
    setTimeout(() => setSubmitted(false), 4000);
  };

  return (
    <section id="contact" className="py-20 md:py-28 bg-white border-t border-gray-100" style={{ paddingTop: "20px" }}>
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-16 items-start">
          {/* Left Column: Contact Details */}
          <motion.div
            initial={{ opacity: 0, x: -30 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6 }}
            className="lg:col-span-5 space-y-8"
          >
            <div>
              <div
                className="inline-block text-xs font-bold uppercase tracking-widest px-3 py-1 rounded-md mb-3"
                style={{ backgroundColor: colors.hover, color: colors.primaryDark }}
              >
                Get In Touch
              </div>
              <h2 className="text-3xl sm:text-4xl font-extrabold text-gray-900 tracking-tight">
                Contact Admissions & Support
              </h2>
              <p className="text-base text-gray-600 mt-3 leading-relaxed">
                Have questions regarding institutional enrollment, system access, or administrative support? Send us a message and our team will get back to you.
              </p>
            </div>

            <div className="space-y-6 pt-2">
              <div className="flex items-start gap-4">
                <div
                  className="p-3 rounded-xl flex-shrink-0 text-white"
                  style={{ backgroundColor: colors.primary }}
                >
                  <MapPin className="w-5 h-5" />
                </div>
                <div>
                  <h4 className="text-sm font-bold text-gray-900">Campus Address</h4>
                  <p className="text-sm text-gray-600">123 University Avenue, Academic District, City 10001</p>
                </div>
              </div>

              <div className="flex items-start gap-4">
                <div
                  className="p-3 rounded-xl flex-shrink-0 text-white"
                  style={{ backgroundColor: colors.primary }}
                >
                  <Mail className="w-5 h-5" />
                </div>
                <div>
                  <h4 className="text-sm font-bold text-gray-900">Email Address</h4>
                  <p className="text-sm text-gray-600">admissions@sims.edu.vn | support@sims.edu.vn</p>
                </div>
              </div>

              <div className="flex items-start gap-4">
                <div
                  className="p-3 rounded-xl flex-shrink-0 text-white"
                  style={{ backgroundColor: colors.primary }}
                >
                  <Phone className="w-5 h-5" />
                </div>
                <div>
                  <h4 className="text-sm font-bold text-gray-900">Telephone Line</h4>
                  <p className="text-sm text-gray-600">+84 (028) 3800 1234 (Office Hours: 8 AM - 5 PM)</p>
                </div>
              </div>
            </div>
          </motion.div>

          {/* Right Column: Contact Form */}
          <motion.div
            initial={{ opacity: 0, x: 30 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ duration: 0.6, delay: 0.2 }}
            className="lg:col-span-7 bg-gray-50/80 p-8 sm:p-10 rounded-2xl border border-gray-200/80 shadow-xs"
          >
            {submitted ? (
              <div className="py-12 text-center space-y-3">
                <CheckCircle2 className="w-12 h-12 mx-auto" style={{ color: colors.primary }} />
                <h3 className="text-xl font-bold text-gray-900">Message Sent Successfully</h3>
                <p className="text-sm text-gray-600">Thank you for reaching out. Our support team will respond shortly.</p>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-6">
                <div>
                  <label className="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                    Full Name
                  </label>
                  <input
                    type="text"
                    required
                    placeholder="Enter your name"
                    className="w-full px-4 py-3 bg-white border border-gray-200 rounded-xl text-sm text-gray-900 focus:outline-none focus:ring-2 transition-all duration-200"
                    style={{ focusRingColor: colors.primary }}
                  />
                </div>

                <div>
                  <label className="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                    Email Address
                  </label>
                  <input
                    type="email"
                    required
                    placeholder="name@example.com"
                    className="w-full px-4 py-3 bg-white border border-gray-200 rounded-xl text-sm text-gray-900 focus:outline-none focus:ring-2 transition-all duration-200"
                  />
                </div>

                <div>
                  <label className="block text-xs font-bold uppercase tracking-wider text-gray-700 mb-2">
                    Message
                  </label>
                  <textarea
                    rows={4}
                    required
                    placeholder="Write your query or message here..."
                    className="w-full px-4 py-3 bg-white border border-gray-200 rounded-xl text-sm text-gray-900 focus:outline-none focus:ring-2 transition-all duration-200 resize-none"
                  />
                </div>

                <button
                  type="submit"
                  className="w-full py-3.5 px-6 text-sm font-semibold text-white rounded-xl shadow-md hover:shadow-lg transition-all duration-200 flex items-center justify-center gap-2 cursor-pointer"
                  style={{ backgroundColor: colors.primary }}
                  onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = colors.primaryDark)}
                  onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = colors.primary)}
                >
                  <Send className="w-4 h-4" />
                  <span>Send Message</span>
                </button>
              </form>
            )}
          </motion.div>
        </div>
      </div>
    </section>
  );
};

export default ContactSection;
