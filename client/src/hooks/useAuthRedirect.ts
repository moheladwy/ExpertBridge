import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

export default function useAuthRedirect(requireVerification = true) {
  const [isLoggedIn, loading, error, authUser, appUser] = useIsUserLoggedIn();
  const navigate = useNavigate();

  useEffect(() => {
    if (loading) return;

    // If not logged in, redirect to login
    if (!isLoggedIn) {
      navigate("/login");
      return;
    }

    if (requireVerification) {
      // Check Firebase auth email verification
      if (authUser && !authUser.emailVerified) {
        navigate("/verify-email");
        return;
      }
    }
  }, [isLoggedIn, loading, authUser, appUser, navigate, requireVerification]);

  return { isLoggedIn, loading, error, authUser, appUser };
}