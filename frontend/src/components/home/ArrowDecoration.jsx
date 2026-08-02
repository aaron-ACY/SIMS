import React from 'react';
import arrowHeadImg from '../../assets/Arrow head.png';

const ArrowDecoration = () => {
  return (
    <div className="relative w-full overflow-hidden pointer-events-none select-none py-4">
      <div className="max-w-7xl mx-auto px-6 sm:px-8 relative">
        <div className="flex items-center justify-start pl-4 sm:pl-12">
          <img
            src={arrowHeadImg}
            alt="Decorative Arrow"
            className="w-24 sm:w-32 md:w-40 h-auto opacity-25 filter grayscale contrast-200 transform -rotate-45 translate-y-2 translate-x-4 transition-opacity duration-300"
          />
        </div>
      </div>
    </div>
  );
};

export default ArrowDecoration;
