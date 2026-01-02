import { useMediaQuery } from "@/hooks/use-media-query";
import { Button } from "@/components/ui/button";
import { ModeToggle } from "@/components/header/mode-toggle";
import { cn } from "@/lib/utils";
import { MenuIcon, XIcon } from "lucide-react";
import React from "react";
import { createPortal } from "react-dom";
import {
	companyLinks,
	companyLinks2,
	productLinks,
} from "@/components/header/nav-links";
import { LinkItem } from "@/components/header/sheard";

export function MobileNav() {
	const [open, setOpen] = React.useState(false);
	const { isMobile } = useMediaQuery();

	// 🚫 Disable body scroll when open
	React.useEffect(() => {
		if (open && isMobile) {
			document.body.style.overflow = "hidden";
		} else {
			document.body.style.overflow = "";
		}
		// Cleanup on unmount too
		return () => {
			document.body.style.overflow = "";
		};
	}, [open, isMobile]);

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
							<div className="flex w-full flex-col gap-y-2">
								<span className="text-sm">Product</span>
								{productLinks.map((link) => (
									<LinkItem
										key={`product-${link.label}`}
										{...link}
									/>
								))}
								<span className="text-sm">Company</span>
								{companyLinks.map((link) => (
									<LinkItem
										key={`company-${link.label}`}
										{...link}
									/>
								))}
								{companyLinks2.map((link) => (
									<LinkItem
										key={`company-${link.label}`}
										{...link}
									/>
								))}
							</div>
							<div className="mt-5 flex flex-col gap-2">
								<ModeToggle />
								<Button className="w-full" variant="outline">
									Sign In
								</Button>
								<Button className="w-full">Get Started</Button>
							</div>
						</div>
					</div>,
					document.body
				)}
		</>
	);
}
