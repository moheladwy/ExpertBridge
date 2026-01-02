"use client";

import * as React from "react";
import { HugeiconsIcon } from "@hugeicons/react";
import {
	SearchIcon,
	FileIcon,
	UserIcon,
	SettingsIcon,
	HelpCircleIcon,
} from "@hugeicons/core-free-icons";

import {
	Command,
	CommandDialog,
	CommandEmpty,
	CommandGroup,
	CommandInput,
	CommandItem,
	CommandList,
	CommandSeparator,
	CommandShortcut,
} from "@/components/ui/command";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

/**
 * Search command palette component.
 *
 * Opens a command palette dialog with search functionality.
 * Supports keyboard shortcut (Cmd/Ctrl + K) to toggle.
 *
 * @example
 * ```tsx
 * <SearchCommand />
 * ```
 */
export function SearchCommand() {
	const [open, setOpen] = React.useState(false);

	React.useEffect(() => {
		const down = (e: KeyboardEvent) => {
			if (e.key === "k" && (e.metaKey || e.ctrlKey)) {
				e.preventDefault();
				setOpen((open) => !open);
			}
		};

		document.addEventListener("keydown", down);
		return () => document.removeEventListener("keydown", down);
	}, []);

	return (
		<>
			<Button
				variant="outline"
				className={cn(
					"relative h-9 w-full justify-start rounded-lg text-sm font-normal text-muted-foreground shadow-none sm:pr-12 md:w-40 lg:w-64"
				)}
				onClick={() => setOpen(true)}
			>
				<HugeiconsIcon icon={SearchIcon} className="mr-2 h-4 w-4" />
				<span className="hidden lg:inline-flex">Search...</span>
				<span className="inline-flex lg:hidden">Search...</span>
				<kbd className="pointer-events-none absolute right-1.5 top-2 hidden h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium opacity-100 sm:flex">
					<span className="text-xs">⌘</span>K
				</kbd>
			</Button>
			<CommandDialog open={open} onOpenChange={setOpen}>
			<Command>
				<CommandInput placeholder="Type a command or search..." />
				<CommandList>
					<CommandEmpty>No results found.</CommandEmpty>
					<CommandGroup heading="Suggestions">
						<CommandItem>
							<HugeiconsIcon
								icon={FileIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Search Posts</span>
						</CommandItem>
						<CommandItem>
							<HugeiconsIcon
								icon={UserIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Find Experts</span>
						</CommandItem>
						<CommandItem>
							<HugeiconsIcon
								icon={FileIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Browse Jobs</span>
						</CommandItem>
					</CommandGroup>
					<CommandSeparator />
					<CommandGroup heading="Quick Actions">
						<CommandItem>
							<HugeiconsIcon
								icon={UserIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Profile</span>
							<CommandShortcut>⌘P</CommandShortcut>
						</CommandItem>
						<CommandItem>
							<HugeiconsIcon
								icon={SettingsIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Settings</span>
							<CommandShortcut>⌘S</CommandShortcut>
						</CommandItem>
						<CommandItem>
							<HugeiconsIcon
								icon={HelpCircleIcon}
								className="mr-2 h-4 w-4"
							/>
							<span>Help</span>
							<CommandShortcut>⌘H</CommandShortcut>
						</CommandItem>
					</CommandGroup>
				</CommandList>
			</Command>
		</CommandDialog>
		</>
	);
}
