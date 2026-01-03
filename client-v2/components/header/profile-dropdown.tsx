"use client";

import { useRouter } from "next/navigation";
import { User, Settings, LogOut } from "lucide-react";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuGroup,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useSignOut } from "@/hooks/useSignOut";
import type { ProfileResponse } from "@/features/profiles/types";

interface ProfileDropdownProps {
	profile: ProfileResponse | null | undefined;
}

/**
 * Dropdown menu for authenticated users.
 * Shows avatar/initials, profile link, and sign out option.
 */
export function ProfileDropdown({ profile }: ProfileDropdownProps) {
	const router = useRouter();
	const { signOut, loading } = useSignOut();

	const handleSignOut = async () => {
		await signOut();
		router.push("/");
	};

	const navigateTo = (path: string) => {
		router.push(path);
	};

	// Get initials for avatar fallback
	const getInitials = () => {
		if (profile?.firstName && profile?.lastName) {
			return `${profile.firstName[0]}${profile.lastName[0]}`.toUpperCase();
		}
		if (profile?.firstName) {
			return profile.firstName[0].toUpperCase();
		}
		if (profile?.email) {
			return profile.email[0].toUpperCase();
		}
		return "U";
	};

	// Get display name
	const displayName = profile?.firstName
		? `${profile.firstName}${
				profile.lastName ? ` ${profile.lastName}` : ""
		  }`
		: profile?.email || "User";

	return (
		<DropdownMenu>
			<DropdownMenuTrigger className="flex items-center gap-2 rounded-full p-1 outline-none ring-offset-background transition-colors hover:bg-accent focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2">
				{/* Avatar */}
				{profile?.profilePictureUrl ? (
					// eslint-disable-next-line @next/next/no-img-element
					<img
						src={profile.profilePictureUrl}
						alt={displayName}
						className="h-8 w-8 rounded-full object-cover"
					/>
				) : (
					<div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary text-xs font-medium text-primary-foreground">
						{getInitials()}
					</div>
				)}
			</DropdownMenuTrigger>

			<DropdownMenuContent align="end" className="w-56">
				<DropdownMenuGroup>
					<DropdownMenuLabel className="font-normal">
						<div className="flex flex-col space-y-1">
							<p className="text-sm font-medium leading-none">
								{displayName}
							</p>
							<p className="text-xs leading-none text-muted-foreground">
								{profile?.email}
							</p>
						</div>
					</DropdownMenuLabel>
				</DropdownMenuGroup>

				<DropdownMenuSeparator />

				<DropdownMenuItem
					onClick={() => navigateTo("/profile")}
					className="cursor-pointer"
				>
					<User className="mr-2 h-4 w-4" />
					<span>Profile</span>
				</DropdownMenuItem>

				<DropdownMenuItem
					onClick={() => navigateTo("/settings")}
					className="cursor-pointer"
				>
					<Settings className="mr-2 h-4 w-4" />
					<span>Settings</span>
				</DropdownMenuItem>

				<DropdownMenuSeparator />

				<DropdownMenuItem
					onClick={handleSignOut}
					disabled={loading}
					className="cursor-pointer text-destructive focus:text-destructive"
				>
					<LogOut className="mr-2 h-4 w-4" />
					<span>{loading ? "Signing out..." : "Sign Out"}</span>
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}
