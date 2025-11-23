import { useState, useEffect, useCallback } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { z } from "zod";
import {
	useUpdatePostMutation,
	useGetPostQuery,
} from "@/features/posts/postsSlice";
import { Button } from "@/views/components/ui/button";
import { Loader2, ArrowLeft } from "lucide-react";
import toast from "react-hot-toast";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { Separator } from "@/views/components/ui/separator";
import {
	Field,
	FieldDescription,
	FieldError,
	FieldLabel,
} from "@/views/components/ui/field";
import { Input } from "@/views/components/ui/input";
import { Textarea } from "@/views/components/ui/textarea";
import PageLoader from "@/components/loaders/PageLoader";

// Character limits
const TITLE_MAX_LENGTH = 256;
const BODY_MAX_LENGTH = 5000;

const editPostSchema = z.object({
	title: z
		.string()
		.min(3, "Title must be at least 3 characters")
		.max(TITLE_MAX_LENGTH, "Title must be less than 256 characters"),
	content: z
		.string()
		.min(3, "Content must be at least 3 characters")
		.max(BODY_MAX_LENGTH, "Content must be less than 5000 characters"),
});

type EditPostFormData = z.infer<typeof editPostSchema>;

const EditPostPage = () => {
	const navigate = useNavigate();
	const { postId } = useParams<{ postId: string }>();
	const [isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();

	// Fetch the post data
	const {
		data: post,
		isLoading: isLoadingPost,
		isError: isErrorPost,
	} = useGetPostQuery(postId || "", { skip: !postId });

	const [formData, setFormData] = useState<EditPostFormData>({
		title: "",
		content: "",
	});
	const [originalData, setOriginalData] = useState<EditPostFormData>({
		title: "",
		content: "",
	});
	const [errors, setErrors] = useState<{ [key: string]: string }>({});
	const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

	const [updatePost, { isLoading }] = useUpdatePostMutation();

	// Initialize form data when post loads
	useEffect(() => {
		if (post) {
			const initialData = {
				title: post.title,
				content: post.content || "",
			};
			setFormData(initialData);
			setOriginalData(initialData);
		}
	}, [post]);

	// Track unsaved changes
	useEffect(() => {
		const hasChanges =
			formData.title !== originalData.title ||
			formData.content !== originalData.content;
		setHasUnsavedChanges(hasChanges);
	}, [formData, originalData]);

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
		if (hasUnsavedChanges && postId) {
			const draft = { ...formData, postId, timestamp: Date.now() };
			localStorage.setItem(
				`post-edit-draft-${postId}`,
				JSON.stringify(draft)
			);
		}
	}, [formData, hasUnsavedChanges, postId]);

	// Load draft on mount
	useEffect(() => {
		if (postId) {
			const savedDraft = localStorage.getItem(
				`post-edit-draft-${postId}`
			);
			if (savedDraft) {
				try {
					const draft = JSON.parse(savedDraft);
					// Only load if draft is less than 24 hours old
					if (Date.now() - draft.timestamp < 24 * 60 * 60 * 1000) {
						setFormData({
							title: draft.title || "",
							content: draft.content || "",
						});
						toast.success("Draft restored");
					} else {
						localStorage.removeItem(`post-edit-draft-${postId}`);
					}
				} catch (error) {
					console.error("Error loading draft:", error);
				}
			}
		}
	}, [postId]);

	const validate = useCallback((): boolean => {
		try {
			editPostSchema.parse(formData);
			setErrors({});
			return true;
		} catch (error) {
			if (error instanceof z.ZodError) {
				const newErrors: { [key: string]: string } = {};
				error.errors.forEach((err) => {
					if (err.path.length > 0) {
						newErrors[err.path[0]] = err.message;
					}
				});
				setErrors(newErrors);
			}
			return false;
		}
	}, [formData]);

	// Re-validate whenever formData changes
	useEffect(() => {
		validate();
	}, [validate]);

	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setFormData({ ...formData, title: newValue });
		}
	};

	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setFormData({ ...formData, content: newValue });
		}
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		if (!validate() || !postId) {
			return;
		}

		try {
			await updatePost({
				postId: postId,
				title: formData.title,
				content: formData.content,
			}).unwrap();
			toast.success("Post updated successfully");
			localStorage.removeItem(`post-edit-draft-${postId}`);
			navigate(`/posts/${postId}`);
		} catch (error) {
			toast.error("Failed to update post");
			console.error("Error updating post:", error);
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

	const titleCharsLeft = TITLE_MAX_LENGTH - formData.title.length;
	const bodyCharsLeft = BODY_MAX_LENGTH - formData.content.length;

	// Redirect if not logged in
	if (!isLoggedIn) {
		navigate("/login");
		return null;
	}

	// Loading state
	if (isLoadingPost) {
		return <PageLoader />;
	}

	// Error state
	if (isErrorPost || !post) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="text-center">
					<h2 className="text-2xl font-bold text-destructive mb-4">
						Post not found
					</h2>
					<Button onClick={() => navigate("/home")}>
						Go to Home
					</Button>
				</div>
			</div>
		);
	}

	// Check if user is the post author
	if (post.author.id !== userProfile?.id) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="text-center">
					<h2 className="text-2xl font-bold text-destructive mb-4">
						You don't have permission to edit this post
					</h2>
					<Button onClick={() => navigate("/home")}>
						Go to Home
					</Button>
				</div>
			</div>
		);
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
						disabled={isLoading}
					>
						<ArrowLeft className="h-5 w-5" />
					</Button>
					<h1 className="text-2xl font-bold text-card-foreground">
						Edit Post
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
					<form onSubmit={handleSubmit}>
						<div className="space-y-6">
							{/* Title Input */}
							<Field>
								<FieldLabel
									htmlFor="post-title"
									className="text-lg font-semibold"
								>
									Title *
								</FieldLabel>
								<Input
									id="post-title"
									value={formData.title}
									onChange={handleTitleChange}
									disabled={isLoading}
									maxLength={TITLE_MAX_LENGTH}
									dir="auto"
									className="text-lg h-12"
									required
								/>
								{errors.title && (
									<FieldError>{errors.title}</FieldError>
								)}
								{!errors.title && (
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
							<Field>
								<FieldLabel
									htmlFor="post-content"
									className="text-lg font-semibold"
								>
									Describe Your Problem *
								</FieldLabel>
								<Textarea
									id="post-content"
									value={formData.content}
									onChange={handleContentChange}
									disabled={isLoading}
									maxLength={BODY_MAX_LENGTH}
									dir="auto"
									className="min-h-[300px] resize-y text-base"
									required
								/>
								{errors.content && (
									<FieldError>{errors.content}</FieldError>
								)}
								{!errors.content && (
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

							{/* Action Buttons */}
							<div className="flex gap-3 pt-4">
								<Button
									type="button"
									variant="outline"
									className="flex-1"
									onClick={handleBack}
									disabled={isLoading}
								>
									Cancel
								</Button>
								<Button
									type="submit"
									disabled={
										isLoading ||
										Object.keys(errors).length > 0
									}
									className="flex-1 bg-primary hover:bg-primary/90 text-lg font-semibold"
								>
									{isLoading ? (
										<>
											<Loader2 className="mr-2 h-5 w-5 animate-spin" />
											Saving...
										</>
									) : (
										"Save Changes"
									)}
								</Button>
							</div>
						</div>
					</form>
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

export default EditPostPage;
