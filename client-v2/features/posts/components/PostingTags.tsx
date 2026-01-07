"use client";

import { Tag } from "@/features/posts/types";

interface PostingTagsProps {
	tags: Tag[];
	language?: string;
	className?: string;
}

/**
 * Gets the display name for a tag based on language preference.
 * Falls back through: preferred language → alternate language → empty string.
 */
function getTagDisplayName(tag: Tag, language: string): string {
	if (language === "ar") {
		return tag?.arabicName ?? tag?.englishName ?? "";
	}
	// Default to English for "en" or any unknown language value
	return tag?.englishName ?? tag?.arabicName ?? "";
}

/**
 * Displays a list of tags as styled badges.
 * Supports both English and Arabic tag names based on language prop.
 */
export default function PostingTags({
	tags,
	language = "en",
	className = "",
}: PostingTagsProps) {
	if (!tags || tags.length === 0) {
		return null;
	}

	return (
		<div className={`flex flex-wrap gap-2 ${className}`}>
			{tags.map((tag) => (
				<span
					key={tag.id}
					className="text-xs bg-primary/10 text-primary px-3 py-1 rounded-full font-medium border border-primary/20"
				>
					{getTagDisplayName(tag, language)}
				</span>
			))}
		</div>
	);
}
