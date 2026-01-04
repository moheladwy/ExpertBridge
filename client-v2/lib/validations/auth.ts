import { z } from "zod";

/**
 * Sign In form validation schema.
 * Simple validation - just needs valid email and non-empty password.
 */
export const signInSchema = z.object({
	email: z.string().email("Please enter a valid email address"),
	password: z.string().min(1, "Password is required"),
});

export type SignInFormData = z.infer<typeof signInSchema>;

/**
 * Sign Up form validation schema.
 * Strict validation matching backend requirements.
 */
export const signUpSchema = z
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

export type SignUpFormData = z.infer<typeof signUpSchema>;

/**
 * Firebase error codes mapped to user-friendly messages.
 */
export const firebaseErrorMessages: Record<string, string> = {
	"auth/email-already-in-use": "An account with this email already exists",
	"auth/invalid-email": "Please enter a valid email address",
	"auth/user-disabled": "This account has been disabled",
	"auth/user-not-found": "No account found with this email",
	"auth/wrong-password": "Incorrect password",
	"auth/invalid-credential": "Invalid email or password",
	"auth/too-many-requests": "Too many attempts. Please try again later",
	"auth/network-request-failed":
		"Network error. Please check your connection",
	"auth/popup-closed-by-user": "Sign-in cancelled",
	"auth/popup-blocked": "Please allow popups for this site",
	"auth/requires-recent-login": "Please sign in again to continue",
};

/**
 * Get user-friendly error message from Firebase error.
 */
export function getFirebaseErrorMessage(error: unknown): string {
	if (error instanceof Error) {
		// Firebase errors have a 'code' property
		const code = (error as { code?: string }).code;
		if (code && code in firebaseErrorMessages) {
			return firebaseErrorMessages[code];
		}
		return error.message;
	}
	return "An unexpected error occurred. Please try again.";
}
