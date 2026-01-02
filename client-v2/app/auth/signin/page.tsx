"use client";

import { useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { AuthCard } from "@/components/auth/AuthCard";
import { SignInForm } from "@/components/auth/SignInForm";
import { GoogleButton } from "@/components/auth/GoogleButton";
import { useSignIn } from "@/hooks/useSignIn";
import { useSignInWithGoogle } from "@/hooks/useSignInWithGoogle";
import { useAuth } from "@/lib/firebase/AuthProvider";
import type { SignInFormData } from "@/lib/validations/auth";

/**
 * Sign In page with email/password and Google OAuth support.
 */
export default function SignInPage() {
	const router = useRouter();
	const searchParams = useSearchParams();
	const { user, loading: authLoading } = useAuth();
	const { signIn, loading: signInLoading, error: signInError } = useSignIn();
	const {
		signInWithGoogle,
		loading: googleLoading,
		error: googleError,
	} = useSignInWithGoogle();

	// Redirect if already logged in
	useEffect(() => {
		if (user && !authLoading) {
			const redirectTo = searchParams.get("redirect") || "/";
			router.push(redirectTo);
		}
	}, [user, authLoading, router, searchParams]);

	const handleSignIn = async (data: SignInFormData) => {
		const success = await signIn(data.email, data.password);
		if (success) {
			const redirectTo = searchParams.get("redirect") || "/";
			router.push(redirectTo);
		}
	};

	const handleGoogleSignIn = async () => {
		const success = await signInWithGoogle();
		if (success) {
			const redirectTo = searchParams.get("redirect") || "/";
			router.push(redirectTo);
		}
	};

	const loading = signInLoading || googleLoading;
	const error = signInError || googleError;

	// Show nothing while checking auth state
	if (authLoading) {
		return (
			<div className="flex min-h-screen items-center justify-center">
				<div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
			</div>
		);
	}

	// Don't render if already logged in (will redirect)
	if (user) {
		return null;
	}

	return (
		<AuthCard
			title="Welcome Back"
			subtitle={
				<>
					Don&apos;t have an account?{" "}
					<Link
						href="/auth/signup"
						className="text-primary underline underline-offset-4 hover:text-primary/80"
					>
						Register
					</Link>
				</>
			}
		>
			{/* Sign In Form */}
			<SignInForm
				onSubmit={handleSignIn}
				loading={loading}
				error={error}
			/>

			{/* Separator */}
			<div className="relative text-center text-sm after:absolute after:inset-0 after:top-1/2 after:z-0 after:flex after:items-center after:border-t after:border-border">
				<span className="relative z-10 bg-card px-2 text-muted-foreground">
					Or
				</span>
			</div>

			{/* Google Sign In */}
			<GoogleButton
				onClick={handleGoogleSignIn}
				loading={googleLoading}
				disabled={loading}
				text="Continue with Google"
			/>

			{/* Continue as Guest */}
			<div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 [&_a]:hover:text-primary">
				<Link href="/">Continue as a guest</Link>
			</div>

			{/* Terms Footer */}
			<div className="text-balance text-center text-xs text-muted-foreground [&_a]:underline [&_a]:underline-offset-4 [&_a]:hover:text-primary">
				By clicking continue, you agree to our{" "}
				<Link href="/privacy">Terms of Service</Link> and{" "}
				<Link href="/privacy">Privacy Policy</Link>.
			</div>
		</AuthCard>
	);
}
