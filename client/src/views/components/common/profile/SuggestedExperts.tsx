import { useGetSuggestedExpertsQuery } from "@/features/profiles/profilesSlice";
import {
	Avatar,
	AvatarImage,
	AvatarFallback,
} from "@/views/components/ui/avatar";
import { Skeleton } from "@/views/components/ui/skeleton";
import { ProfileResponse } from "@/features/profiles/types";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { Link } from "react-router";

const TopReputationUsers = ({ limit = 5 }) => {
	const { data, isLoading, isError, refetch } =
		useGetSuggestedExpertsQuery(limit);

	useRefetchOnLogin(refetch);

	if (isLoading) {
		return (
			<div className="bg-card rounded-2xl shadow-lg border border-border overflow-hidden">
				<div className="p-6 border-b border-border">
					<div className="flex items-center gap-3">
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
			<div className="bg-card rounded-2xl shadow-lg border border-destructive/20 p-6">
				<div className="text-destructive text-center">
					<div className="mb-2">⚠️</div>
					<div className="text-sm">Unable to load top users</div>
				</div>
			</div>
		);
	}

	return (
		<div className="bg-card rounded-2xl border overflow-hidden transition-all duration-300 hover:shadow-lg">
			<div className="p-6 border-b border-border bg-muted/30">
				<div className="flex items-center justify-center">
					<div className="inline-block rounded-full bg-primary/10 px-3 py-1.5 text-sm font-medium text-primary mr-2">
						Suggested
					</div>
					<h3 className="font-semibold text-card-foreground text-center">
						Experts in Your Area
					</h3>
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
									className="group flex items-center gap-4 p-3 rounded-xl hover:bg-muted/50 transition-all duration-200 cursor-pointer relative overflow-hidden"
									style={{
										animationDelay: `${index * 100}ms`,
									}}
								>
									{index < 3 && (
										<div className="absolute inset-0 bg-linear-to-r from-transparent via-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
									)}

									<div className="relative flex items-center gap-3">
										<div className="relative">
											<Avatar className="w-12 h-12 ring-2 ring-border shadow-sm group-hover:ring-primary transition-all duration-200">
												<AvatarImage
													src={user.profilePictureUrl}
													alt={`${user.firstName} ${user.lastName}`}
												/>
												<AvatarFallback className="bg-muted">
													{user.firstName?.[0]}
													{user.lastName?.[0]}
												</AvatarFallback>
											</Avatar>
										</div>
									</div>

									<div className="flex-1 min-w-0 relative z-10">
										<div className="flex items-center gap-2">
											<h4 className="font-medium text-card-foreground truncate group-hover:text-primary transition-colors">
												{user.firstName} {user.lastName}
											</h4>
										</div>
										<p className="text-sm text-muted-foreground truncate">
											{user.jobTitle || "No title"}
										</p>
									</div>

									<div className="relative z-10 flex items-center gap-2">
										<div className="flex items-center gap-1 px-3 py-1 bg-primary/10 rounded-full">
											<span className="text-sm font-semibold text-primary">
												{user.reputation?.toLocaleString() ||
													0}
											</span>
										</div>
									</div>
								</div>
							</Link>
						))}
				</div>

				<div className="mt-6 pt-4 border-t border-border"></div>
			</div>
		</div>
	);
};

export default TopReputationUsers;
