/**
 * @file SignUpPage.tsx
 * @description User registration page that allows sign up with email/password or Google OAuth.
 * Features comprehensive form validation using Zod schema with specific requirements for names,
 * email, and passwords. Provides a consistent UI matching the login page, with proper error
 * handling and user feedback through toast notifications.
 */
import React, { JSX, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { auth } from "@/lib/firebase";
import GoogleLogo from "@/assets/Login-SignupAssets/Google-Logo.svg";
import { useCreateUser } from "@/hooks/useCreateUser";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import LogoIcon from "@/assets/Logo-Icon/Logo-Icon.svg";
import { z } from "zod";
import { toast } from "sonner";
import { Button } from "@/views/components/ui/button";
import { Label } from "@/views/components/ui/label";
import { Input } from "@/views/components/ui/input";

/**
 * Zod schema for signup form validation
 *
 * Validation rules:
 * - First and last name: 3-256 characters, letters only
 * - Email: Valid email format
 * - Password: 12-4096 characters with requirements for uppercase, lowercase, numbers, and special characters
 * - Password confirmation must match the password
 */
const signupSchema = z
	.object({
		firstName: z
			.string()
			.trim()
			.min(3, "First name must be at least 3 characters")
			.max(256, "First name must be at most 256 characters")
			.regex(
				/^[\p{Script=Latin}\p{Script=Arabic}]+$/u,
				"First name must contain only English or Arabic letters"
			),
		lastName: z
			.string()
			.trim()
			.min(3, "Last name must be at least 3 characters")
			.max(256, "Last name must be at most 256 characters")
			.regex(
				/^[\p{Script=Latin}\p{Script=Arabic}]+$/u,
				"Last name must contain only English or Arabic letters"
			),
		email: z.string().email("Please enter a valid email address"),
		password: z
			.string()
			.min(12, "Password must be at least 12 characters")
			.max(4096, "Password must be at most 4096 characters")
			.regex(
				/[A-Z]/,
				"Password must contain at least one uppercase letter"
			)
			.regex(
				/[a-z]/,
				"Password must contain at least one lowercase letter"
			)
			.regex(/[0-9]/, "Password must contain at least one number")
			.regex(
				/[^A-Za-z0-9]/,
				"Password must contain at least one special character"
			),
		confirmPassword: z.string().min(1, "Please confirm your password"),
	})
	.refine((data) => data.password === data.confirmPassword, {
		message: "Passwords do not match",
		path: ["confirmPassword"],
	});

/**
 * Type definition for signup form data inferred from Zod schema
 * Contains all form fields with their respective types
 */
type SignupFormData = z.infer<typeof signupSchema>;

/**
 * SignUpPage Component
 *
 * A comprehensive registration page that allows users to create accounts using
 * email/password or Google OAuth. Features form validation, error handling,
 * and a consistent UI that matches the login page.
 *
 * @returns {JSX.Element} The SignUpPage component
 */
const SignUpPage: React.FC = (): JSX.Element => {
	// Authentication state hook to check if user is already logged in
	const [isLoggedIn, isLoggedInLoading, isLoggedInError] =
		useIsUserLoggedIn();
	const navigate = useNavigate();

	// Component state
	const [loading, setLoading] = useState(false);
	const [success, setSuccess] = useState(false);
	// Add these state variables for password visibility
	const [showPassword, setShowPassword] = useState(false);
	const [showConfirmPassword, setShowConfirmPassword] = useState(false);
	const [showErrorMessage, setShowErrorMessage] = useState<boolean>(false);

	/**
	 * Hook for user creation and authentication
	 * Provides methods for email/password and Google authentication
	 */
	const [
		signInWithGoogle,
		signUpWithEmailAndPassword,
		authUser,
		createdAppUser,
		createUserLoading,
		authError,
		createUserErrorMessage,
		createUserSuccess,
	] = useCreateUser(auth);

	/**
	 * Form state management
	 * Tracks form field values, validation errors, and submission errors
	 */
	const [formData, setFormData] = useState<SignupFormData>({
		firstName: "",
		lastName: "",
		email: "",
		password: "",
		confirmPassword: "",
	});
	const [errors, setErrors] = useState<{ [key: string]: string }>({});
	const [signUpError, setSignUpError] = useState<string>("");

	/**
	 * Redirects to home page when sign up is successful
	 */
	useEffect(() => {
		if (authUser) {
			if (authUser && authUser.user.emailVerified) {
				navigate("/home");
			} else {
				navigate("/verify-email");
			}
		}
	}, [success, navigate, authUser]);

	/**
	 * Updates success state based on authentication status
	 * Shows success toast when account is created
	 */
	useEffect(() => {
		setSuccess(createUserSuccess || isLoggedIn);
		if (createUserSuccess) {
			toast.success(
				"Account created successfully! Redirecting to login..."
			);
		}
	}, [createUserSuccess, isLoggedIn]);

	/**
	 * Handles user creation errors from the API
	 * Maps error messages to user-friendly notifications
	 */
	useEffect(() => {
		if (createUserErrorMessage) {
			const errorMessage =
				"An error occurred while creating your account. Please try again.";
			setSignUpError(errorMessage);
			setShowErrorMessage(true); // Show the error box
			console.log(createUserErrorMessage);

			// Map Firebase error messages to user-friendly messages
			let userFriendlyMessage = errorMessage;
			if (createUserErrorMessage.includes("email-already-in-use")) {
				userFriendlyMessage =
					"This email is already registered. Please use a different email or try logging in.";
			} else if (createUserErrorMessage.includes("invalid-email")) {
				userFriendlyMessage = "Please provide a valid email address.";
			} else if (createUserErrorMessage.includes("weak-password")) {
				userFriendlyMessage =
					"Your password is too weak. Please choose a stronger password.";
			} else if (
				createUserErrorMessage.includes("network-request-failed")
			) {
				userFriendlyMessage =
					"Network error. Please check your internet connection and try again.";
			}

			setSignUpError(userFriendlyMessage);
			toast.error(userFriendlyMessage);
		}
	}, [createUserErrorMessage]);

	/**
	 * Handles Firebase authentication errors
	 * Provides specific error messages based on error codes
	 */
	useEffect(() => {
		if (authError) {
			console.error("Sign-up error:", authError);
			const errorMessage = "Sign-up failed. Please try again.";
			setSignUpError(errorMessage);
			setShowErrorMessage(true); // Show the error box

			// Extract error code and provide user-friendly message
			let userFriendlyMessage = errorMessage;
			if (authError.code) {
				switch (authError.code) {
					case "auth/email-already-in-use":
						userFriendlyMessage =
							"This email is already registered. Please use a different email or try logging in.";
						break;
					case "auth/invalid-email":
						userFriendlyMessage =
							"Please provide a valid email address.";
						break;
					case "auth/operation-not-allowed":
						userFriendlyMessage =
							"Account creation is currently disabled. Please try again later.";
						break;
					case "auth/weak-password":
						userFriendlyMessage =
							"Your password is too weak. Please choose a stronger password.";
						break;
					case "auth/network-request-failed":
						userFriendlyMessage =
							"Network error. Please check your internet connection and try again.";
						break;
					default:
						userFriendlyMessage =
							"An error occurred during sign-up. Please try again.";
				}
			}

			setSignUpError(userFriendlyMessage);
			toast.error(userFriendlyMessage);
		}
	}, [authError]);

	/**
	 * Handles errors related to checking the login status
	 */
	useEffect(() => {
		if (isLoggedInError) {
			console.error("Auth status error:", isLoggedInError);
			toast.error(
				"There was a problem checking your login status. Please refresh the page."
			);
		}
	}, [isLoggedInError]);

	/**
	 * Form validation function using Zod schema for the signup form
	 * Validates all form fields based on the schema and updates error state
	 *
	 * @returns {boolean} True if validation passes, false otherwise
	 */
	const validate = (): boolean => {
		try {
			// Parse form data with Zod schema
			signupSchema.parse(formData);
			setErrors({});
			return true;
		} catch (error) {
			if (error instanceof z.ZodError) {
				// Extract validation errors and update error state
				const newErrors: { [key: string]: string } = {};
				error.errors.forEach((err) => {
					if (err.path) {
						newErrors[err.path[0]] = err.message;
					}
				});
				setErrors(newErrors);
				// Show only the first error as a toast to avoid overwhelming the user
				if (error.errors.length > 0) {
					toast.error(error.errors[0].message);
				}
			}
			return false;
		}
	};

	/**
	 * Handles input field changes and clears previous errors
	 *
	 * @param {React.ChangeEvent<HTMLInputElement>} e - Input change event
	 */
	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		setFormData({ ...formData, [e.target.name]: e.target.value });
		setSignUpError("");
		setShowErrorMessage(false);
	};

	/**
	 * Handles form submission for email/password registration
	 * Validates form and creates user account if validation passes
	 *
	 * @param {React.FormEvent} e - Form submission event
	 */
	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		const isValid = validate();
		if (!isValid) {
			// Individual errors are shown by the validate function
			return;
		}
		try {
			const user = {
				firstName: formData.firstName,
				lastName: formData.lastName,
				email: formData.email,
			};
			await signUpWithEmailAndPassword(
				formData.email,
				formData.password,
				user
			);
		} catch (error) {
			console.error("Unexpected error during sign up:", error);
			toast.error(
				"An unexpected error occurred. Please try again later."
			);
		}
	};

	/**
	 * Handles Google sign-in/registration
	 * Initiates Google OAuth flow
	 */
	const handleGoogleSignIn = async () => {
		try {
			await signInWithGoogle();
		} catch (error) {
			console.error("Google sign-in error:", error);
			toast.error("Failed to sign in with Google. Please try again.");
		}
	};

	/**
	 * Handles closing the error message box
	 * Sets the showErrorMessage state to false to hide the error message
	 */
	const handleCloseErrorMessage = () => {
		setShowErrorMessage(false);
	};

	/**
	 * Updates loading state based on authentication processes
	 */
	useEffect(() => {
		setLoading(createUserLoading);
	}, [createUserLoading]);

	return (
		<div
			className="flex justify-center items-center min-h-screen"
			style={{ backgroundColor: "rgb(15 23 42 / 1)" }}
		>
			<div className="p-8 w-full h-screen sm:h-auto sm:max-w-md bg-gray-800 sm:rounded-lg sm:shadow-lg text-white">
				<div className="flex flex-col gap-6">
					{/* Sign-Up Form */}
					<form onSubmit={handleSubmit}>
						<div className="flex flex-col gap-6">
							{/* Header with Logo and Title */}
							<div className="flex flex-col items-center gap-3">
								{/* Logo and title content remains the same */}
								<div className="flex h-14 w-14 items-center justify-center rounded-md bg-primary">
									<img
										src={LogoIcon}
										alt="Logo"
										className="h-10 w-10"
									/>
								</div>
								<h1 className="text-2xl font-bold">
									Create Your Account
								</h1>
								<div className="text-center text-sm text-muted-foreground">
									Already have an account?{" "}
									<Link
										to="/login"
										className="text-primary underline underline-offset-4 hover:text-primary/80"
									>
										Login
									</Link>
								</div>
							</div>

							{/* Error Message Box */}
							{showErrorMessage && signUpError && (
								<div className="bg-red-600 text-white p-4 rounded-md flex justify-between items-center">
									<span className="text-sm">
										{signUpError}
									</span>
									<button
										type="button"
										onClick={handleCloseErrorMessage}
										className="ml-2 text-white hover:text-gray-200 focus:outline-none"
										aria-label="Close error message"
									>
										<svg
											xmlns="http://www.w3.org/2000/svg"
											className="h-5 w-5"
											viewBox="0 0 20 20"
											fill="currentColor"
										>
											<path
												fillRule="evenodd"
												d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
												clipRule="evenodd"
											/>
										</svg>
									</button>
								</div>
							)}

							{/* Form Fields */}
							<div className="flex flex-col gap-4">
								{/* Name Fields (side by side) */}
								<div className="grid grid-cols-2 gap-4">
									{/* First Name Field */}
									<div className="grid gap-2">
										<Label
											htmlFor="firstName"
											className="text-gray-300"
										>
											First Name
										</Label>
										<Input
											id="firstName"
											name="firstName"
											type="text"
											value={formData.firstName}
											onChange={handleChange}
											placeholder="Enter your first name"
											disabled={loading}
											className="border-gray-700 bg-gray-700 text-white"
											required
										/>
									</div>

									{/* Last Name Field */}
									<div className="grid gap-2">
										<Label
											htmlFor="lastName"
											className="text-gray-300"
										>
											Last Name
										</Label>
										<Input
											id="lastName"
											name="lastName"
											type="text"
											value={formData.lastName}
											onChange={handleChange}
											placeholder="Enter your last name"
											disabled={loading}
											className="border-gray-700 bg-gray-700 text-white"
											required
										/>
									</div>

									{/* Error messages for first and last name */}
									<div className="grid grid-cols-2 gap-4 col-span-2 mt-1">
										<div>
											{errors.firstName && (
												<p className="text-red-400 text-sm">
													{errors.firstName}
												</p>
											)}
										</div>
										<div>
											{errors.lastName && (
												<p className="text-red-400 text-sm">
													{errors.lastName}
												</p>
											)}
										</div>
									</div>
								</div>

								{/* Email Field */}
								<div className="grid gap-2">
									<Label
										htmlFor="email"
										className="text-gray-300"
									>
										Email Address
									</Label>
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
									{errors.email && (
										<p className="text-red-400 text-sm">
											{errors.email}
										</p>
									)}
								</div>

								{/* Password Field */}
								<div className="grid gap-2">
									<Label
										htmlFor="password"
										className="text-gray-300"
									>
										Create Password
									</Label>
									<div className="relative">
										<Input
											id="password"
											name="password"
											type={
												showPassword
													? "text"
													: "password"
											}
											value={formData.password}
											onChange={handleChange}
											placeholder="Enter password"
											disabled={loading}
											className="border-gray-700 bg-gray-700 text-white pr-10"
											required
										/>
										<button
											type="button"
											className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-white"
											onClick={() =>
												setShowPassword(!showPassword)
											}
											aria-label={
												showPassword
													? "Hide password"
													: "Show password"
											}
										>
											{showPassword ? (
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="20"
													height="20"
													viewBox="0 0 24 24"
													fill="none"
													stroke="currentColor"
													strokeWidth="2"
													strokeLinecap="round"
													strokeLinejoin="round"
												>
													<path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"></path>
													<path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"></path>
													<path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"></path>
													<line
														x1="2"
														x2="22"
														y1="2"
														y2="22"
													></line>
												</svg>
											) : (
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="20"
													height="20"
													viewBox="0 0 24 24"
													fill="none"
													stroke="currentColor"
													strokeWidth="2"
													strokeLinecap="round"
													strokeLinejoin="round"
												>
													<path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path>
													<circle
														cx="12"
														cy="12"
														r="3"
													></circle>
												</svg>
											)}
										</button>
									</div>
									{errors.password && (
										<p className="text-red-400 text-sm">
											{errors.password}
										</p>
									)}
								</div>

								{/* Confirm Password Field */}
								<div className="grid gap-2">
									<Label
										htmlFor="confirmPassword"
										className="text-gray-300"
									>
										Confirm Password
									</Label>
									<div className="relative">
										<Input
											id="confirmPassword"
											name="confirmPassword"
											type={
												showConfirmPassword
													? "text"
													: "password"
											}
											value={formData.confirmPassword}
											onChange={handleChange}
											placeholder="Confirm your password"
											disabled={loading}
											className="border-gray-700 bg-gray-700 text-white pr-10"
											required
										/>
										<button
											type="button"
											className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-white"
											onClick={() =>
												setShowConfirmPassword(
													!showConfirmPassword
												)
											}
											aria-label={
												showConfirmPassword
													? "Hide password"
													: "Show password"
											}
										>
											{showConfirmPassword ? (
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="20"
													height="20"
													viewBox="0 0 24 24"
													fill="none"
													stroke="currentColor"
													strokeWidth="2"
													strokeLinecap="round"
													strokeLinejoin="round"
												>
													<path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"></path>
													<path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"></path>
													<path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"></path>
													<line
														x1="2"
														x2="22"
														y1="2"
														y2="22"
													></line>
												</svg>
											) : (
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="20"
													height="20"
													viewBox="0 0 24 24"
													fill="none"
													stroke="currentColor"
													strokeWidth="2"
													strokeLinecap="round"
													strokeLinejoin="round"
												>
													<path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"></path>
													<circle
														cx="12"
														cy="12"
														r="3"
													></circle>
												</svg>
											)}
										</button>
									</div>
									{errors.confirmPassword && (
										<p className="text-red-400 text-sm">
											{errors.confirmPassword}
										</p>
									)}
								</div>

								{/* Sign-Up Button */}
								<Button
									type="submit"
									className="w-full bg-primary hover:bg-primary/90 text-primary-foreground"
									disabled={loading}
								>
									{createUserLoading
										? "Signing Up..."
										: "Sign Up"}
								</Button>
							</div>

							{/* Separator Line with "Or" Text */}
							<div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-gray-700">
								<span className="relative z-10 bg-gray-800 px-2 text-gray-400">
									Or
								</span>
							</div>

							{/* Google Sign-Up Button */}
							<Button
								type="button"
								variant="outline"
								className="w-full border-gray-700 text-black hover:text-white dark:hover:text-white dark:bg-white dark:hover:bg-gray-700 hover:bg-gray-700"
								disabled={loading}
								onClick={handleGoogleSignIn}
							>
								<div className="flex items-center">
									<img
										src={GoogleLogo}
										alt="Google Logo"
										className="h-5 w-5 mr-2"
									/>
									{createUserLoading
										? "Signing up with Google..."
										: "Continue with Google"}
								</div>
							</Button>
						</div>
					</form>

					{/* Terms of Service and Privacy Policy Footer */}
					<div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 hover:[&_a]:text-primary">
						By clicking continue, you agree to our{" "}
						<Link to="/privacy-policy">Terms of Service</Link> and{" "}
						<Link to="/privacy-policy">Privacy Policy</Link>.
					</div>
				</div>
			</div>
		</div>
	);
};

export default SignUpPage;
