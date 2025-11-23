import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCreatePostMutation } from "@/features/posts/postsSlice.ts";
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
import { ArrowLeft, Loader2 } from "lucide-react";

// Character limits
const TITLE_MAX_LENGTH = 256;
const BODY_MAX_LENGTH = 5000;

// Zod schema for form validation
const postSchema = z.object({
	title: z
		.string()
		.min(1, "Title is required")
		.max(TITLE_MAX_LENGTH, "Title cannot exceed 256 characters")
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 3, {
			message: "Title must be at least 3 words",
		}),
	content: z
		.string()
		.min(1, "Content is required")
		.max(BODY_MAX_LENGTH, "Content cannot exceed 5000 characters")
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 10, {
			message: "Content must be at least 10 words",
		}),
});

const CreatePostPage = () => {
	const navigate = useNavigate();
	const [title, setTitle] = useState("");
	const [body, setBody] = useState("");
	const [titleError, setTitleError] = useState("");
	const [bodyError, setBodyError] = useState("");
	const [mediaList, setMediaList] = useState<MediaObject[]>([]);
	const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);
	const [isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();

	const [createPost, createPostResult] = useCreatePostMutation();

	const { uploadResult, uploadMedia } = useCallbackOnMediaUploadSuccess(
		createPost,
		{ title, content: body }
	);

	const { isSuccess, isError, isLoading } = createPostResult;

	// Track unsaved changes
	useEffect(() => {
		setHasUnsavedChanges(title.trim() !== "" || body.trim() !== "");
	}, [title, body]);

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
			const draft = { title, body, timestamp: Date.now() };
			localStorage.setItem("post-draft", JSON.stringify(draft));
		}
	}, [title, body, hasUnsavedChanges]);

	// Load draft on mount
	useEffect(() => {
		const savedDraft = localStorage.getItem("post-draft");
		if (savedDraft) {
			try {
				const draft = JSON.parse(savedDraft);
				// Only load if draft is less than 24 hours old
				if (Date.now() - draft.timestamp < 24 * 60 * 60 * 1000) {
					setTitle(draft.title || "");
					setBody(draft.body || "");
					toast.success("Draft restored");
				} else {
					localStorage.removeItem("post-draft");
				}
			} catch (error) {
				console.error("Error loading draft:", error);
			}
		}
	}, []);

	const resetForm = useCallback(() => {
		setTitle("");
		setBody("");
		setTitleError("");
		setBodyError("");
		setMediaList([]);
		localStorage.removeItem("post-draft");
		setHasUnsavedChanges(false);
	}, []);

	useEffect(() => {
		if (isError) toast.error("An error occurred while creating your post");
		if (isSuccess) {
			toast.success("Post created successfully");
			resetForm();
			navigate("/home");
		}
	}, [isSuccess, isError, navigate, resetForm]);

	const handleSubmit = async () => {
		try {
			postSchema.parse({ title, content: body });
			setTitleError("");
			setBodyError("");
			await uploadMedia({ mediaList });
		} catch (err) {
			if (err instanceof z.ZodError) {
				err.errors.forEach((error) => {
					if (error.path[0] === "title") {
						setTitleError(error.message);
					} else if (error.path[0] === "content") {
						setBodyError(error.message);
					}
				});
			}
		}
	};

	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setTitle(newValue);
			if (newValue.trim()) {
				setTitleError("");
			}
		}
	};

	const handleBodyChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setBody(newValue);
			if (newValue.trim()) {
				setBodyError("");
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
	const bodyCharsLeft = BODY_MAX_LENGTH - body.length;

	// Redirect if not logged in
	if (!isLoggedIn) {
		navigate("/login");
		return null;
	}

	return (
		<div className="min-h-screen bg-background">
			{/* Header */}
			<div className="sticky top-0 z-10 bg-card border-b border-border">
				<div className="max-w-4xl mx-auto px-4 py-4 flex items-center gap-4">
					<Button
						variant="ghost"
						size="icon"
						onClick={handleBack}
						disabled={isLoading || uploadResult.isLoading}
					>
						<ArrowLeft className="h-5 w-5" />
					</Button>
					<h1 className="text-2xl font-bold text-card-foreground">
						Create Post
					</h1>
				</div>
			</div>

			{/* Main Content */}
			<div className="max-w-4xl mx-auto px-4 py-6">
				<div className="bg-card rounded-lg border border-border p-6 shadow-sm">
					{/* User Profile Info */}
					<div className="flex items-center mb-6">
						<div className="mr-3">
							{userProfile?.profilePictureUrl ? (
								<img
									src={userProfile.profilePictureUrl}
									width={48}
									height={48}
									className="rounded-full"
									alt="Profile"
								/>
							) : (
								<img
									src={defaultProfile}
									alt="Profile Picture"
									width={48}
									height={48}
									className="rounded-full"
								/>
							)}
						</div>
						<div>
							<div className="font-semibold text-card-foreground">
								{authUser?.displayName || "User"}
							</div>
							<div className="text-sm text-muted-foreground">
								{`@${userProfile?.username || "username"}`}
							</div>
						</div>
					</div>

					<Separator className="mb-6" />

					{/* Form */}
					<div className="space-y-6">
						{/* Title Input */}
						<Field className="w-full" data-invalid={!!titleError}>
							<FieldLabel
								htmlFor="post-title"
								className="text-lg font-semibold"
							>
								Question Title *
							</FieldLabel>
							<Input
								id="post-title"
								value={title}
								onChange={handleTitleChange}
								maxLength={TITLE_MAX_LENGTH}
								dir="auto"
								required
								disabled={isLoading || uploadResult.isLoading}
								className="text-lg h-12"
								placeholder="What do you want to ask?"
							/>
							{titleError && (
								<FieldError>{titleError}</FieldError>
							)}
							{!titleError && (
								<FieldDescription
									className={
										titleCharsLeft < 50
											? "text-orange-500"
											: titleCharsLeft < 1
												? "text-destructive"
												: "text-muted-foreground"
									}
								>
									{titleCharsLeft} characters left
								</FieldDescription>
							)}
						</Field>

						{/* Content Input */}
						<Field className="w-full" data-invalid={!!bodyError}>
							<FieldLabel
								htmlFor="post-content"
								className="text-lg font-semibold"
							>
								Describe Your Problem *
							</FieldLabel>
							<Textarea
								id="post-content"
								value={body}
								onChange={handleBodyChange}
								maxLength={BODY_MAX_LENGTH}
								className="min-h-[300px] resize-y text-base"
								required
								disabled={isLoading || uploadResult.isLoading}
								placeholder="Provide details about your question..."
								dir="auto"
							/>
							{bodyError && <FieldError>{bodyError}</FieldError>}
							{!bodyError && (
								<FieldDescription
									className={
										bodyCharsLeft < 100
											? "text-orange-500"
											: bodyCharsLeft < 1
												? "text-destructive"
												: "text-muted-foreground"
									}
								>
									{bodyCharsLeft} characters left
								</FieldDescription>
							)}
						</Field>

						{/* Media Upload Section */}
						<div className="border border-border rounded-lg p-4">
							<div className="mb-3">
								<h3 className="text-base font-semibold text-card-foreground mb-1">
									Add Media
								</h3>
								<p className="text-sm text-muted-foreground">
									You can upload up to 3 images or videos
								</p>
							</div>
							<div className="flex items-center gap-3">
								<Button
									type="button"
									variant="outline"
									onClick={() =>
										document
											.getElementById("media-upload")
											?.click()
									}
									disabled={
										isLoading || uploadResult.isLoading
									}
								>
									<svg
										xmlns="http://www.w3.org/2000/svg"
										width="20"
										height="20"
										fill="currentColor"
										viewBox="0 0 16 16"
										className="mr-2"
									>
										<path d="M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" />
										<path d="M2.002 1a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2h-12zm12 1a1 1 0 0 1 1 1v6.5l-3.777-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12V3a1 1 0 0 1 1-1h12z" />
									</svg>
									Add Images
								</Button>
								<Button
									type="button"
									variant="outline"
									onClick={() =>
										document
											.getElementById("media-upload")
											?.click()
									}
									disabled={
										isLoading || uploadResult.isLoading
									}
								>
									<svg
										xmlns="http://www.w3.org/2000/svg"
										width="20"
										height="20"
										fill="currentColor"
										viewBox="0 0 16 16"
										className="mr-2"
									>
										<path d="M0 5a2 2 0 0 1 2-2h7.5a2 2 0 0 1 1.983 1.738l3.11-1.382A1 1 0 0 1 16 4.269v7.462a1 1 0 0 1-1.406.913l-3.111-1.382A2 2 0 0 1 9.5 13H2a2 2 0 0 1-2-2V5z" />
									</svg>
									Add Videos
								</Button>
							</div>
							<div className="mt-4">
								<FileUploadForm
									onSubmit={() => {}}
									setParentMediaList={setMediaList}
								/>
							</div>
						</div>

						{/* Action Buttons */}
						<div className="flex gap-3 pt-4">
							<Button
								variant="outline"
								className="flex-1"
								onClick={handleBack}
								disabled={isLoading || uploadResult.isLoading}
							>
								Cancel
							</Button>
							<Button
								className="flex-1 bg-primary hover:bg-primary/90 text-lg font-semibold"
								onClick={handleSubmit}
								disabled={isLoading || uploadResult.isLoading}
							>
								{isLoading || uploadResult.isLoading ? (
									<>
										<Loader2 className="mr-2 h-5 w-5 animate-spin" />
										Publishing...
									</>
								) : (
									"Publish Question"
								)}
							</Button>
						</div>
					</div>
				</div>

				{/* Auto-save indicator */}
				{hasUnsavedChanges && (
					<div className="mt-4 text-center text-sm text-muted-foreground">
						Draft auto-saved
					</div>
				)}
			</div>
		</div>
	);
};

export default CreatePostPage;
