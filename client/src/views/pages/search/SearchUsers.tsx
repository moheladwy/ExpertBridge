import { useSearchParams } from "react-router-dom";
import { useSearchUsersQuery } from "@/features/search/searchSlice";
import { Skeleton } from "@/views/components/ui/skeleton";
import SearchUserCard from "@/views/components/common/search/SearchUserCard";
import { useCallback } from "react";

const SearchUsers = () => {
	const [searchParams] = useSearchParams();
	const searchQuery = searchParams.get("query") || "";

	const {
		data: searchResults,
		isLoading,
		isError,
		error,
	} = useSearchUsersQuery(searchQuery, { skip: !searchQuery });

	// Render loading skeletons
	const renderSkeletons = () => {
		return Array(3)
			.fill(0)
			.map((_, index) => (
				<Skeleton key={index} className="h-32 w-full rounded-lg mb-4" />
			));
	};

	// Render search results or appropriate message
	const renderContent = useCallback(() => {
		if (isLoading) {
			return renderSkeletons();
		}

		if (isError) {
			return (
				<div className="p-8 text-center bg-card rounded-xl border border-destructive/20 shadow-lg">
					<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-destructive/10 mb-4">
						<svg
							className="w-8 h-8 text-destructive"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
							/>
						</svg>
					</div>
					<h3 className="text-xl font-bold text-card-foreground mb-2">
						Error loading search results
					</h3>
					<p className="text-muted-foreground">
						{error ? String(error) : "An unknown error occurred"}
					</p>
				</div>
			);
		}

		const users = searchResults ? Object.values(searchResults) : [];

		if (users.length === 0 && searchQuery) {
			return (
				<div className="p-8 text-center bg-card rounded-xl border border-border shadow-lg">
					<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted/50 mb-4">
						<svg
							className="w-8 h-8 text-muted-foreground"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
							/>
						</svg>
					</div>
					<h3 className="text-xl font-bold text-card-foreground mb-2">
						No results found
					</h3>
					<p className="text-muted-foreground">
						We couldn't find any users matching:{" "}
						<span className="font-semibold text-card-foreground">
							"{searchQuery}"
						</span>
					</p>
				</div>
			);
		}

		// Sort users by Rank in descending order (higher rank first)
		const sortedUsers = [...users].sort((a, b) => b.rank - a.rank);
		console.log(sortedUsers);

		return sortedUsers.map((user) => (
			<SearchUserCard
				key={user.id}
				id={user.id}
				email={user.email}
				rank={user.rank}
				firstName={user.firstName}
				lastName={user.lastName}
				profilePictureUrl={user.profilePictureUrl}
				jobTitle={user.jobTitle}
				bio={user.bio}
				username={user.username}
			/>
		));
	}, [error, isError, isLoading, searchQuery, searchResults]);

	return (
		<div className="container mx-auto px-4 py-8 max-w-5xl">
			<div className="p-6 bg-card rounded-xl border border-border shadow-lg">
				<div className="mb-6">
					<h2 className="text-2xl font-bold text-card-foreground mb-2">
						Search Results for:{" "}
						<span className="text-primary">{searchQuery}</span>
					</h2>
					{!isLoading && searchResults && (
						<span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary/10 text-primary">
							{Object.values(searchResults).length}{" "}
							{Object.values(searchResults).length === 1
								? "result"
								: "results"}
						</span>
					)}
				</div>
				<div className="space-y-3">{renderContent()}</div>
			</div>
		</div>
	);
};

export default SearchUsers;
