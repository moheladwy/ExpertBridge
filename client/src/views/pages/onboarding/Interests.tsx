import { Button } from "@/views/components/ui/button";
import { useEffect, useState, KeyboardEvent } from "react";
import { useGetTagsQuery } from "@/features/tags/tagsSlice";
import { useOnboardUserMutation } from "@/features/profiles/profilesSlice";
import toast from "react-hot-toast";
import { useNavigate } from "react-router";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { cn } from "@/lib/util/utils";

function Interests() {
	const navigate = useNavigate();
	const { data: tags = [], isLoading: isTagsLoading } = useGetTagsQuery();
	const [onboardUser, { isLoading: isUpdating, isSuccess, isError, error }] =
		useOnboardUserMutation();

	// State for managing selected tags
	const [selectedTags, setSelectedTags] = useState<string[]>([]);
	const [inputValue, setInputValue] = useState("");

	const [, , , , userProfile] = useIsUserLoggedIn();

	// Check if we can submit (5 or more tags selected)
	const canSubmit = selectedTags.length >= 5;

	// Handler for tag input
	const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		setInputValue(event.target.value);
	};

	// Handler for adding a tag
	const addTag = (tagName: string) => {
		if (!tagName.trim()) return;

		const normalizedTagName = tagName.trim().toLowerCase();

		// Don't add if already exists
		if (selectedTags.includes(normalizedTagName)) return;

		// Check if tag exists in the tags list
		const matchedTag = tags.find(
			(tag) =>
				(tag.englishName &&
					tag.englishName.toLowerCase() === normalizedTagName) ||
				(tag.arabicName &&
					tag.arabicName.toLowerCase() === normalizedTagName)
		);

		// Add tag (use matched tag name if found, otherwise use input)
		const tagToAdd = matchedTag
			? matchedTag.englishName.toLowerCase()
			: normalizedTagName;
		setSelectedTags([...selectedTags, tagToAdd]);
		setInputValue(""); // Clear input after adding
	};

	// Handler for removing a tag
	const removeTag = (tagToRemove: string) => {
		setSelectedTags(
			selectedTags.filter((tag) => tag !== tagToRemove.toLowerCase())
		);
	};

	// Handler for keyboard input (Enter to add tag)
	const handleKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
		if (event.key === "Enter") {
			event.preventDefault();
			addTag(inputValue);
		}
	};

	// Handle submission of interests
	const handleSubmitInterests = async () => {
		if (canSubmit) {
			console.log(
				`Selected Tags to be submitted to the user => [${selectedTags}]`
			);
			await onboardUser({
				tags: selectedTags,
			});
		} else {
			toast.error("Please select at least 5 tags before continuing");
		}
	};

	// Redirect if already onboarded
	useEffect(() => {
		if (userProfile && userProfile.isOnboarded) {
			navigate("/home");
		}
	}, [userProfile, navigate]);

	// Handle onboarding result
	useEffect(() => {
		if (isSuccess) {
			toast.success("Onboarding successful");
			// navigate("/home");
		} else if (isError) {
			toast.error(`Onboarding failed ${isError}`);
			console.error(error);
		}
	}, [isSuccess, isError, navigate, error]);

	return (
		<div className="min-h-screen w-full flex justify-center items-center bg-linear-to-b from-muted/30 to-muted/50 p-4 transition-colors duration-200">
			<div className="w-full max-w-3xl bg-card rounded-xl border border-border shadow-xl backdrop-blur-sm overflow-hidden transition-colors duration-200">
				{/* Header Section */}
				<div className="space-y-3 px-6 py-8 bg-linear-to-r from-primary/5 to-primary/10">
					<h1 className="text-center font-bold text-3xl text-card-foreground transition-colors duration-200">
						What are you good at / interested in?
					</h1>
					<h2 className="text-center text-lg text-muted-foreground transition-colors duration-200">
						Select topics to shape your personalized experience.
					</h2>
				</div>

				{/* Divider */}
				<div className="h-px bg-border transition-colors duration-200" />

				{/* Input Section */}
				<div className="p-6">
					<div className="relative mb-6">
						<input
							type="text"
							placeholder="Type a keyword and press Enter..."
							value={inputValue}
							onChange={handleInputChange}
							onKeyDown={handleKeyDown}
							className="w-full px-4 py-3 rounded-lg bg-background border border-border text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary transition-all duration-200"
						/>
					</div>

					{/* Selected Tags Display */}
					<div className="mb-6">
						<div className="flex items-center justify-between mb-3">
							<h3 className="text-sm font-semibold text-card-foreground transition-colors duration-200">
								Selected Keywords
							</h3>
							<span className="text-xs font-medium px-2.5 py-1 rounded-full bg-primary/10 text-primary">
								{selectedTags.length} / 5 min
							</span>
						</div>
						<div className="flex flex-wrap gap-2 min-h-[60px] p-4 rounded-lg border border-border bg-muted/30">
							{selectedTags.length === 0 ? (
								<p className="text-sm text-muted-foreground italic transition-colors duration-200 w-full text-center py-2">
									No keywords selected yet. Select at least 5
									to continue.
								</p>
							) : (
								selectedTags.map((tag, index) => (
									<button
										key={index}
										onClick={() => removeTag(tag)}
										className="px-4 py-1.5 text-sm text-primary-foreground rounded-full bg-primary shadow-sm hover:shadow-md hover:bg-primary/90 transition-all duration-200 font-medium"
									>
										{tag}
										<span className="ml-2 text-base">
											×
										</span>
									</button>
								))
							)}
						</div>
					</div>

					{/* Suggestions Section (if needed) */}
					{inputValue.trim() !== "" && !isTagsLoading && (
						<div className="mb-6">
							<h3 className="text-sm font-semibold text-card-foreground mb-3 transition-colors duration-200">
								Suggestions
							</h3>
							<div className="flex flex-wrap gap-2">
								{tags
									.filter((tag) => {
										const normalizedInput = inputValue
											.trim()
											.toLowerCase();
										return (
											((tag.englishName &&
												tag.englishName
													.toLowerCase()
													.includes(
														normalizedInput
													)) ||
												(tag.arabicName &&
													tag.arabicName
														.toLowerCase()
														.includes(
															normalizedInput
														))) &&
											!selectedTags.includes(
												tag.englishName.toLowerCase()
											)
										);
									})
									.slice(0, 5)
									.map((tag) => (
										<button
											key={tag.tagId}
											onClick={() =>
												addTag(tag.englishName)
											}
											className="px-4 py-1.5 text-sm bg-muted text-card-foreground rounded-full hover:bg-primary/10 hover:text-primary border border-border hover:border-primary/50 transition-all duration-200 font-medium"
										>
											{tag.englishName}
										</button>
									))}
							</div>
						</div>
					)}

					{/* Submit Button */}
					<Button
						className={cn(
							"w-full py-3 rounded-full transition-all duration-200 font-semibold text-base",
							canSubmit
								? "bg-linear-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 text-white shadow-md hover:shadow-lg"
								: "bg-muted text-muted-foreground cursor-not-allowed"
						)}
						disabled={!canSubmit || isUpdating}
						onClick={handleSubmitInterests}
					>
						{isUpdating ? (
							<span className="flex items-center justify-center">
								<svg
									className="animate-spin -ml-1 mr-2 h-5 w-5 text-white"
									xmlns="http://www.w3.org/2000/svg"
									fill="none"
									viewBox="0 0 24 24"
								>
									<circle
										className="opacity-25"
										cx="12"
										cy="12"
										r="10"
										stroke="currentColor"
										strokeWidth="4"
									></circle>
									<path
										className="opacity-75"
										fill="currentColor"
										d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
									></path>
								</svg>
								Saving...
							</span>
						) : canSubmit ? (
							"Continue"
						) : (
							`Add ${5 - selectedTags.length} more keyword${
								selectedTags.length === 4 ? "" : "s"
							} to continue`
						)}
					</Button>
				</div>
			</div>
		</div>
	);
}

export default Interests;
