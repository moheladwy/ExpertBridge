"use client";

import { useState, useCallback } from "react";
import {
	createUserWithEmailAndPassword,
	sendEmailVerification,
} from "firebase/auth";
import { auth } from "@/lib/firebase";
import { getFirebaseErrorMessage } from "@/lib/validations/auth";
import { useCreateOrUpdateUserMutation } from "@/features/profiles/profilesSlice";

export interface SignUpData {
	firstName: string;
	lastName: string;
	email: string;
	password: string;
}

interface UseSignUpReturn {
	signUp: (data: SignUpData) => Promise<boolean>;
	loading: boolean;
	error: string | null;
	clearError: () => void;
	emailSent: boolean;
}

/**
 * Hook for email/password sign up with backend user creation.
 *
 * Handles:
 * - Firebase user creation
 * - Email verification sending
 * - Backend user creation
 * - Rollback on backend failure
 *
 * @example
 * ```tsx
 * const { signUp, loading, error, emailSent } = useSignUp();
 *
 * const handleSubmit = async (data) => {
 *   const success = await signUp(data);
 *   if (success) {
 *     // Show "check your email" message
 *   }
 * };
 * ```
 */
export function useSignUp(): UseSignUpReturn {
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [emailSent, setEmailSent] = useState(false);
	const [createOrUpdateUser] = useCreateOrUpdateUserMutation();

	const clearError = useCallback(() => {
		setError(null);
	}, []);

	const signUp = useCallback(
		async (data: SignUpData): Promise<boolean> => {
			setLoading(true);
			setError(null);
			setEmailSent(false);

			try {
				// Create Firebase user
				const userCredential = await createUserWithEmailAndPassword(
					auth,
					data.email,
					data.password
				);
				const user = userCredential.user;

				// Send email verification
				try {
					await sendEmailVerification(user);
					setEmailSent(true);
				} catch (emailError) {
					console.warn(
						"Failed to send verification email:",
						emailError
					);
					// Continue anyway - user can request resend later
				}

				// Create user in backend
				try {
					await createOrUpdateUser({
						firstName: data.firstName,
						lastName: data.lastName,
						email: data.email,
						username: data.email,
						providerId: user.uid,
						isEmailVerified: user.emailVerified,
					}).unwrap();
				} catch (backendError) {
					// Rollback: delete Firebase user if backend fails
					console.error(
						"Backend user creation failed:",
						backendError
					);
					try {
						await user.delete();
					} catch (deleteError) {
						console.error(
							"Failed to rollback Firebase user:",
							deleteError
						);
					}
					await auth.signOut();
					setError(
						"Failed to create your account. Please try again."
					);
					return false;
				}

				// Sign out - user needs to verify email first
				await auth.signOut();

				return true;
			} catch (err) {
				setError(getFirebaseErrorMessage(err));
				return false;
			} finally {
				setLoading(false);
			}
		},
		[createOrUpdateUser]
	);

	return { signUp, loading, error, clearError, emailSent };
}
