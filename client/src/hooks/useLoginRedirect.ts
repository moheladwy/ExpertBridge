import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

export default function useLoginRedirect() {
  const [isLoggedIn, loading, error, authUser, appUser] = useIsUserLoggedIn();
  const navigate = useNavigate();

  useEffect(() => {
    if (loading) return;

    if (isLoggedIn) {
      // First check if email is verified
      if (authUser && !authUser.emailVerified) {
        navigate("/verify-email");
        return;
      }
      
      // If verified, go to feed
      navigate("/feed");
    }
  }, [isLoggedIn, loading, authUser, appUser, navigate]);

  return { isLoggedIn, loading, error };
}