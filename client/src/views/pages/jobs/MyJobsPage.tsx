import React, { useState } from "react";
import { CheckCircle, Clock } from "lucide-react";
import { useGetMyJobsQuery } from "@/features/jobs/jobsSlice";
import { JobCard } from "../../components/common/jobs/JobCard";
import { useNavigate } from "react-router";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { Skeleton } from "@/views/components/ui/skeleton";
import toast from "react-hot-toast";
import {
	Tabs,
	TabsContent,
	TabsList,
	TabsTrigger,
} from "@/views/components/ui/tabs";

// Jobs List Page Component
export const MyJobsPage: React.FC = () => {
	const { data: jobs, isLoading, error } = useGetMyJobsQuery();
	const [selectedTab, setSelectedTab] = useState<"active" | "completed">(
		"active"
	);
	const [_, __, ___, authUser, appUser] = useIsUserLoggedIn();
	const navigate = useNavigate();

	// Check if user is logged in
	if (!authUser || !appUser) {
		return (
			<div className="text-center p-8">
				<p className="text-gray-600 dark:text-gray-400">
					Please log in to view your jobs.
				</p>
			</div>
		);
	}

	if (isLoading) {
		return (
			<div className="max-w-6xl mx-auto p-6">
				<div className="mb-8">
					<Skeleton className="h-8 w-48 mb-2" />
					<Skeleton className="h-5 w-64" />
				</div>
				<div className="flex space-x-1 mb-6">
					<Skeleton className="h-10 w-32" />
					<Skeleton className="h-10 w-32" />
				</div>
				<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
					{[1, 2, 3].map((i) => (
						<div
							key={i}
							className="bg-white dark:bg-gray-800 rounded-lg shadow-sm border dark:border-gray-700 p-6"
						>
							<Skeleton className="h-6 w-3/4 mb-4" />
							<div className="space-y-3">
								<Skeleton className="h-4 w-full" />
								<Skeleton className="h-4 w-2/3" />
								<Skeleton className="h-4 w-1/2" />
							</div>
							<Skeleton className="h-10 w-full mt-6" />
						</div>
					))}
				</div>
			</div>
		);
	}

	if (error) {
		toast.error("Error loading jobs. Please try again.");
		return (
			<div className="text-center text-red-600 dark:text-red-400 p-8">
				<p>Error loading jobs. Please try again.</p>
			</div>
		);
	}

	const activeJobs = jobs?.filter((job) => !job.isCompleted) || [];
	const completedJobs = jobs?.filter((job) => job.isCompleted) || [];

	return (
		<div className="w-full flex justify-center">
			<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-white dark:bg-gray-800 rounded-lg shadow-md border dark:border-gray-700 p-6">
				<div className="flex flex-col items-center mb-4">
					<h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-2">
						My Jobs
					</h1>
					<p className="text-gray-600 dark:text-gray-400">
						Manage your active and completed jobs
					</p>
				</div>

				{/* Tab Navigation using your Tabs component */}
				<Tabs
					defaultValue="active"
					onValueChange={(value) =>
						setSelectedTab(value as "active" | "completed")
					}
				>
					<TabsList className="grid grid-cols-2 mb-6 dark:bg-gray-700">
						<TabsTrigger
							value="active"
							className="data-[state=active]:bg-white dark:data-[state=active]:bg-gray-600 dark:text-gray-200"
						>
							Active Jobs ({activeJobs.length})
						</TabsTrigger>
						<TabsTrigger
							value="completed"
							className="data-[state=active]:bg-white dark:data-[state=active]:bg-gray-600 dark:text-gray-200"
						>
							Completed ({completedJobs.length})
						</TabsTrigger>
					</TabsList>

					<TabsContent value="active" className="space-y-4">
						{activeJobs.length > 0 ? (
							<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
								{activeJobs.map((job) => (
									<JobCard
										key={job.id}
										job={job}
										appUser={appUser}
										onViewDetails={() =>
											navigate(`/my-jobs/${job.id}`)
										}
									/>
								))}
							</div>
						) : (
							<div className="text-center py-12">
								<Clock
									size={48}
									className="mx-auto text-gray-400 dark:text-gray-500 mb-4"
								/>
								<h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
									No active jobs
								</h3>
								<p className="text-gray-500 dark:text-gray-400">
									You don't have any active jobs at the
									moment.
								</p>
							</div>
						)}
					</TabsContent>

					<TabsContent value="completed" className="space-y-4">
						{completedJobs.length > 0 ? (
							<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
								{completedJobs.map((job) => (
									<JobCard
										key={job.id}
										job={job}
										appUser={appUser}
										onViewDetails={() =>
											navigate(`/my-jobs/${job.id}`)
										}
									/>
								))}
							</div>
						) : (
							<div className="text-center py-12">
								<CheckCircle
									size={48}
									className="mx-auto text-gray-400 dark:text-gray-500 mb-4"
								/>
								<h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
									No completed jobs
								</h3>
								<p className="text-gray-500 dark:text-gray-400">
									You haven't completed any jobs yet.
								</p>
							</div>
						)}
					</TabsContent>
				</Tabs>
			</div>
		</div>
	);
};
