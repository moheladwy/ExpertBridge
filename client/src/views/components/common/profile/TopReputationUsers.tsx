import { useGetTopReputationProfilesQuery } from "@/features/profiles/profilesSlice";
import {
	Avatar,
	AvatarImage,
	AvatarFallback,
} from "@/views/components/ui/avatar";
import { Skeleton } from "@/views/components/ui/skeleton";
import { ProfileResponse } from "@/features/profiles/types";
import { Star, Trophy, Crown, Medal } from "lucide-react";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { Link } from "react-router";

const TopReputationUsers = ({ limit = 5 }) => {
	const { data, isLoading, isError, refetch } =
		useGetTopReputationProfilesQuery(limit);

	useRefetchOnLogin(refetch);

	const getRankIcon = (index: number) => {
		switch (index) {
			case 0:
				return <Crown className="w-5 h-5 text-yellow-500" />;
			case 1:
				return <Medal className="w-5 h-5 text-gray-400" />;
			case 2:
				return <Medal className="w-5 h-5 text-amber-600" />;
			default:
				return <Trophy className="w-4 h-4 text-blue-500" />;
		}
	};

	const getRankBadge = (index: number) => {
		const badges = [
			"bg-linear-to-r from-yellow-400 to-yellow-600 text-white",
			"bg-linear-to-r from-gray-300 to-gray-500 text-white",
			"bg-linear-to-r from-amber-400 to-amber-600 text-white",
			"bg-linear-to-r from-blue-400 to-blue-600 text-white",
			"bg-linear-to-r from-purple-400 to-purple-600 text-white",
		];
		return (
			badges[index] ||
			"bg-linear-to-r from-gray-400 to-gray-600 text-white"
		);
	};

	if (isLoading) {
		return (
			<div className="bg-card rounded-2xl shadow-lg border border-border overflow-hidden">
				<div className="p-6 border-b border-border">
					<div className="flex items-center gap-3">
						<div className="p-2 bg-yellow-100 rounded-lg">
							<Trophy className="w-5 h-5 text-yellow-600" />
						</div>
						<Skeleton className="h-6 w-[120px]" />
					</div>
				</div>
				<div className="p-6 space-y-4">
					{[...Array(limit)].map((_, i) => (
						<div key={i} className="flex items-center gap-3">
							<Skeleton className="h-12 w-12 rounded-full" />
							<div className="flex-1 space-y-2">
								<Skeleton className="h-5 w-[70%]" />
								<Skeleton className="h-4 w-[50%]" />
							</div>
							<Skeleton className="h-6 w-[60px] rounded-full" />
						</div>
					))}
				</div>
			</div>
		);
	}

	if (isError) {
		return (
			<div className="bg-card rounded-2xl shadow-lg border border-red-100 p-6">
				<div className="text-red-500 text-center">
					<div className="text-red-400 mb-2">⚠️</div>
					<div className="text-sm">Unable to load top users</div>
				</div>
			</div>
		);
	}

	return (
		<div className="bg-card rounded-2xl shadow-lg border border-border overflow-hidden transition-all duration-300 hover:shadow-xl">
			<div className="p-6 border-b border-border bg-linear-to-r from-yellow-50 to-amber-50">
				<div className="flex items-center justify-between">
					<div className="flex items-center gap-3">
						<div className="p-2 bg-yellow-100 rounded-lg">
							<Trophy className="w-5 h-5 text-yellow-600" />
						</div>
						<h3 className="font-semibold text-card-foreground">
							Top Reputation
						</h3>
					</div>
					<div className="text-xs text-yellow-600 font-medium px-2 py-1 bg-yellow-100 rounded-full">
						This Week
					</div>
				</div>
			</div>

			<div className="p-6">
				<div className="space-y-4">
					{data
						?.filter(
							(user: ProfileResponse) =>
								(user.reputation || 0) >= 0
						)
						?.sort(
							(a: ProfileResponse, b: ProfileResponse) =>
								(b.reputation || 0) - (a.reputation || 0)
						)
						?.map((user: ProfileResponse, index) => (
							<Link to={`/profile/${user.id}`}>
								<div
									key={user.id}
									className="group flex items-center gap-4 p-3 rounded-xl hover:bg-secondary transition-all duration-200 cursor-pointer relative overflow-hidden"
									style={{
										animationDelay: `${index * 100}ms`,
									}}
								>
									{index < 3 && (
										<div className="absolute inset-0 bg-linear-to-r from-transparent via-yellow-50/30 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
									)}

									<div className="relative flex items-center gap-3">
										<div
											className={`w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold ${getRankBadge(index)} shadow-md`}
										>
											{index + 1}
										</div>

										<div className="relative">
											<Avatar className="w-12 h-12 ring-2 ring-white shadow-md group-hover:ring-yellow-200 transition-all duration-200">
												<AvatarImage
													src={user.profilePictureUrl}
													alt={`${user.firstName} ${user.lastName}`}
												/>
												<AvatarFallback>
													{user.firstName?.[0]}
													{user.lastName?.[0]}
												</AvatarFallback>
											</Avatar>
											{index < 3 && (
												<div className="absolute -top-1 -right-1">
													{getRankIcon(index)}
												</div>
											)}
										</div>
									</div>

									<div className="flex-1 min-w-0 relative z-10">
										<div className="flex items-center gap-2">
											<h4 className="font-medium text-card-foreground truncate group-hover:text-yellow-600 transition-colors">
												{user.firstName} {user.lastName}
											</h4>
											{index === 0 && (
												<span className="text-xs bg-yellow-100 text-yellow-700 px-2 py-1 rounded-full font-medium">
													Champion
												</span>
											)}
										</div>
										<p className="text-sm text-muted-foreground truncate">
											{user.jobTitle || "No title"}
										</p>
									</div>

									<div className="relative z-10 flex items-center gap-2">
										<div className="flex items-center gap-1 px-3 py-1 bg-linear-to-r from-yellow-100 to-amber-100 rounded-full">
											<Star className="w-4 h-4 text-yellow-500" />
											<span className="text-sm font-semibold text-yellow-700">
												{user.reputation?.toLocaleString() ||
													0}
											</span>
										</div>
									</div>
								</div>
							</Link>
						))}
				</div>

				<div className="mt-6 pt-4 border-t border-border">
					<button className="w-full text-sm font-medium text-yellow-600 hover:text-yellow-700 py-2 rounded-lg hover:bg-yellow-50 transition-colors">
						View Full Leaderboard
					</button>
				</div>
			</div>
		</div>
	);
};

export default TopReputationUsers;
