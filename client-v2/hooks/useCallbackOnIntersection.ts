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

	// Cleanup observer on unmount to prevent memory leaks
	return useCallback((node: HTMLElement | null) => {
		if (intersectionObserverRef.current) {
			intersectionObserverRef.current.disconnect();
		}
		return () => {
			if (intersectionObserverRef.current) {
				intersectionObserverRef.current.disconnect();
				intersectionObserverRef.current = null;
			}
		};
	}, []);
}
