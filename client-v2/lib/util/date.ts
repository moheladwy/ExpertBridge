/**
 * Date formatting utilities for the application.
 */

/**
 * Format a date string as relative time (e.g., "5m ago", "2h ago", "3d ago").
 * Falls back to formatted date for older dates.
 *
 * @param dateString - ISO date string to format
 * @returns Formatted relative time string
 *
 * @example
 * ```ts
 * formatTimeAgo("2026-01-04T10:00:00Z") // "5m ago"
 * formatTimeAgo("2026-01-01T10:00:00Z") // "3d ago"
 * formatTimeAgo("2025-06-01T10:00:00Z") // "Jun 1, 2025"
 * ```
 */
export function formatTimeAgo(dateString: string): string {
	const date = new Date(dateString);
	const now = new Date();
	const diffMs = now.getTime() - date.getTime();
	const diffMins = Math.floor(diffMs / 60000);
	const diffHours = Math.floor(diffMs / 3600000);
	const diffDays = Math.floor(diffMs / 86400000);

	if (diffMins < 1) return "Just now";
	if (diffMins < 60) return `${diffMins}m ago`;
	if (diffHours < 24) return `${diffHours}h ago`;
	if (diffDays < 7) return `${diffDays}d ago`;

	return date.toLocaleDateString("en-US", {
		month: "short",
		day: "numeric",
		year: date.getFullYear() !== now.getFullYear() ? "numeric" : undefined,
	});
}

/**
 * Format a date string as a short date (e.g., "Jan 2026").
 *
 * @param dateString - ISO date string to format
 * @returns Formatted date string or "Unknown" if not provided
 *
 * @example
 * ```ts
 * formatShortDate("2026-01-04T10:00:00Z") // "Jan 2026"
 * formatShortDate(undefined) // "Unknown"
 * ```
 */
export function formatShortDate(dateString?: string): string {
	if (!dateString) return "Unknown";
	return new Date(dateString).toLocaleDateString("en-US", {
		month: "short",
		year: "numeric",
	});
}
