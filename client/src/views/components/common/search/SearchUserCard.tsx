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
		<Link to={`/profile/${id}`} className="block">
			<div className="flex p-4 rounded-lg hover:bg-secondary transition-colors">
				<div className="shrink-0">
					{profilePictureUrl ? (
						<img
							src={profilePictureUrl}
							width={48}
							height={48}
							alt={fullName}
							className="rounded-full object-cover"
						/>
					) : (
						<img
							src={defaultProfile}
							width={48}
							height={48}
							alt="Default profile"
							className="rounded-full"
						/>
					)}
				</div>

				<div className="ml-4 flex-1 overflow-hidden">
					{/* Name */}
					<h3 className="text-md font-semibold text-card-foreground">
						{fullName}
					</h3>

					{/* Username and Job Title row */}
					<div className="flex flex-wrap items-center gap-x-2 text-sm">
						{username && (
							<span className="text-muted-foreground">
								@{username}
							</span>
						)}
						-
						<span className="text-muted-foreground font-medium">
							{jobTitle ? jobTitle : "No Job Title"}
						</span>
					</div>
					<p className="mt-1 text-sm text-muted-foreground line-clamp-2">
						{bio ? bio : "No Bio Avaliable"}
					</p>
				</div>
			</div>
		</Link>
	);
}
