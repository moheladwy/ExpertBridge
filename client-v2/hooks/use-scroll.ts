"use client";
import { useCallback, useEffect, useState } from "react";

export function useScroll(threshold: number) {
  const [scrolled, setScrolled] = useState(false);

  const onScroll = useCallback(() => {
    const isScrolled = window.scrollY > threshold;
    setScrolled(isScrolled);
  }, [threshold]);

  useEffect(() => {
    window.addEventListener("scroll", onScroll);
    return () => { window.removeEventListener("scroll", onScroll); };
  }, [onScroll]);

  // also check on first load
  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    onScroll();
  }, [onScroll]);

  return scrolled;
}
