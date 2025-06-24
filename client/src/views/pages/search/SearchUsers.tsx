import { useSearchParams } from "react-router-dom";
import { useSearchUsersQuery } from "@/features/search/searchSlice";
import { Skeleton } from "@mui/material";
import SearchUserCard from "@/views/components/custom/SearchUserCard";
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
			.map((_, index) => <Skeleton key={index} />);
	};

	// Render search results or appropriate message
	const renderContent = useCallback(() => {
		if (isLoading) {
			return renderSkeletons();
		}

		if (isError) {
			return (
				<div className="p-6 text-center bg-white dark:bg-gray-800 rounded-lg shadow">
					<h3 className="text-lg font-medium text-red-600">
						Error loading search results
					</h3>
					<p className="mt-2 text-gray-600 dark:text-gray-300">
						{error ? String(error) : "An unknown error occurred"}
					</p>
				</div>
			);
		}

		const users = searchResults ? Object.values(searchResults) : [];

		if (users.length === 0 && searchQuery) {
			return (
				<div className="p-6 text-center bg-white dark:bg-gray-800 rounded-lg shadow">
					<h3 className="text-lg font-medium">No results found</h3>
					<p className="mt-2 text-gray-600 dark:text-gray-300">
						We couldn't find any users matching: "{searchQuery}"
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
			/>
		));
	}, [error, isError, isLoading, searchQuery, searchResults]);

	return (
		<div className="container mx-auto px-4 py-8 max-w-5xl">
			<div className="p-6 bg-white dark:bg-gray-800 rounded-lg shadow">
				<h2 className="text-2xl font-semibold mb-4">
					Search Results for: {searchQuery}
				</h2>
				<div className="space-y-4">{renderContent()}</div>
			</div>
		</div>
	);
};

export default SearchUsers;
