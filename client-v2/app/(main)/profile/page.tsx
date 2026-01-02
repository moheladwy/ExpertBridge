"use client";

import { ProtectedRoute } from "@/components/shared/ProtectedRoute";
import { useAuth } from "@/lib/firebase/AuthProvider";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { Button } from "@/components/ui/button";
import { useSignOut } from "@/hooks/useSignOut";
import { useRouter } from "next/navigation";

/**
 * Protected profile page - only accessible to authenticated users.
 * Used to test the auth flow and protected route functionality.
 */
export default function ProfilePage() {
	return (
		<ProtectedRoute>
			<ProfileContent />
		</ProtectedRoute>
	);
}

function ProfileContent() {
	const router = useRouter();
	const { user } = useAuth();
	const { signOut, loading: signOutLoading } = useSignOut();
	const {
		data: profile,
		isLoading: profileLoading,
		error: profileError,
	} = useGetCurrentUserProfileQuery();

	const handleSignOut = async () => {
		const success = await signOut();
		if (success) {
			router.push("/");
		}
	};

	return (
		<main className="container mx-auto min-h-[60vh] px-4 py-8">
			<div className="mx-auto max-w-2xl">
				<h1 className="mb-6 text-3xl font-bold">My Profile</h1>

				{/* Firebase User Info */}
				<div className="mb-6 rounded-lg border border-border bg-card p-6">
					<h2 className="mb-4 text-xl font-semibold">
						Firebase Account
					</h2>
					<div className="space-y-2 text-sm">
						<p>
							<span className="text-muted-foreground">
								Email:{" "}
							</span>
							{user?.email}
						</p>
						<p>
							<span className="text-muted-foreground">UID: </span>
							<code className="rounded bg-muted px-1 py-0.5 text-xs">
								{user?.uid}
							</code>
						</p>
						<p>
							<span className="text-muted-foreground">
								Email Verified:{" "}
							</span>
							{user?.emailVerified ? (
								<span className="text-green-600">Yes ✓</span>
							) : (
								<span className="text-amber-600">No ✗</span>
							)}
						</p>
						<p>
							<span className="text-muted-foreground">
								Display Name:{" "}
							</span>
							{user?.displayName || "Not set"}
						</p>
					</div>
				</div>

				{/* Backend Profile Info */}
				<div className="mb-6 rounded-lg border border-border bg-card p-6">
					<h2 className="mb-4 text-xl font-semibold">
						Backend Profile
					</h2>
					{profileLoading ? (
						<div className="flex items-center gap-2">
							<div className="h-4 w-4 animate-spin rounded-full border-2 border-primary border-t-transparent" />
							<span className="text-muted-foreground">
								Loading...
							</span>
						</div>
					) : profileError ? (
						<p className="text-destructive">
							Failed to load profile. Please try again later.
						</p>
					) : profile ? (
						<div className="space-y-2 text-sm">
							<p>
								<span className="text-muted-foreground">
									Name:{" "}
								</span>
								{profile.firstName} {profile.lastName}
							</p>
							<p>
								<span className="text-muted-foreground">
									Username:{" "}
								</span>
								{profile.username || "Not set"}
							</p>
							<p>
								<span className="text-muted-foreground">
									Job Title:{" "}
								</span>
								{profile.jobTitle || "Not set"}
							</p>
							<p>
								<span className="text-muted-foreground">
									Reputation:{" "}
								</span>
								{profile.reputation}
							</p>
							<p>
								<span className="text-muted-foreground">
									Onboarded:{" "}
								</span>
								{profile.isOnboarded ? "Yes" : "No"}
							</p>
						</div>
					) : (
						<p className="text-muted-foreground">
							No profile data available.
						</p>
					)}
				</div>

				{/* Sign Out Button */}
				<Button
					variant="destructive"
					onClick={handleSignOut}
					disabled={signOutLoading}
					className="w-full"
				>
					{signOutLoading ? "Signing out..." : "Sign Out"}
				</Button>
			</div>
		</main>
	);
}
