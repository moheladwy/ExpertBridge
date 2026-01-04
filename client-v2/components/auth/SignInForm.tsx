"use client";

import { useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PasswordInput } from "@/components/auth/PasswordInput";
import { signInSchema, type SignInFormData } from "@/lib/validations/auth";

interface SignInFormProps {
	onSubmit: (data: SignInFormData) => Promise<void>;
	loading?: boolean;
	error?: string | null;
}

/**
 * Sign in form component with email and password fields.
 *
 * @example
 * ```tsx
 * <SignInForm
 *   onSubmit={handleSignIn}
 *   loading={isLoading}
 *   error={signInError}
 * />
 * ```
 */
export function SignInForm({
	onSubmit,
	loading = false,
	error,
}: SignInFormProps) {
	const [formData, setFormData] = useState<SignInFormData>({
		email: "",
		password: "",
	});
	const [errors, setErrors] = useState<
		Partial<Record<keyof SignInFormData, string>>
	>({});

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		setFormData((prev) => ({ ...prev, [name]: value }));
		// Clear field error when user types
		if (errors[name as keyof SignInFormData]) {
			setErrors((prev) => ({ ...prev, [name]: undefined }));
		}
	};

	const validate = (): boolean => {
		const result = signInSchema.safeParse(formData);

		if (!result.success) {
			// Build field errors using reduce - spreads create new objects
			const fieldErrors = result.error.issues.reduce<
				Partial<Record<keyof SignInFormData, string>>
			>((acc, issue) => {
				const field = issue.path[0];
				if (
					typeof field === "string" &&
					(field === "email" || field === "password") &&
					!Object.prototype.hasOwnProperty.call(acc, field)
				) {
					return { ...acc, [field]: issue.message };
				}
				return acc;
			}, {});
			setErrors(fieldErrors);
			return false;
		}

		setErrors({});
		return true;
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!validate()) return;

		await onSubmit(formData);
	};

	const onFormSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		void handleSubmit(e);
	};

	return (
		<form onSubmit={onFormSubmit}>
			<div className="flex flex-col gap-4">
				{/* Error Message Box */}
				{error && (
					<div className="rounded-md border border-destructive/20 bg-destructive/10 px-4 py-3 text-destructive">
						<p className="text-sm">{error}</p>
					</div>
				)}

				{/* Email Field */}
				<div className="grid gap-2">
					<Label htmlFor="email" className="text-muted-foreground">
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
						aria-invalid={!!errors.email}
						autoComplete="email"
					/>
					{errors.email && (
						<p className="text-sm text-destructive">
							{errors.email}
						</p>
					)}
				</div>

				{/* Password Field */}
				<div className="grid gap-2">
					<div className="flex items-center justify-between">
						<Label
							htmlFor="password"
							className="text-muted-foreground"
						>
							Password
						</Label>
						<Link
							href="/auth/forgot-password"
							className="text-xs text-primary hover:text-primary/80 hover:underline"
						>
							Forgot Password?
						</Link>
					</div>
					<PasswordInput
						id="password"
						name="password"
						value={formData.password}
						onChange={handleChange}
						placeholder="Enter password"
						disabled={loading}
						error={!!errors.password}
						autoComplete="current-password"
					/>
					{errors.password && (
						<p className="text-sm text-destructive">
							{errors.password}
						</p>
					)}
				</div>

				{/* Submit Button */}
				<Button
					type="submit"
					className="w-full rounded-full"
					disabled={loading}
				>
					{loading ? "Signing in..." : "Sign In"}
				</Button>
			</div>
		</form>
	);
}
