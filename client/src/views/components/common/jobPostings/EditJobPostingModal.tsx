import React, { useState, useEffect, useCallback } from "react";
import { Button } from "@/views/components/ui/button";
import {
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTitle,
} from "@/views/components/ui/dialog";
import {
	Field,
	FieldDescription,
	FieldError,
	FieldLabel,
} from "@/views/components/ui/field";
import { Input } from "@/views/components/ui/input";
import { Textarea } from "@/views/components/ui/textarea";
import { z } from "zod";
import toast from "react-hot-toast";
import { Separator } from "@/views/components/ui/separator";
import { useUpdateJobPostingMutation } from "@/features/jobPostings/jobPostingsSlice";
import { JobPosting } from "@/features/jobPostings/types";

// Character limits
const TITLE_MAX_LENGTH = 256;
const CONTENT_MAX_LENGTH = 5000;
const AREA_MAX_LENGTH = 100;

// Zod schema for form validation
const editJobPostingSchema = z.object({
	title: z
		.string()
		.min(1, "Job title is required")
		.max(TITLE_MAX_LENGTH, "Title cannot exceed 256 characters")
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 3, {
			message: "Title must be at least 3 words",
		}),
	content: z
		.string()
		.min(1, "Job description is required")
		.max(CONTENT_MAX_LENGTH, "Description cannot exceed 5000 characters")
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 10, {
			message: "Description must be at least 10 words",
		}),
	budget: z
		.number()
		.min(1, "Budget must be at least $1")
		.max(1000000, "Budget cannot exceed $1,000,000"),
	area: z
		.string()
		.max(AREA_MAX_LENGTH, "Location cannot exceed 100 characters")
		.optional(),
});

interface EditJobPostingModalProps {
	jobPosting: JobPosting;
	isOpen: boolean;
	onClose: () => void;
}

const EditJobPostingModal: React.FC<EditJobPostingModalProps> = ({
	jobPosting,
	isOpen,
	onClose,
}) => {
	const [title, setTitle] = useState(jobPosting.title);
	const [content, setContent] = useState(jobPosting.content);
	const [budget, setBudget] = useState(jobPosting.budget.toString());
	const [area, setArea] = useState(jobPosting.area || "");
	const [titleError, setTitleError] = useState("");
	const [contentError, setContentError] = useState("");
	const [budgetError, setBudgetError] = useState("");
	const [areaError, setAreaError] = useState("");

	const [updateJobPosting, { isLoading, isSuccess, isError }] =
		useUpdateJobPostingMutation();

	// Reset form when modal opens with new job posting data
	useEffect(() => {
		if (isOpen) {
			setTitle(jobPosting.title);
			setContent(jobPosting.content);
			setBudget(jobPosting.budget.toString());
			setArea(jobPosting.area || "");
			setTitleError("");
			setContentError("");
			setBudgetError("");
			setAreaError("");
		}
	}, [isOpen, jobPosting]);

	useEffect(() => {
		if (isError)
			toast.error("An error occurred while updating your job posting");
		if (isSuccess) {
			toast.success("Job posting updated successfully");
			onClose();
		}
	}, [isSuccess, isError, onClose]);

	const resetForm = useCallback(() => {
		setTitle(jobPosting.title);
		setContent(jobPosting.content);
		setBudget(jobPosting.budget.toString());
		setArea(jobPosting.area || "");
		setTitleError("");
		setContentError("");
		setBudgetError("");
		setAreaError("");
	}, [jobPosting]);

	const handleClose = useCallback(() => {
		onClose();
		resetForm();
	}, [onClose, resetForm]);

	const handleSubmit = async () => {
		try {
			editJobPostingSchema.parse({
				title,
				content,
				budget: Number(budget),
				area: area || undefined,
			});
			setTitleError("");
			setContentError("");
			setBudgetError("");
			setAreaError("");

			await updateJobPosting({
				postingId: jobPosting.id,
				title: title.trim(),
				content: content.trim(),
				budget: Number(budget),
				area: area.trim() || undefined,
			}).unwrap();
		} catch (err) {
			if (err instanceof z.ZodError) {
				err.errors.forEach((error) => {
					if (error.path[0] === "title") {
						setTitleError(error.message);
					} else if (error.path[0] === "content") {
						setContentError(error.message);
					} else if (error.path[0] === "budget") {
						setBudgetError(error.message);
					} else if (error.path[0] === "area") {
						setAreaError(error.message);
					}
				});
			}
		}
	};

	// Handle title input with character limit
	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setTitle(newValue);
			if (newValue.trim()) {
				setTitleError("");
			}
		}
	};

	// Handle content input with character limit
	const handleContentChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= CONTENT_MAX_LENGTH) {
			setContent(newValue);
			if (newValue.trim()) {
				setContentError("");
			}
		}
	};

	// Handle budget input
	const handleBudgetChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue === "" || /^\d+$/.test(newValue)) {
			setBudget(newValue);
			if (newValue.trim()) {
				setBudgetError("");
			}
		}
	};

	// Handle area input with character limit
	const handleAreaChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= AREA_MAX_LENGTH) {
			setArea(newValue);
			if (areaError) {
				setAreaError("");
			}
		}
	};

	const titleCharsLeft = TITLE_MAX_LENGTH - title.length;
	const contentCharsLeft = CONTENT_MAX_LENGTH - content.length;
	const areaCharsLeft = AREA_MAX_LENGTH - area.length;

	return (
		<Dialog open={isOpen} onOpenChange={(open) => !open && handleClose()}>
			<DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
				<DialogHeader>
					<DialogTitle className="text-center text-2xl font-bold">
						Edit Job Posting
					</DialogTitle>
					<Separator className="dark:bg-gray-600" />
				</DialogHeader>

				<div className="flex flex-col gap-4 w-full mb-4 mt-4">
					{/* Job Title Input */}
					<Field>
						<FieldLabel>Job Title *</FieldLabel>
						<Input
							value={title}
							onChange={handleTitleChange}
							placeholder="e.g., Frontend Developer for E-commerce Platform"
							maxLength={TITLE_MAX_LENGTH}
							dir="auto"
							className="dark:bg-gray-700"
						/>
						{titleError && <FieldError>{titleError}</FieldError>}
						{!titleError && (
							<FieldDescription>
								<span
									className={
										titleCharsLeft < 1
											? "text-red-500"
											: "text-green-500 dark:text-green-400"
									}
								>
									{titleCharsLeft} characters left
								</span>
							</FieldDescription>
						)}
					</Field>

					{/* Budget and Area Row */}
					<div className="flex gap-4 w-full">
						{/* Budget Input */}
						<Field className="flex-1">
							<FieldLabel>Budget (USD) *</FieldLabel>
							<Input
								value={budget}
								onChange={handleBudgetChange}
								placeholder="e.g., 5000"
								type="number"
								className="dark:bg-gray-700"
							/>
							{budgetError && (
								<FieldError>{budgetError}</FieldError>
							)}
						</Field>

						{/* Area/Location Input */}
						<Field className="flex-1">
							<FieldLabel>Location</FieldLabel>
							<Input
								value={area}
								onChange={handleAreaChange}
								placeholder="e.g., Remote, New York, London"
								maxLength={AREA_MAX_LENGTH}
								className="dark:bg-gray-700"
							/>
							{areaError && <FieldError>{areaError}</FieldError>}
							{!areaError && area && (
								<FieldDescription>
									<span
										className={
											areaCharsLeft < 1
												? "text-red-500"
												: "text-green-500 dark:text-green-400"
										}
									>
										{areaCharsLeft} characters left
									</span>
								</FieldDescription>
							)}
						</Field>
					</div>

					{/* Job Description Input */}
					<Field>
						<FieldLabel>Job Description *</FieldLabel>
						<Textarea
							value={content}
							onChange={(
								e: React.ChangeEvent<HTMLTextAreaElement>
							) => handleContentChange(e as any)}
							placeholder="Describe the job requirements, expectations, and any other relevant details..."
							maxLength={CONTENT_MAX_LENGTH}
							dir="auto"
							className="min-h-[150px] resize-none dark:bg-gray-700"
						/>
						{contentError && (
							<FieldError>{contentError}</FieldError>
						)}
						{!contentError && (
							<FieldDescription>
								<span
									className={
										contentCharsLeft < 1
											? "text-red-500"
											: "text-green-500 dark:text-green-400"
									}
								>
									{contentCharsLeft} characters left
								</span>
							</FieldDescription>
						)}
					</Field>
				</div>

				{/* Action Buttons */}
				<div className="flex justify-end space-x-3">
					<Button
						onClick={handleClose}
						disabled={isLoading}
						className="dark:text-gray-300 dark:border-gray-600"
					>
						Cancel
					</Button>
					<Button
						onClick={handleSubmit}
						disabled={isLoading}
						className="bg-[#162955] hover:bg-[#0e1c3b] dark:bg-blue-700 dark:hover:bg-blue-800"
					>
						{isLoading ? "Updating..." : "Update Job Posting"}
					</Button>
				</div>
			</DialogContent>
		</Dialog>
	);
};

export default EditJobPostingModal;
