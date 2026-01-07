"use client";
import { useScroll } from "@/hooks/use-scroll";
import { Logo } from "@/components/shared/logo";
import { ModeToggle } from "@/components/header/mode-toggle";
import { SearchCommand } from "@/components/header/search-command";
import { ProfileDropdown } from "@/components/header/profile-dropdown";

import { cn } from "@/lib/utils";
import { DesktopNav } from "@/components/header/desktop-nav";
import { MobileNav } from "@/components/header/mobile-nav";
import Link from "next/link";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import { Skeleton } from "@/components/ui/skeleton";
import { PenSquare } from "lucide-react";

/**
 * Skeleton component for header loading state.
 */
function HeaderSkeleton() {
	return (
		<div className="hidden items-center gap-2 md:flex">
			<Skeleton className="h-8 w-8 rounded-md" />
			<Skeleton className="h-8 w-16 rounded-lg" />
			<Skeleton className="h-8 w-20 rounded-lg" />
		</div>
	);
}

/**
 * Auth buttons for non-authenticated users.
 */
function AuthButtons() {
	return (
		<>
			<Link
				href="/auth/signin"
				className="rounded-lg border border-border bg-background px-2.5 h-8 inline-flex items-center justify-center text-sm font-medium hover:bg-muted hover:text-foreground transition-all"
			>
				Sign In
			</Link>
			<Link
				href="/auth/signup"
				className="rounded-lg bg-primary text-primary-foreground px-2.5 h-8 inline-flex items-center justify-center text-sm font-medium hover:bg-primary/80 transition-all"
			>
				Get Started
			</Link>
		</>
	);
}

export function Header() {
	const scrolled = useScroll(10);
	const { isAuthenticated, isLoading, profile } = useIsUserLoggedIn();

	return (
		<header
			className={cn(
				"sticky top-0 z-50 w-full border-transparent border-b",
				{
					"border-border bg-background/95 backdrop-blur-sm supports-backdrop-filter:bg-background/50":
						scrolled,
				}
			)}
		>
			<nav className="mx-auto flex h-14 w-full max-w-5xl items-center justify-between px-1">
				<div className="flex items-center gap-5">
					<Link
						className="rounded-md px-3 py-2.5 hover:bg-accent"
						href={isAuthenticated ? "/feed" : "/"}
					>
						<Logo className="h-4" />
					</Link>
					<DesktopNav isAuthenticated={isAuthenticated} />
				</div>
				<div className="hidden flex-1 items-center justify-center px-4 md:flex md:max-w-md">
					<SearchCommand />
				</div>
				<div className="hidden items-center gap-2 md:flex">
					<ModeToggle />
					{isLoading ? (
						<HeaderSkeleton />
					) : isAuthenticated ? (
						<>
							<Link
								href="/posts/create"
								className="inline-flex h-8 items-center justify-center gap-1.5 rounded-md border border-border bg-background px-3 text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground"
								aria-label="Create Post"
							>
								<PenSquare className="h-4 w-4" />
							</Link>
							<ProfileDropdown profile={profile} />
						</>
					) : (
						<AuthButtons />
					)}
				</div>
				<MobileNav
					isAuthenticated={isAuthenticated}
					profile={profile}
				/>
			</nav>
		</header>
	);
}
