"use client";

import { useTheme } from "next-themes";
import { HugeiconsIcon } from "@hugeicons/react";
import {
	Sun03Icon,
	Moon02Icon,
	ComputerIcon,
} from "@hugeicons/core-free-icons";

import { Button } from "@/components/ui/button";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

/**
 * Theme toggle component with dropdown menu.
 *
 * Allows users to switch between light, dark, and system themes.
 * Uses next-themes for theme management and persists preference.
 *
 * @example
 * ```tsx
 * // In navbar or header
 * <ModeToggle />
 * ```
 */
export function ModeToggle() {
	const { setTheme } = useTheme();

	return (
		<DropdownMenu>
			<DropdownMenuTrigger>
				<Button variant="outline" size="icon">
					<HugeiconsIcon
						icon={Sun03Icon}
						className="h-[1.2rem] w-[1.2rem] scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90"
					/>
					<HugeiconsIcon
						icon={Moon02Icon}
						className="absolute h-[1.2rem] w-[1.2rem] scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0"
					/>
					<span className="sr-only">Toggle theme</span>
				</Button>
			</DropdownMenuTrigger>
			<DropdownMenuContent align="end">
				<DropdownMenuItem onClick={() => setTheme("light")}>
					<HugeiconsIcon icon={Sun03Icon} className="mr-2 h-4 w-4" />
					<span>Light</span>
				</DropdownMenuItem>
				<DropdownMenuItem onClick={() => setTheme("dark")}>
					<HugeiconsIcon icon={Moon02Icon} className="mr-2 h-4 w-4" />
					<span>Dark</span>
				</DropdownMenuItem>
				<DropdownMenuItem onClick={() => setTheme("system")}>
					<HugeiconsIcon
						icon={ComputerIcon}
						className="mr-2 h-4 w-4"
					/>
					<span>System</span>
				</DropdownMenuItem>
			</DropdownMenuContent>
		</DropdownMenu>
	);
}
