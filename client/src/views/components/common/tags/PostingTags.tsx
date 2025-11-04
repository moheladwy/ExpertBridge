import { Tag } from "@/features/tags/types";

interface PostingTagsProps {
	tags: Tag[];
	language?: string;
	className?: string;
}

const PostingTags: React.FC<PostingTagsProps> = ({
	tags,
	language,
	className = "",
}) => {
	if (!tags || tags.length === 0) {
		return null;
	}

	return (
		<div className={`flex flex-wrap gap-2 ${className}`}>
			{tags.map((tag, index) => (
				<span
					key={index}
					className="text-xs bg-primary/10 text-primary px-3 py-1 rounded-full font-medium border border-primary/20 hover:bg-primary/20 transition-colors cursor-pointer"
				>
					{language === "Arabic" ? tag.arabicName : tag.englishName}
				</span>
			))}
		</div>
	);
};

export default PostingTags;
