import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { userLoggedIn } from "@/features/auth/authSlice";

const useAuthCheck = () => {
	const [authCheck, setAuthCheck] = useState(false);
	const dispatch = useDispatch();

	useEffect(() => {
		const userJson = localStorage.getItem("user");
		if (userJson) {
			const user = JSON.parse(userJson);
			if (user) {
				dispatch(
					userLoggedIn({
						currentUser: user,
					})
				);
			}
		}

		setAuthCheck(true);
	}, [dispatch]);

	return authCheck;
};

export default useAuthCheck;
