"use client";

import { useState } from "react";
import { ProtectedRoute } from "@/components/shared/ProtectedRoute";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetPostsByUserIdQuery } from "@/features/posts/postsSlice";
import { useGetCommentsByUserIdQuery } from "@/features/comments/commentsSlice";
import {
	ProfileSidebar,
	ProfileContentTabs,
	ProfilePageSkeleton,
	UpdateProfile,
} from "@/components/profile";
import { Dialog, DialogContent } from "@/components/ui/dialog";

/**
 * Protected profile page - only accessible to authenticated users.
 * Two-column layout with sidebar (profile info) and main content (tabs).
 */
export default function ProfilePage() {
	return (
		<ProtectedRoute>
			<ProfileContent />
		</ProtectedRoute>
	);
}

function ProfileContent() {
	const [isEditOpen, setIsEditOpen] = useState(false);

	// Fetch profile data
	const {
		data: profile,
		isLoading: isProfileLoading,
		error: profileError,
	} = useGetCurrentUserProfileQuery(undefined);

	// Fetch user's posts and comments
	const { data: posts, isLoading: isPostsLoading } = useGetPostsByUserIdQuery(
		profile?.id ?? "",
		{
			skip: !profile?.id,
		}
	);

	const { data: comments, isLoading: isCommentsLoading } =
		useGetCommentsByUserIdQuery(profile?.id ?? "", {
			skip: !profile?.id,
		});

	// Show skeleton while loading profile
	if (isProfileLoading) {
		return <ProfilePageSkeleton />;
	}

	// Show error state
	if (profileError) {
		return (
			<div className="mx-auto max-w-6xl px-4 py-12 text-center">
				<h1 className="text-2xl font-bold text-destructive mb-4">
					Failed to load profile
				</h1>
				<p className="text-muted-foreground">
					Please try again later or contact support if the problem
					persists.
				</p>
			</div>
		);
	}

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
							onEditProfile={() => {
								setIsEditOpen(true);
							}}
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

			{/* Edit Profile Dialog */}
			<Dialog open={isEditOpen} onOpenChange={setIsEditOpen}>
				<DialogContent className="sm:max-w-lg max-h-[90vh] overflow-y-auto">
					<UpdateProfile
						onClose={() => {
							setIsEditOpen(false);
						}}
					/>
				</DialogContent>
			</Dialog>
		</div>
	);
}
