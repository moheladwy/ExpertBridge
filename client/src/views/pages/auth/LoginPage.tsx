import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import useSignInWithEmailAndPassword from "@/lib/firebase/EmailAuth/useSignInWithEmailAndPassword";
import { auth } from "@/lib/firebase";
import { useSignInWithGoogle } from "@/lib/firebase/useSignInWithPopup";
import GoogleLogo from "@/assets/Login-SignupAssets/Google-Logo.svg";
import { useCreateUser } from "@/features/auth/useCreateUser";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { GalleryVerticalEnd } from "lucide-react";
import LogoIcon from "@/assets/Logo-Icon/Logo-Icon.svg";
import { z } from "zod";

// Zod schema for form validation
const loginSchema = z.object({
  email: z.string().email("Please enter a valid email address"),
  password: z.string()
    .min(12, "Password must be at least 12 characters")
    .max(4096, "Password must be at most 4096 characters")
    .regex(/[A-Z]/, "Password must contain at least one uppercase letter")
    .regex(/[a-z]/, "Password must contain at least one lowercase letter")
    .regex(/[0-9]/, "Password must contain at least one number")
    .regex(/[^A-Za-z0-9]/, "Password must contain at least one special character")
});
type LoginFormData = z.infer<typeof loginSchema>;

const LoginPage: React.FC = () => {
  const [isLoggedIn, isLoggedInLoading, isLoggedInError] = useIsUserLoggedIn();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (success) {
      navigate('/home');
    }
  }, [success, navigate]);


  // Email/Password Login Hook
  const [
        loginWithEmailAndPassword, 
        loggedInUser, 
        loginLoading, 
        error] = useSignInWithEmailAndPassword(auth);

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

  // Form State
  const [formData, setFormData] = useState<LoginFormData>({ email: "", password: "" });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  const [signInError, setSignInError] = useState<string>("");

  useEffect(() => {
    if (error) {
      console.error("Login error:", error);
      toast.error("Invalid email or password.");
      setSignInError("Invalid email or password.");
    } else if (createError || createErrorMessage) {
      console.error("Google login error:", createError || createErrorMessage);
      toast.error("Google login failed. Please try again.");
      setSignInError("Google login failed. Please try again.");
    }
  }, [error, createError, createErrorMessage]);

  useEffect(() => {
    setSuccess(isLoggedIn || loggedInUser != null || createUserSuccess);
    if (isLoggedIn || loggedInUser != null || createUserSuccess) {
      toast.success("Login successful!");
    }
  }, [isLoggedIn, loggedInUser, createUserSuccess]);

  // Form Validation using Zod
  const validate = () => {
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

  const __ = loading
    ? console.log('LoginPage: loading...')
    : console.log('LoginPage: not loading');
  
  // Handle Input Changes
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setSignInError("");
  };

  // Handle Form Submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) {
      Object.values(errors).forEach(error => {
        toast.error(error);
      });
      return;
    }
    await loginWithEmailAndPassword(formData.email, formData.password);
  };

  const handleGoogleSignIn = async () => {
    await signInWithGoogle();
  }

  useEffect(() => {
    setLoading(isLoggedInLoading || createLoading || loginLoading);
  }, [isLoggedInLoading, createLoading, loginLoading]);

  return (
    <div className="flex justify-center items-center min-h-screen" style={{ backgroundColor: "rgb(15 23 42 / 1)" }}>
      <div className="p-8 w-full max-w-md bg-gray-800 rounded-lg shadow-lg text-white">
        <div className="flex flex-col gap-6">
          <form onSubmit={handleSubmit}>
            <div className="flex flex-col gap-6">
              <div className="flex flex-col items-center gap-3">
                <div className="flex h-14 w-14 items-center justify-center rounded-md bg-indigo-600">
                  <img src={LogoIcon} alt="Logo" className="h-10 w-10" />
                </div>
                <h1 className="text-2xl font-bold">Welcome Back</h1>
                <div className="text-center text-sm text-gray-400">
                  Don&apos;t have an account?{" "}
                  <Link to="/signup" className="text-indigo-400 underline underline-offset-4 hover:text-indigo-300">
                    Register
                  </Link>
                </div>
              </div>
              
              <div className="flex flex-col gap-4">
                <div className="grid gap-2">
                  <Label htmlFor="email" className="text-gray-300">Email</Label>
                  <Input
                    id="email"
                    name="email"
                    type="email"
                    value={formData.email}
                    onChange={handleChange}
                    placeholder="Enter your email"
                    disabled={loading}
                    className="border-gray-700 bg-gray-700 text-white"
                    required
                  />
                  {errors.email && <p className="text-red-400 text-sm">{errors.email}</p>}
                </div>
                
                <div className="grid gap-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="password" className="text-gray-300">Password</Label>
                    <Link to="/forgot-password" className="text-xs text-indigo-400 hover:underline">
                      Forget Password?
                    </Link>
                  </div>
                  <Input
                    id="password"
                    name="password"
                    type="password"
                    value={formData.password}
                    onChange={handleChange}
                    placeholder="Enter password"
                    disabled={loading}
                    className="border-gray-700 bg-gray-700 text-white"
                    required
                  />
                  {errors.password && <p className="text-red-400 text-sm">{errors.password}</p>}
                </div>
                
                <Button 
                  type="submit" 
                  className="w-full bg-indigo-600 hover:bg-indigo-700" 
                  disabled={loading}
                >
                  {loginLoading ? "Logging in..." : "Login"}
                </Button>
              </div>
              
              <div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-gray-700">
                <span className="relative z-10 bg-gray-800 px-2 text-gray-400">
                  Or
                </span>
              </div>
              
              <Button 
                type="button" 
                variant="outline" 
                className="w-full border-gray-700 text-white hover:bg-gray-700" 
                disabled={createLoading || loading}
                onClick={handleGoogleSignIn}
              >
                <div className="flex items-center text-black">
                  <svg width="20" height="20" viewBox="0 0 24 24" className="mr-2">
                    <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" />
                    <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" />
                    <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05" />
                    <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" />
                  </svg>
                  {createLoading ? "Signing in with Google..." : "Continue with Google"}
                </div>
              </Button>
            </div>
          </form>
          
          <div className="text-balance text-center text-xs text-gray-400 [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-indigo-400">
            By clicking continue, you agree to our <a href="#">Terms of Service</a>{" "}
            and <a href="#">Privacy Policy</a>.
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
