import { useEffect, useState } from "react";
import { onAuthStateChanged } from "firebase/auth";
import { auth } from "@/lib/firebase";
import { type User } from "firebase/auth";

export function useCurrentAuthUser() {
	const [user, setUser] = useState(() => auth.currentUser); // initial value might be null

	const [returnedUser, setReturnedUser] = useState<User | null | undefined>();

	useEffect(() => {
		setReturnedUser(user);
	}, [user]);

	useEffect(() => {
		const unsubscribe = onAuthStateChanged(auth, setUser);
		return () => unsubscribe();
	}, []);

	return returnedUser;
}
