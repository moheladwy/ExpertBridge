import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useSignInWithEmailAndPassword from "@/lib/firebase/EmailAuth/useSignInWithEmailAndPassword";
import { auth } from "@/lib/firebase";
import { useSignInWithGoogle } from "@/lib/firebase/useSignInWithPopup";
import GoogleLogo from "../../assets/Login-SignupAssets/Google-Logo.svg";

const LoginPage: React.FC = () => {
  const navigate = useNavigate();

  // Email/Password Login Hook
  const [loginWithEmailAndPassword, loggedInUser, loading, error] =
    useSignInWithEmailAndPassword(auth);

  // Google Login Hook
  const [signInWithGoogle, googleUser, googleLoading, googleError] =
    useSignInWithGoogle(auth);

  // Form State
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [signInError, setSignInError] = useState<string>("");

  useEffect(() => {
    if (error) {
      console.error("Login error:", error);
      setSignInError("Invalid email or password.");
    } else if (googleError) {
      console.error("Google login error:", googleError);
      setSignInError("Google login failed. Please try again.");
    }
  }, [error, googleError]);

  // Form Validation
  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!formData.email.includes("@")) newErrors.email = "Invalid email";
    if (formData.password.length < 6)
      newErrors.password = "Password must be at least 6 characters";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle Input Changes
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setSignInError("");
  };

  // Handle Form Submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    await loginWithEmailAndPassword(formData.email, formData.password);
  };

  return (
    <div className="flex justify-center items-center h-screen bg-main-blue">
      <div className="p-6 w-1/3 max-xl:w-1/2 max-sm:w-2/3">
        <h2 className="font-bold mb-4 text-white text-6xl text-left max-xl:text-5xl max-lg:text-4xl max-sm:text-3xl">
          Nice to see you again.
        </h2>

        {/* Login Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Email Input */}
          <div>
            <label className="block text-sm font-medium text-white">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full p-2 border rounded-md"
              placeholder="Enter Your Email"
              disabled={loading}
            />
            {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}
          </div>

          {/* Password Input */}
          <div>
            <label className="block text-sm font-medium text-white">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="w-full p-2 border rounded-md"
              placeholder="Enter Password"
              disabled={loading}
            />
            {errors.password && <p className="text-red-500 text-sm">{errors.password}</p>}
          </div>

          {/* Forgot Password */}
          <div className="w-full text-right">
            <a href="/forgot-password" className="text-sm text-orange-400">
              Forget Password?
            </a>
          </div>

          {/* Login Button */}
          <button
            type="submit"
            className="w-full bg-main-purple text-white font-bold p-4 rounded hover:bg-purple-900 disabled:bg-purple-500"
            disabled={loading || googleLoading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>

          {/* Google Login Button */}
          <button
            type="button"
            className="w-full bg-white text-black font-bold p-2 rounded hover:bg-gray-300 disabled:bg-gray-400 disabled:text-gray-600"
            disabled={googleLoading || loading}
            onClick={() => signInWithGoogle()}
          >
            <div className="flex justify-center items-center">
              <img src={GoogleLogo} alt="Google Logo" className="w-10 h-10 mr-4" />
              <div>{googleLoading ? "Signing in with Google..." : "Login with Google"}</div>
            </div>
          </button>

          {/* Error Messages */}
          {signInError && <p className="text-red-500 text-sm text-center">{signInError}</p>}
        </form>

        {/* Sign Up Redirect */}
        <div className="text-white text-sm text-center m-5">
          Don't have an account?{" "}
          <a href="/signup" className="underline">
            Register
          </a>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
