import React, { useState, useEffect, useCallback } from "react";
import {
	Button,
	TextField,
	Typography,
	Box,
	Modal,
	IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
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
		<Modal
			open={isOpen}
			onClose={handleClose}
			aria-labelledby="edit-job-modal"
			aria-disabled={isLoading}
			className="flex items-center justify-center"
		>
			<div className="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 relative dark:text-white max-h-[90vh] overflow-y-auto">
				{/* Close Button */}
				<div className="absolute top-3 right-3">
					<IconButton onClick={handleClose}>
						<CloseIcon className="dark:text-gray-300" />
					</IconButton>
				</div>

				<Typography
					variant="h6"
					gutterBottom
					id="edit-job-modal"
					className="max-sm:text-md dark:text-white"
				>
					Edit Job Posting
				</Typography>

				<Separator className="dark:bg-gray-600" />

				<Box className="flex flex-col gap-4 w-full mb-4 mt-4">
					{/* Job Title Input */}
					<div className="w-full">
						<TextField
							fullWidth
							label="Job Title"
							variant="outlined"
							value={title}
							onChange={handleTitleChange}
							placeholder="e.g., Frontend Developer for E-commerce Platform"
							slotProps={{
								htmlInput: {
									maxLength: TITLE_MAX_LENGTH,
									dir: "auto",
									className: "text-lg dark:text-white",
								},
							}}
							required
							error={!!titleError}
							helperText={titleError || ""}
							className="dark:bg-gray-700 dark:rounded"
							InputLabelProps={{
								className: "dark:text-gray-300",
							}}
						/>
						{!titleError && (
							<div className="flex justify-end mt-1">
								<Typography
									variant="caption"
									color={
										titleCharsLeft < 1 ? "error" : "green"
									}
									className={
										titleCharsLeft < 1
											? "text-red-500"
											: "text-green-500 dark:text-green-400"
									}
								>
									{titleCharsLeft} characters left
								</Typography>
							</div>
						)}
					</div>

					{/* Budget and Area Row */}
					<div className="flex gap-4 w-full">
						{/* Budget Input */}
						<div className="flex-1">
							<TextField
								fullWidth
								label="Budget ($)"
								variant="outlined"
								value={budget}
								onChange={handleBudgetChange}
								placeholder="e.g., 5000"
								type="number"
								slotProps={{
									htmlInput: {
										min: 1,
										max: 1000000,
										className: "text-lg dark:text-white",
									},
								}}
								required
								error={!!budgetError}
								helperText={budgetError || ""}
								className="dark:bg-gray-700 dark:rounded"
								InputLabelProps={{
									className: "dark:text-gray-300",
								}}
							/>
						</div>

						{/* Area/Location Input */}
						<div className="flex-1">
							<TextField
								fullWidth
								label="Location/Area"
								variant="outlined"
								value={area}
								onChange={handleAreaChange}
								placeholder="e.g., Remote, New York, London"
								slotProps={{
									htmlInput: {
										maxLength: AREA_MAX_LENGTH,
										dir: "auto",
										className: "text-lg dark:text-white",
									},
								}}
								error={!!areaError}
								helperText={areaError || ""}
								className="dark:bg-gray-700 dark:rounded"
								InputLabelProps={{
									className: "dark:text-gray-300",
								}}
							/>
							{!areaError && area && (
								<div className="flex justify-end mt-1">
									<Typography
										variant="caption"
										color={
											areaCharsLeft < 1
												? "error"
												: "green"
										}
										className={
											areaCharsLeft < 1
												? "text-red-500"
												: "text-green-500 dark:text-green-400"
										}
									>
										{areaCharsLeft} characters left
									</Typography>
								</div>
							)}
						</div>
					</div>

					{/* Job Description Input */}
					<div className="w-full">
						<TextField
							fullWidth
							label="Job Description"
							variant="outlined"
							multiline
							rows={6}
							value={content}
							onChange={handleContentChange}
							placeholder="Describe the job requirements, expectations, and any other relevant details..."
							slotProps={{
								htmlInput: {
									maxLength: CONTENT_MAX_LENGTH,
									dir: "auto",
									className: "text-md dark:text-white",
								},
							}}
							required
							error={!!contentError}
							helperText={contentError || ""}
							className="dark:bg-gray-700 dark:rounded"
							InputLabelProps={{
								className: "dark:text-gray-300",
							}}
						/>
						{!contentError && (
							<div className="flex justify-end mt-1">
								<Typography
									variant="caption"
									color={
										contentCharsLeft < 1 ? "error" : "green"
									}
									className={
										contentCharsLeft < 1
											? "text-red-500"
											: "text-green-500 dark:text-green-400"
									}
								>
									{contentCharsLeft} characters left
								</Typography>
							</div>
						)}
					</div>
				</Box>

				{/* Action Buttons */}
				<div className="flex justify-end space-x-3">
					<Button
						variant="outlined"
						onClick={handleClose}
						disabled={isLoading}
						className="dark:text-gray-300 dark:border-gray-600"
					>
						Cancel
					</Button>
					<Button
						variant="contained"
						onClick={handleSubmit}
						disabled={isLoading}
						sx={{
							backgroundColor: "#162955",
							"&:hover": { backgroundColor: "#0e1c3b" },
						}}
						className="dark:bg-blue-700 dark:hover:bg-blue-800"
					>
						{isLoading ? "Updating..." : "Update Job Posting"}
					</Button>
				</div>
			</div>
		</Modal>
	);
};

export default EditJobPostingModal;
