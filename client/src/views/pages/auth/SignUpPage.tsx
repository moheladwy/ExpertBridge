import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { auth } from "@/lib/firebase";
import GoogleLogo from "@/assets/Login-SignupAssets/Google-Logo.svg";
import { useCreateUser } from "@/features/auth/useCreateUser";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

const SignUpPage: React.FC = () => {
  const [isLoggedIn, isLoggedInLoading, isLoggedInError] = useIsUserLoggedIn();
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoggedIn) {
      navigate('/home');
    }
  }, [isLoggedIn, navigate]);

  const [
    signInWithGoogle,
    signUpWithEmailAndPassword,
    authUser,
    createdAppUser,
    createUserLoading,
    authError,
    createUserErrorMessage,
    createUserSuccess
  ] = useCreateUser(auth);

  // Form State
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [signUpError, setSignUpError] = useState<string>("");

  useEffect(() => {
    if (createUserSuccess) {
      navigate('/login');
    }
  })

  useEffect(() => {
    if (createUserErrorMessage) {
      setSignUpError('An error occurred while creating your account. Please try again.');
      console.log(createUserErrorMessage);
    }
  }, [createUserErrorMessage]);

  useEffect(() => {
    if (authError) {
      console.error("Sign-up error:", authError);
      setSignUpError("Sign-up failed. Please try again.");
    }
  }, [authError]);

  // Form Validation
  const validate = async () => {
    const newErrors: { [key: string]: string } = {};

    if (!formData.firstName.trim()) newErrors.firstName = "First name is required";
    if (!formData.lastName.trim()) newErrors.lastName = "Last name is required";
    if (!formData.username.trim()) newErrors.username = "Username is required";
    if (!formData.email.includes("@")) newErrors.email = "Invalid email format";
    if (formData.password.length < 12) newErrors.password = "Password must be at least 12 characters";
    if (formData.password !== formData.confirmPassword) newErrors.confirmPassword = "Passwords do not match";

    // TODO: Make a request to the api checking if the email is already used. 
    // That brings a very good question: 
    // Can the user create an 'email login' with the same Google email used by another
    // 'Google' login? Because firebase allows that, but should we allow it in the api? 

    // Placeholder for backend username check
    // if (await isUsernameTaken(formData.username)) {
    //   newErrors.username = "Username is already taken";
    // }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Handle Input Changes
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setSignUpError("");
  };

  // Handle Form Submission (Email Sign Up)
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const isValid = await validate();
    if (!isValid) return;

    const user = {
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email,
      username: formData.username,
    }

    await signUpWithEmailAndPassword(formData.email, formData.password, user);
  };

  const handleGoogleSignIn = async () => {
    await signInWithGoogle();
    // navigate('/home');
  }

  const loading = createUserLoading || isLoggedInLoading;

  return (
    <div className="flex justify-center items-center h-screen bg-main-blue">
      <div className="p-6 w-1/3 max-xl:w-1/2 max-sm:w-2/3">
        <h2 className="font-bold mb-4 text-white text-5xl text-left max-xl:text-5xl max-lg:text-4xl max-sm:text-3xl">
          Create Your Account.
        </h2>

        {/* Sign-Up Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Name Fields */}
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
              {errors.firstName && <p className="text-red-500 text-sm">{errors.firstName}</p>}
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
              {errors.lastName && <p className="text-red-500 text-sm">{errors.lastName}</p>}
            </div>
          </div>

          {/* Username Field */}
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
            {errors.username && <p className="text-red-500 text-sm">{errors.username}</p>}
          </div>

          {/* Email Field */}
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
            {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}
          </div>

          {/* Password Fields */}
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
            {errors.password && <p className="text-red-500 text-sm">{errors.password}</p>}
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
            {errors.confirmPassword && <p className="text-red-500 text-sm">{errors.confirmPassword}</p>}
          </div>

          {/* Sign-Up Button */}
          <button
            type="submit"
            className="w-full bg-main-purple text-white font-bold p-4 rounded hover:bg-purple-900 disabled:bg-purple-500"
            disabled={loading}
          >
            {createUserLoading ? "Signing Up..." : "Sign Up"}
          </button>

          {/* Google Sign-Up Button */}
          <button
            type="button"
            className="w-full bg-white text-black font-bold p-2 rounded hover:bg-gray-300 disabled:bg-gray-400 disabled:text-gray-600"
            disabled={loading}
            onClick={handleGoogleSignIn}
          >
            <div className="flex justify-center items-center">
              <img src={GoogleLogo} alt="Google Logo" className="w-10 h-10 mr-4" />
              <div>{loading ? "Signing up with Google..." : "Signup with Google"}</div>
            </div>
          </button>

          {/* Error Messages */}
          {signUpError && <p className="text-red-500 text-sm text-center">{signUpError}</p>}
        </form>
      </div>
    </div>
  );
};

export default SignUpPage;
