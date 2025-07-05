import { Tag } from "@/features/tags/types";

interface PostingTagsProps {
	tags: Tag[];
	language?: string;
	className?: string;
}

const PostingTags: React.FC<PostingTagsProps> = ({ tags, language, className = "" }) => {
	if (!tags || tags.length === 0) {
		return null;
	}

	return (
		<div className={`flex flex-wrap gap-2 ${className}`}>
			{tags.map((tag, index) => (
				<span
					key={index}
					className="text-xs bg-purple-100 dark:bg-purple-900 text-purple-800 dark:text-purple-200 px-3 py-1 rounded-full font-medium border border-purple-200 dark:border-purple-700 hover:bg-purple-200 dark:hover:bg-purple-800 transition-colors cursor-pointer"
				>
					{language === 'Arabic' ? tag.arabicName : tag.englishName}
				</span>
			))}
		</div>
	);
};

export default PostingTags;
