import React, { useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import CommentsSection from "../comments/CommentsSection";
import { ArrowLeftCircle as CircleArrowLeft } from "lucide-react";
import { CircleEllipsis as Ellipsis } from "lucide-react";
import { Link2 } from "lucide-react";
import DeleteIcon from "@mui/icons-material/Delete";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
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

import toast from "react-hot-toast";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import {
  MapPinIcon,
  CurrencyDollarIcon,
  StarIcon,
  BriefcaseIcon
} from '@heroicons/react/24/outline';
import { StarIcon as StarIconSolid } from '@heroicons/react/24/solid';
import { JobPosting } from "@/features/jobPostings/types";
import { useGetSimilarJobsQuery } from "@/features/jobPostings/jobPostingsSlice";
import TimeAgo from "../../custom/TimeAgo";
import MediaCarousel from "../media/MediaCarousel";
import JobPostingVoteButtons from "./JobPostingVoteButtons";
import EditJobPostingModal from "./EditJobPostingModal";
import SimilarJobs from "./SimilarJobs";
import JobPostingCommentsSection from "../comments/JobPostingCommentsSection";

interface FullJobPostingWithCommentsProps {
  jobPosting: JobPosting;
  deleteJobPosting: (...args: any) => any;
}

const FullJobPostingWithComments: React.FC<FullJobPostingWithCommentsProps> = ({
  jobPosting,
  deleteJobPosting,
}) => {
  const [, , , , userProfile] = useIsUserLoggedIn();
  const memoizedJobPosting = useMemo(() => jobPosting, [jobPosting]);

  const {
    data: similarJobs,
    error: similarJobsError,
    isLoading: similarJobsLoading,
  } = useGetSimilarJobsQuery(jobPosting.id, { skip: !jobPosting.id });

  // confirm delete dialog
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
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
          <StarIconSolid className="absolute inset-0 h-4 w-4 text-yellow-400 overflow-hidden" style={{ width: '50%' }} />
        </div>
      );
    }

    const remainingStars = 5 - Math.ceil(rating);
    for (let i = 0; i < remainingStars; i++) {
      stars.push(
        <StarIcon key={`empty-${i}`} className="h-4 w-4 text-gray-300" />
      );
    }

    return stars;
  };

  return (
    <>
      <div className="w-full flex justify-center">
        <div className="w-5/6 mx-auto py-4 flex gap-3 max-lg:flex-col max-sm:w-full">
          {/* Main Job Posting Content - Left Side */}
          <div className="w-5/6 max-lg:w-full">
            {jobPosting ? (
              <div className="flex flex-col gap-3">
                <div className="flex flex-col gap-3 bg-white dark:bg-gray-800 shadow-md rounded-lg p-6 border border-gray-200 dark:border-gray-700">
                  {/* Job Posting Header */}
                  <div className="flex items-center justify-between pb-3 border-b border-gray-300 dark:border-gray-600">
                    {/* Back Icon */}
                    <div
                      onClick={() => {
                        navigate(-1);
                      }}
                      className="cursor-pointer"
                    >
                      <CircleArrowLeft className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:cursor-pointer" />
                    </div>

                    {/* More Options */}
                    <DropdownMenu>
                      <DropdownMenuTrigger>
                        <Ellipsis className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:cursor-pointer" />
                      </DropdownMenuTrigger>
                      <DropdownMenuContent>
                        {/* Copy Link */}
                        <DropdownMenuItem>
                          <div
                            className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
                            onClick={handleCopyLink}
                          >
                            <Link2 className="w-5" />
                            <h6>Copy link</h6>
                          </div>
                        </DropdownMenuItem>

                        {/* Edit Job Posting */}
                        {userProfile?.id === jobPosting.author.id ? (
                          <DropdownMenuItem>
                            <div
                              className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
                              onClick={() => setIsEditModalOpen(true)}
                            >
                              <BriefcaseIcon className="w-5" />
                              <h6>Edit job posting</h6>
                            </div>
                          </DropdownMenuItem>
                        ) : null}

                        {/* Delete Job Posting */}
                        {jobPosting.author.id === userProfile?.id && (
                          <DropdownMenuItem
                            onClick={() => setShowDeleteDialog(true)}
                          >
                            <div className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer">
                              <DeleteIcon className="w-5 text-red-700" />
                              <h6 className="text-red-700">Delete job posting</h6>
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
                            This action cannot be undone. This will permanently
                            delete your job posting.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => {
                              handleDeleteJobPosting();
                              setShowDeleteDialog(false);
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
                        <Link to={`/profile/${jobPosting.author.id}`}>
                          {jobPosting.author.profilePictureUrl ? (
                            <img
                              src={jobPosting.author.profilePictureUrl}
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
                          <Link to={`/profile/${jobPosting.author.id}`}>
                            <h3 className="text-lg font-semibold dark:text-white">
                              {jobPosting.author.firstName + jobPosting.author.lastName}
                            </h3>
                          </Link>
                          {/* Client Rating */}
                          {jobPosting.author.rating && (
                            <div className="flex items-center space-x-1">
                              <div className="flex">
                                {renderStars(jobPosting.author.rating)}
                              </div>
                              <span className="text-sm text-gray-600 dark:text-gray-400">
                                {jobPosting.author.rating.toFixed(1)}
                              </span>
                            </div>
                          )}
                          {/* Publish Date */}
                          <TimeAgo timestamp={jobPosting.createdAt} />
                        </div>
                      </div>

                      {/* Budget Badge */}
                      <div className="flex items-center bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 px-4 py-2 rounded-full">
                        <CurrencyDollarIcon className="h-5 w-5 mr-1" />
                        <span className="font-bold text-lg">{formatBudget(jobPosting.budget)}</span>
                      </div>
                    </div>

                    {/* Job Title */}
                    <div className="break-words">
                      <h1
                        className="text-2xl font-bold text-gray-900 dark:text-white whitespace-pre-wrap"
                        dir="auto"
                      >
                        {jobPosting.title}
                      </h1>
                    </div>

                    {/* Location/Area */}
                    {jobPosting.area && (
                      <div className="flex items-center text-gray-600 dark:text-gray-400">
                        <MapPinIcon className="h-5 w-5 mr-2" />
                        <span className="text-lg">{jobPosting.area}</span>
                      </div>
                    )}

                    {/* Skills/Tags */}
                    {jobPosting.postTags && jobPosting.postTags.length > 0 && (
                      <div className="flex flex-wrap gap-2">
                        {jobPosting.postTags.map((tag) => (
                          <span
                            key={tag.id}
                            className="px-3 py-1 bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 text-sm rounded-full font-medium"
                          >
                            {tag.name}
                          </span>
                        ))}
                      </div>
                    )}

                    {/* Job Description */}
                    <div className="break-words">
                      <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                        Job Description
                      </h3>
                      <p
                        className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap leading-relaxed"
                        dir="auto"
                      >
                        {jobPosting.content}
                      </p>
                    </div>

                    {/* Media */}
                    <MediaCarousel medias={jobPosting.medias} />

                    {/* Job Posting Voting */}
                    <JobPostingVoteButtons jobPosting={jobPosting} />

                    {/* Comments */}
                    <JobPostingCommentsSection jobPostingId={jobPosting.id} />
                  </div>
                </div>

                {/* Edit Modal */}
                {userProfile?.id === jobPosting.author.id ? (
                  <EditJobPostingModal
                    jobPosting={memoizedJobPosting}
                    isOpen={isEditModalOpen}
                    onClose={() => setIsEditModalOpen(false)}
                  />
                ) : null}
              </div>
            ) : (
              <p className="dark:text-white">Job posting not found.</p>
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
