import React, { useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
	ArrowLeftCircle as CircleArrowLeft,
	CircleEllipsis as Ellipsis,
	Link2,
	Trash2,
} from "lucide-react";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/views/components/ui/dropdown-menu";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/views/components/ui/alert-dialog";
import { Button } from "@/views/components/ui/button";
import toast from "react-hot-toast";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import {
	MapPinIcon,
	CurrencyDollarIcon,
	StarIcon,
	BriefcaseIcon,
	UserGroupIcon,
} from "@heroicons/react/24/outline";
import { StarIcon as StarIconSolid } from "@heroicons/react/24/solid";
import { JobPosting } from "@/features/jobPostings/types";
import TimeAgo from "../../custom/TimeAgo";
import MediaCarousel from "../media/MediaCarousel";
import JobPostingVoteButtons from "./JobPostingVoteButtons";
import EditJobPostingModal from "./EditJobPostingModal";
import SimilarJobs from "./SimilarJobs";
import JobPostingCommentsSection from "../comments/JobPostingCommentsSection";
import PostingTags from "../tags/PostingTags";
import ApplyToJobModal from "./ApplyToJobModal";

interface FullJobPostingWithCommentsProps {
	jobPosting: JobPosting;
	deleteJobPosting: (...args: any) => any;
}

const FullJobPostingWithComments: React.FC<FullJobPostingWithCommentsProps> = ({
	jobPosting,
	deleteJobPosting,
}) => {
	const [isLoggedIn, , , , userProfile] = useIsUserLoggedIn();
	const memoizedJobPosting = useMemo(() => jobPosting, [jobPosting]);

	// Modal states
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [isEditModalOpen, setIsEditModalOpen] = useState(false);
	const [isApplyModalOpen, setIsApplyModalOpen] = useState(false);

	const navigate = useNavigate();

	const handleCopyLink = () => {
		const jobUrl = `${window.location.origin}/jobs/${jobPosting?.id}`;
		navigator.clipboard
			.writeText(jobUrl)
			.then(() => {
				toast.success("Link copied successfully");
			})
			.catch((err) => {
				toast.error("Failed to copy link");
				console.error("Failed to copy link: ", err);
			});
	};

	const handleDeleteJobPosting = async () => {
		deleteJobPosting(jobPosting.id);
		navigate("/jobs");
	};

	const handleOpenApplyModal = () => {
		setIsApplyModalOpen(true);
	};

	const handleViewApplicants = () => {
		navigate(`/job/${jobPosting.id}/applications`);
	};

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

	const isJobAuthor = userProfile?.id === jobPosting.author.id;

	return (
		<>
			<div className="w-full flex justify-center">
				<div className="w-5/6 mx-auto py-4 flex gap-3 max-lg:flex-col max-sm:w-full">
					{/* Main Job Posting Content - Left Side */}
					<div className="w-5/6 max-lg:w-full">
						{jobPosting ? (
							<div className="flex flex-col gap-3">
								<div className="flex flex-col gap-3 bg-card shadow-md rounded-lg p-6 border border-border">
									{/* Job Posting Header */}
									<div className="flex items-center justify-between pb-3 border-b border-border">
										{/* Back Icon */}
										<div
											onClick={() => {
												navigate(-1);
											}}
											className="cursor-pointer"
										>
											<CircleArrowLeft className="text-muted-foreground hover:text-card-foreground hover:cursor-pointer" />
										</div>

										{/* More Options */}
										<DropdownMenu>
											<DropdownMenuTrigger>
												<Ellipsis className="text-muted-foreground hover:text-card-foreground hover:cursor-pointer" />
											</DropdownMenuTrigger>
											<DropdownMenuContent>
												{/* Copy Link */}
												<DropdownMenuItem>
													<div
														className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer"
														onClick={handleCopyLink}
													>
														<Link2 className="w-5" />
														<h6>Copy link</h6>
													</div>
												</DropdownMenuItem>

												{/* Edit Job Posting */}
												{userProfile?.id ===
												jobPosting.author.id ? (
													<DropdownMenuItem>
														<div
															className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer"
															onClick={() =>
																setIsEditModalOpen(
																	true
																)
															}
														>
															<BriefcaseIcon className="w-5" />
															<h6>
																Edit job posting
															</h6>
														</div>
													</DropdownMenuItem>
												) : null}

												{/* Delete Job Posting */}
												{jobPosting.author.id ===
													userProfile?.id && (
													<DropdownMenuItem
														onClick={() =>
															setShowDeleteDialog(
																true
															)
														}
													>
														<div className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer">
															<Trash2 className="w-5 text-red-700" />
															<h6 className="text-red-700">
																Delete job
																posting
															</h6>
														</div>
													</DropdownMenuItem>
												)}
											</DropdownMenuContent>
										</DropdownMenu>

										{/* Delete confirmation dialog */}
										<AlertDialog
											open={showDeleteDialog}
											onOpenChange={setShowDeleteDialog}
										>
											<AlertDialogContent>
												<AlertDialogHeader>
													<AlertDialogTitle>
														Are you absolutely sure?
													</AlertDialogTitle>
													<AlertDialogDescription>
														This action cannot be
														undone. This will
														permanently delete your
														job posting.
													</AlertDialogDescription>
												</AlertDialogHeader>
												<AlertDialogFooter>
													<AlertDialogCancel>
														Cancel
													</AlertDialogCancel>
													<AlertDialogAction
														onClick={() => {
															handleDeleteJobPosting();
															setShowDeleteDialog(
																false
															);
														}}
														className="bg-red-700 hover:bg-red-900"
													>
														Delete
													</AlertDialogAction>
												</AlertDialogFooter>
											</AlertDialogContent>
										</AlertDialog>
									</div>

									{/* Job Posting Content */}
									<div className="flex flex-col justify-center gap-4">
										{/* Client Info */}
										<div className="flex items-center justify-between">
											<div className="flex items-center space-x-3">
												<Link
													to={`/profile/${jobPosting.author.id}`}
												>
													{jobPosting.author
														.profilePictureUrl ? (
														<img
															src={
																jobPosting
																	.author
																	.profilePictureUrl
															}
															width={50}
															height={50}
															className="rounded-full object-cover"
														/>
													) : (
														<img
															src={defaultProfile}
															width={50}
															height={50}
															className="rounded-full"
														/>
													)}
												</Link>
												<div>
													{/* Client Name */}
													<Link
														to={`/profile/${jobPosting.author.id}`}
													>
														<h3 className="text-lg font-semibold text-card-foreground">
															{jobPosting.author
																.firstName +
																jobPosting
																	.author
																	.lastName}
														</h3>
													</Link>
													{/* Client Rating */}
													{jobPosting.author
														.rating && (
														<div className="flex items-center space-x-1">
															<div className="flex">
																{renderStars(
																	jobPosting
																		.author
																		.rating
																)}
															</div>
															<span className="text-sm text-muted-foreground">
																{jobPosting.author.rating.toFixed(
																	1
																)}
															</span>
														</div>
													)}
													{/* Publish Date */}
													<TimeAgo
														timestamp={
															jobPosting.createdAt
														}
													/>
												</div>
											</div>

											{/* Budget Badge */}
											<div className="flex items-center bg-green-100 text-green-800 px-4 py-2 rounded-full">
												<CurrencyDollarIcon className="h-5 w-5 mr-1" />
												<span className="font-bold text-lg">
													{formatBudget(
														jobPosting.budget
													)}
												</span>
											</div>
										</div>

										{/* Job Title */}
										<div className="break-words">
											<h1
												className="text-2xl font-bold text-card-foreground whitespace-pre-wrap"
												dir="auto"
											>
												{jobPosting.title}
											</h1>
										</div>

										{/* Location/Area */}
										{jobPosting.area && (
											<div className="flex items-center text-muted-foreground">
												<MapPinIcon className="h-5 w-5 mr-2" />
												<span className="text-lg">
													{jobPosting.area}
												</span>
											</div>
										)}

										{/* Job Description */}
										<div className="break-words">
											<h3 className="text-lg font-semibold text-card-foreground mb-2">
												Job Description
											</h3>
											<p
												className="text-card-foreground whitespace-pre-wrap leading-relaxed"
												dir="auto"
											>
												{jobPosting.content}
											</p>
										</div>

										{/* Media */}
										<MediaCarousel
											medias={jobPosting.medias}
										/>

										{/* Tags */}
										<PostingTags
											tags={jobPosting.tags}
											language={jobPosting.language}
										/>

										{/* Application Action Button */}
										{isLoggedIn && (
											<div className="border-t border-border pt-4">
												{isJobAuthor ? (
													<Button
														onClick={
															handleViewApplicants
														}
														className="w-full bg-blue-600 hover:bg-blue-700 text-white py-3 text-lg font-semibold"
													>
														<UserGroupIcon className="h-5 w-5 mr-2" />
														View Applicants
													</Button>
												) : (
													<Button
														onClick={
															handleOpenApplyModal
														}
														className="w-full bg-green-600 hover:bg-green-700 text-white py-3 text-lg font-semibold"
														disabled={
															jobPosting.isAppliedFor
														}
													>
														<BriefcaseIcon className="h-5 w-5 mr-2" />
														{jobPosting.isAppliedFor
															? "You have already applied for this Job"
															: "Apply for this Job"}
													</Button>
												)}
											</div>
										)}

										{/* Job Posting Voting */}
										<JobPostingVoteButtons
											jobPosting={jobPosting}
										/>

										{/* Comments */}
										<JobPostingCommentsSection
											jobPostingId={jobPosting.id}
										/>
									</div>
								</div>

								{/* Edit Modal */}
								{userProfile?.id === jobPosting.author.id ? (
									<EditJobPostingModal
										jobPosting={memoizedJobPosting}
										isOpen={isEditModalOpen}
										onClose={() =>
											setIsEditModalOpen(false)
										}
									/>
								) : null}

								{/* Apply Modal */}
								<ApplyToJobModal
									isOpen={isApplyModalOpen}
									onClose={() => setIsApplyModalOpen(false)}
									jobPosting={jobPosting}
								/>
							</div>
						) : (
							<p className="text-card-foreground">
								Job posting not found.
							</p>
						)}
					</div>

					{/* Similar Jobs - Right Side */}
					{jobPosting && (
						<div className="w-1/6 max-lg:w-full">
							<SimilarJobs currentJobId={jobPosting.id} />
						</div>
					)}
				</div>
			</div>
		</>
	);
};

export default FullJobPostingWithComments;
