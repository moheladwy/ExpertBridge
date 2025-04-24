import { useNavigate, useParams } from "react-router-dom";
import { Button } from "@/views/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/views/components/ui/tabs";
import { Badge } from "@/views/components/ui/badge";
import { useEffect, useState } from "react";
import { Separator } from "@/views/components/ui/separator";
import { useGetProfileByIdQuery } from "@/features/profiles/profilesSlice";
import { UserPlusIcon } from "lucide-react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../assets/Profile-pic/ProfilePic.svg";
import { Skeleton } from "@/views/components/ui/skeleton";
import toast from "react-hot-toast";

const UserProfilePage = () => {
	const { userId } = useParams<{ userId: string }>();
	const navigate = useNavigate();
	const [_, __, ___, authUser, appUser] = useIsUserLoggedIn();
	const {
		data: profile,
		isLoading,
		error,
	} = useGetProfileByIdQuery(userId || "");
	const [activeTab, setActiveTab] = useState("latest");

	// Check if the requested profile is the current user's profile
	useEffect(() => {
		if (userId && authUser && appUser && userId === appUser.id) {
			navigate("/profile");
		}
	}, [userId, authUser, navigate, appUser]);

	// Show toast notification when error occurs
	useEffect(() => {
		if (error) {
			toast.error("Error loading profile. This user may not exist or you may not have permission to view their profile.");
		}
	}, [error]);

	const fullName = `${profile?.firstName || ""} ${profile?.lastName || ""}`;
	const jobTitle = profile?.jobTitle || "Expert";
	const location = "Giza, Egypt"; // This would come from profile data if available
	const bio = profile?.email || "No bio available";

	// Placeholder stats - these would come from API calls
	const stats = {
		topAnswers: 104,
		upVotes: "+50K",
		completedJobs: 30,
	};

	// Placeholder posts - these would come from API calls
	const posts = [
		{
			id: 1,
			title: "How To fix this tap?",
			date: "20 Dec 2024",
			upVotes: "10K Up Votes",
			content:
				'"You can do these steps first...\nsecond...\nthen...\nand finaly..."',
		},
		{
			id: 2,
			title: "How To fix this tap?",
			date: "20 Dec 2024",
			upVotes: "10K Up Votes",
			content:
				'"You can do these steps first...\nsecond...\nthen...\nand finaly..."',
		},
		{
			id: 3,
			title: "How To fix this tap?",
			date: "20 Dec 2024",
			upVotes: "10K Up Votes",
			content:
				'"You can do these steps first...\nsecond...\nthen...\nand finaly..."',
		},
	];

	// Don't render anything if error and no profile
	if (error && !profile) return null;

	return (
		<>
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-white rounded-lg shadow-md border p-3">
					{/* Profile Header */}
					<div className="border-gray-200">
						{/* Cover Photo */}
						{isLoading ? (
							<Skeleton className="h-48 rounded-t-lg" />
						) : (
							<div className="h-48 bg-gray-200 rounded-t-lg"></div>
						)}

						{/* Profile Info Section */}
						<div className="relative px-8 pb-6">
							{/* Avatar */}
							<div className="absolute -top-16 left-8">
								{isLoading ? (
									<Skeleton className="rounded-full w-[110px] h-[110px] border-white border-4" />
								) : (
									<div className="flex justify-center items-center rounded-full border-white	border-4 text-white text-4xl font-bold">
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

							{/* Connect/Hire Button */}
							<div className="flex justify-end pt-4">
								{isLoading ? (
									<Skeleton className="h-9 w-28" />
								) : (
									<Button className="bg-indigo-600 hover:bg-indigo-700 text-white gap-2">
										<UserPlusIcon size={16} />
										<span>Hire Me</span>
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
											<h1 className="text-2xl font-bold mr-4">
												{fullName}
											</h1>
											<Badge className="bg-indigo-600 hover:bg-indigo-700">
												Top Rated
											</Badge>
										</div>

										<div className="flex items-center text-gray-500 mt-2">
											<span className="mr-4">{location}</span>
											<span>{jobTitle}</span>
										</div>

										<div className="mt-4 whitespace-pre-line text-gray-700">
											{bio}
										</div>
									</>
								)}
							</div>
						</div>
					</div>

					<Separator className="my-3" style={{ height: "2px" }} />

					{/* Stats Section */}
					<div className="grid grid-cols-3 gap-4 my-6">
						{isLoading ? (
							<>
								<div className="text-center">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
								<div className="text-center">
									<Skeleton className="h-10 w-16 mx-auto mb-2" />
									<Skeleton className="h-5 w-24 mx-auto" />
								</div>
							</>
						) : (
							<>
								<div className="text-center">
									<div className="text-3xl font-semibold">
										{stats.topAnswers}
									</div>
									<div className="text-sm text-gray-500">Top Answers</div>
								</div>
								<div className="text-center">
									<div className="text-3xl font-semibold">
										{stats.upVotes}
									</div>
									<div className="text-sm text-gray-500">Total Up Votes</div>
								</div>
								<div className="text-center">
									<div className="text-3xl font-semibold">
										{stats.completedJobs}
									</div>
									<div className="text-sm text-gray-500">Completed Jobs</div>
								</div>
							</>
						)}
					</div>

					<Separator className="my-3" style={{ height: "2px" }} />

					{/* Tabs Section */}
					<div className="my-6">
						{isLoading ? (
							<>
								<Skeleton className="h-10 w-full mb-6" />
								<div className="space-y-4">
									{[1, 2, 3].map((i) => (
										<div key={i} className="border-b pb-4">
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
								defaultValue="latest"
								onValueChange={setActiveTab}
								className="w-full"
							>
								<TabsList className="grid grid-cols-2 mb-6">
									<TabsTrigger value="latest">Latest Answers</TabsTrigger>
									<TabsTrigger value="posted">Posted Jobs</TabsTrigger>
								</TabsList>

								{/* Tab Content */}
								<TabsContent value="latest" className="space-y-4">
									{posts.map((post) => (
										<div key={post.id} className="border-b pb-4">
											<h3 className="text-lg font-semibold">
												{post.title}
											</h3>
											<div className="flex text-sm text-gray-500 my-1">
												<span className="mr-4">{post.date}</span>
												<span>{post.upVotes}</span>
											</div>
											<div className="whitespace-pre-line">
												{post.content}
											</div>
										</div>
									))}
								</TabsContent>

								<TabsContent value="posted">
									<div className="text-center py-8 text-gray-500">
										No posted jobs yet.
									</div>
								</TabsContent>
							</Tabs>
						)}
					</div>
				</div>
			</div>
		</>
	);
};

export default UserProfilePage;
