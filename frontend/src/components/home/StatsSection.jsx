import React, { useRef, useEffect } from 'react';
import { motion, useInView, animate } from 'framer-motion';
import { themes } from '../../context/ThemeContext';

const AnimatedCounter = ({ value, suffix = '' }) => {
  const ref = useRef(null);
  const isInView = useInView(ref, { once: true, margin: '-50px' });

  useEffect(() => {
    if (isInView && ref.current) {
      const node = ref.current;
      const controls = animate(0, value, {
        duration: 2,
        ease: 'easeOut',
        onUpdate(val) {
          node.textContent = Math.round(val).toLocaleString() + suffix;
        },
      });
      return () => controls.stop();
    }
  }, [isInView, value, suffix]);

  return <span ref={ref}>0{suffix}</span>;
};

const StatsSection = () => {
  const colors = themes.jungle.colors;

  const stats = [
    { label: 'Years of Excellence', value: 25, suffix: '+' },
    { label: 'Active Students', value: 15000, suffix: '+' },
    { label: 'Faculty Members', value: 800, suffix: '+' },
    { label: 'Academic Programs', value: 120, suffix: '+' },
  ];

  return (
    <section className="py-12 bg-gray-50/70 border-y border-gray-100 relative">
      <div className="max-w-7xl mx-auto px-6 sm:px-8">
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-6 md:gap-8">
          {stats.map((stat, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
              className="bg-white p-6 sm:p-8 rounded-2xl border border-gray-200/70 text-center shadow-xs hover:shadow-md transition-shadow duration-300"
            >
              <div
                className="text-3xl sm:text-4xl lg:text-5xl font-extrabold tracking-tight mb-2"
                style={{ color: colors.primary }}
              >
                <AnimatedCounter value={stat.value} suffix={stat.suffix} />
              </div>
              <div className="text-xs sm:text-sm font-semibold uppercase tracking-wider text-gray-500">
                {stat.label}
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default StatsSection;
