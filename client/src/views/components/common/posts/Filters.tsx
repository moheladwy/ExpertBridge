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
			<ToggleGroupItem value="Recent">Recent</ToggleGroupItem>
			<ToggleGroupItem value="Most Upvoted">Most Upvoted</ToggleGroupItem>
			<ToggleGroupItem value="Trending">Trending</ToggleGroupItem>
		</ToggleGroup>
	);
};

export default Filters;
