import { useCallback, useEffect, useState } from "react";
import { useCreatePostMutation } from "@/features/posts/postsSlice.ts";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import toast from "react-hot-toast";
import {
	Button,
	TextField,
	Typography,
	Box,
	Modal,
	IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";
import FileUploadForm from "@/views/components/custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import { z } from "zod";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Separator } from "@/views/components/ui/separator";

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
	const [media, setMedia] = useState<File[]>([]);
	const [error, setError] = useState("");
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

	useEffect(() => {
		if (isError) toast.error("An error occurred while creating your post");
		if (isSuccess) {
			toast.success("Post created successfully");
			handleClose();
		}
	}, [isSuccess, isError]);

	const handleOpen = () => {
		if (isLoggedIn) {
			setOpen(true);
		} else {
			showAuthPrompt();
		}
	};

	const resetForm = useCallback(() => {
		setTitle("");
		setBody("");
		setMedia([]);
		setError("");
		setTitleError("");
		setBodyError("");
	}, []);

	const handleClose = useCallback(() => {
		setOpen(false);
		resetForm();
	}, [setOpen, resetForm]);

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
	const handleBodyChange = (e: React.ChangeEvent<HTMLInputElement>) => {
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
				className="flex justify-center items-center gap-2 bg-white dark:bg-gray-800 shadow-md rounded-lg p-4 border border-gray-200 dark:border-gray-700"
				onClick={handleOpen}
			>
				{isLoggedIn && (
					<div className="bg-white dark:bg-gray-800 flex justify-center items-center">
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
				<Button className="bg-gray-100 dark:bg-gray-700 text-gray-500 dark:text-gray-300 px-5 hover:bg-gray-200 dark:hover:bg-gray-600 hover:text-main-blue dark:hover:text-blue-400 w-full rounded-full">
					<div className="w-full text-left">
						What do you want to ask?
					</div>
				</Button>
			</div>

			{/* New Modal */}
			<Modal
				open={open}
				onClose={handleClose}
				aria-labelledby="create-post-modal"
				aria-disabled={isLoading || uploadResult.isLoading}
				className="flex items-center justify-center"
			>
				<div className="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-4/5 md:w-3/4 lg:w-2/3 xl:w-1/2 relative dark:text-white">
					{/* Close Button */}
					<div className="absolute top-3 right-3">
						<IconButton onClick={handleClose}>
							<CloseIcon className="dark:text-gray-300" />
						</IconButton>
					</div>

					<Typography
						variant="h6"
						gutterBottom
						id="create-post-modal"
						className="max-sm:text-md dark:text-white"
					>
						Ask Your Question
					</Typography>

					<Separator className="dark:bg-gray-600" />

					{/* User Profile Info */}
					<Box className="flex items-center mb-4 mt-2">
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
						<Typography
							variant="subtitle1"
							className="font-medium dark:text-white"
						>
							{authUser?.displayName || "User"}
							<span className="text-gray-500 dark:text-gray-400 block text-sm">
								{` @${userProfile?.username || "username"}`}
							</span>
						</Typography>
					</Box>

					<Box className="flex flex-col gap-2 w-full mb-4">
						{/* Title Input */}
						<div className="w-full">
							<TextField
								fullWidth
								label="Start Asking Your Question"
								variant="outlined"
								value={title}
								onChange={handleTitleChange}
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
											titleCharsLeft < 1
												? "error"
												: "green"
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

						{/* Content Input */}
						<div className="w-full">
							<TextField
								fullWidth
								label="Describe Your Problem"
								variant="outlined"
								multiline
								rows={4}
								value={body}
								onChange={handleBodyChange}
								slotProps={{
									htmlInput: {
										maxLength: BODY_MAX_LENGTH,
										dir: "auto",
										className: "text-md dark:text-white",
									},
								}}
								required
								error={!!bodyError}
								helperText={bodyError || ""}
								className="dark:bg-gray-700 dark:rounded"
								InputLabelProps={{
									className: "dark:text-gray-300",
								}}
							/>
							{!bodyError && (
								<div className="flex justify-end mt-1">
									<Typography
										variant="caption"
										color={
											bodyCharsLeft < 1
												? "error"
												: "green"
										}
										className={
											bodyCharsLeft < 1
												? "text-red-500"
												: "text-green-500 dark:text-green-400"
										}
									>
										{bodyCharsLeft} characters left
									</Typography>
								</div>
							)}
						</div>

						{/* Media Upload Section */}
						<div className="w-full">
							<Box className="border border-gray-300 dark:border-gray-600 rounded p-3 mt-2">
								<Box className="flex items-center justify-between">
									<Typography
										variant="body2"
										className="dark:text-gray-200"
									>
										Add to your question
									</Typography>
									<Typography
										variant="caption"
										color="textSecondary"
										className="dark:text-gray-400"
									>
										You can upload up to 3 images or videos
									</Typography>

									<Box className="flex items-center gap-2">
										<Box className="flex gap-1">
											<IconButton
												color="primary"
												onClick={() =>
													document
														.getElementById(
															"media-upload"
														)
														?.click()
												}
												size="small"
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
											</IconButton>
											<IconButton
												color="primary"
												onClick={() =>
													document
														.getElementById(
															"media-upload"
														)
														?.click()
												}
												size="small"
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
											</IconButton>
										</Box>
									</Box>
								</Box>

								{/* Hidden file input that will be triggered by the IconButtons */}
								<div style={{}}>
									<FileUploadForm
										// id="media-upload"
										onSubmit={() => {}}
										setParentMediaList={setMediaList}
									/>
								</div>
							</Box>
						</div>
					</Box>

					{/* Publish Button */}
					<Button
						variant="contained"
						fullWidth
						onClick={handleSubmit}
						disabled={isLoading || uploadResult.isLoading}
						sx={{
							backgroundColor: "#162955",
							"&:hover": { backgroundColor: "#0e1c3b" },
							py: 1.5,
						}}
						className="dark:bg-blue-700 dark:hover:bg-blue-800"
					>
						Publish Your Question
					</Button>
				</div>
			</Modal>
		</>
	);
};

export default CreatePostModal;
