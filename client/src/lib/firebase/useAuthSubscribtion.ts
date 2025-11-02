import { Auth, User } from "firebase/auth";
import { useEffect, useState } from "react";
import { LoadingHook, useLoadingValue } from "@/lib/util";

export type AuthStateHook = LoadingHook<User | null, Error>;

type AuthStateOptions = {
	onUserChanged?: (user: User | null) => Promise<void>;
};

export default (auth: Auth, options?: AuthStateOptions): AuthStateHook => {
	const { error, loading, value } = useLoadingValue<User | null, Error>(
		() => auth.currentUser
	);

	const [globalLoading, setGlobalLoading] = useState(false);

	useEffect(() => {
		setGlobalLoading(loading);
	}, [loading]);

	useEffect(() => {
		if (value || error) {
			setGlobalLoading(false);
		}
	}, [value, error]);

	return [value, globalLoading, error];
};
