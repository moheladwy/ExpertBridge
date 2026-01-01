/* eslint-disable react-hooks/refs */
"use client";

import type { AppStore } from "@/lib/redux/store";
import { makeStore } from "@/lib/redux/store";
import { setupListeners } from "@reduxjs/toolkit/query";
import type { ReactNode } from "react";
import { useEffect, useRef } from "react";
import { Provider } from "react-redux";

interface Props {
	readonly children: ReactNode;
}

const StoreProvider = ({ children }: Props) => {
	const storeRef = useRef<AppStore | null>(null);

	if (!storeRef.current) {
		// Create the store instance the first time this renders
		storeRef.current = makeStore();
	}

	useEffect(() => {
		if (storeRef.current != null) {
			// configure listeners using the provided defaults
			// optional, but required for `refetchOnFocus`/`refetchOnReconnect` behaviors
			const unsubscribe = setupListeners(storeRef.current.dispatch);
			return unsubscribe;
		}
	}, []);

	return <Provider store={storeRef.current}>{children}</Provider>;
};

export default StoreProvider;
