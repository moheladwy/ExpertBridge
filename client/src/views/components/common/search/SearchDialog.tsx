import { Button } from "@/views/components/ui/button";
import { DialogTitle, DialogDescription } from "@radix-ui/react-dialog";
import {
	DropdownMenu,
	DropdownMenuTrigger,
	DropdownMenuContent,
	DropdownMenuItem,
} from "@/views/components/ui/dropdown-menu";
import { CommandDialog, CommandInput } from "@/views/components/ui/command";
import {
	FileQuestion,
	User,
	Briefcase,
	PanelRight,
	MapPin,
	DollarSign,
	MonitorSmartphone,
	Sparkles,
	Tag,
	Clock,
	History,
	Bookmark,
} from "lucide-react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

interface SearchDialogProps {
	open: boolean;
	setOpen: (open: boolean) => void;
}

const SearchDialog = ({ open, setOpen }: SearchDialogProps) => {
	const navigate = useNavigate();
	const [isLoggedIn] = useIsUserLoggedIn();

	const [searchType, setSearchType] = useState<"posts" | "users" | "jobs">(
		"posts"
	);
	const [searchInput, setSearchInput] = useState("");
	const [showFilters, setShowFilters] = useState(false);
	const [searchFilters, setSearchFilters] = useState({
		location: "",
		minBudget: "",
		maxBudget: "",
		isRemote: false,
	});
	const [recentSearches, setRecentSearches] = useState<
		{ term: string; type: "posts" | "users" | "jobs" }[]
	>([]);

	// Load recent searches from localStorage
	useEffect(() => {
		const savedSearches = localStorage.getItem("recentSearches");
		if (savedSearches) {
			try {
				setRecentSearches(JSON.parse(savedSearches).slice(0, 5));
			} catch (e) {
				console.error("Error parsing recent searches:", e);
			}
		}
	}, []);

	// Save search to recent searches
	const saveToRecentSearches = (
		term: string,
		type: "posts" | "users" | "jobs"
	) => {
		if (!term.trim()) return;

		const newSearch = { term, type };
		const updatedSearches = [
			newSearch,
			...recentSearches
				.filter((s) => !(s.term === term && s.type === type))
				.slice(0, 4),
		];

		setRecentSearches(updatedSearches);
		localStorage.setItem("recentSearches", JSON.stringify(updatedSearches));
	};

	const handleSearch = () => {
		if (searchInput.trim()) {
			setOpen(false);
			let searchPath;
			const params = new URLSearchParams();

			params.set("query", searchInput.trim());

			if (searchType === "posts") {
				searchPath = "/search/p";
			} else if (searchType === "users") {
				searchPath = "/search/u";
			} else if (searchType === "jobs") {
				searchPath = "/search/jobs";

				// Add job-specific filters
				if (showFilters) {
					if (searchFilters.location)
						params.set("area", searchFilters.location);
					if (searchFilters.minBudget)
						params.set("minBudget", searchFilters.minBudget);
					if (searchFilters.maxBudget)
						params.set("maxBudget", searchFilters.maxBudget);
					if (searchFilters.isRemote) params.set("isRemote", "true");
				}
			}

			// Save to recent searches
			saveToRecentSearches(searchInput.trim(), searchType);

			navigate(`${searchPath}?${params.toString()}`);
		}
	};

	const selectRecentSearch = (search: {
		term: string;
		type: "posts" | "users" | "jobs";
	}) => {
		setSearchInput(search.term);
		setSearchType(search.type);
		setOpen(false);

		const searchPath =
			search.type === "posts"
				? "/search/p"
				: search.type === "users"
					? "/search/u"
					: "/search/jobs";

		navigate(`${searchPath}?query=${encodeURIComponent(search.term)}`);
	};
	return (
		<CommandDialog
			open={open}
			onOpenChange={setOpen}
			// className="w-[90vw] max-w-[90vw] md:w-[75vw] md:max-w-[75vw] lg:w-[65vw] lg:max-w-[65vw]"
		>
			<DialogTitle className="sr-only">Search</DialogTitle>
			<DialogDescription className="sr-only">
				Search about questions / users / jobs
			</DialogDescription>

			<div className="flex flex-col h-full w-full bg-card rounded-lg shadow-xl">
				<div className="flex items-center border-b px-3 py-2 bg-card gap-2">
					<DropdownMenu>
						<DropdownMenuTrigger asChild className="bg-muted">
							<Button
								variant="outline"
								size="sm"
								className="bg-muted border-input min-w-[110px] justify-center"
							>
								{searchType === "posts" ? (
									<>
										<FileQuestion className="h-4 w-4 mr-2" />{" "}
										Questions
									</>
								) : searchType === "users" ? (
									<>
										<User className="h-4 w-4 mr-2" /> Users
									</>
								) : (
									<>
										<Briefcase className="h-4 w-4 mr-2" />{" "}
										Jobs
									</>
								)}
							</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent align="end" className="w-[180px]">
							<DropdownMenuItem
								onClick={() => {
									setSearchType("posts");
									setShowFilters(false);
								}}
								className={
									searchType === "posts"
										? "bg-primary/10"
										: ""
								}
							>
								<FileQuestion className="h-4 w-4 mr-2" />
								Search Questions
							</DropdownMenuItem>
							<DropdownMenuItem
								onClick={() => {
									setSearchType("users");
									setShowFilters(false);
								}}
								className={
									searchType === "users"
										? "bg-primary/10"
										: ""
								}
							>
								<User className="h-4 w-4 mr-2" />
								Search Users
							</DropdownMenuItem>
							<DropdownMenuItem
								onClick={() => setSearchType("jobs")}
								className={
									searchType === "jobs" ? "bg-primary/10" : ""
								}
							>
								<Briefcase className="h-4 w-4 mr-2" />
								Search Jobs
							</DropdownMenuItem>
						</DropdownMenuContent>
					</DropdownMenu>
					<CommandInput
						placeholder={`Search ${
							searchType === "posts"
								? "questions"
								: searchType === "users"
									? "users"
									: "jobs"
						}...`}
						value={searchInput}
						onValueChange={setSearchInput}
						onKeyDown={(e) => {
							if (e.key === "Enter" && searchInput.trim()) {
								handleSearch();
							}
						}}
					/>
					{searchType === "jobs" && (
						<Button
							variant="ghost"
							size="sm"
							onClick={() => setShowFilters(!showFilters)}
							className="mr-1 hover:bg-accent"
							aria-label={
								showFilters ? "Hide filters" : "Show filters"
							}
							title={
								showFilters ? "Hide filters" : "Show filters"
							}
						>
							<PanelRight
								className={`h-4 w-4 transition-colors ${showFilters ? "text-blue-500" : ""}`}
							/>
						</Button>
					)}
				</div>

				{/* Job Search Filters */}
				{searchType === "jobs" && showFilters && (
					<div className="p-4 border-b border-border grid grid-cols-1 md:grid-cols-2 gap-4 bg-secondary/50">
						<div className="space-y-1">
							<label className="text-xs font-medium text-muted-foreground">
								Location
							</label>
							<div className="flex items-center relative">
								<MapPin className="h-4 w-4 absolute left-2 text-gray-400" />
								<input
									type="text"
									value={searchFilters.location}
									onChange={(e) =>
										setSearchFilters({
											...searchFilters,
											location: e.target.value,
										})
									}
									placeholder="City, state, country..."
									className="w-full pl-8 py-2 text-sm border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent transition-colors"
								/>
							</div>
						</div>

						<div className="space-y-1">
							<label className="text-xs font-medium text-muted-foreground">
								Budget Range
							</label>
							<div className="flex items-center space-x-2">
								<div className="relative flex-1">
									<DollarSign className="h-4 w-4 absolute left-2 text-gray-400" />
									<input
										type="number"
										placeholder="Min"
										value={searchFilters.minBudget}
										onChange={(e) =>
											setSearchFilters({
												...searchFilters,
												minBudget: e.target.value,
											})
										}
										className="w-full pl-8 py-2 text-sm border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent transition-colors"
									/>
								</div>
								<span className="text-gray-500">-</span>
								<div className="relative flex-1">
									<DollarSign className="h-4 w-4 absolute left-2 text-gray-400" />
									<input
										type="number"
										placeholder="Max"
										value={searchFilters.maxBudget}
										onChange={(e) =>
											setSearchFilters({
												...searchFilters,
												maxBudget: e.target.value,
											})
										}
										className="w-full pl-8 py-2 text-sm border border-input rounded-md bg-background text-foreground focus:ring-2 focus:ring-ring focus:border-transparent transition-colors"
									/>
								</div>
							</div>
						</div>

						<div className="flex items-center space-x-2 col-span-full">
							<input
								id="remote-filter"
								type="checkbox"
								checked={searchFilters.isRemote}
								onChange={(e) =>
									setSearchFilters({
										...searchFilters,
										isRemote: e.target.checked,
									})
								}
								className="h-5 w-5 text-primary focus:ring-ring border-input rounded cursor-pointer"
							/>
							<label
								htmlFor="remote-filter"
								className="text-sm text-muted-foreground flex items-center cursor-pointer"
							>
								<MonitorSmartphone className="h-4 w-4 mr-1" />
								Remote only
							</label>
						</div>
					</div>
				)}

				{/* Suggestions */}
				{searchInput.trim().length > 0 && (
					<div className="p-3 border-b border-border">
						<div className="text-xs font-medium text-muted-foreground mb-2 flex items-center">
							<Sparkles className="h-3 w-3 mr-1" /> Suggested{" "}
							{searchType === "posts"
								? "Questions"
								: searchType === "users"
									? "Users"
									: "Jobs"}
						</div>
						<div className="space-y-1">
							{searchType === "jobs" ? (
								<>
									<Button
										variant="ghost"
										size="sm"
										className="w-full justify-start text-sm text-card-foreground hover:bg-accent"
										onClick={() => {
											setSearchInput(
												`${searchInput} developer`
											);
										}}
									>
										<Tag className="h-3 w-3 mr-2" />
										{searchInput} developer
									</Button>
									<Button
										variant="ghost"
										size="sm"
										className="w-full justify-start text-sm text-card-foreground hover:bg-accent"
										onClick={() => {
											setSearchInput(
												`${searchInput} remote`
											);
										}}
									>
										<Tag className="h-3 w-3 mr-2" />
										{searchInput} remote
									</Button>
								</>
							) : (
								<div className="text-sm text-muted-foreground italic">
									Press Enter to search for "{searchInput}"
								</div>
							)}
						</div>
					</div>
				)}

				{/* Recent Searches */}
				{recentSearches.length > 0 && !searchInput.trim() && (
					<div className="p-3 border-b border-border bg-secondary/50">
						<div className="text-xs font-medium text-muted-foreground mb-2 flex items-center">
							<History className="h-3 w-3 mr-1" /> Recent Searches
						</div>
						<div className="space-y-1">
							{recentSearches.map((search, index) => (
								<Button
									key={index}
									variant="ghost"
									size="sm"
									className="w-full justify-start text-sm text-card-foreground hover:bg-accent transition-colors"
									onClick={() => selectRecentSearch(search)}
								>
									<Clock className="h-3 w-3 mr-2" />
									{search.term}
									<span className="ml-2 text-xs text-muted-foreground">
										{search.type === "posts"
											? "Question"
											: search.type === "users"
												? "User"
												: "Job"}
									</span>
								</Button>
							))}
						</div>
					</div>
				)}

				{/* Quick Actions */}
				<div className="p-3 pt-4">
					<div className="text-xs font-medium text-muted-foreground mb-3">
						Quick Actions
					</div>
					<div className="grid grid-cols-1 md:grid-cols-2 gap-2">
						<Button
							variant="ghost"
							size="sm"
							className="justify-start text-sm text-card-foreground"
							onClick={() => navigate("/jobs")}
						>
							<Briefcase className="h-4 w-4 mr-2" />
							Browse All Jobs
						</Button>
						{isLoggedIn && (
							<Button
								variant="ghost"
								size="sm"
								className="justify-start text-sm text-card-foreground hover:bg-accent p-3 h-auto"
								onClick={() => navigate("/my-jobs")}
							>
								<Bookmark className="h-4 w-4 mr-2" />
								My Job Postings
							</Button>
						)}
					</div>
				</div>
			</div>
		</CommandDialog>
	);
};

export default SearchDialog;
