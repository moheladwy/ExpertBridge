import type React from "react";

/**
 * ExpertBridge logo SVG component.
 * Exported as both Logo and LogoIcon for backward compatibility.
 */
export const Logo = (props: React.ComponentProps<"svg">) => (
	<svg
		xmlns="http://www.w3.org/2000/svg"
		viewBox="0 0 1080 1080"
		fill="currentColor"
		{...props}
	>
		<title>ExpertBridge Logo</title>
		<path
			d="M956.8,432.5L1069.2,9.2L646.6,123.5L543.3,20.6L226.7,338.8l-103.5-103L10.8,659l422.6-114.2l103.2,102.8
	l-112.4,423.3L847,956.5l-103.5-103l316.6-318.1L956.8,432.5z M646.6,123.7L750,226.6l-316.5,318L330,441.7L646.6,123.7z
	 M536.7,647.6l316.6-318.1l103.4,103L640.1,750.5L536.7,647.6z"
		/>
	</svg>
);

// Alias for backward compatibility
export const LogoIcon = Logo;
