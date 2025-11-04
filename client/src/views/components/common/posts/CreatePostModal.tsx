import { useCallback, useEffect, useState } from "react";
import { useCreatePostMutation } from "@/features/posts/postsSlice.ts";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import toast from "react-hot-toast";
// Icons can be added later if needed
// import { ImageIcon, Video } from "lucide-react";
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";
import FileUploadForm from "@/views/components/custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import { z } from "zod";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
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
import {
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTitle,
} from "@/views/components/ui/dialog";

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

const CreatePostModal: React.FC = () => {
	useEffect(() => {
		console.log("modal mounting...");
	}, []);

	const [open, setOpen] = useState(false);
	const [title, setTitle] = useState("");
	const [body, setBody] = useState("");
	const [titleError, setTitleError] = useState("");
	const [bodyError, setBodyError] = useState("");
	const [mediaList, setMediaList] = useState<MediaObject[]>([]);
	const [isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();
	const { showAuthPrompt } = useAuthPrompt();

	const [createPost, createPostResult] = useCreatePostMutation();

	const { uploadResult, uploadMedia } = useCallbackOnMediaUploadSuccess(
		createPost,
		{ title, content: body }
	);

	const { isSuccess, isError, isLoading } = createPostResult;

	const resetForm = useCallback(() => {
		setTitle("");
		setBody("");
		setTitleError("");
		setBodyError("");
	}, []);

	const handleClose = useCallback(() => {
		setOpen(false);
		resetForm();
	}, [setOpen, resetForm]);

	useEffect(() => {
		if (isError) toast.error("An error occurred while creating your post");
		if (isSuccess) {
			toast.success("Post created successfully");
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
			postSchema.parse({ title, content: body });
			setTitleError("");
			setBodyError("");
			await uploadMedia({ mediaList });
		} catch (err) {
			if (err instanceof z.ZodError) {
				// Set errors for specific fields
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

	// Handle body input with character limit
	const handleBodyChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setBody(newValue);
			if (newValue.trim()) {
				setBodyError(""); // Clear error only if there's valid content
			}
		}
	};

	const titleCharsLeft = TITLE_MAX_LENGTH - title.length;
	const bodyCharsLeft = BODY_MAX_LENGTH - body.length;

	return (
		<>
			<div
				className="flex justify-center items-center gap-2 bg-card shadow-md rounded-lg p-4 border border-border"
				onClick={handleOpen}
			>
				{isLoggedIn && (
					<div className="bg-card flex justify-center items-center">
						{userProfile?.profilePictureUrl ? (
							<img
								src={userProfile.profilePictureUrl}
								width={45}
								height={45}
								className="rounded-full"
								alt="Profile Picture"
							/>
						) : (
							<img
								src={defaultProfile}
								alt="Profile Picture"
								width={45}
								height={45}
								className="rounded-full"
							/>
						)}
					</div>
				)}
				<Button className="bg-muted text-muted-foreground px-5 hover:bg-accent hover:text-primary w-full rounded-full">
					<div className="w-full text-left">
						What do you want to ask?
					</div>
				</Button>
			</div>

			{/* New Dialog */}
			<Dialog open={open} onOpenChange={setOpen}>
				<DialogContent className="bg-card w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 max-w-4xl max-h-[90vh] overflow-y-auto">
					<DialogHeader>
						<DialogTitle className="text-xl max-sm:text-lg text-card-foreground">
							Ask Your Question
						</DialogTitle>
					</DialogHeader>
					<Separator className="bg-border" />{" "}
					{/* User Profile Info */}
					<div className="flex items-center mb-4 mt-2">
						<div className="mr-2">
							{userProfile?.profilePictureUrl ? (
								<img
									src={userProfile.profilePictureUrl}
									width={40}
									height={40}
									className="rounded-full"
									alt="Profile"
								/>
							) : (
								<img
									src={defaultProfile}
									alt="Profile Picture"
									width={40}
									height={40}
									className="rounded-full"
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
					<div className="flex flex-col gap-2 w-full mb-4">
						{/* Title Input */}
						<Field className="w-full" data-invalid={!!titleError}>
							<FieldLabel htmlFor="post-title">
								Start Asking Your Question *
							</FieldLabel>
							<Input
								id="post-title"
								value={title}
								onChange={handleTitleChange}
								maxLength={TITLE_MAX_LENGTH}
								dir="auto"
								required
								disabled={isLoading || uploadResult.isLoading}
								className="text-lg"
							/>
							{titleError && (
								<FieldError>{titleError}</FieldError>
							)}
							{!titleError && (
								<FieldDescription
									className={
										titleCharsLeft < 1
											? "text-destructive"
											: "text-green-500"
									}
								>
									{titleCharsLeft} characters left
								</FieldDescription>
							)}
						</Field>{" "}
						{/* Content Input */}
						<div className="w-full">
							<Textarea
								id="post-content"
								value={body}
								onChange={handleBodyChange}
								className="min-h-[120px] resize-none bg-muted rounded"
								required
							/>
							{!bodyError && (
								<div className="flex justify-end mt-1">
									<div
										className={
											bodyCharsLeft < 1
												? "text-destructive"
												: "text-green-600"
										}
									>
										{bodyCharsLeft} characters left
									</div>
								</div>
							)}
						</div>
						{/* Media Upload Section */}
						<div className="w-full">
							<div className="border border-border rounded p-3 mt-2">
								<div className="flex items-center justify-between">
									<div className="text-card-foreground">
										Add to your question
									</div>
									<div className="text-muted-foreground">
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
												className="text-primary"
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
												className="text-primary"
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
						className="w-full bg-primary hover:bg-primary/90 py-3 rounded-full text-lg font-semibold"
						onClick={handleSubmit}
						disabled={isLoading || uploadResult.isLoading}
					>
						Publish Your Question
					</Button>
				</DialogContent>
			</Dialog>
		</>
	);
};

export default CreatePostModal;
