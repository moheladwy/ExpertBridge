import {
	AdjustmentsHorizontalIcon,
	MagnifyingGlassIcon,
	ComputerDesktopIcon,
} from "@heroicons/react/24/outline";
import { MapPinIcon, XCircleIcon } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router";

interface SearchSidebarProps {
	currentQuery: string;
	currentArea: string;
	currentMinBudget: number;
	currentMaxBudget: number;
	currentIsRemote: boolean;
}

const SearchSidebar: React.FC<SearchSidebarProps> = ({
	currentQuery,
	currentArea,
	currentMinBudget,
	currentMaxBudget,
	currentIsRemote,
}) => {
	const navigate = useNavigate();
	const [query, setQuery] = useState(currentQuery);
	const [area, setArea] = useState(currentArea);
	const [minBudget, setMinBudget] = useState(currentMinBudget);
	const [maxBudget, setMaxBudget] = useState(currentMaxBudget);
	const [isRemote, setIsRemote] = useState(currentIsRemote);

	const handleSearch = (e: React.FormEvent) => {
		e.preventDefault();

		const searchParams = new URLSearchParams();
		if (query) searchParams.set("query", query);
		if (area) searchParams.set("area", area);
		if (minBudget > 0) searchParams.set("minBudget", minBudget.toString());
		if (maxBudget > 0) searchParams.set("maxBudget", maxBudget.toString());
		if (isRemote) searchParams.set("isRemote", "true");

		navigate(`/search/jobs?${searchParams.toString()}`);
	};

	const clearFilters = () => {
		setQuery("");
		setArea("");
		setMinBudget(0);
		setMaxBudget(0);
		setIsRemote(false);
		navigate(`/search/jobs?query=${currentQuery}`);
	};

	return (
		<div className="bg-card rounded-lg shadow-md p-4 sticky top-20">
			<div className="flex items-center justify-between mb-4">
				<h2 className="text-lg font-semibold text-card-foreground">
					Filter Jobs
				</h2>
				<AdjustmentsHorizontalIcon className="h-5 w-5 text-muted-foreground" />
			</div>

			<form onSubmit={handleSearch} className="space-y-4">
				{/* Search input */}
				<div>
					<label
						htmlFor="search-query"
						className="block text-sm font-medium text-card-foreground mb-1"
					>
						Keywords
					</label>
					<div className="relative">
						<input
							id="search-query"
							type="text"
							value={query}
							onChange={(e) => setQuery(e.target.value)}
							placeholder="Search jobs..."
							className="w-full px-4 py-2 border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent pr-10"
						/>
						<MagnifyingGlassIcon className="absolute right-3 top-2.5 h-5 w-5 text-muted-foreground" />
					</div>
				</div>

				{/* Location input */}
				<div>
					<label
						htmlFor="area"
						className="block text-sm font-medium text-card-foreground mb-1"
					>
						Location
					</label>
					<div className="relative">
						<input
							id="area"
							type="text"
							value={area}
							onChange={(e) => setArea(e.target.value)}
							placeholder="City, state, country..."
							className="w-full px-4 py-2 border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent pr-10"
						/>
						<MapPinIcon className="absolute right-3 top-2.5 h-5 w-5 text-muted-foreground" />
					</div>
				</div>

				{/* Budget range */}
				<div>
					<label className="block text-sm font-medium text-card-foreground mb-1">
						Budget Range
					</label>
					<div className="flex items-center space-x-2">
						<div className="relative flex-1">
							<div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
								<span className="text-muted-foreground">$</span>
							</div>
							<input
								type="number"
								value={minBudget || ""}
								onChange={(e) =>
									setMinBudget(parseInt(e.target.value) || 0)
								}
								placeholder="Min"
								min="0"
								className="w-full pl-7 px-3 py-2 border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent"
							/>
						</div>
						<span className="text-muted-foreground">-</span>
						<div className="relative flex-1">
							<div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
								<span className="text-muted-foreground">$</span>
							</div>
							<input
								type="number"
								value={maxBudget || ""}
								onChange={(e) =>
									setMaxBudget(parseInt(e.target.value) || 0)
								}
								placeholder="Max"
								min="0"
								className="w-full pl-7 px-3 py-2 border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent"
							/>
						</div>
					</div>
				</div>

				{/* Remote toggle */}
				<div className="flex items-center">
					<input
						id="remote-toggle"
						type="checkbox"
						checked={isRemote}
						onChange={(e) => setIsRemote(e.target.checked)}
						className="h-4 w-4 text-primary focus:ring-ring border-input rounded"
					/>
					<label
						htmlFor="remote-toggle"
						className="ml-2 block text-sm text-card-foreground"
					>
						Remote only
					</label>
					<ComputerDesktopIcon className="ml-auto h-5 w-5 text-muted-foreground" />
				</div>

				{/* Action buttons */}
				<div className="pt-4 border-t border-border space-y-3">
					<button
						type="submit"
						className="w-full bg-primary hover:bg-primary/90 text-primary-foreground font-medium py-2 px-4 rounded-md transition duration-150"
					>
						Apply Filters
					</button>

					<button
						type="button"
						onClick={clearFilters}
						className="w-full bg-secondary hover:bg-secondary/90 text-secondary-foreground font-medium py-2 px-4 rounded-md transition duration-150 flex items-center justify-center"
					>
						<XCircleIcon className="h-5 w-5 mr-1" />
						Clear All Filters
					</button>
				</div>
			</form>
		</div>
	);
};

export default SearchSidebar;
