import { useAppDispatch } from "@/app/hooks";
import { userLoggedOut } from "@/features/auth/authSlice";
import { notificationsApiSlice } from "@/features/notifications/notificationsSlice";
import { profilesApiSlice } from "@/features/profiles/profilesSlice";
import { Auth, AuthError } from "firebase/auth";
import { useCallback, useState } from "react";

export type SignOutHook = [
	() => Promise<boolean>,
	boolean,
	AuthError | Error | undefined,
];

export default (auth: Auth): SignOutHook => {
	const [error, setError] = useState<AuthError>();
	const [loading, setLoading] = useState<boolean>(false);
	const dispatch = useAppDispatch();

	const signOut = useCallback(async () => {
		setLoading(true);
		setError(undefined);
		try {
			await auth.signOut();
			dispatch(userLoggedOut());
			dispatch(profilesApiSlice.util.resetApiState());
			dispatch(notificationsApiSlice.util.resetApiState());
			return true;
		} catch (err) {
			setError(err as AuthError);
			return false;
		} finally {
			setLoading(false);
		}
	}, [auth, dispatch]);

	return [signOut, loading, error];
};
