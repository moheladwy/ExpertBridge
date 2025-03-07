import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useSignInWithEmailAndPassword from "@/lib/firebase/EmailAuth/useSignInWithEmailAndPassword";
import { auth } from "@/lib/firebase";
import GoogleLogo from '../../assets/Login-SignupAssets/Google-Logo.svg'

const LoginPage: React.FC = () => {
  const navigate = useNavigate();

  const [
    loginWithEmailAndPassword,
    loggedInUser,
    loading,
    error] = useSignInWithEmailAndPassword(auth);

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const [signInError, setSignInError] = useState<string>('');

  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    if (loggedInUser) {
      if (loggedInUser.user.emailVerified) {
        navigate('/home');
      } else {
        setSignInError("Please verify your email before logging in.");
      }
    }
  }, [loggedInUser, navigate]);

  useEffect(() => {
    if (error) {
      console.error('useEffecting', error);
    }
  }, [error]);

  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!formData.email.includes("@")) newErrors.email = "Invalid email";
    if (formData.password.length < 6)
      newErrors.password = "Password must be at least 6 characters";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setSignInError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    await loginWithEmailAndPassword(formData.email, formData.password);

  }

  return (
    <div className="flex justify-center items-center h-screen bg-main-blue">
      <div className="p-6 w-1/3 max-xl:w-1/2 max-sm:w-2/3">
        <h2 className="font-bold mb-4 text-white text-6xl text-left max-xl:text-5xl max-lg:text-4xl max-sm:text-3xl">Nice to see you again.</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
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
            {errors.email && <p className="text-red-500 text-sm max-md:text-xs">{errors.email}</p>}
          </div>
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
            {errors.password && <p className="text-red-500 text-sm max-md:text-xs">{errors.password}</p>}
          </div>
          <div className="w-full text-right">
            <a href="" className="text-sm text-orange-400"> Forget Password?</a>
          </div>
          <button
            type="submit"
            className="w-full bg-main-purple text-white font-bold p-4 rounded hover:bg-purple-900 disabled:bg-purple-500"
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>

          <button
            className="w-full bg-white text-black font-bold p-2 rounded hover:bg-gray-300 disabled:bg-gray-400 disabled:text-gray-600"
            disabled={loading}
          >
            <div className="flex justify-center items-center">
              <img src={GoogleLogo} alt="" className="w-10 h-10 mr-4" />
              <div>Login with Google</div>
            </div>
          </button>

          {error && <p className="text-red-500 text-sm text-center">Invalid email or password. {error.message}</p>}
          {signInError && <p className="text-red-500 text-sm text-center">Invalid email or password. {signInError}</p>}
        </form>

        <div className="text-white text-sm text-center m-5">Don't have an account? <a href="/signup" className="underline">Register</a></div>
      </div>
    </div>
  );
};

export default LoginPage;
