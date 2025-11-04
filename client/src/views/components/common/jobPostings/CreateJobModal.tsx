import { useCallback, useEffect, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import toast from "react-hot-toast";
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
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";
import FileUploadForm from "@/views/components/custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import { z } from "zod";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Separator } from "@/views/components/ui/separator";
import { useCreateJobPostingMutation } from "@/features/jobPostings/jobPostingsSlice";
import { Briefcase } from "lucide-react";

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

const CreateJobModal: React.FC = () => {
	useEffect(() => {
		console.log("job modal mounting...");
	}, []);

	const [open, setOpen] = useState(false);
	const [title, setTitle] = useState("");
	const [content, setContent] = useState("");
	const [budget, setBudget] = useState("");
	const [area, setArea] = useState("");
	const [titleError, setTitleError] = useState("");
	const [contentError, setContentError] = useState("");
	const [budgetError, setBudgetError] = useState("");
	const [areaError, setAreaError] = useState("");
	const [mediaList, setMediaList] = useState<MediaObject[]>([]);
	const [isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();
	const { showAuthPrompt } = useAuthPrompt();

	const [createJobPosting, createJobResult] = useCreateJobPostingMutation();

	const { uploadResult, uploadMedia } = useCallbackOnMediaUploadSuccess(
		createJobPosting,
		{ title, content, budget: Number(budget), area: area || "Remote" }
	);

	const { isSuccess, isError, isLoading } = createJobResult;

	const resetForm = useCallback(() => {
		setTitle("");
		setContent("");
		setBudget("");
		setArea("");
		setTitleError("");
		setContentError("");
		setBudgetError("");
		setAreaError("");
	}, []);

	const handleClose = useCallback(() => {
		setOpen(false);
		resetForm();
	}, [setOpen, resetForm]);

	useEffect(() => {
		if (isError)
			toast.error("An error occurred while creating your job posting");
		if (isSuccess) {
			toast.success("Job posting created successfully");
			handleClose();
		}
	}, [isSuccess, isError, handleClose]);

	const handleOpen = () => {
		if (isLoggedIn) {
			setOpen(true);
		} else {
			showAuthPrompt();
		}
	};

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
				// Set errors for specific fields
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
				setTitleError(""); // Clear error only if there's valid content
			}
		}
	};

	// Handle content input with character limit
	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= CONTENT_MAX_LENGTH) {
			setContent(newValue);
			if (newValue.trim()) {
				setContentError(""); // Clear error only if there's valid content
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
				setBudgetError(""); // Clear error only if there's valid content
			}
		}
	};

	// Handle area input with character limit
	const handleAreaChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= AREA_MAX_LENGTH) {
			setArea(newValue);
			if (areaError) {
				setAreaError(""); // Clear error
			}
		}
	};

	const titleCharsLeft = TITLE_MAX_LENGTH - title.length;
	const contentCharsLeft = CONTENT_MAX_LENGTH - content.length;
	const areaCharsLeft = AREA_MAX_LENGTH - area.length;

	return (
		<>
			<div
				className="flex justify-center items-center gap-3 bg-card border border-border shadow-sm hover:shadow-md rounded-xl p-4 cursor-pointer transition-all duration-200"
				onClick={handleOpen}
			>
				{isLoggedIn && (
					<div className="flex justify-center items-center">
						{userProfile?.profilePictureUrl ? (
							<img
								src={userProfile.profilePictureUrl}
								width={45}
								height={45}
								className="rounded-full ring-2 ring-border"
								alt="Profile Picture"
							/>
						) : (
							<img
								src={defaultProfile}
								alt="Profile Picture"
								width={45}
								height={45}
								className="rounded-full ring-2 ring-border"
							/>
						)}
					</div>
				)}
				<Button className="bg-muted text-muted-foreground px-5 hover:bg-primary/10 hover:text-primary w-full rounded-full font-medium">
					<div className="w-full text-left">
						Post a job opportunity
					</div>
				</Button>
			</div>

			{/* Job Posting Modal */}
			<Dialog
				open={open}
				onOpenChange={(isOpen) => !isOpen && handleClose()}
			>
				<DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
					<DialogHeader>
						<DialogTitle className="flex items-center gap-3 text-2xl font-bold">
							<div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
								<Briefcase className="w-5 h-5 text-primary" />
							</div>
							Post a Job Opportunity
						</DialogTitle>
						<Separator className="bg-border" />
					</DialogHeader>

					{/* User Profile Info */}
					<div className="flex items-center gap-3 p-3 bg-muted/30 rounded-lg mb-4 mt-2">
						<div>
							{userProfile?.profilePictureUrl ? (
								<img
									src={userProfile.profilePictureUrl}
									width={40}
									height={40}
									className="rounded-full ring-2 ring-border"
									alt="Profile"
								/>
							) : (
								<img
									src={defaultProfile}
									alt="Profile Picture"
									width={40}
									height={40}
									className="rounded-full ring-2 ring-border"
								/>
							)}
						</div>
						<div className="font-medium text-card-foreground">
							{authUser?.displayName || "User"}
							<span className="text-muted-foreground block text-sm">
								{` @${userProfile?.username || "username"}`}
							</span>
						</div>
					</div>

					<div className="flex flex-col gap-4 w-full mb-4">
						{/* Job Title Input */}
						<Field>
							<FieldLabel>Job Title *</FieldLabel>
							<Input
								value={title}
								onChange={handleTitleChange}
								placeholder="e.g., Frontend Developer for E-commerce Platform"
								maxLength={TITLE_MAX_LENGTH}
								dir="auto"
							/>
							{titleError && (
								<FieldError>{titleError}</FieldError>
							)}
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
						<div className="flex gap-4 w-full">
							{/* Budget Input */}
							<Field className="flex-1">
								<FieldLabel>Budget (USD) *</FieldLabel>
								<Input
									value={budget}
									onChange={handleBudgetChange}
									placeholder="e.g., 5000"
									type="number"
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
								/>
								{areaError && (
									<FieldError>{areaError}</FieldError>
								)}
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
								onChange={(
									e: React.ChangeEvent<HTMLTextAreaElement>
								) => handleContentChange(e as any)}
								placeholder="Describe the job requirements, expectations, and any other relevant details..."
								maxLength={CONTENT_MAX_LENGTH}
								dir="auto"
								className="min-h-[150px] resize-none"
							/>
							{contentError && (
								<FieldError>{contentError}</FieldError>
							)}
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
						<div className="w-full">
							<div className="border border-border rounded-lg p-4 mt-2 bg-muted/20">
								<div className="flex items-center justify-between mb-2">
									<div className="text-card-foreground font-medium">
										Add to your job posting
									</div>
									<div className="text-muted-foreground text-sm">
										You can upload up to 3 images or videos
									</div>

									<div className="flex items-center gap-2">
										<div className="flex gap-1">
											<Button
												onClick={() =>
													document
														.getElementById(
															"media-upload"
														)
														?.click()
												}
												size="sm"
												className="dark:text-blue-400"
											>
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="22"
													height="22"
													fill="currentColor"
													viewBox="0 0 16 16"
												>
													<path d="M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" />
													<path d="M2.002 1a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2h-12zm12 1a1 1 0 0 1 1 1v6.5l-3.777-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12V3a1 1 0 0 1 1-1h12z" />
												</svg>
											</Button>
											<Button
												onClick={() =>
													document
														.getElementById(
															"media-upload"
														)
														?.click()
												}
												size="sm"
												className="dark:text-blue-400"
											>
												<svg
													xmlns="http://www.w3.org/2000/svg"
													width="22"
													height="22"
													fill="currentColor"
													viewBox="0 0 16 16"
												>
													<path d="M0 5a2 2 0 0 1 2-2h7.5a2 2 0 0 1 1.983 1.738l3.11-1.382A1 1 0 0 1 16 4.269v7.462a1 1 0 0 1-1.406.913l-3.111-1.382A2 2 0 0 1 9.5 13H2a2 2 0 0 1-2-2V5z" />
												</svg>
											</Button>
										</div>
									</div>
								</div>

								{/* Hidden file input that will be triggered by the IconButtons */}
								<div style={{}}>
									<FileUploadForm
										// id="media-upload"
										onSubmit={() => {}}
										setParentMediaList={setMediaList}
									/>
								</div>
							</div>
						</div>
					</div>

					{/* Publish Button */}
					<Button
						onClick={handleSubmit}
						disabled={isLoading || uploadResult.isLoading}
						className="w-full py-6 bg-primary hover:bg-primary/90 text-primary-foreground rounded-full font-semibold text-lg"
					>
						Post Job Opportunity
					</Button>
				</DialogContent>
			</Dialog>
		</>
	);
};

export default CreateJobModal;
