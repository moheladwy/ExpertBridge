"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PasswordInput } from "@/components/auth/PasswordInput";
import { signUpSchema, type SignUpFormData } from "@/lib/validations/auth";

interface SignUpFormProps {
	onSubmit: (data: SignUpFormData) => Promise<void>;
	loading?: boolean;
	error?: string | null;
}

/**
 * Sign up form component with all registration fields.
 *
 * @example
 * ```tsx
 * <SignUpForm
 *   onSubmit={handleSignUp}
 *   loading={isLoading}
 *   error={signUpError}
 * />
 * ```
 */
export function SignUpForm({
	onSubmit,
	loading = false,
	error,
}: SignUpFormProps) {
	const [formData, setFormData] = useState<SignUpFormData>({
		firstName: "",
		lastName: "",
		email: "",
		password: "",
		confirmPassword: "",
	});
	const [errors, setErrors] = useState<
		Partial<Record<keyof SignUpFormData, string>>
	>({});

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;
		setFormData((prev) => ({ ...prev, [name]: value }));
		// Clear field error when user types
		if (errors[name as keyof SignUpFormData]) {
			setErrors((prev) => ({ ...prev, [name]: undefined }));
		}
	};

	const validate = (): boolean => {
		const result = signUpSchema.safeParse(formData);

		if (!result.success) {
			// Build field errors using reduce - spreads create new objects
			const fieldErrors = result.error.issues.reduce<
				Partial<Record<keyof SignUpFormData, string>>
			>((acc, issue) => {
				const field = issue.path[0];
				if (
					typeof field === "string" &&
					(field === "firstName" ||
						field === "lastName" ||
						field === "email" ||
						field === "password" ||
						field === "confirmPassword") &&
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

				{/* Name Fields (side by side) */}
				<div className="grid grid-cols-2 gap-4">
					{/* First Name Field */}
					<div className="grid gap-2">
						<Label
							htmlFor="firstName"
							className="text-muted-foreground"
						>
							First Name
						</Label>
						<Input
							id="firstName"
							name="firstName"
							type="text"
							value={formData.firstName}
							onChange={handleChange}
							placeholder="First name"
							disabled={loading}
							aria-invalid={!!errors.firstName}
							autoComplete="given-name"
						/>
						{errors.firstName && (
							<p className="text-sm text-destructive">
								{errors.firstName}
							</p>
						)}
					</div>

					{/* Last Name Field */}
					<div className="grid gap-2">
						<Label
							htmlFor="lastName"
							className="text-muted-foreground"
						>
							Last Name
						</Label>
						<Input
							id="lastName"
							name="lastName"
							type="text"
							value={formData.lastName}
							onChange={handleChange}
							placeholder="Last name"
							disabled={loading}
							aria-invalid={!!errors.lastName}
							autoComplete="family-name"
						/>
						{errors.lastName && (
							<p className="text-sm text-destructive">
								{errors.lastName}
							</p>
						)}
					</div>
				</div>

				{/* Email Field */}
				<div className="grid gap-2">
					<Label htmlFor="email" className="text-muted-foreground">
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
					<Label htmlFor="password" className="text-muted-foreground">
						Create Password
					</Label>
					<PasswordInput
						id="password"
						name="password"
						value={formData.password}
						onChange={handleChange}
						placeholder="Enter password"
						disabled={loading}
						error={!!errors.password}
						autoComplete="new-password"
					/>
					{errors.password && (
						<p className="text-sm text-destructive">
							{errors.password}
						</p>
					)}
					<p className="text-xs text-muted-foreground">
						Min. 12 characters with uppercase, lowercase, number,
						and special character.
					</p>
				</div>

				{/* Confirm Password Field */}
				<div className="grid gap-2">
					<Label
						htmlFor="confirmPassword"
						className="text-muted-foreground"
					>
						Confirm Password
					</Label>
					<PasswordInput
						id="confirmPassword"
						name="confirmPassword"
						value={formData.confirmPassword}
						onChange={handleChange}
						placeholder="Confirm your password"
						disabled={loading}
						error={!!errors.confirmPassword}
						autoComplete="new-password"
					/>
					{errors.confirmPassword && (
						<p className="text-sm text-destructive">
							{errors.confirmPassword}
						</p>
					)}
				</div>

				{/* Submit Button */}
				<Button
					type="submit"
					className="w-full rounded-full"
					disabled={loading}
				>
					{loading ? "Creating Account..." : "Sign Up"}
				</Button>
			</div>
		</form>
	);
}
