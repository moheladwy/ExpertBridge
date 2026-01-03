"use client";

import { Suspense, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { AuthCard } from "@/components/auth/AuthCard";
import { SignUpForm } from "@/components/auth/SignUpForm";
import { GoogleButton } from "@/components/auth/GoogleButton";
import { useSignUp, type SignUpData } from "@/hooks/useSignUp";
import { useSignInWithGoogle } from "@/hooks/useSignInWithGoogle";
import { useAuth } from "@/lib/firebase/AuthProvider";
import { useAppLoading } from "@/components/shared/AppLoadingProvider";
import { LoadingScreen } from "@/components/shared/LoadingScreen";
import type { SignUpFormData } from "@/lib/validations/auth";
import { HugeiconsIcon } from "@hugeicons/react";
import { MailSend02Icon } from "@hugeicons/core-free-icons";

/**
 * Sign Up page with email/password and Google OAuth support.
 */
function SignUpPageContent() {
	const router = useRouter();
	const searchParams = useSearchParams();
	const { user, loading: authLoading } = useAuth();
	const {
		signUp,
		loading: signUpLoading,
		error: signUpError,
		emailSent,
	} = useSignUp();
	const {
		signInWithGoogle,
		loading: googleLoading,
		error: googleError,
	} = useSignInWithGoogle();
	const { showLoading, hideLoading } = useAppLoading();

	// Redirect if already logged in
	useEffect(() => {
		if (user && !authLoading) {
			const redirectTo = searchParams.get("redirect") || "/";
			router.push(redirectTo);
		}
	}, [user, authLoading, router, searchParams]);

	const handleSignUp = async (data: SignUpFormData) => {
		const signUpData: SignUpData = {
			firstName: data.firstName,
			lastName: data.lastName,
			email: data.email,
			password: data.password,
		};

		await signUp(signUpData);
		// emailSent state will trigger the success message
	};

	const handleGoogleSignIn = async () => {
		showLoading(); // Show branded loading screen
		const success = await signInWithGoogle();
		if (success) {
			const redirectTo = searchParams.get("redirect") || "/";
			router.push(redirectTo);
			// Loading will hide automatically via AppInitializer when profile loads
		} else {
			hideLoading(); // Hide on error
		}
	};

	// Don't render if already logged in (will redirect)
	if (user) {
		return null;
	}

	// Show email verification message (derived from emailSent, no state needed)
	if (emailSent) {
		return (
			<AuthCard
				title="Check Your Email"
				subtitle="We've sent you a verification link"
			>
				<div className="flex flex-col items-center gap-4 py-4">
					<div className="flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
						<HugeiconsIcon
							icon={MailSend02Icon}
							className="h-8 w-8 text-primary"
						/>
					</div>
					<p className="text-center text-muted-foreground">
						Please check your inbox and click the verification link
						to activate your account. Then you can sign in.
					</p>
					<Link
						href="/auth/signin"
						className="text-primary underline underline-offset-4 hover:text-primary/80"
					>
						Go to Sign In
					</Link>
				</div>
			</AuthCard>
		);
	}

	const loading = signUpLoading || googleLoading;
	const error = signUpError || googleError;

	return (
		<AuthCard
			title="Create Your Account"
			subtitle={
				<>
					Already have an account?{" "}
					<Link
						href="/auth/signin"
						className="text-primary underline underline-offset-4 hover:text-primary/80"
					>
						Sign In
					</Link>
				</>
			}
		>
			{/* Sign Up Form */}
			<SignUpForm
				onSubmit={handleSignUp}
				loading={signUpLoading}
				error={error}
			/>

			{/* Separator */}
			<div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-border">
				<span className="relative z-10 bg-card px-2 text-muted-foreground">
					Or
				</span>
			</div>

			{/* Google Sign Up */}
			<GoogleButton
				onClick={handleGoogleSignIn}
				loading={googleLoading}
				disabled={loading}
				text="Continue with Google"
			/>

			{/* Terms Footer */}
			<div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 [&_a]:hover:text-primary">
				By clicking continue, you agree to our{" "}
				<Link href="/privacy">Terms of Service</Link> and{" "}
				<Link href="/privacy">Privacy Policy</Link>.
			</div>
		</AuthCard>
	);
}
export default function SignUpPage() {
	return (
		<Suspense fallback={<LoadingScreen />}>
			<SignUpPageContent />
		</Suspense>
	);
}