import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCreateJobPostingMutation } from "@/features/jobPostings/jobPostingsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import toast from "react-hot-toast";
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";
import FileUploadForm from "@/views/components/custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import { z } from "zod";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { Separator } from "@/views/components/ui/separator";
import { Button } from "@/views/components/ui/button";
import { Input } from "@/views/components/ui/input";
import { Textarea } from "@/views/components/ui/textarea";
import {
	Field,
	FieldLabel,
	FieldError,
	FieldDescription,
} from "@/views/components/ui/field";
import { ArrowLeft, Loader2, Briefcase } from "lucide-react";

// Character limits
const TITLE_MAX_LENGTH = 256;
const CONTENT_MAX_LENGTH = 5000;
const AREA_MAX_LENGTH = 100;

// Zod schema for form validation
const jobPostingSchema = z.object({
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

const CreateJobPostingPage = () => {
	const navigate = useNavigate();
	const [title, setTitle] = useState("");
	const [content, setContent] = useState("");
	const [budget, setBudget] = useState("");
	const [area, setArea] = useState("");
	const [titleError, setTitleError] = useState("");
	const [contentError, setContentError] = useState("");
	const [budgetError, setBudgetError] = useState("");
	const [areaError, setAreaError] = useState("");
	const [mediaList, setMediaList] = useState<MediaObject[]>([]);
	const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
	const [_isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();

	const [createJobPosting, createJobResult] = useCreateJobPostingMutation();

	const { uploadResult, uploadMedia } = useCallbackOnMediaUploadSuccess(
		createJobPosting,
		{ title, content, budget: Number(budget), area: area || "Remote" }
	);

	const { isSuccess, isError, isLoading } = createJobResult;

	// Track unsaved changes
	useEffect(() => {
		setHasUnsavedChanges(
			title.trim() !== "" ||
				content.trim() !== "" ||
				budget.trim() !== "" ||
				area.trim() !== ""
		);
	}, [title, content, budget, area]);

	// Warn before leaving with unsaved changes
	useEffect(() => {
		const handleBeforeUnload = (e: BeforeUnloadEvent) => {
			if (hasUnsavedChanges) {
				e.preventDefault();
				e.returnValue = "";
			}
		};

		window.addEventListener("beforeunload", handleBeforeUnload);
		return () =>
			window.removeEventListener("beforeunload", handleBeforeUnload);
	}, [hasUnsavedChanges]);

	// Auto-save to localStorage
	useEffect(() => {
		if (hasUnsavedChanges) {
			const draft = {
				title,
				content,
				budget,
				area,
				timestamp: Date.now(),
			};
			localStorage.setItem("job-posting-draft", JSON.stringify(draft));
		}
	}, [title, content, budget, area, hasUnsavedChanges]);

	// Load draft on mount
	useEffect(() => {
		const savedDraft = localStorage.getItem("job-posting-draft");
		if (savedDraft) {
			try {
				const draft = JSON.parse(savedDraft);
				// Only load if draft is less than 24 hours old
				if (Date.now() - draft.timestamp < 24 * 60 * 60 * 1000) {
					setTitle(draft.title || "");
					setContent(draft.content || "");
					setBudget(draft.budget || "");
					setArea(draft.area || "");
					toast.success("Draft restored");
				} else {
					localStorage.removeItem("job-posting-draft");
				}
			} catch (error) {
				console.error("Error loading draft:", error);
			}
		}
	}, []);

	const resetForm = useCallback(() => {
		setTitle("");
		setContent("");
		setBudget("");
		setArea("");
		setTitleError("");
		setContentError("");
		setBudgetError("");
		setAreaError("");
		setMediaList([]);
		localStorage.removeItem("job-posting-draft");
		setHasUnsavedChanges(false);
	}, []);

	useEffect(() => {
		if (isError)
			toast.error("An error occurred while creating your job posting");
		if (isSuccess) {
			toast.success("Job posting created successfully");
			resetForm();
			navigate("/");
		}
	}, [isSuccess, isError, navigate, resetForm]);

	const handleSubmit = async () => {
		try {
			jobPostingSchema.parse({
				title,
				content,
				budget: Number(budget),
				area: area || undefined,
			});
			setTitleError("");
			setContentError("");
			setBudgetError("");
			setAreaError("");
			await uploadMedia({ mediaList });
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
	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
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
		// Only allow numbers
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

	const handleBack = () => {
		if (hasUnsavedChanges) {
			if (
				window.confirm(
					"You have unsaved changes. Are you sure you want to leave?"
				)
			) {
				navigate(-1);
			}
		} else {
			navigate(-1);
		}
	};

	const titleCharsLeft = TITLE_MAX_LENGTH - title.length;
	const contentCharsLeft = CONTENT_MAX_LENGTH - content.length;
	const areaCharsLeft = AREA_MAX_LENGTH - area.length;

	return (
		<div className="w-full max-w-4xl mx-auto px-4 py-8">
			{/* Header */}
			<div className="mb-6">
				<Button
					variant="ghost"
					onClick={handleBack}
					className="mb-4 hover:bg-muted"
				>
					<ArrowLeft className="w-4 h-4 mr-2" />
					Back
				</Button>
				<div className="flex items-center gap-3">
					<div className="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
						<Briefcase className="w-6 h-6 text-primary" />
					</div>
					<h1 className="text-3xl font-bold text-card-foreground">
						Post a Job Opportunity
					</h1>
				</div>
			</div>

			<Separator className="mb-6" />

			{/* User Profile Info */}
			<div className="flex items-center gap-3 p-4 bg-muted/30 rounded-lg mb-6">
				<div>
					{userProfile?.profilePictureUrl ? (
						<img
							src={userProfile.profilePictureUrl}
							width={48}
							height={48}
							className="rounded-full ring-2 ring-border"
							alt="Profile"
						/>
					) : (
						<img
							src={defaultProfile}
							alt="Profile Picture"
							width={48}
							height={48}
							className="rounded-full ring-2 ring-border"
						/>
					)}
				</div>
				<div>
					<div className="font-semibold text-card-foreground">
						{authUser?.displayName || "User"}
					</div>
					<div className="text-sm text-muted-foreground">
						{` @${userProfile?.username || "username"}`}
					</div>
				</div>
			</div>

			{/* Form */}
			<div className="space-y-6">
				{/* Job Title Input */}
				<Field>
					<FieldLabel>Job Title *</FieldLabel>
					<Input
						value={title}
						onChange={handleTitleChange}
						placeholder="e.g., Frontend Developer for E-commerce Platform"
						maxLength={TITLE_MAX_LENGTH}
						dir="auto"
						className="text-lg"
					/>
					{titleError && <FieldError>{titleError}</FieldError>}
					{!titleError && (
						<FieldDescription>
							<span
								className={
									titleCharsLeft < 1
										? "text-destructive"
										: "text-green-600"
								}
							>
								{titleCharsLeft} characters left
							</span>
						</FieldDescription>
					)}
				</Field>

				{/* Budget and Area Row */}
				<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
					{/* Budget Input */}
					<Field>
						<FieldLabel>Budget (USD) *</FieldLabel>
						<Input
							value={budget}
							onChange={handleBudgetChange}
							placeholder="e.g., 5000"
							type="number"
						/>
						{budgetError && <FieldError>{budgetError}</FieldError>}
						{!budgetError && (
							<FieldDescription>
								Enter the total budget for this project
							</FieldDescription>
						)}
					</Field>

					{/* Area/Location Input */}
					<Field>
						<FieldLabel>Location</FieldLabel>
						<Input
							value={area}
							onChange={handleAreaChange}
							placeholder="e.g., Remote, New York, London"
							maxLength={AREA_MAX_LENGTH}
						/>
						{areaError && <FieldError>{areaError}</FieldError>}
						{!areaError && area && (
							<FieldDescription>
								<span
									className={
										areaCharsLeft < 1
											? "text-destructive"
											: "text-green-600"
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
						onChange={handleContentChange}
						placeholder="Describe the job requirements, expectations, and any other relevant details..."
						maxLength={CONTENT_MAX_LENGTH}
						dir="auto"
						className="min-h-[200px]"
					/>
					{contentError && <FieldError>{contentError}</FieldError>}
					{!contentError && (
						<FieldDescription>
							<span
								className={
									contentCharsLeft < 1
										? "text-destructive"
										: "text-green-600"
								}
							>
								{contentCharsLeft} characters left
							</span>
						</FieldDescription>
					)}
				</Field>

				{/* Media Upload Section */}
				<div className="border border-border rounded-lg p-3 bg-muted/20">
					<div className="mb-2">
						<p className="text-sm text-muted-foreground text-center">
							Upload images or videos to showcase your job
							opportunity (up to 3 files)
						</p>
					</div>
					<FileUploadForm
						onSubmit={() => {}}
						setParentMediaList={setMediaList}
					/>
				</div>

				{/* Action Buttons */}
				<div className="flex justify-center gap-3 pt-4">
					<Button
						variant="outline"
						onClick={handleBack}
						disabled={isLoading || uploadResult.isLoading}
						className="rounded-full px-8"
					>
						Cancel
					</Button>
					<Button
						onClick={handleSubmit}
						disabled={isLoading || uploadResult.isLoading}
						className="bg-primary hover:bg-primary/90 text-primary-foreground rounded-full px-8"
					>
						{isLoading || uploadResult.isLoading ? (
							<>
								<Loader2 className="w-4 h-4 mr-2 animate-spin" />
								Posting...
							</>
						) : (
							"Post Job Opportunity"
						)}
					</Button>
				</div>
			</div>

			{/* Auto-save indicator */}
			{hasUnsavedChanges && (
				<div className="mt-4 text-center text-sm text-muted-foreground">
					Draft auto-saved
				</div>
			)}
		</div>
	);
};

export default CreateJobPostingPage;
