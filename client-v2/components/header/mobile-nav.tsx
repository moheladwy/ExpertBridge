"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useMediaQuery } from "@/hooks/use-media-query";
import { Button } from "@/components/ui/button";
import { ModeToggle } from "@/components/header/mode-toggle";
import { cn } from "@/lib/utils";
import { MenuIcon, XIcon, User, LogOut, PenSquare } from "lucide-react";
import React from "react";
import { createPortal } from "react-dom";
import { getNavLinks } from "@/components/header/nav-links";
import { useSignOut } from "@/hooks/useSignOut";
import type { ProfileResponse } from "@/features/profiles/types";

interface MobileNavProps {
	isAuthenticated: boolean;
	profile: ProfileResponse | null | undefined;
}

/**
 * Mobile navigation with hamburger menu.
 * Shows different links and actions based on authentication state.
 */
export function MobileNav({ isAuthenticated, profile }: MobileNavProps) {
	const [open, setOpen] = React.useState(false);
	const { isMobile } = useMediaQuery();
	const router = useRouter();
	const { signOut, loading: signOutLoading } = useSignOut();

	const navLinks = getNavLinks(isAuthenticated);

	// Disable body scroll when open
	React.useEffect(() => {
		if (open && isMobile) {
			document.body.style.overflow = "hidden";
		} else {
			document.body.style.overflow = "";
		}
		return () => {
			document.body.style.overflow = "";
		};
	}, [open, isMobile]);

	const handleSignOut = async () => {
		setOpen(false);
		await signOut();
		router.push("/");
	};

	// Get display name
	const displayName = profile?.firstName
		? `${profile.firstName}${
				profile.lastName ? ` ${profile.lastName}` : ""
		  }`
		: profile?.email || "User";

	return (
		<>
			<Button
				aria-controls="mobile-menu"
				aria-expanded={open}
				aria-label="Toggle menu"
				className="md:hidden"
				onClick={() => setOpen(!open)}
				size="icon"
				variant="outline"
			>
				<div
					className={cn(
						"transition-all",
						open ? "scale-100 opacity-100" : "scale-0 opacity-0"
					)}
				>
					<XIcon aria-hidden="true" className="size-4.5" />
				</div>
				<div
					className={cn(
						"absolute transition-all",
						open ? "scale-0 opacity-0" : "scale-100 opacity-100"
					)}
				>
					<MenuIcon aria-hidden="true" className="size-4.5" />
				</div>
			</Button>
			{open &&
				createPortal(
					<div
						className={cn(
							"bg-background/95 backdrop-blur-sm supports-backdrop-filter:bg-background/50",
							"fixed top-14 right-0 bottom-0 left-0 z-40 flex flex-col overflow-hidden border-t md:hidden"
						)}
						id="mobile-menu"
					>
						<div
							className={cn(
								"data-[slot=open]:zoom-in-97 ease-out data-[slot=open]:animate-in",
								"size-full overflow-y-auto overflow-x-hidden p-4"
							)}
							data-slot={open ? "open" : "closed"}
						>
							{/* User Info (if authenticated) */}
							{isAuthenticated && profile && (
								<div className="mb-4 flex items-center gap-3 rounded-lg border bg-muted/50 p-3">
									{profile.profilePictureUrl ? (
										// eslint-disable-next-line @next/next/no-img-element
										<img
											src={profile.profilePictureUrl}
											alt={displayName}
											className="h-10 w-10 rounded-full object-cover"
										/>
									) : (
										<div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary text-sm font-medium text-primary-foreground">
											{profile.firstName?.[0]?.toUpperCase() ||
												"U"}
										</div>
									)}
									<div>
										<p className="font-medium">
											{displayName}
										</p>
										<p className="text-xs text-muted-foreground">
											{profile.email}
										</p>
									</div>
								</div>
							)}

							{/* Create Post Button (if authenticated) */}
							{isAuthenticated && (
								<Link
									href="/posts/create"
									className="mb-3 flex w-full items-center justify-center gap-2 rounded-lg bg-primary px-3 py-2.5 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
									onClick={() => setOpen(false)}
								>
									<PenSquare className="h-4 w-4" />
									Create Post
								</Link>
							)}

							{/* Navigation Links */}
							<div className="flex w-full flex-col gap-y-1">
								{navLinks.map((link) => (
									<Link
										key={link.href}
										href={link.href}
										className="rounded-md px-3 py-2 text-sm font-medium hover:bg-accent"
										onClick={() => setOpen(false)}
									>
										{link.label}
									</Link>
								))}

								{/* Profile link for authenticated users */}
								{isAuthenticated && (
									<Link
										href="/profile"
										className="flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium hover:bg-accent"
										onClick={() => setOpen(false)}
									>
										<User className="h-4 w-4" />
										Profile
									</Link>
								)}
							</div>

							{/* Bottom Actions */}
							<div className="mt-5 flex flex-col gap-2">
								<ModeToggle />

								{isAuthenticated ? (
									<Button
										onClick={handleSignOut}
										disabled={signOutLoading}
										variant="destructive"
										className="w-full"
									>
										<LogOut className="mr-2 h-4 w-4" />
										{signOutLoading
											? "Signing out..."
											: "Sign Out"}
									</Button>
								) : (
									<>
										<Link
											href="/auth/signin"
											className="w-full rounded-lg border border-border bg-background px-2.5 h-8 inline-flex items-center justify-center text-sm font-medium hover:bg-muted hover:text-foreground transition-all"
											onClick={() => setOpen(false)}
										>
											Sign In
										</Link>
										<Link
											href="/auth/signup"
											className="w-full rounded-lg bg-primary text-primary-foreground px-2.5 h-8 inline-flex items-center justify-center text-sm font-medium hover:bg-primary/80 transition-all"
											onClick={() => setOpen(false)}
										>
											Get Started
										</Link>
									</>
								)}
							</div>
						</div>
					</div>,
					document.body
				)}
		</>
	);
}
