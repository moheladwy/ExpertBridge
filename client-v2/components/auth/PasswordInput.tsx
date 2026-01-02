"use client";

import { forwardRef, useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { HugeiconsIcon } from "@hugeicons/react";
import { ViewIcon, ViewOffSlashIcon } from "@hugeicons/core-free-icons";
import { cn } from "@/lib/utils";

export interface PasswordInputProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	/** Error state for styling */
	error?: boolean;
}

/**
 * Password input with visibility toggle.
 *
 * @example
 * ```tsx
 * <PasswordInput
 *   placeholder="Enter password"
 *   error={!!errors.password}
 *   {...register("password")}
 * />
 * ```
 */
const PasswordInput = forwardRef<HTMLInputElement, PasswordInputProps>(
	({ className, error, disabled, ...props }, ref) => {
		const [showPassword, setShowPassword] = useState(false);

		return (
			<div className="relative">
				<Input
					ref={ref}
					type={showPassword ? "text" : "password"}
					className={cn(
						"pr-10",
						error &&
							"border-destructive focus-visible:border-destructive",
						className
					)}
					disabled={disabled}
					{...props}
				/>
				<Button
					type="button"
					variant="ghost"
					size="sm"
					className="absolute right-0 top-0 h-full px-3 py-2 hover:bg-transparent"
					onClick={() => setShowPassword(!showPassword)}
					disabled={disabled}
					aria-label={
						showPassword ? "Hide password" : "Show password"
					}
				>
					<HugeiconsIcon
						icon={showPassword ? ViewOffSlashIcon : ViewIcon}
						className="h-4 w-4 text-muted-foreground"
					/>
				</Button>
			</div>
		);
	}
);

PasswordInput.displayName = "PasswordInput";

export { PasswordInput };
