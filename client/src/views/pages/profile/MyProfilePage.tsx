import {
	useGetCurrentUserProfileQuery,
	useGetCurrentUserSkillsQuery,
} from "@/features/profiles/profilesSlice";
import { Button } from "@/views/components/ui/button";
import {
	Tabs,
	TabsContent,
	TabsList,
	TabsTrigger,
} from "@/views/components/ui/tabs";
import { Pencil } from "lucide-react";
import { Badge } from "@/views/components/ui/badge";
import { useEffect, useMemo, useState } from "react";
import { Separator } from "@/views/components/ui/separator";
import defaultProfile from "../../../assets/Profile-pic/ProfilePic.svg";
import { Skeleton } from "@/views/components/ui/skeleton";
import toast from "react-hot-toast";
import { useGetAllPostsByProfileIdQuery } from "@/features/posts/postsSlice";
import { useGetCommentsByUserIdQuery } from "@/features/comments/commentsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import ProfilePostCard from "@/views/components/common/posts/ProfilePostCard";
import ProfileCommentCard from "@/views/components/common/comments/ProfileCommentCard";
import {
	Dialog,
	DialogContent,
	DialogOverlay,
	DialogPortal,
} from "@/views/components/ui/dialog";
import UpdateProfile from "@/views/components/common/profile/UpdateProfile";
import { Comment } from "@/features/comments/types";

const MyProfilePage = () => {
	const [_, userLoading, userError, authUser, appUser] = useIsUserLoggedIn();
	const { data: profile, isLoading, error } = useGetCurrentUserProfileQuery();
	const [activeTab, setActiveTab] = useState("questions");
	// Get user skills
	const { data: userSkills, isLoading: isSkillsLoading } =
		useGetCurrentUserSkillsQuery();
	const [isEditProfileOpen, setIsEditProfileOpen] = useState(false);
	const {
		data: allPosts,
		isLoading: isPostsLoading,
		isError: isPostsError,
		error: postsError,
		isFetching: isPostsFetching,
	} = useGetAllPostsByProfileIdQuery(appUser?.id || "");

	// Get user comments
	const {
		data: userComments,
		isLoading: isCommentsLoading,
		isError: isCommentsError,
		error: commentsError,
		isFetching: isCommentsFetching,
	} = useGetCommentsByUserIdQuery(appUser?.id || "");

	// Calculate total upvotes from all user posts
	const totalUpvotes = useMemo(() => {
		return allPosts?.reduce((sum, post) => sum + post.upvotes, 0);
	}, [allPosts]);

	const totalDownvotes = useMemo(() => {
		return allPosts?.reduce((sum, post) => sum + post.downvotes, 0);
	}, [allPosts]);

	useEffect(() => {
		if (error) {
			toast.error("Failed to load profile data. Please try again later!");
		}

		if (isCommentsError && commentsError) {
			toast.error("Error loading your comments.");
			console.error("Error loading comments:", commentsError);
		}
	}, [error, isCommentsError, commentsError]);

	const fullName = `${profile?.firstName || ""} ${profile?.lastName || ""}`;
	const jobTitle = profile?.jobTitle || "Expert";
	const username = profile?.username;
	const bio = profile?.bio || "No bio available";

	// User statistics
	const stats = {
		questions: allPosts?.length,
		upvotes: totalUpvotes,
		downvotes: totalDownvotes,
		answers: userComments?.length || 0,
		skills: profile?.skills?.length || 0,
	};

	// Helper function to find post title by post ID
	const getPostTitleById = (postId: string) => {
		const post = allPosts?.find((p) => p.id === postId);
		return post ? post.title : "Unknown Post";
	};

	const handleEditProfile = () => {
		setIsEditProfileOpen(true);
	};

	const handleCloseEditProfile = () => {
		setIsEditProfileOpen(false);
	};

	if (userLoading) {
		return (
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-card rounded-lg shadow-md border border-border p-6">
					<div className="flex flex-col items-center justify-center py-12">
						<div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
						<p className="mt-4 text-muted-foreground">
							Loading your profile...
						</p>
					</div>
				</div>
			</div>
		);
	}

	if (userError) {
		return (
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-card rounded-lg shadow-md border border-border p-6">
					<div className="flex flex-col items-center justify-center py-12">
						<div className="text-destructive mb-4">
							<svg
								className="w-16 h-16"
								fill="none"
								stroke="currentColor"
								viewBox="0 0 24 24"
							>
								<path
									strokeLinecap="round"
									strokeLinejoin="round"
									strokeWidth={2}
									d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
								/>
							</svg>
						</div>
						<h2 className="text-xl font-semibold text-card-foreground mb-2">
							Authentication Error
						</h2>
						<p className="text-muted-foreground text-center mb-4">
							There was an error loading your profile. Please try
							signing in again.
						</p>
						<Button
							onClick={() => window.location.reload()}
							className="bg-primary hover:bg-primary/90 text-primary-foreground rounded-full"
						>
							Reload Page
						</Button>
					</div>
				</div>
			</div>
		);
	}

	return (
		<>
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-card rounded-lg shadow-md border border-border p-3">
					{/* Profile Header */}
					<div className="border-border">
						{/* Cover Photo */}
						{isLoading ? (
							<Skeleton className="h-48 rounded-t-lg" />
						) : (
							<div className="h-48 bg-secondary rounded-t-lg"></div>
						)}

						{/* Profile Info Section */}
						<div className="relative px-8 pb-6">
							{/* Avatar */}
							<div className="absolute -top-16 left-8">
								{isLoading ? (
									<Skeleton className="rounded-full w-[110px] h-[110px] border-background border-4" />
								) : (
									<div className="flex justify-center items-center rounded-full border-background border-4 text-white text-4xl font-bold">
										{profile?.profilePictureUrl ? (
											<img
												src={profile.profilePictureUrl}
												alt={fullName}
												className="rounded-full"
												width={110}
												height={110}
											/>
										) : (
											<img
												src={defaultProfile}
												className="rounded-full"
												width={110}
												height={110}
											/>
										)}
									</div>
								)}
							</div>

							{/* Edit Button */}
							<div className="flex justify-end pt-4">
								{isLoading ? (
									<Skeleton className="h-9 w-24" />
								) : (
									<Button
										variant="outline"
										size="sm"
										className="gap-2"
										onClick={handleEditProfile}
									>
										<Pencil size={16} />
										<span>Edit</span>
									</Button>
								)}
							</div>

							{/* User Info */}
							<div className="mt-12">
								{isLoading ? (
									<>
										<div className="flex items-center">
											<Skeleton className="h-8 w-48 mr-4" />
											<Skeleton className="h-6 w-24" />
										</div>
										<div className="flex items-center mt-2">
											<Skeleton className="h-5 w-32 mr-4" />
											<Skeleton className="h-5 w-24" />
										</div>
										<Skeleton className="h-16 w-full mt-4" />
									</>
								) : (
									<>
										<div className="flex items-center">
											<h1 className="text-2xl font-bold mr-4 text-card-foreground">
												{fullName}
											</h1>
											<Badge className="bg-primary hover:bg-primary/90 text-primary-foreground">
												Top Rated
											</Badge>
										</div>

										<div className="flex items-center text-muted-foreground mt-2">
											<span className="mr-1">
												@{username}
											</span>
											-
											<span className="ml-1">
												{jobTitle}
											</span>
										</div>

										<div className="mt-4 whitespace-pre-line text-muted-foreground">
											{bio}
										</div>
									</>
								)}
							</div>
						</div>
					</div>

					<Separator
						className="my-3 bg-border"
						style={{ height: "2px" }}
					/>

					{/* Stats Section */}
					<div className="grid grid-cols-5 gap-4 my-6">
						{isLoading || isCommentsLoading ? (
							<>
								<div className="text-center p-4 rounded-xl bg-muted/30">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center p-4 rounded-xl bg-muted/30">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center p-4 rounded-xl bg-muted/30">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center p-4 rounded-xl bg-muted/30">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center p-4 rounded-xl bg-muted/30">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
							</>
						) : (
							<>
								<div className="text-center p-4 rounded-xl bg-muted/30 hover:bg-muted/50 transition-colors">
									<div className="text-3xl font-bold text-card-foreground mb-1">
										{stats.questions}
									</div>
									<div className="text-sm text-muted-foreground font-medium">
										Questions
									</div>
								</div>
								<div className="text-center p-4 rounded-xl bg-green-500/10 hover:bg-green-500/20 transition-colors">
									<div className="text-3xl font-bold text-green-600 mb-1">
										{stats.upvotes}
									</div>
									<div className="text-sm text-green-600 font-medium">
										Upvotes
									</div>
								</div>
								<div className="text-center p-4 rounded-xl bg-red-500/10 hover:bg-red-500/20 transition-colors">
									<div className="text-3xl font-bold text-red-600 mb-1">
										{stats.downvotes}
									</div>
									<div className="text-sm text-red-600 font-medium">
										Downvotes
									</div>
								</div>
								<div className="text-center p-4 rounded-xl bg-muted/30 hover:bg-muted/50 transition-colors">
									<div className="text-3xl font-bold text-card-foreground mb-1">
										{stats.answers}
									</div>
									<div className="text-sm text-muted-foreground font-medium">
										Answers
									</div>
								</div>
								<div className="text-center p-4 rounded-xl bg-primary/10 hover:bg-primary/20 transition-colors">
									<div className="text-3xl font-bold text-primary mb-1">
										{stats.skills}
									</div>
									<div className="text-sm text-primary font-medium">
										Skills
									</div>
								</div>
							</>
						)}
					</div>

					<Separator
						className="my-3 bg-border"
						style={{ height: "2px" }}
					/>

					{/* Tabs Section */}
					<div className="my-6">
						{isLoading ||
						(activeTab === "answers" && isCommentsLoading) ? (
							<>
								<Skeleton className="h-10 w-full mb-6" />
								<div className="space-y-4">
									{[1, 2, 3].map((i) => (
										<div
											key={i}
											className="border-b border-border pb-4"
										>
											<Skeleton className="h-6 w-3/4 mb-2" />
											<div className="flex my-1">
												<Skeleton className="h-4 w-24 mr-4" />
												<Skeleton className="h-4 w-32" />
											</div>
											<Skeleton className="h-16 w-full" />
										</div>
									))}
								</div>
							</>
						) : (
							<Tabs
								defaultValue="questions"
								onValueChange={setActiveTab}
								className="w-full"
							>
								<TabsList className="grid grid-cols-3 mb-6 bg-muted">
									<TabsTrigger
										value="questions"
										className="data-[state=active]:bg-background data-[state=active]:text-primary"
									>
										My Questions
									</TabsTrigger>
									<TabsTrigger
										value="answers"
										className="data-[state=active]:bg-background data-[state=active]:text-primary"
									>
										My Answers
									</TabsTrigger>
									<TabsTrigger
										value="skills"
										className="data-[state=active]:bg-background data-[state=active]:text-primary"
									>
										My Skills
									</TabsTrigger>
								</TabsList>{" "}
								{/* Latest Questions (Posts) Tab Content */}
								<TabsContent
									value="questions"
									className="space-y-4"
								>
									{isCommentsLoading ? (
										<div className="space-y-4">
											{[1, 2, 3].map((i) => (
												<div
													key={i}
													className="border rounded-lg p-4 border-border"
												>
													<Skeleton className="h-6 w-3/4 mb-2" />
													<div className="flex my-1">
														<Skeleton className="h-4 w-24 mr-4" />
														<Skeleton className="h-4 w-32" />
													</div>
													<Skeleton className="h-16 w-full" />
												</div>
											))}
										</div>
									) : allPosts && allPosts.length > 0 ? (
										(() => {
											const sortedPosts = [
												...allPosts,
											].sort((a, b) =>
												b.createdAt.localeCompare(
													a.createdAt
												)
											);
											return sortedPosts.map((post) => {
												return (
													<ProfilePostCard
														key={post.id}
														post={post}
													/>
												);
											});
										})()
									) : (
										<div className="text-center py-8 text-muted-foreground">
											You haven't asked any questions yet.
										</div>
									)}
								</TabsContent>
								{/* Answered Questions (Comments) Tab Content */}
								<TabsContent
									value="answers"
									className="space-y-4"
								>
									{isCommentsLoading ? (
										<div className="space-y-4">
											{[1, 2, 3].map((i) => (
												<div
													key={i}
													className="border rounded-lg p-4 border-border"
												>
													<Skeleton className="h-6 w-3/4 mb-2" />
													<div className="flex my-1">
														<Skeleton className="h-4 w-24 mr-4" />
														<Skeleton className="h-4 w-32" />
													</div>
													<Skeleton className="h-16 w-full" />
												</div>
											))}
										</div>
									) : userComments &&
									  userComments.length > 0 ? (
										(() => {
											const sortedComments = [
												...userComments,
											].sort((a, b) =>
												b.createdAt.localeCompare(
													a.createdAt
												)
											);
											return sortedComments.map(
												(comment: Comment) => (
													<ProfileCommentCard
														key={comment.id}
														comment={comment}
														postTitle={getPostTitleById(
															comment.postId!
														)}
													/>
												)
											);
										})()
									) : (
										<div className="text-center py-8 text-muted-foreground">
											You haven't answered any questions
											yet.
										</div>
									)}
								</TabsContent>
								{/* Skills Tab Content */}
								<TabsContent value="skills" className="py-4">
									{isSkillsLoading ? (
										<div className="grid grid-cols-3 gap-4">
											{[1, 2, 3, 4, 5, 6].map((i) => (
												<Skeleton
													key={i}
													className="h-10 w-full rounded-full"
												/>
											))}
										</div>
									) : profile?.skills &&
									  profile.skills.length > 0 ? (
										<div className="grid grid-cols-6 gap-4 max-lg:grid-cols-2 max-sm:grid-cols-1">
											{profile.skills.map(
												(skill, index) => (
													<div
														key={index}
														className="rounded-full px-1 py-2 text-white text-center font-medium bg-indigo-700 bg-linear-to-r"
													>
														{skill}
													</div>
												)
											)}
										</div>
									) : (
										<div className="text-center py-8 text-muted-foreground">
											You haven't added any skills yet.
										</div>
									)}
								</TabsContent>
							</Tabs>
						)}
					</div>
				</div>
			</div>

			{/* Edit Profile Dialog */}
			<Dialog
				open={isEditProfileOpen}
				onOpenChange={setIsEditProfileOpen}
			>
				<DialogPortal>
					<DialogOverlay className="bg-black/50" />
					<DialogContent className="sm:max-w-[600px] p-0 max-h-[90vh] overflow-y-auto">
						<UpdateProfile onClose={handleCloseEditProfile} />
					</DialogContent>
				</DialogPortal>
			</Dialog>
		</>
	);
};

export default MyProfilePage;
