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
		<div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 sticky top-20">
			<div className="flex items-center justify-between mb-4">
				<h2 className="text-lg font-semibold text-gray-900 dark:text-white">
					Filter Jobs
				</h2>
				<AdjustmentsHorizontalIcon className="h-5 w-5 text-gray-500 dark:text-gray-400" />
			</div>

			<form onSubmit={handleSearch} className="space-y-4">
				{/* Search input */}
				<div>
					<label
						htmlFor="search-query"
						className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
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
							className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent pr-10"
						/>
						<MagnifyingGlassIcon className="absolute right-3 top-2.5 h-5 w-5 text-gray-400 dark:text-gray-500" />
					</div>
				</div>

				{/* Location input */}
				<div>
					<label
						htmlFor="area"
						className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
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
							className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent pr-10"
						/>
						<MapPinIcon className="absolute right-3 top-2.5 h-5 w-5 text-gray-400 dark:text-gray-500" />
					</div>
				</div>

				{/* Budget range */}
				<div>
					<label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
						Budget Range
					</label>
					<div className="flex items-center space-x-2">
						<div className="relative flex-1">
							<div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
								<span className="text-gray-500 dark:text-gray-400">
									$
								</span>
							</div>
							<input
								type="number"
								value={minBudget || ""}
								onChange={(e) =>
									setMinBudget(parseInt(e.target.value) || 0)
								}
								placeholder="Min"
								min="0"
								className="w-full pl-7 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent"
							/>
						</div>
						<span className="text-gray-500 dark:text-gray-400">
							-
						</span>
						<div className="relative flex-1">
							<div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
								<span className="text-gray-500 dark:text-gray-400">
									$
								</span>
							</div>
							<input
								type="number"
								value={maxBudget || ""}
								onChange={(e) =>
									setMaxBudget(parseInt(e.target.value) || 0)
								}
								placeholder="Max"
								min="0"
								className="w-full pl-7 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent"
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
						className="h-4 w-4 text-blue-600 dark:text-blue-500 focus:ring-blue-500 dark:focus:ring-blue-400 border-gray-300 dark:border-gray-600 rounded"
					/>
					<label
						htmlFor="remote-toggle"
						className="ml-2 block text-sm text-gray-700 dark:text-gray-300"
					>
						Remote only
					</label>
					<ComputerDesktopIcon className="ml-auto h-5 w-5 text-gray-400 dark:text-gray-500" />
				</div>

				{/* Action buttons */}
				<div className="pt-4 border-t border-gray-200 dark:border-gray-700 space-y-3">
					<button
						type="submit"
						className="w-full bg-blue-600 hover:bg-blue-700 dark:bg-blue-700 dark:hover:bg-blue-600 text-white font-medium py-2 px-4 rounded-md transition duration-150"
					>
						Apply Filters
					</button>

					<button
						type="button"
						onClick={clearFilters}
						className="w-full bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300 font-medium py-2 px-4 rounded-md transition duration-150 flex items-center justify-center"
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
