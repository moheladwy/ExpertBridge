import { SearchUsersResponse } from "@/features/search/types";
import { Link } from "react-router-dom";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";

export default function SearchUserCard({
	id,
	firstName,
	lastName,
	username,
	jobTitle,
	bio,
	profilePictureUrl,
}: SearchUsersResponse) {
	const fullName =
		[firstName, lastName].filter(Boolean).join(" ") || "Anonymous User";

	return (
		<Link to={`/profile/${id}`} className="block group">
			<div className="flex p-4 rounded-xl border border-border hover:border-primary/50 hover:shadow-md bg-card transition-all duration-200">
				<div className="shrink-0">
					{profilePictureUrl ? (
						<img
							src={profilePictureUrl}
							width={48}
							height={48}
							alt={fullName}
							className="rounded-full object-cover ring-2 ring-border group-hover:ring-primary transition-all"
						/>
					) : (
						<img
							src={defaultProfile}
							width={48}
							height={48}
							alt="Default profile"
							className="rounded-full ring-2 ring-border group-hover:ring-primary transition-all"
						/>
					)}
				</div>

				<div className="ml-4 flex-1 overflow-hidden">
					{/* Name */}
					<h3 className="text-md font-semibold text-card-foreground group-hover:text-primary transition-colors">
						{fullName}
					</h3>

					{/* Username and Job Title row */}
					<div className="flex flex-wrap items-center gap-x-2 text-sm">
						{username && (
							<span className="text-muted-foreground">
								@{username}
							</span>
						)}
						{username && jobTitle && (
							<span className="text-muted-foreground">•</span>
						)}
						<span className="text-muted-foreground font-medium">
							{jobTitle ? jobTitle : "No Job Title"}
						</span>
					</div>
					<p className="mt-1 text-sm text-muted-foreground line-clamp-2 leading-relaxed">
						{bio ? bio : "No Bio Available"}
					</p>
				</div>
			</div>
		</Link>
	);
}
