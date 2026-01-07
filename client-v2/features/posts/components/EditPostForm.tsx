"use client";

import { useState, useEffect, useCallback, useMemo } from "react";
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

import { useUpdatePostMutation } from "@/features/posts/postsSlice";
import type { Post } from "@/features/posts/types";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";

// Character limits
const TITLE_MAX_LENGTH = 256;
const BODY_MAX_LENGTH = 5000;

// Zod schema for form validation
const editPostSchema = z.object({
	title: z
		.string()
		.min(3, "Title must be at least 3 characters")
		.max(
			TITLE_MAX_LENGTH,
			`Title must be less than ${TITLE_MAX_LENGTH} characters`
		),
	content: z
		.string()
		.min(3, "Content must be at least 3 characters")
		.max(
			BODY_MAX_LENGTH,
			`Content must be less than ${BODY_MAX_LENGTH} characters`
		),
});

type EditPostFormData = z.infer<typeof editPostSchema>;

interface EditPostFormProps {
	post: Post;
}

/**
 * Form component for editing existing posts.
 * Includes prefilling, Zod validation, character counters, auto-save, and unsaved changes warning.
 */
const EditPostForm = ({ post }: EditPostFormProps) => {
	const router = useRouter();
	const { profile, firebaseUser } = useIsUserLoggedIn();

	const [formData, setFormData] = useState<EditPostFormData>({
		title: post.title,
		content: post.content,
	});
	// Keep original data as constant for comparison (derived from prop)
	const originalData = useMemo<EditPostFormData>(
		() => ({
			title: post.title,
			content: post.content,
		}),
		[post.title, post.content]
	);
	const [errors, setErrors] = useState<{ title?: string; content?: string }>(
		{}
	);
	const [isDiscardDialogOpen, setIsDiscardDialogOpen] = useState(false);

	const [updatePost, { isLoading }] = useUpdatePostMutation();

	// Derive unsaved changes
	const hasUnsavedChanges =
		formData.title !== originalData.title ||
		formData.content !== originalData.content;

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
		if (hasUnsavedChanges && post.id) {
			const draft = {
				...formData,
				postId: post.id,
				timestamp: Date.now(),
			};
			localStorage.setItem(
				`post-edit-draft-${post.id}`,
				JSON.stringify(draft)
			);
		}
	}, [formData, hasUnsavedChanges, post.id]);

	// Load draft on mount
	useEffect(() => {
		if (post.id) {
			const savedDraft = localStorage.getItem(
				`post-edit-draft-${post.id}`
			);
			if (savedDraft) {
				try {
					const draft = JSON.parse(savedDraft);
					// Only load if draft is less than 24 hours old
					if (Date.now() - draft.timestamp < 24 * 60 * 60 * 1000) {
						// Use microtask to avoid synchronous setState in effect
						Promise.resolve().then(() => {
							setFormData({
								title: draft.title || post.title,
								content: draft.content || post.content,
							});
							toast.success("Draft restored");
						});
					} else {
						localStorage.removeItem(`post-edit-draft-${post.id}`);
					}
				} catch (err) {
					console.error("Error loading draft:", err);
				}
			}
		}
	}, [post.id, post.title, post.content]);

	// Validate form data
	const validate = useCallback((): boolean => {
		try {
			editPostSchema.parse(formData);
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
	}, [formData]);

	// Re-validate whenever formData changes
	useEffect(() => {
		// Use microtask to avoid synchronous setState in effect
		Promise.resolve().then(() => {
			validate();
		});
	}, [validate]);

	// Handle title input with character limit
	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setFormData((prev) => ({ ...prev, title: newValue }));
		}
	};

	// Handle content input with character limit
	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setFormData((prev) => ({ ...prev, content: newValue }));
		}
	};

	// Handle form submission
	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!validate()) {
			return;
		}

		try {
			await updatePost({
				postId: post.id,
				title: formData.title.trim(),
				content: formData.content.trim(),
			}).unwrap();

			toast.success("Post updated successfully!");
			localStorage.removeItem(`post-edit-draft-${post.id}`);
			router.push(`/feed/${post.id}`);
		} catch {
			toast.error("Failed to update post. Please try again.");
		}
	};

	// Handle back navigation with unsaved changes warning
	const handleBack = () => {
		if (hasUnsavedChanges) {
			setIsDiscardDialogOpen(true);
		} else {
			router.back();
		}
	};

	// Handle confirm discard
	const handleConfirmDiscard = () => {
		setIsDiscardDialogOpen(false);
		router.back();
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
				<CardTitle className="text-center text-2xl font-bold text-card-foreground">
					Edit Post
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
				<CardContent className="space-y-4">
					{/* Title Input */}
					<Field data-invalid={!!errors.title}>
						<FieldLabel>Title *</FieldLabel>
						<Input
							value={formData.title}
							onChange={handleTitleChange}
							disabled={isLoading}
							maxLength={TITLE_MAX_LENGTH}
							required
							dir="auto"
							className="text-lg bg-muted text-card-foreground"
						/>
						{errors.title && (
							<FieldError>{errors.title}</FieldError>
						)}
						{!errors.title && (
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

					{/* Content Input */}
					<Field data-invalid={!!errors.content}>
						<FieldLabel>Describe Your Problem *</FieldLabel>
						<Textarea
							value={formData.content}
							onChange={handleContentChange}
							disabled={isLoading}
							maxLength={BODY_MAX_LENGTH}
							dir="auto"
							className="min-h-45 resize-none bg-muted text-card-foreground"
						/>
						{errors.content && (
							<FieldError>{errors.content}</FieldError>
						)}
						{!errors.content && (
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
				</CardContent>

				<CardFooter className="flex flex-col gap-3 bg-card">
					{/* Save Button */}
					<Button
						type="submit"
						disabled={isLoading || Object.keys(errors).length > 0}
						className="w-full py-6 bg-primary hover:bg-primary/90 rounded-full text-lg font-semibold"
					>
						{isLoading ? (
							<>
								<Loader2 className="mr-2 h-4 w-4 animate-spin" />
								Saving...
							</>
						) : (
							"Save Changes"
						)}
					</Button>

					{/* Cancel Button */}
					<Button
						type="button"
						variant="outline"
						className="w-full py-6 rounded-full text-lg font-semibold"
						onClick={handleBack}
						disabled={isLoading}
					>
						Cancel
					</Button>

					{/* Auto-save indicator */}
					{hasUnsavedChanges && (
						<div className="text-center text-sm text-muted-foreground">
							Draft auto-saved
						</div>
					)}
				</CardFooter>
			</form>

			{/* Discard Changes Confirmation Dialog */}
			<Dialog
				open={isDiscardDialogOpen}
				onOpenChange={setIsDiscardDialogOpen}
			>
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
							onClick={() => setIsDiscardDialogOpen(false)}
						>
							Cancel
						</Button>
						<Button
							variant="destructive"
							onClick={handleConfirmDiscard}
						>
							Discard
						</Button>
					</DialogFooter>
				</DialogContent>
			</Dialog>
		</Card>
	);
};

export default EditPostForm;
