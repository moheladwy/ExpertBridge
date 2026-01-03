import {
	NavigationMenu,
	NavigationMenuLink,
	NavigationMenuList,
} from "@/components/ui/navigation-menu";
import { getNavLinks } from "@/components/header/nav-links";

interface DesktopNavProps {
	isAuthenticated: boolean;
}

/**
 * Desktop navigation with links based on authentication state.
 */
export function DesktopNav({ isAuthenticated }: DesktopNavProps) {
	const navLinks = getNavLinks(isAuthenticated);

	return (
		<NavigationMenu className="hidden md:flex">
			<NavigationMenuList>
				{navLinks.map((link) => (
					<NavigationMenuLink
						key={link.href}
						className="px-4"
						href={link.href}
					>
						{link.label}
					</NavigationMenuLink>
				))}
			</NavigationMenuList>
		</NavigationMenu>
	);
}
