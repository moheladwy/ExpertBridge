import { useCallback, useRef } from "react"

export function useCallbackOnIntersection(onIntersectCallback: () => void) {
  const intersectionObserverRef = useRef<IntersectionObserver | null>(null);

  return useCallback(
    (node: HTMLDivElement | null) => {
      if (intersectionObserverRef.current) {
        intersectionObserverRef.current.disconnect();
      }

      intersectionObserverRef.current = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting) {
          onIntersectCallback();
        }
      })

      if (node) {
        intersectionObserverRef.current.observe(node);
      }
    },
    [onIntersectCallback],
  );
}
