"use client";

import { type ReactNode } from "react";
import Link from "next/link";
import { cn } from "@/lib/utils";

interface AuthCardProps {
	children: ReactNode;
	title: string;
	subtitle?: ReactNode;
	className?: string;
}

/**
 * Wrapper component for auth pages (Sign In, Sign Up, etc.)
 * Provides consistent styling with logo, title, and card layout.
 *
 * @example
 * ```tsx
 * <AuthCard
 *   title="Welcome Back"
 *   subtitle={<>Don't have an account? <Link href="/auth/signup">Register</Link></>}
 * >
 *   <SignInForm />
 * </AuthCard>
 * ```
 */
export function AuthCard({
	children,
	title,
	subtitle,
	className,
}: AuthCardProps) {
	return (
		<div className="flex min-h-screen items-center justify-center bg-background p-4">
			<div
				className={cn(
					"w-full max-w-md rounded-lg border border-border bg-card p-8 shadow-lg",
					"sm:max-w-md",
					className
				)}
			>
				<div className="flex flex-col gap-6">
					{/* Header with Logo and Title */}
					<div className="flex flex-col items-center gap-3">
						<Link
							href="/"
							className="flex h-14 w-14 items-center justify-center rounded-md bg-primary"
						>
							<svg
								className="h-10 w-10 text-primary-foreground"
								viewBox="0 0 1080 1080"
								fill="currentColor"
							>
								<path d="M956.8,432.5L1069.2,9.2L646.6,123.5L543.3,20.6L226.7,338.8l-103.5-103L10.8,659l422.6-114.2l103.2,102.8l-112.4,423.3L847,956.5l-103.5-103l316.6-318.1L956.8,432.5z M646.6,123.7L750,226.6l-316.5,318L330,441.7L646.6,123.7z M536.7,647.6l316.6-318.1l103.4,103L640.1,750.5L536.7,647.6z" />
							</svg>
						</Link>
						<h1 className="text-2xl font-bold">{title}</h1>
						{subtitle && (
							<div className="text-center text-sm text-muted-foreground">
								{subtitle}
							</div>
						)}
					</div>

					{/* Content */}
					{children}
				</div>
			</div>
		</div>
	);
}
