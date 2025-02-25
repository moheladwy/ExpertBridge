import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useCreateUserWithEmailAndPassword from "@/lib/firebase/EmailAuth/useCreateUserWithEmailAndPassword";
import { auth } from "@/lib/firebase";
import GoogleLogo from '../../assets/Login-SignupAssets/Google-Logo.svg'

const SignUpPage: React.FC = () => {
  const navigate = useNavigate();
  // const [signUp, { isLoading, error }] = useSignUpMutation();
  const [
    registerWithEmailAndPassword,
    registeredUser,
    loading,
    error
  ] = useCreateUserWithEmailAndPassword(auth, { sendEmailVerification: true });

  const [formData, setFormData] = useState({
    firstName:"",
    lastName:"",
    username:"",
    email: "",
    password: "",
    confirmPassword:""
  });

  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    if (registeredUser) {
      navigate("/login");
    }
  }, [registeredUser, navigate]);

  // handle errors
  useEffect(() => {
    if (error) {
      console.error('useEffecting', error);
    }
  }, [error]);

  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!formData.email.includes("@")) newErrors.email = "Invalid email";
    if (formData.password.length < 12) //SHOULD BE EDITED
      newErrors.password = "Password must be at least 12 characters";
    if (false) //IF THE USERNAME IS USED BEFORE
      newErrors.username = "Username should be unique";
    if(formData.password !== formData.confirmPassword)
      newErrors.confermPassword = "The Passwords doesn't match"

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    await registerWithEmailAndPassword(formData.email, formData.password);
  };

  return (
    <div className="flex justify-center items-center h-screen bg-main-blue">
      <div className="p-6 w-1/3 max-xl:w-1/2 max-sm:w-2/3">
        <h2 className="font-bold mb-4 text-white text-5xl text-left max-xl:text-5xl max-lg:text-4xl max-sm:text-3xl">Create Your Account.</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="flex w-full gap-x-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-white">First Name</label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                className="w-full p-2 border rounded-md"
                placeholder="Enter Your First Name"
                disabled={loading}
              />
            </div>
            <div className="flex-1">
              <label className="block text-sm font-medium text-white">Last Name</label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                className="w-full p-2 border rounded-md"
                placeholder="Enter Your Last Name"
                disabled={loading}
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-white">Username</label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              className="w-full p-2 border rounded-md"
              placeholder="Enter Your Username"
              disabled={loading}
            />
            {errors.username && <p className="text-red-500 text-sm max-md:text-xs">{errors.username}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-white">Email Address</label>
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
            <label className="block text-sm font-medium text-white">Create Password</label>
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
          <div>
            <label className="block text-sm font-medium text-white">Confirm Password</label>
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              className="w-full p-2 border rounded-md"
              placeholder="Enter Your Password Again"
              disabled={loading}
            />
            {errors.confermPassword && <p className="text-red-500 text-sm max-md:text-xs">{errors.confermPassword}</p>}
          </div>
          <button
            type="submit"
            className="w-full bg-main-purple text-white font-bold p-4 rounded hover:bg-purple-900 disabled:bg-purple-500"
            disabled={loading}
          >
            {loading ? "Signing Up..." : "Sign Up"}
          </button>

          <button
            className="w-full bg-white text-black font-bold p-2 rounded hover:bg-gray-300 disabled:bg-gray-400 disabled:text-gray-600"
            disabled={loading}
          >
            <div className="flex justify-center items-center">
              <img src={GoogleLogo} alt="" className="w-10 h-10 mr-4"/>
              <div>Signup with Google</div>
            </div>
          </button>
          {error && <p className="text-red-500 text-sm text-center">Signup failed {error.message}</p>}
        </form>
      </div>
    </div>
  );
};

export default SignUpPage;
