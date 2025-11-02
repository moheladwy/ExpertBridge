import React, { useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ArrowLeftCircle as CircleArrowLeft } from "lucide-react";
import {
	UserIcon,
	CurrencyDollarIcon,
	StarIcon,
	BriefcaseIcon,
	CalendarIcon,
	TrophyIcon,
} from "@heroicons/react/24/outline";
import { StarIcon as StarIconSolid } from "@heroicons/react/24/solid";
import { Button } from "@/views/components/ui/button";
import {
	Card,
	CardContent,
	CardHeader,
	CardTitle,
} from "@/views/components/ui/card";
import { Badge } from "@/views/components/ui/badge";
import {
	useGetJobPostingQuery,
	useGetJobApplicationsQuery,
} from "@/features/jobPostings/jobPostingsSlice";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import TimeAgo from "@/views/components/custom/TimeAgo";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { acceptApplication } from "@/api/jobService";

const JobApplicationsPage: React.FC = () => {
	const { jobPostingId } = useParams<{ jobPostingId: string }>();
	const navigate = useNavigate();
	const [selectedApplicant, setSelectedApplicant] = useState<string | null>(
		null
	);
	const [acceptingApplication, setAcceptingApplication] = useState<
		string | null
	>(null);

	// Fetch job posting and applications
	const {
		data: jobPosting,
		isLoading: jobLoading,
		error: jobError,
	} = useGetJobPostingQuery(jobPostingId!, { skip: !jobPostingId });

	const {
		data: applications,
		isLoading: applicationsLoading,
		error: applicationsError,
		refetch,
	} = useGetJobApplicationsQuery(jobPostingId!, { skip: !jobPostingId });

	useRefetchOnLogin(refetch);

	const formatBudget = (budget: number) => {
		if (budget >= 1000) {
			return `$${(budget / 1000).toFixed(1)}k`;
		}
		return `$${budget}`;
	};

	const renderStars = (rating: number) => {
		const stars = [];
		const fullStars = Math.floor(rating);
		const hasHalfStar = rating % 1 !== 0;

		for (let i = 0; i < fullStars; i++) {
			stars.push(
				<StarIconSolid key={i} className="h-4 w-4 text-yellow-400" />
			);
		}

		if (hasHalfStar) {
			stars.push(
				<div key="half" className="relative">
					<StarIcon className="h-4 w-4 text-gray-300" />
					<StarIconSolid
						className="absolute inset-0 h-4 w-4 text-yellow-400 overflow-hidden"
						style={{ width: "50%" }}
					/>
				</div>
			);
		}

		const remainingStars = 5 - Math.ceil(rating);
		for (let i = 0; i < remainingStars; i++) {
			stars.push(
				<StarIcon
					key={`empty-${i}`}
					className="h-4 w-4 text-gray-300"
				/>
			);
		}

		return stars;
	};

	const getCostDifference = (offeredCost: number, jobBudget: number) => {
		const difference = offeredCost - jobBudget;
		const percentage = ((difference / jobBudget) * 100).toFixed(0);

		if (difference === 0) {
			return {
				text: "Exact match",
				color: "text-green-600",
				bgColor: "bg-green-100",
			};
		} else if (difference < 0) {
			return {
				text: `${Math.abs(Number(percentage))}% under budget`,
				color: "text-green-600",
				bgColor: "bg-green-100",
			};
		} else {
			return {
				text: `${percentage}% over budget`,
				color: "text-red-600",
				bgColor: "bg-red-100",
			};
		}
	};

	const handleAcceptApplication = async (applicationId: string) => {
		setAcceptingApplication(applicationId);
		try {
			const jobResponse = await acceptApplication(applicationId);

			// Redirect to the job page using the returned job ID
			navigate(`/my-jobs/${jobResponse.id}`);
		} catch (error) {
			console.error("Failed to accept application:", error);
			// Handle error (show toast, etc.)
			// You might want to show an error message to the user
		} finally {
			setAcceptingApplication(null);
		}
	};

	if (jobLoading || applicationsLoading) {
		return (
			<div className="flex justify-center items-center min-h-screen">
				<div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
			</div>
		);
	}

	if (jobError || applicationsError || !jobPosting) {
		return (
			<div className="flex flex-col items-center justify-center min-h-screen">
				<h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
					Job not found
				</h2>
				<Button onClick={() => navigate("/jobs")} variant="outline">
					Back to Jobs
				</Button>
			</div>
		);
	}

	const sortedApplications = [...applications!]?.sort((a, b) => {
		// Sort by cost (ascending) then by application date (descending)
		if (a.offeredCost !== b.offeredCost) {
			return a.offeredCost - b.offeredCost;
		}
		return (
			new Date(b.appliedAt).getTime() - new Date(a.appliedAt).getTime()
		);
	});

	return (
		<div className="w-full flex justify-center">
			<div className="w-5/6 mx-auto py-6 max-sm:w-full">
				{/* Header */}
				<div className="mb-6">
					<div className="flex items-center gap-4 mb-4">
						<button
							onClick={() => navigate(-1)}
							className="cursor-pointer"
						>
							<CircleArrowLeft className="h-6 w-6 text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300" />
						</button>
						<h1 className="text-3xl font-bold text-gray-900 dark:text-white">
							Job Applications
						</h1>
					</div>

					{/* Job Summary Card */}
					<Card className="mb-6">
						<CardHeader>
							<CardTitle className="flex items-center justify-between">
								<div className="flex items-center gap-3">
									<BriefcaseIcon className="h-6 w-6 text-blue-600" />
									<span className="text-xl">
										{jobPosting.title}
									</span>
								</div>
								<Badge
									variant="secondary"
									className="text-lg px-4 py-2"
								>
									{applications?.length || 0} Applications
								</Badge>
							</CardTitle>
						</CardHeader>
						<CardContent>
							<div className="flex items-center justify-between">
								<div className="flex items-center gap-4">
									<div className="flex items-center gap-1 text-gray-600 dark:text-gray-400">
										<CurrencyDollarIcon className="h-5 w-5" />
										<span className="font-semibold">
											Budget:{" "}
											{formatBudget(jobPosting.budget)}
										</span>
									</div>
									<div className="flex items-center gap-1 text-gray-600 dark:text-gray-400">
										<CalendarIcon className="h-5 w-5" />
										<TimeAgo
											timestamp={jobPosting.createdAt}
										/>
									</div>
								</div>
								<Button
									onClick={() =>
										navigate(`/jobs/${jobPostingId}`)
									}
									variant="outline"
									size="sm"
								>
									View Job Details
								</Button>
							</div>
						</CardContent>
					</Card>
				</div>

				{/* Applications List */}
				{!applications || applications.length === 0 ? (
					<Card>
						<CardContent className="text-center py-12">
							<UserIcon className="h-12 w-12 text-gray-400 mx-auto mb-4" />
							<h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
								No Applications Yet
							</h3>
							<p className="text-gray-600 dark:text-gray-400">
								Your job posting hasn't received any
								applications yet. Share it to attract more
								candidates!
							</p>
						</CardContent>
					</Card>
				) : (
					<div className="grid gap-4">
						{sortedApplications?.map((application) => {
							const applicant = application.applicant;
							const costDiff = getCostDifference(
								application.offeredCost,
								jobPosting.budget
							);
							const isAccepting =
								acceptingApplication === application.id;

							return (
								<Card
									key={application.applicantId}
									className={`transition-all duration-200 hover:shadow-lg cursor-pointer ${
										selectedApplicant ===
										application.applicantId
											? "ring-2 ring-blue-500 shadow-lg"
											: ""
									}`}
									onClick={() =>
										setSelectedApplicant(
											selectedApplicant ===
												application.applicantId
												? null
												: application.applicantId
										)
									}
								>
									<CardContent className="p-6">
										<div className="flex items-start justify-between">
											{/* Applicant Info */}
											<div className="flex items-start gap-4 flex-1">
												<Link
													to={`/profile/${applicant?.userId}`}
													onClick={(e) =>
														e.stopPropagation()
													}
												>
													{applicant?.profilePictureUrl ? (
														<img
															src={
																applicant.profilePictureUrl
															}
															alt={`${applicant.firstName}'s profile`}
															className="w-12 h-12 rounded-full object-cover"
														/>
													) : (
														<img
															src={defaultProfile}
															alt="Default profile"
															className="w-12 h-12 rounded-full"
														/>
													)}
												</Link>

												<div className="flex-1">
													<div className="flex items-center gap-2 mb-1">
														<Link
															to={`/profile/${applicant?.userId}`}
															onClick={(e) =>
																e.stopPropagation()
															}
															className="font-semibold text-lg text-gray-900 dark:text-white hover:text-blue-600"
														>
															{
																applicant?.firstName
															}{" "}
															{
																applicant?.lastName
															}
														</Link>
														{applicant?.username && (
															<span className="text-sm text-gray-500">
																@
																{
																	applicant.username
																}
															</span>
														)}
													</div>

													{/* Applicant Stats */}
													<div className="flex items-center gap-4 mb-2">
														{applicant?.rating && (
															<div className="flex items-center gap-1">
																<div className="flex">
																	{renderStars(
																		applicant.rating
																	)}
																</div>
																<span className="text-sm text-gray-600 dark:text-gray-400">
																	{applicant.rating.toFixed(
																		1
																	)}
																</span>
															</div>
														)}

														<div className="flex items-center gap-1 text-sm text-gray-600 dark:text-gray-400">
															<BriefcaseIcon className="h-4 w-4" />
															<span>
																{
																	applicant?.jobsDone
																}{" "}
																jobs completed
															</span>
														</div>
														<div className="flex items-center gap-1 text-sm text-gray-600 dark:text-gray-400">
															<TrophyIcon className="h-4 w-4" />
															<span>
																{
																	applicant?.reputation
																}{" "}
																reputation
															</span>
														</div>
													</div>

													{applicant?.jobTitle && (
														<p className="text-sm text-gray-600 dark:text-gray-400 mb-2">
															{applicant.jobTitle}
														</p>
													)}

													<div className="flex items-center gap-2 text-sm text-gray-500">
														<CalendarIcon className="h-4 w-4" />
														<span>
															Applied{" "}
															<TimeAgo
																timestamp={
																	application.appliedAt
																}
															/>
														</span>
													</div>
												</div>
											</div>

											{/* Cost Info */}
											<div className="text-right">
												<div className="text-2xl font-bold text-gray-900 dark:text-white mb-1">
													{formatBudget(
														application.offeredCost
													)}
												</div>
												<Badge
													variant="secondary"
													className={`${costDiff.bgColor} ${costDiff.color} border-0`}
												>
													{costDiff.text}
												</Badge>
											</div>
										</div>

										{/* Cover Letter - Expandable */}
										{selectedApplicant ===
											application.applicantId &&
											application.coverLetter && (
												<div className="mt-6 pt-4 border-t border-gray-200 dark:border-gray-700">
													<h4 className="font-semibold text-gray-900 dark:text-white mb-2">
														Cover Letter
													</h4>
													<div className="bg-gray-50 dark:bg-gray-800 rounded-lg p-4">
														<p className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap leading-relaxed">
															{
																application.coverLetter
															}
														</p>
													</div>

													{/* Action Buttons */}
													<div className="flex gap-3 mt-4">
														<Button
															className="bg-green-600 hover:bg-green-700 text-white"
															onClick={(e) => {
																e.stopPropagation();
																handleAcceptApplication(
																	application.id
																);
															}}
															disabled={
																isAccepting
															}
														>
															{isAccepting ? (
																<>
																	<div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
																	Hiring...
																</>
															) : (
																"Hire This Applicant"
															)}
														</Button>
														<Button
															variant="outline"
															onClick={(e) => {
																e.stopPropagation();
																// TODO: Implement message functionality
																console.log(
																	"Message applicant:",
																	application.applicantId
																);
															}}
															disabled={
																isAccepting
															}
														>
															Send Message
														</Button>
													</div>
												</div>
											)}
									</CardContent>
								</Card>
							);
						})}
					</div>
				)}
			</div>
		</div>
	);
};

export default JobApplicationsPage;
