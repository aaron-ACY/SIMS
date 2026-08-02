import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { Eye, EyeOff, ArrowLeft, GraduationCap, ShieldCheck, Lock, User, Sparkles } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { themes } from "../../context/ThemeContext";
import overviewImg from "../../assets/Overview.jpg";

const Login = () => {
  const navigate = useNavigate();
  const { login } = useAuth();
  const colors = themes.jungle.colors;

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    setIsLoading(true);

    try {
      const result = await login(username, password);
      if (result && result.success) {
        if (result.redirect === "/admin") {
          navigate("/admin/dashboard");
        } else {
          navigate(result.redirect);
        }
      } else {
        setError(result?.message || "Invalid username or password!");
        setIsLoading(false);
      }
    } catch (err) {
      setError(err?.message || "An error occurred during login.");
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
        {/* SIMS Brand Logo */}
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

        {/* Back to Home CTA */}
        <button
          onClick={() => navigate("/")}
          className="group flex items-center gap-2 text-xs font-semibold text-white/80 hover:text-white bg-white/10 hover:bg-white/20 backdrop-blur-md px-4 py-2.5 rounded-xl border border-white/15 transition-all duration-200 cursor-pointer"
        >
          <ArrowLeft className="w-4 h-4 transition-transform group-hover:-translate-x-1" />
          <span>Back to Landing Page</span>
        </button>
      </header>

      {/* Main Content Area */}
      <div className="relative z-10 w-full max-w-7xl mx-auto px-6 py-28 flex flex-col lg:flex-row items-center justify-between gap-12 lg:gap-16">
        
        {/* Left Side Info (Desktop) */}
        <motion.div
          initial={{ opacity: 0, x: -40 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.7, ease: "easeOut" }}
          className="hidden lg:flex flex-1 flex-col justify-center text-white space-y-8 pr-6"
        >
          <h1 className="text-5xl xl:text-6xl font-extrabold leading-[1.15] tracking-tight text-white">
            Smart Management for <br />
            <span style={{ color: colors.primary }}>Modern Education</span>
          </h1>

          <p className="text-lg text-white/70 font-normal max-w-xl leading-relaxed">
            Access real-time academic records, course schedules, grading portals, and administrative tools on a single unified platform.
          </p>

          <div className="space-y-4 pt-2">
            {[
              "Role-based authentication for Students, Faculty & Admins",
              "Enterprise-grade security and encrypted data protection",
              "Instant synchronization with campus information systems"
            ].map((text, idx) => (
              <div key={idx} className="flex items-center gap-3">
                <div
                  className="w-5 h-5 rounded-full flex items-center justify-center flex-shrink-0 text-white"
                  style={{ backgroundColor: colors.primary }}
                >
                  <ShieldCheck className="w-3.5 h-3.5" />
                </div>
                <span className="text-sm text-white/85 font-medium">{text}</span>
              </div>
            ))}
          </div>
        </motion.div>

        {/* Right Side Login Form Card */}
        <motion.div
          initial={{ opacity: 0, y: 30, scale: 0.98 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          transition={{ duration: 0.6, delay: 0.1 }}
          className="w-full lg:w-[480px] bg-white/95 backdrop-blur-xl rounded-3xl shadow-2xl p-8 sm:p-10 border border-white/40 relative"
          style={{ boxShadow: "0 25px 50px -12px rgba(0, 0, 0, 0.5)" }}
        >
          {/* Header */}
          <div className="mb-8 text-center sm:text-left">
            <h2 className="text-2xl sm:text-3xl font-extrabold text-gray-900 tracking-tight mb-2">
              Sign In to Portal
            </h2>
            <p className="text-sm text-gray-500 font-normal">
              Enter your credentials to access your SIMS account.
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleLogin} className="space-y-5">
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

            {/* Username / Email */}
            <div className="space-y-1.5">
              <label className="block text-xs font-bold uppercase tracking-wider text-gray-700">
                Username / Email
              </label>
              <div className="relative">
                <input
                  type="text"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  placeholder="Enter student ID or email"
                  required
                  className="w-full pl-11 pr-4 py-3 bg-gray-50/80 border border-gray-200 rounded-xl text-sm font-medium text-gray-900 placeholder:text-gray-400 focus:outline-none focus:bg-white focus:ring-2 transition-all duration-200"
                  style={{ focusRingColor: colors.primary }}
                />
                <User className="w-4 h-4 text-gray-400 absolute left-3.5 top-1/2 -translate-y-1/2" />
              </div>
            </div>

            {/* Password */}
            <div className="space-y-1.5">
              <div className="flex items-center justify-between">
                <label className="block text-xs font-bold uppercase tracking-wider text-gray-700">
                  Password
                </label>
                <button
                  type="button"
                  onClick={() => alert("Please contact the administrator office for password reset assistance.")}
                  className="text-xs font-semibold transition-colors hover:underline"
                  style={{ color: colors.primary }}
                >
                  Forgot password?
                </button>
              </div>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="••••••••"
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
              disabled={isLoading}
              className="w-full py-3.5 px-6 text-sm font-semibold text-white rounded-xl shadow-md hover:shadow-lg transition-all duration-200 flex items-center justify-center gap-2 cursor-pointer disabled:opacity-70 disabled:cursor-not-allowed mt-2"
              style={{ backgroundColor: colors.primary }}
              onMouseEnter={(e) => (e.currentTarget.style.backgroundColor = colors.primaryDark)}
              onMouseLeave={(e) => (e.currentTarget.style.backgroundColor = colors.primary)}
            >
              {isLoading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : (
                <span>Sign In to Account</span>
              )}
            </button>
          </form>

          {/* Footer Note */}
          <div className="mt-6 pt-6 border-t border-gray-100 text-center space-y-3">
            <p className="text-sm text-gray-600 font-medium">
              Don't have an account?{" "}
              <button
                type="button"
                onClick={() => navigate("/register")}
                className="font-bold hover:underline"
                style={{ color: colors.primary }}
              >
                Register Here
              </button>
            </p>
            <p className="text-xs text-gray-400 font-medium">
              Need access or account support?{" "}
              <button
                type="button"
                onClick={() => navigate("/")}
                className="font-semibold hover:underline"
                style={{ color: colors.primary }}
              >
                Contact Administration
              </button>
            </p>
          </div>
        </motion.div>
      </div>
    </div>
  );
};

export default Login;
