import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { User, Lock, Mail, ArrowLeft, GraduationCap, Eye, EyeOff } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { themes } from "../../context/ThemeContext";
import overviewImg from "../../assets/Overview.jpg";
import { authService } from "../../api/services";

const Register = () => {
  const navigate = useNavigate();
  const colors = themes.jungle.colors;

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleRegister = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess(false);
    setIsLoading(true);

    try {
      const response = await authService.register(username, password, email);
      if (response && response.success) {
        setSuccess(true);
        // If the backend returns a token upon registration, you could also log them in automatically
        // For now, redirect to login after a brief delay
        setTimeout(() => {
          navigate("/login");
        }, 2000);
      } else {
        setError(response?.message || "Registration failed!");
      }
    } catch (err) {
      setError(err?.response?.data?.message || err?.message || "An error occurred during registration. Note: Your email must be pre-registered by the admin.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div
      className="relative min-h-screen w-full bg-cover bg-center bg-no-repeat flex items-center justify-center font-sans overflow-x-hidden"
      style={{
        backgroundImage: `url(${overviewImg})`,
      }}
    >
      {/* Dark Overlay with Blur */}
      <div className="absolute inset-0 bg-slate-950/75 backdrop-blur-sm" />

      {/* Header Bar */}
      <header className="fixed top-0 left-0 right-0 z-50 p-6 sm:p-8 flex items-center justify-between">
        <div
          onClick={() => navigate("/")}
          className="flex items-center gap-2.5 cursor-pointer group transition-transform duration-200 hover:scale-[1.02]"
        >
          <div
            className="w-10 h-10 rounded-xl flex items-center justify-center text-white shadow-md"
            style={{ backgroundColor: colors.primary }}
          >
            <GraduationCap className="w-5 h-5" />
          </div>
          <span className="text-2xl font-bold tracking-tight text-white">
            SIMS<span style={{ color: colors.primary }}>.</span>
          </span>
        </div>

        <button
          onClick={() => navigate("/")}
          className="group flex items-center gap-2 text-xs font-semibold text-white/80 hover:text-white bg-white/10 hover:bg-white/20 backdrop-blur-md px-4 py-2.5 rounded-xl border border-white/15 transition-all duration-200 cursor-pointer"
        >
          <ArrowLeft className="w-4 h-4 transition-transform group-hover:-translate-x-1" />
          <span>Back to Landing Page</span>
        </button>
      </header>

      {/* Main Content Area */}
      <div className="relative z-10 w-full max-w-lg mx-auto px-6 py-28 flex flex-col items-center justify-center">
        
        {/* Registration Form Card */}
        <motion.div
          initial={{ opacity: 0, y: 30, scale: 0.98 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          transition={{ duration: 0.6 }}
          className="w-full bg-white/95 backdrop-blur-xl rounded-3xl shadow-2xl p-8 sm:p-10 border border-white/40 relative"
          style={{ boxShadow: "0 25px 50px -12px rgba(0, 0, 0, 0.5)" }}
        >
          <div className="mb-8 text-center sm:text-left">
            <h2 className="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight mb-2">
              Create an Account
            </h2>
            <p className="text-sm text-gray-500 font-normal">
              Register using your pre-authorized email address.
            </p>
          </div>

          <form onSubmit={handleRegister} className="space-y-5">
            {/* Error Message */}
            <AnimatePresence mode="wait">
              {error && (
                <motion.div
                  initial={{ opacity: 0, y: -10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -10 }}
                  className="p-3.5 bg-red-50 border border-red-200 text-red-700 text-xs font-semibold rounded-xl flex items-center gap-2.5"
                >
                  <div className="w-4 h-4 rounded-full bg-red-600 text-white flex items-center justify-center text-[10px] font-bold flex-shrink-0">
                    !
                  </div>
                  <span>{error}</span>
                </motion.div>
              )}
            </AnimatePresence>

            {/* Success Message */}
            <AnimatePresence mode="wait">
              {success && (
                <motion.div
                  initial={{ opacity: 0, y: -10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -10 }}
                  className="p-3.5 bg-green-50 border border-green-200 text-green-700 text-xs font-semibold rounded-xl flex items-center gap-2.5"
                >
                  <div className="w-4 h-4 rounded-full bg-green-600 text-white flex items-center justify-center text-[10px] font-bold flex-shrink-0">
                    ✓
                  </div>
                  <span>Registration successful! Redirecting to login...</span>
                </motion.div>
              )}
            </AnimatePresence>

            {/* Username */}
            <div className="space-y-1.5">
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-700">
                Username
              </label>
              <div className="relative">
                <input
                  type="text"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder="Choose a username"
                  required
                  className="w-full pl-11 pr-4 py-3 bg-gray-50/80 border border-gray-200 rounded-xl text-sm font-medium text-gray-900 placeholder:text-gray-400 focus:outline-none focus:bg-white focus:ring-2 transition-all duration-200"
                  style={{ focusRingColor: colors.primary }}
                />
                <User className="w-4 h-4 text-gray-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
              </div>
            </div>

            {/* Email */}
            <div className="space-y-1.5">
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-700">
                Email Address
              </label>
              <div className="relative">
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="Enter your authorized email"
                  required
                  className="w-full pl-11 pr-4 py-3 bg-gray-50/80 border border-gray-200 rounded-xl text-sm font-medium text-gray-900 placeholder:text-gray-400 focus:outline-none focus:bg-white focus:ring-2 transition-all duration-200"
                  style={{ focusRingColor: colors.primary }}
                />
                <Mail className="w-4 h-4 text-gray-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
              </div>
            </div>

            {/* Password */}
            <div className="space-y-1.5">
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-700">
                Password
              </label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="Create a strong password"
                  required
                  className="w-full pl-11 pr-11 py-3 bg-gray-50/80 border border-gray-200 rounded-xl text-sm font-medium text-gray-900 placeholder:text-gray-400 focus:outline-none focus:bg-white focus:ring-2 transition-all duration-200"
                  style={{ focusRingColor: colors.primary }}
                />
                <Lock className="w-4 h-4 text-gray-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3.5 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-700 transition-colors p-1"
                >
                  {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              </div>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              disabled={isLoading || success}
              className="w-full py-3.5 px-6 text-sm font-semibold text-white rounded-xl shadow-md hover:shadow-lg transition-all duration-200 flex items-center justify-center gap-2 cursor-pointer disabled:opacity-70 disabled:cursor-not-allowed mt-2"
              style={{ backgroundColor: colors.primary }}
              onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = colors.primaryDark)}
              onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = colors.primary)}
            >
              {isLoading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <span>Register Account</span>
              )}
            </button>
          </form>

          {/* Footer Note */}
          <div className="mt-8 pt-6 border-t border-gray-100 text-center">
            <p className="text-sm text-gray-600 font-medium">
              Already have an account?{" "}
              <button
                type="button"
                onClick={() => navigate("/login")}
                className="font-bold hover:underline"
                style={{ color: colors.primary }}
              >
                Sign In
              </button>
            </p>
          </div>
        </motion.div>
      </div>
    </div>
  );
};

export default Register;
