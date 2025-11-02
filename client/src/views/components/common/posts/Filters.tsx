import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import { useState } from "react";

const Filters = () => {
	const [filter, setFilter] = useState("Recent");

	const handleChange = (
		event: React.MouseEvent<HTMLElement>,
		newFilter: string
	) => {
		setFilter(newFilter);
	};

	return (
		<ToggleButtonGroup
			color="primary"
			value={filter}
			exclusive
			onChange={handleChange}
			aria-label="Platform"
		>
			<ToggleButton value="Recent" className="dark:text-white">
				Recent
			</ToggleButton>
			<ToggleButton value="Most Upvoted" className="dark:text-white">
				Most Upvoted
			</ToggleButton>
			<ToggleButton value="Trending" className="dark:text-white">
				Trending
			</ToggleButton>
		</ToggleButtonGroup>
	);
};

export default Filters;
