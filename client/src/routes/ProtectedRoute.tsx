import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useSignOut from "@/lib/firebase/useSignOut";
import { useEffect } from "react";
import { Navigate, useNavigate } from "react-router-dom";

// ✅ Protected Route Component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  // const [authUser, authLoading, authError] = useAuthSubscribtion(auth);
  // const {
  //   data: appUser,
  //   isLoading: appUserLoading,
  //   error: appUserError,
  //   isError: appUserIsError
  // } = useGetCurrentUserProfileQuery();
  const [signOut, loading] = useSignOut(auth);
  const navigate = useNavigate();

  const [
    isLoggedIn,
    loginLoading,
    loginError,
    authUser,
    appUser
  ] = useIsUserLoggedIn();

  useEffect(() => {
    // TODO: Handle the error here.
    if (loginError) {
      console.log('An error occurred while authenticating the user', loginError);
      console.log('challenging the user');
      signOut();
    }
  }, [loginError, signOut]);

  useEffect(() => {
    if (authUser && appUser) {
      if (!authUser.emailVerified) {
        signOut();
        console.log('challenging the user, email unverified');
      }

      //appUser.isOnboarded
      const isOnboarded = true;

      if (!isOnboarded) {
        console.log('onboarding...');
        navigate('/onboarding');
      }
    }
  }, [authUser, appUser, signOut, navigate]);

  if (loginLoading || loading) return <div>Loading...</div>;

  return children;
};

export default ProtectedRoute;