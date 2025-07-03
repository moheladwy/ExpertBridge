import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogClose,
} from "@/views/components/custom/dialog";

import { Button } from "@/views/components/ui/button";
import { Input } from "@/views/components/ui/input";
import { Textarea } from "@/views/components/ui/textarea";
import { Label } from "@/views/components/ui/label";
import toast from "react-hot-toast";
import { JobPosting, ApplyToJobPostingRequest } from "@/features/jobPostings/types";
import { useApplyToJobPostingMutation } from "@/features/jobPostings/jobPostingsSlice";

interface ApplyJobModalProps {
  isOpen: boolean;
  onClose: () => void;
  jobPosting: JobPosting;
}

const ApplyToJobModal: React.FC<ApplyJobModalProps> = ({
  isOpen,
  onClose,
  jobPosting,
}) => {
  const [applyToJobPosting, { isLoading: isApplying }] = useApplyToJobPostingMutation();
  
  // Application form states
  const [coverLetter, setCoverLetter] = useState("");
  const [offeredCost, setOfferedCost] = useState(0);

  // Reset form when modal opens
  useEffect(() => {
    if (isOpen) {
      setCoverLetter("I would be delighted to work on this project for you and deliver excellent results.");
      setOfferedCost(jobPosting.budget);
    }
  }, [isOpen, jobPosting.budget]);

  const formatBudget = (budget: number) => {
    if (budget >= 1000) {
      return `$${(budget / 1000).toFixed(1)}k`;
    }
    return `$${budget}`;
  };

  const handleApplyToJob = async () => {
    if (!coverLetter.trim()) {
      toast.error("Please provide a cover letter");
      return;
    }

    if (offeredCost <= 0) {
      toast.error("Please provide a valid offered cost");
      return;
    }

    try {
      const applicationData: ApplyToJobPostingRequest = {
        jobPostingId: jobPosting.id,
        coverLetter: coverLetter.trim(),
        offeredCost: offeredCost,
      };

      await applyToJobPosting(applicationData).unwrap();
      toast.success("Application submitted successfully!");
      onClose();
    } catch (error) {
      toast.error("Failed to submit application. Please try again.");
      console.error("Application error:", error);
    }
  };

  const handleClose = () => {
    if (!isApplying) {
      onClose();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Apply for Job</DialogTitle>
          <DialogDescription>
            Submit your application for "{jobPosting.title}"
          </DialogDescription>
        </DialogHeader>
        <div className="space-y-4 py-4">
          <div className="space-y-2">
            <Label htmlFor="coverLetter">Cover Letter</Label>
            <Textarea
              id="coverLetter"
              placeholder="Tell the client why you're the right person for this job..."
              value={coverLetter}
              onChange={(e) => setCoverLetter(e.target.value)}
              rows={4}
              className="resize-none"
              disabled={isApplying}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="offeredCost">Your Proposed Cost ($)</Label>
            <Input
              id="offeredCost"
              type="number"
              min="1"
              value={offeredCost}
              onChange={(e) => setOfferedCost(Number(e.target.value))}
              placeholder="Enter your proposed cost"
              disabled={isApplying}
            />
            <p className="text-sm text-gray-500 dark:text-gray-400">
              Job budget: {formatBudget(jobPosting.budget)}
            </p>
          </div>
        </div>
        <DialogFooter className="flex gap-2">
          <Button
            variant="outline"
            onClick={handleClose}
            disabled={isApplying}
          >
            Cancel
          </Button>
          <Button
            onClick={handleApplyToJob}
            disabled={isApplying || !coverLetter.trim() || offeredCost <= 0}
            className="bg-green-600 hover:bg-green-700"
          >
            {isApplying ? "Submitting..." : "Submit Application"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

export default ApplyToJobModal;
