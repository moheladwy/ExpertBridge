/**
 * @file LoginPage.tsx
 * @description Authentication page that allows users to sign in with email/password or Google OAuth.
 * Features form validation using Zod schema and provides feedback through toast notifications.
 * Redirects to home page upon successful login.
 */
import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import useSignInWithEmailAndPassword from "@/lib/firebase/EmailAuth/useSignInWithEmailAndPassword";
import { auth } from "@/lib/firebase";
import { useCreateUser } from "@/hooks/useCreateUser";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { toast } from "sonner";
import { Button } from "@/views/components/ui/button";
import { Label } from "@/views/components/ui/label";
import { Input } from "@/views/components/ui/input";
import LogoIcon from "@/assets/Logo-Icon/Logo-Icon.svg";
import GoogleLogo from "@/assets/Login-SignupAssets/Google-Logo.svg";
import { z } from "zod";
import { EyeIcon, EyeOffIcon } from "lucide-react";
import useSignOut from "@/lib/firebase/useSignOut";

/**
 * Zod schema for login form validation
 * Validates:
 * - Email format
 * - Password is not empty
 */
const loginSchema = z.object({
  email: z.string().email("Please enter a valid email address"),
  password: z.string().min(1, "Password is required"),
});

/**
 * Type definition for login form data inferred from Zod schema
 */
type LoginFormData = z.infer<typeof loginSchema>;

/**
 * LoginPage Component
 *
 * Provides authentication functionality for users to sign in to the application.
 * Supports email/password login and Google OAuth sign-in.
 * Includes form validation, loading states, and error handling.
 */
const LoginPage: React.FC = () => {
  const [isLoggedIn] = useIsUserLoggedIn();
  const navigate = useNavigate();
  const [signOut] = useSignOut(auth);

  // Main component state
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [showPassword, setShowPassword] = useState(false); // State for password visibility

  /**
   * Email/Password Login Hook
   * loginWithEmailAndPassword: Function to authenticate with email/password
   * loggedInUser: User object returned after successful login
   * loginLoading: Loading state for email/password login process
   * error: Error object if authentication fails
   */
  const [loginWithEmailAndPassword, loggedInUser, loginLoading, error] =
    useSignInWithEmailAndPassword(auth);

  /**
   * Google Authentication Hook - Creates or signs in user with Google
   * signInWithGoogle: Function to initiate Google sign-in flow
   * authUser: Firebase auth user object
   * createdAppUser: Application user object created/retrieved
   * createLoading: Loading state for Google sign-in process
   * createError/createErrorMessage: Error states for Google sign-in
   * createUserSuccess: Success state for Google sign-in
   */
  const [
    signInWithGoogle,
    _,
    authUser,
    createdAppUser,
    createLoading,
    createError,
    createErrorMessage,
    createUserSuccess,
  ] = useCreateUser(auth);

  // Form State Management
  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [signInError, setSignInError] = useState<string>("");

  /**
   * Navigate to home page on successful login
   */
  useEffect(() => {
    if (success) {
      if (loggedInUser || authUser) {
        navigate("/home");
      }
    }
  }, [success, navigate, authUser]);

  /**
   * Handle authentication errors and display appropriate messages
   */
  useEffect(() => {
    if (error) {
      console.error("Login error:", error);
      toast.error("Invalid email or password.");
      setSignInError("Invalid email or password.");
      signOut();
    } else if (createError || createErrorMessage) {
      console.error("Google login error:", createError || createErrorMessage);
      toast.error("Google login failed. Please try again.");
      setSignInError("Google login failed. Please try again.");
      signOut();
    }
  }, [error, createError, createErrorMessage, signOut]);

  /**
   * Update success state when authentication succeeds through any method
   */
  useEffect(() => {
    setSuccess(isLoggedIn || loggedInUser != null || createUserSuccess);
    if (isLoggedIn || loggedInUser != null || createUserSuccess) {
      toast.success("Login successful!");
    }
  }, [isLoggedIn, loggedInUser, createUserSuccess]);

  /**
   * Form validation function using Zod schema for the login form in the LoginPage.
   * @returns {boolean} - True if validation passes, false otherwise
   */
  const validate = (): boolean => {
    try {
      loginSchema.parse(formData);
      setErrors({});
      return true;
    } catch (error) {
      if (error instanceof z.ZodError) {
        const newErrors: { [key: string]: string } = {};
        error.errors.forEach((err) => {
          if (err.path) {
            newErrors[err.path[0]] = err.message;
          }
        });
        setErrors(newErrors);
      }
      return false;
    }
  };

  // Debug logging for loading state
  const __ = loading
    ? console.log("LoginPage: loading...")
    : console.log("LoginPage: not loading");

  /**
   * Handles form input changes and clears any previous sign-in errors
   * @param {React.ChangeEvent<HTMLInputElement>} e - Input change event
   */
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setSignInError("");
  };

  /**
   * Handles form submission for email/password login
   * Validates form data before submitting authentication request
   * @param {React.FormEvent} e - Form submission event
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) {
      Object.values(errors).forEach((error) => {
        toast.error(error);
      });
      return;
    }

    await loginWithEmailAndPassword(formData.email, formData.password);
  };

  /**
   * Initiates Google sign-in process
   */
  const handleGoogleSignIn = async () => {
    await signInWithGoogle();
  };

  /**
   * Update overall loading state based on individual loading states
   */
  useEffect(() => {
    setLoading(createLoading || loginLoading);
  }, [createLoading, loginLoading]);

  /**
   * Toggle password visibility
   */
  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  return (
    <div
      className="flex justify-center items-center min-h-screen"
      style={{ backgroundColor: "rgb(15 23 42 / 1)" }}
    >
      <div className="p-8 w-full h-screen sm:h-auto sm:max-w-md bg-gray-800 sm:rounded-lg sm:shadow-lg text-white">
        <div className="flex flex-col gap-6">
          {/* Login Form */}
          <form onSubmit={handleSubmit}>
            <div className="flex flex-col gap-6">
              {/* Header with Logo and Title */}
              <div className="flex flex-col items-center gap-3">
                <div className="flex h-14 w-14 items-center justify-center rounded-md bg-indigo-600">
                  <img src={LogoIcon} alt="Logo" className="h-10 w-10" />
                </div>
                <h1 className="text-2xl font-bold">Welcome Back</h1>
                <div className="text-center text-sm text-gray-400">
                  Don&apos;t have an account?{" "}
                  <Link
                    to="/signup"
                    className="text-indigo-400 underline underline-offset-4 hover:text-indigo-300"
                  >
                    Register
                  </Link>
                </div>
              </div>

              {/* Error Message Box */}
              {signInError && (
                <div className="bg-red-900/50 border border-red-500 text-red-100 px-4 py-3 rounded-md mb-2">
                  <p className="text-sm">{signInError}</p>
                </div>
              )}

              {/* Form Fields */}
              <div className="flex flex-col gap-4">
                {/* Email Field */}
                <div className="grid gap-2">
                  <Label htmlFor="email" className="text-gray-300">
                    Email
                  </Label>
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    value={formData.email}
                    onChange={handleChange}
                    placeholder="Enter your email"
                    disabled={loading}
                    className={`border-gray-700 bg-gray-700 text-white ${errors.email ? "border-red-500 focus:border-red-500" : ""
                      }`}
                    required
                  />
                  {errors.email && (
                    <p className="text-red-400 text-sm">{errors.email}</p>
                  )}
                </div>

                {/* Password Field */}
                <div className="grid gap-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="password" className="text-gray-300">
                      Password
                    </Label>
                    <Link
                      to="/forgot-password"
                      className="text-xs text-indigo-400 hover:underline"
                    >
                      Forget Password?
                    </Link>
                  </div>
                  <div className="relative">
                    <Input
                      id="password"
                      name="password"
                      type={showPassword ? "text" : "password"}
                      value={formData.password}
                      onChange={handleChange}
                      placeholder="Enter password"
                      disabled={loading}
                      className={`border-gray-700 bg-gray-700 text-white pr-10 ${errors.password ? "border-red-500 focus:border-red-500" : ""
                        }`}
                      required
                    />
                    <button
                      type="button"
                      className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-white"
                      onClick={togglePasswordVisibility}
                    >
                      {showPassword ? (
                        <EyeOffIcon className="h-5 w-5" />
                      ) : (
                        <EyeIcon className="h-5 w-5" />
                      )}
                    </button>
                  </div>
                  {errors.password && (
                    <p className="text-red-400 text-sm">{errors.password}</p>
                  )}
                </div>

                {/* Login Button */}
                <Button
                  type="submit"
                  className="w-full bg-indigo-600 hover:bg-indigo-700"
                  disabled={loading}
                >
                  {loginLoading ? "Logging in..." : "Login"}
                </Button>
              </div>

              {/* Separator Line with "Or" Text */}
              <div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-gray-700">
                <span className="relative z-10 bg-gray-800 px-2 text-gray-400">
                  Or
                </span>
              </div>

              {/* Google Sign-in Button */}
              <Button
                type="button"
                variant="outline"
                className="w-full border-gray-700 text-black hover:text-white hover:bg-gray-700"
                disabled={loading}
                onClick={handleGoogleSignIn}
              >
                <div className="flex items-center">
                  <img
                    src={GoogleLogo}
                    alt="Google Logo"
                    className="h-5 w-5 mr-2"
                  />
                  <span className="">
                    {loading
                      ? "Signing in with Google..."
                      : "Continue with Google"}
                  </span>
                </div>
              </Button>
            </div>
          </form>

          <div className="text-balance text-center text-xs text-gray-400 [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-indigo-400">
            <Link to="/home"> Continue as a guest </Link>
          </div>

          {/* Terms of Service and Privacy Policy Footer */}
          <div className="text-balance text-center text-xs text-gray-400 [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-indigo-400">
            By clicking continue, you agree to our{" "}
            <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a>.
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
