"use client";

import { useCallback, useRef } from "react";

/**
 * Hook that returns a ref callback for triggering a function when an element
 * enters the viewport. Useful for infinite scroll implementations.
 *
 * @param onIntersectCallback - Function to call when the observed element becomes visible
 * @returns A ref callback to attach to the target element
 *
 * @example
 * ```tsx
 * const loadMoreRef = useCallbackOnIntersection(() => fetchNextPage());
 * return <div ref={loadMoreRef}>Loading more...</div>;
 * ```
 */
export function useCallbackOnIntersection(onIntersectCallback: () => void) {
	const intersectionObserverRef = useRef<IntersectionObserver | null>(null);

	return useCallback(
		(node: HTMLElement | null) => {
			// Disconnect previous observer if it exists
			if (intersectionObserverRef.current) {
				intersectionObserverRef.current.disconnect();
			}

			// Create new observer
			intersectionObserverRef.current = new IntersectionObserver(
				(entries) => {
					if (entries[0].isIntersecting) {
						onIntersectCallback();
					}
				}
			);

			// Start observing the node if it exists
			if (node) {
				intersectionObserverRef.current.observe(node);
			}
		},
		[onIntersectCallback]
	);
}
