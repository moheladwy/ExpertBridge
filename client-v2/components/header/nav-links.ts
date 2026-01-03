/**
 * Navigation links for the header.
 * Organized by visibility context.
 */

export interface NavLink {
	label: string;
	href: string;
}

/**
 * Main navigation links shown to all users.
 */
export const mainNavLinks: NavLink[] = [
	{ label: "Home", href: "/" },
	{ label: "Jobs", href: "/jobs" },
];

/**
 * Navigation links shown to guest users.
 */
export const guestNavLinks: NavLink[] = [
	{ label: "About Us", href: "/about" },
	{ label: "Privacy Policy", href: "/privacy" },
];

/**
 * Additional navigation links shown only to authenticated users.
 */
export const authenticatedNavLinks: NavLink[] = [
	{ label: "Offers", href: "/offers" },
	{ label: "My Jobs", href: "/my-jobs" },
];

/**
 * Get navigation links based on authentication status.
 * @param isAuthenticated - Whether the user is logged in
 * @returns Combined array of navigation links
 */
export function getNavLinks(isAuthenticated: boolean): NavLink[] {
	if (isAuthenticated) {
		return [...mainNavLinks, ...authenticatedNavLinks, ...guestNavLinks];
	}
	return [...mainNavLinks, ...guestNavLinks];
}
