"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { z } from "zod";
import { Loader2 } from "lucide-react";
import toast from "react-hot-toast";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Separator } from "@/components/ui/separator";
import {
	Field,
	FieldLabel,
	FieldError,
	FieldDescription,
} from "@/components/ui/field";
import {
	Card,
	CardContent,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogFooter,
	DialogHeader,
	DialogTitle,
} from "@/components/ui/dialog";

import { useCreatePostMutation } from "@/features/posts/postsSlice";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";

// Character limits
const TITLE_MAX_LENGTH = 256;
const BODY_MAX_LENGTH = 5000;

// Zod schema for form validation
const postSchema = z.object({
	title: z
		.string()
		.min(1, "Title is required")
		.max(
			TITLE_MAX_LENGTH,
			`Title cannot exceed ${TITLE_MAX_LENGTH} characters`
		)
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 3, {
			message: "Title must be at least 3 words",
		}),
	content: z
		.string()
		.min(1, "Content is required")
		.max(
			BODY_MAX_LENGTH,
			`Content cannot exceed ${BODY_MAX_LENGTH} characters`
		)
		.refine((val) => val.trim().split(/\s+/).filter(Boolean).length >= 10, {
			message: "Content must be at least 10 words",
		}),
});

type PostFormData = z.infer<typeof postSchema>;

/**
 * Form component for creating new posts.
 * Includes Zod validation, character counters, and media upload placeholder.
 */
const CreatePostForm = () => {
	const router = useRouter();
	const { profile, firebaseUser } = useIsUserLoggedIn();

	const [formData, setFormData] = useState<PostFormData>({
		title: "",
		content: "",
	});
	const [errors, setErrors] = useState<{ title?: string; content?: string }>(
		{}
	);
	const [showCancelDialog, setShowCancelDialog] = useState(false);

	const [createPost, { isLoading, isSuccess, error, reset }] =
		useCreatePostMutation();

	// Handle success - navigate to feed
	useEffect(() => {
		if (isSuccess) {
			toast.success("Post created successfully!");
			reset();
			router.push("/feed");
		}
	}, [isSuccess, router, reset]);

	// Handle error
	useEffect(() => {
		if (error) {
			toast.error("Failed to create post. Please try again.");
			reset();
		}
	}, [error, reset]);

	// Validate form data
	const validate = (): boolean => {
		try {
			postSchema.parse(formData);
			setErrors({});
			return true;
		} catch (err) {
			if (err instanceof z.ZodError) {
				const newErrors: { title?: string; content?: string } = {};
				err.issues.forEach((issue) => {
					if (issue.path[0] === "title") {
						newErrors.title = issue.message;
					} else if (issue.path[0] === "content") {
						newErrors.content = issue.message;
					}
				});
				setErrors(newErrors);
			}
			return false;
		}
	};

	// Handle title input with character limit
	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setFormData((prev) => ({ ...prev, title: newValue }));
			if (newValue.trim() && errors.title) {
				setErrors((prev) => ({ ...prev, title: undefined }));
			}
		}
	};

	// Handle content input with character limit
	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setFormData((prev) => ({ ...prev, content: newValue }));
			if (newValue.trim() && errors.content) {
				setErrors((prev) => ({ ...prev, content: undefined }));
			}
		}
	};

	// Handle form submission
	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!validate()) {
			return;
		}

		try {
			await createPost({
				title: formData.title.trim(),
				content: formData.content.trim(),
				media: [], // Media upload not implemented yet
			}).unwrap();
		} catch {
			// Error handled in useEffect
		}
	};

	// Handle cancel
	const handleCancel = () => {
		if (formData.title || formData.content) {
			setShowCancelDialog(true);
		} else {
			router.push("/feed");
		}
	};

	// Handle confirm leave
	const handleConfirmLeave = () => {
		setShowCancelDialog(false);
		router.push("/feed");
	};

	const titleCharsLeft = TITLE_MAX_LENGTH - formData.title.length;
	const contentCharsLeft = BODY_MAX_LENGTH - formData.content.length;

	const authorName =
		firebaseUser?.displayName ||
		[profile?.firstName, profile?.lastName].filter(Boolean).join(" ") ||
		"User";

	return (
		<Card className="w-full max-w-4xl mx-auto bg-card text-card-foreground">
			<CardHeader>
				<CardTitle className="text-xl max-sm:text-lg text-card-foreground">
					Ask Your Question
				</CardTitle>
				<Separator className="bg-border mt-2" />

				{/* User Profile Info */}
				<div className="flex items-center mb-2 mt-2">
					<div className="mr-2">
						{profile?.profilePictureUrl ? (
							<Image
								src={profile.profilePictureUrl}
								width={40}
								height={40}
								className="rounded-full object-cover"
								alt="Profile"
							/>
						) : (
							<div className="w-10 h-10 rounded-full bg-muted flex items-center justify-center">
								<span className="text-sm font-medium text-muted-foreground">
									{authorName.charAt(0).toUpperCase()}
								</span>
							</div>
						)}
					</div>
					<div className="font-medium text-card-foreground">
						{authorName}
						<span className="text-muted-foreground block text-sm">
							@{profile?.username || "username"}
						</span>
					</div>
				</div>
			</CardHeader>

			<form onSubmit={handleSubmit}>
				<CardContent className="flex flex-col gap-2 w-full">
					{/* Title Input */}
					<Field className="w-full" data-invalid={!!errors.title}>
						<FieldLabel htmlFor="post-title">
							Start Asking Your Question *
						</FieldLabel>
						<Input
							id="post-title"
							value={formData.title}
							onChange={handleTitleChange}
							disabled={isLoading}
							maxLength={TITLE_MAX_LENGTH}
							dir="auto"
							className="text-lg"
							required
						/>
						{errors.title && (
							<FieldError>{errors.title}</FieldError>
						)}
						{!errors.title && (
							<FieldDescription
								className={
									titleCharsLeft < 1
										? "text-destructive"
										: "text-muted-foreground"
								}
							>
								{titleCharsLeft} characters left
							</FieldDescription>
						)}
					</Field>

					{/* Content Input */}
					<div className="w-full">
						<Textarea
							id="post-content"
							value={formData.content}
							onChange={handleContentChange}
							disabled={isLoading}
							maxLength={BODY_MAX_LENGTH}
							dir="auto"
							className="min-h-30 resize-none bg-muted rounded"
							required
						/>
						{errors.content && (
							<FieldError className="mt-1">
								{errors.content}
							</FieldError>
						)}
						{!errors.content && (
							<div className="flex justify-end mt-1">
								<div
									className={
										contentCharsLeft < 1
											? "text-destructive"
											: "text-muted-foreground"
									}
								>
									{contentCharsLeft} characters left
								</div>
							</div>
						)}
					</div>

					{/* Media Upload Section */}
					<div className="w-full">
						<div className="border border-border rounded p-3 mt-2">
							<div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-2">
								<div className="text-card-foreground">
									Add to your question
								</div>
								<div className="text-muted-foreground text-sm">
									Media upload coming soon
								</div>
							</div>
						</div>
					</div>
				</CardContent>

				<CardFooter className="flex flex-col gap-3 bg-card">
					{/* Publish Button */}
					<Button
						type="submit"
						disabled={isLoading}
						className="w-full bg-primary hover:bg-primary/90 py-6 rounded-full text-lg font-semibold"
					>
						{isLoading ? (
							<>
								<Loader2 className="mr-2 h-4 w-4 animate-spin" />
								Publishing...
							</>
						) : (
							"Publish Your Question"
						)}
					</Button>

					{/* Cancel Button */}
					<Button
						type="button"
						variant="outline"
						className="w-full py-6 rounded-full text-lg font-semibold"
						onClick={handleCancel}
						disabled={isLoading}
					>
						Cancel
					</Button>
				</CardFooter>
			</form>

			{/* Unsaved Changes Confirmation Dialog */}
			<Dialog open={showCancelDialog} onOpenChange={setShowCancelDialog}>
				<DialogContent showCloseButton={false}>
					<DialogHeader>
						<DialogTitle>Unsaved Changes</DialogTitle>
						<DialogDescription>
							You have unsaved changes. Are you sure you want to
							leave? Your changes will be lost.
						</DialogDescription>
					</DialogHeader>
					<DialogFooter>
						<Button
							variant="outline"
							onClick={() => setShowCancelDialog(false)}
						>
							Stay
						</Button>
						<Button
							variant="destructive"
							onClick={handleConfirmLeave}
						>
							Leave
						</Button>
					</DialogFooter>
				</DialogContent>
			</Dialog>
		</Card>
	);
};

export default CreatePostForm;
