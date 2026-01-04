import type { Metadata } from "next";

export const metadata: Metadata = {
	title: "Authentication - ExpertBridge",
	description: "Sign in or create an account on ExpertBridge",
};

/**
 * Auth layout - no header/footer for clean auth experience.
 * All auth pages (signin, signup, forgot-password, verify-email) use this layout.
 */
export default function AuthLayout({
	children,
}: {
	children: React.ReactNode;
}) {
	return <>{children}</>;
}
