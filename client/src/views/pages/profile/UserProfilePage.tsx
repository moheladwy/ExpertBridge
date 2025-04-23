import { useNavigate, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Badge } from "@/components/ui/badge";
import { useEffect, useState } from "react";
import { Avatar } from "@/components/ui/avatar";
import { Separator } from "@/components/ui/separator";
import { useGetProfileByIdQuery } from "@/features/profiles/profilesSlice";
import { UserPlusIcon } from "lucide-react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

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

	if (isLoading)
		return <div className="p-8 text-center">Loading profile...</div>;
	if (error || !profile)
		return (
			<div className="p-8 text-center text-red-500">
				Error loading profile. This user may not exist or you may not
				have permission to view their profile.
			</div>
		);

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

	return (
		<>
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 bg-white rounded-lg shadow-md border p-3">
					{/* Profile Header */}
					<div className="border-gray-200">
						{/* Cover Photo */}
						<div className="h-48 bg-gray-200 rounded-t-lg"></div>

						{/* Profile Info Section */}
						<div className="relative px-8 pb-6">
							{/* Avatar */}
							<div className="absolute -top-16 left-8">
								<Avatar className="h-32 w-32 ring-4 ring-white bg-green-700 text-white text-4xl font-bold">
									{profile?.profilePictureUrl ? (
										<img
											src={profile.profilePictureUrl}
											alt={fullName}
										/>
									) : (
										<span>{fullName.charAt(0).toUpperCase()}</span>
									)}
								</Avatar>
							</div>

							{/* Connect/Hire Button */}
							<div className="flex justify-end pt-4">
								<Button className="bg-indigo-600 hover:bg-indigo-700 text-white gap-2">
									<UserPlusIcon size={16} />
									<span>Hire Me</span>
								</Button>
							</div>

							{/* User Info */}
							<div className="mt-12">
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
							</div>
						</div>
					</div>

					<Separator className="my-3" style={{ height: "2px" }} />

					{/* Stats Section */}
					<div className="grid grid-cols-3 gap-4 my-6">
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
					</div>

					<Separator className="my-3" style={{ height: "2px" }} />

					{/* Tabs Section */}
					<div className="my-6">
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
					</div>
				</div>
			</div>
		</>
	);
};

export default UserProfilePage;
