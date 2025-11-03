import { useState } from "react";
import {
	ToggleGroup,
	ToggleGroupItem,
} from "@/views/components/ui/toggle-group";

const Filters = () => {
	const [filter, setFilter] = useState("Recent");

	return (
		<ToggleGroup
			type="single"
			value={filter}
			onValueChange={(value) => value && setFilter(value)}
		>
			<ToggleGroupItem value="Recent" className="dark:text-white">
				Recent
			</ToggleGroupItem>
			<ToggleGroupItem value="Most Upvoted" className="dark:text-white">
				Most Upvoted
			</ToggleGroupItem>
			<ToggleGroupItem value="Trending" className="dark:text-white">
				Trending
			</ToggleGroupItem>
		</ToggleGroup>
	);
};

export default Filters;
