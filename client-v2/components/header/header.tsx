"use client";
import { useScroll } from "@/hooks/use-scroll";
import { Logo } from "@/components/shared/logo";
import { Button } from "@/components/ui/button";
import { ModeToggle } from "@/components/header/mode-toggle";
import { SearchCommand } from "@/components/header/search-command";

import { cn } from "@/lib/utils";
import { DesktopNav } from "@/components/header/desktop-nav";
import { MobileNav } from "@/components/header/mobile-nav";

export function Header() {
	const scrolled = useScroll(10);

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
			<nav className="mx-auto flex h-14 w-full max-w-5xl items-center justify-between gap-4 px-4">
				<div className="flex items-center gap-5">
					<a
						className="rounded-md px-3 py-2.5 hover:bg-accent"
						href="#"
					>
						<Logo className="h-4" />
					</a>
					<DesktopNav />
				</div>
				<div className="hidden flex-1 items-center justify-center px-4 md:flex md:max-w-md">
					<SearchCommand />
				</div>
				<div className="hidden items-center gap-2 md:flex">
					<ModeToggle />
					<Button variant="outline">Sign In</Button>
					<Button>Get Started</Button>
				</div>
				<MobileNav />
			</nav>
		</header>
	);
}
