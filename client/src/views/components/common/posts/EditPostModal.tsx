import React, { useState, useEffect } from "react"; // Import useEffect
import { z } from "zod";
import { useUpdatePostMutation } from "@/features/posts/postsSlice";
import { Post } from "@/features/posts/types";
import { Button } from "@/views/components/ui/button";
import {
	Card,
	CardContent,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/views/components/ui/card";
import { Loader2 } from "lucide-react";
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
import { X } from "lucide-react";

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

interface EditPostModalProps {
	post: Post;
	isOpen: boolean;
	onClose: () => void;
}

const EditPostModal: React.FC<EditPostModalProps> = ({
	post,
	isOpen,
	onClose,
}) => {
	const [formData, setFormData] = useState<EditPostFormData>({
		title: post.title,
		content: post.content || "",
	});
	const [errors, setErrors] = useState<{ [key: string]: string }>({});

	const [updatePost, { isLoading }] = useUpdatePostMutation();

	// Function to validate the form data
	const validate = (): boolean => {
		try {
			editPostSchema.parse(formData);
			setErrors({}); // Clear errors if validation passes
			return true;
		} catch (error) {
			if (error instanceof z.ZodError) {
				const newErrors: { [key: string]: string } = {};
				error.errors.forEach((err) => {
					if (err.path.length > 0) {
						// Check path length
						newErrors[err.path[0]] = err.message;
					}
				});
				setErrors(newErrors); // Set new errors if validation fails
			}
			return false;
		}
	};

	// Re-validate whenever formData changes
	useEffect(() => {
		validate();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [formData]); // Dependency array ensures validation runs when formData updates

	// Handle title input with character limit
	const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= TITLE_MAX_LENGTH) {
			setFormData({ ...formData, title: newValue });
		}
	};

	// Handle content input with character limit
	const handleContentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
		const newValue = e.target.value;
		if (newValue.length <= BODY_MAX_LENGTH) {
			setFormData({ ...formData, content: newValue });
		}
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		// Re-run validation on submit just to be absolutely sure and prevent submission if invalid
		if (!validate()) {
			return;
		}

		try {
			await updatePost({
				postId: post.id,
				title: formData.title,
				content: formData.content,
			}).unwrap();
			toast.success("Post updated successfully");
			onClose();
		} catch (error) {
			toast.error("Failed to update post");
			console.error("Error updating post:", error);
		}
	};

	const [, , , authUser, userProfile] = useIsUserLoggedIn();

	const titleCharsLeft = TITLE_MAX_LENGTH - formData.title.length;
	const bodyCharsLeft = BODY_MAX_LENGTH - formData.content.length;

	if (!isOpen) return null;

	return (
		<div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
			{/* Added backdrop-blur-sm for better background effect */}
			<Card className="w-full max-w-lg mx-auto dark:bg-gray-800 dark:text-white relative">
				{/* Close Button */}
				<div className="absolute top-3 right-3 z-10">
					<Button onClick={onClose} variant="ghost" size="icon">
						<X className="h-4 w-4 dark:text-gray-300" />
					</Button>
				</div>

				<CardHeader>
					{/* Centered, larger, and bold title */}
					<CardTitle className="text-center text-2xl font-bold dark:text-white">
						Edit Post
					</CardTitle>
					<Separator className="dark:bg-gray-600 mt-2" />

					{/* User Profile Info */}
					<div className="flex items-center mb-2 mt-2">
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
						<div className="font-medium dark:text-white">
							{authUser?.displayName || "User"}
							<span className="text-gray-500 dark:text-gray-400 block text-sm">
								{` @${userProfile?.username || "username"}`}
							</span>
						</div>
					</div>
				</CardHeader>
				<form onSubmit={handleSubmit}>
					<CardContent className="space-y-4">
						<Field>
							<FieldLabel>Title *</FieldLabel>
							<Input
								value={formData.title}
								onChange={handleTitleChange}
								disabled={isLoading}
								maxLength={TITLE_MAX_LENGTH}
								dir="auto"
								className="text-lg dark:bg-gray-700 dark:text-white"
							/>
							{errors.title && (
								<FieldError>{errors.title}</FieldError>
							)}
							{!errors.title && (
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
						<Field>
							<FieldLabel>Describe Your Problem *</FieldLabel>
							<Textarea
								value={formData.content}
								onChange={handleContentChange}
								disabled={isLoading}
								maxLength={BODY_MAX_LENGTH}
								dir="auto"
								className="min-h-[180px] resize-none dark:bg-gray-700 dark:text-white"
							/>
							{errors.content && (
								<FieldError>{errors.content}</FieldError>
							)}
							{!errors.content && (
								<FieldDescription>
									<span
										className={
											bodyCharsLeft < 1
												? "text-red-500"
												: "text-green-500 dark:text-green-400"
										}
									>
										{bodyCharsLeft} characters left
									</span>
								</FieldDescription>
							)}
						</Field>
					</CardContent>
					<CardFooter className="flex justify-center dark:bg-gray-800">
						<Button
							type="submit"
							disabled={
								isLoading || Object.keys(errors).length > 0
							}
							className="w-full py-6 dark:bg-blue-700 dark:hover:bg-blue-800 dark:text-white bg-[#162955] hover:bg-[#0e1c3b]"
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
					</CardFooter>
				</form>
			</Card>
		</div>
	);
};

export default EditPostModal;
