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
		<div className="min-h-screen w-full flex justify-center items-center bg-gradient-to-b from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800 p-4 transition-colors duration-200">
			<div className="w-full max-w-3xl bg-white dark:bg-gray-900 rounded-xl border border-gray-200 dark:border-gray-700 shadow-lg dark:shadow-gray-800/20 backdrop-blur-sm overflow-hidden transition-colors duration-200">
				{/* Header Section */}
				<div className="space-y-2 px-6 py-8">
					<h1 className="text-center font-bold text-3xl bg-gradient-to-r from-blue-600 to-indigo-600 dark:from-blue-400 dark:to-indigo-400 bg-clip-text text-transparent transition-colors duration-200">
						What are you good at / interested in?
					</h1>
					<h2 className="text-center text-lg text-gray-600 dark:text-gray-300 transition-colors duration-200">
						Select topics to shape your personalized experience.
					</h2>
				</div>

				{/* Divider */}
				<div className="h-px bg-gray-200 dark:bg-gray-800 transition-colors duration-200" />

				{/* Input Section */}
				<div className="p-6">
					<div className="relative mb-6">
						<input
							type="text"
							placeholder="Type a keyword and press Enter..."
							value={inputValue}
							onChange={handleInputChange}
							onKeyDown={handleKeyDown}
							className="w-full px-4 py-3 rounded-lg bg-gray-100 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 text-gray-900 dark:text-gray-100 placeholder:text-gray-500 dark:placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 transition-all duration-200"
						/>
					</div>

					{/* Selected Tags Display */}
					<div className="mb-6">
						<h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2 transition-colors duration-200">
							Selected Keywords
						</h3>
						<div className="flex flex-wrap gap-2">
							{selectedTags.length === 0 ? (
								<p className="text-sm text-gray-500 dark:text-gray-400 italic transition-colors duration-200">
									No keywords selected yet. Select at least 5
									to continue.
								</p>
							) : (
								selectedTags.map((tag, index) => (
									<button
										key={index}
										onClick={() => removeTag(tag)}
										className="px-4 py-1.5 text-sm text-white rounded-full bg-gradient-to-r from-blue-600 to-indigo-600 dark:from-blue-500 dark:to-indigo-500 shadow-md hover:shadow-lg hover:opacity-90 transition-all duration-200 dark:shadow-gray-900/40"
									>
										{tag}
										<span className="ml-2">×</span>
									</button>
								))
							)}
						</div>
					</div>

					{/* Suggestions Section (if needed) */}
					{inputValue.trim() !== "" && !isTagsLoading && (
						<div className="mb-6">
							<h3 className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2 transition-colors duration-200">
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
											className="px-4 py-1.5 text-sm bg-gray-100 dark:bg-gray-800 text-gray-800 dark:text-gray-200 rounded-full hover:bg-gray-200 dark:hover:bg-gray-700 border border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600 transition-all duration-200"
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
							"w-full py-3 rounded-lg transition-all duration-200",
							canSubmit
								? "bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600 text-white shadow-md hover:shadow-lg dark:shadow-gray-900/30 dark:hover:shadow-gray-900/50"
								: "bg-gray-400 dark:bg-gray-700 text-white dark:text-gray-300 cursor-not-allowed opacity-90 dark:opacity-80"
						)}
						disabled={!canSubmit || isUpdating}
						onClick={handleSubmitInterests}
					>
						{isUpdating ? (
							<span className="flex items-center justify-center">
								<svg
									className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
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
										className="opacity-75 dark:opacity-90"
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
