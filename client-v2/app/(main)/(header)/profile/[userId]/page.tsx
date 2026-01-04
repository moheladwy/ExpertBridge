"use client";

import { use, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useGetProfileByIdQuery } from "@/features/profiles/profilesSlice";
import { useGetPostsByUserIdQuery } from "@/features/posts/postsSlice";
import { useGetCommentsByUserIdQuery } from "@/features/comments/commentsSlice";
import {
	ProfileSidebar,
	ProfileContentTabs,
	ProfilePageSkeleton,
	HireMeButton,
} from "@/components/profile";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import { ErrorBoundary } from "@/components/shared/ErrorBoundary";

interface PageProps {
	params: Promise<{ userId: string }>;
}

/**
 * Public user profile page.
 * Accessible to anyone (authenticated or not).
 * Shows "Hire Me" button instead of "Edit Profile".
 */
export default function UserProfilePage({ params }: PageProps) {
	const { userId } = use(params);
	return (
		<ErrorBoundary>
			<UserProfileContent userId={userId} />
		</ErrorBoundary>
	);
}

function UserProfileContent({ userId }: { userId: string }) {
	const router = useRouter();
	const { profile: currentUserProfile, isLoading: isAuthLoading } =
		useIsUserLoggedIn();

	// Fetch profile data - hooks must be called unconditionally
	const {
		data: profile,
		isLoading: isProfileLoading,
		error: profileError,
	} = useGetProfileByIdQuery(userId);

	// Fetch user's posts and comments
	const { data: posts, isLoading: isPostsLoading } = useGetPostsByUserIdQuery(
		userId,
		{
			skip: !userId,
		}
	);

	const { data: comments, isLoading: isCommentsLoading } =
		useGetCommentsByUserIdQuery(userId, {
			skip: !userId,
		});

	// Redirect to own profile page if viewing self (using useEffect to avoid render-time redirect)
	const isOwnProfile = !isAuthLoading && currentUserProfile?.id === userId;

	useEffect(() => {
		if (isOwnProfile) {
			router.replace("/profile");
		}
	}, [isOwnProfile, router]);

	// Show skeleton while redirecting to own profile
	if (isOwnProfile) {
		return <ProfilePageSkeleton />;
	}

	// Show skeleton while loading profile
	if (isProfileLoading) {
		return <ProfilePageSkeleton />;
	}

	// Show error state for non-existent profiles
	if (profileError || !profile) {
		return (
			<div className="mx-auto max-w-6xl px-4 py-12 text-center">
				<h1 className="text-2xl font-bold text-destructive mb-4">
					Profile not found
				</h1>
				<p className="text-muted-foreground">
					The user profile you&apos;re looking for doesn&apos;t exist
					or has been removed.
				</p>
			</div>
		);
	}

	const profileName = [profile.firstName, profile.lastName]
		.filter(Boolean)
		.join(" ");

	return (
		<div className="mx-auto max-w-6xl px-4 py-6 min-h-[calc(100vh-64px)]">
			<div className="flex flex-col md:flex-row gap-6 md:items-start">
				{/* Sidebar - Fixed width on desktop */}
				<aside className="w-full md:w-82 shrink-0">
					<div className="md:sticky md:top-20">
						<ProfileSidebar
							profile={profile}
							postsCount={posts?.length ?? 0}
							commentsCount={comments?.length ?? 0}
							isLoading={isProfileLoading}
							customAction={
								<HireMeButton
									profileId={userId}
									profileName={profileName}
									className="w-full"
								/>
							}
						/>
					</div>
				</aside>

				{/* Main Content - Flex grow */}
				<div className="flex-1 min-w-0 items-center align-middle">
					<ProfileContentTabs
						posts={posts}
						comments={comments}
						isPostsLoading={isPostsLoading}
						isCommentsLoading={isCommentsLoading}
					/>
				</div>
			</div>
		</div>
	);
}
