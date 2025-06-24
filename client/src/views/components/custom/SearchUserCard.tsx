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
			<div className="flex p-4 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors">
				<div className="flex-shrink-0">
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
					<h3 className="text-md font-semibold text-gray-800 dark:text-gray-100">
						{fullName}
					</h3>

					{/* Username and Job Title row */}
					<div className="flex flex-wrap items-center gap-x-2 text-sm">
						{username && (
							<span className="text-gray-500 dark:text-gray-400">
								@{username}
							</span>
						)}

						<span className="text-gray-600 dark:text-gray-300 font-medium">
							{jobTitle ? jobTitle : "No Job Title"}
						</span>
					</div>
					<p className="mt-1 text-sm text-gray-600 dark:text-gray-300 line-clamp-2">
						{bio ? bio : "No Bio Avaliable"}
					</p>
				</div>
			</div>
		</Link>
	);
}
