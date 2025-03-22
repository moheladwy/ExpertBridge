import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useSignOut from "@/lib/firebase/useSignOut";
import { Navigate } from "react-router";

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

  const [
    isLoggedIn,
    loginLoading,
    loginError,
    authUser,
    appUser
  ] = useIsUserLoggedIn();

  if (loginLoading || loading) return <div>Loading...</div>;

  if (!authUser || !appUser || loginError) {
    // TODO: Handle the error here.
    if (loginError) {
      console.error('An error occurred while authenticating the user', loginError);
    }

    console.log('challenging the user');
    return <Navigate to="/login"/>;
  }

  if (!authUser.emailVerified) {
    signOut();
    console.log('challenging the user, email unverified');
    return <Navigate to="/login" />;
  }

  //appUser.isOnboarded
  const isOnboarded = true;

  if (!isOnboarded) {
    console.log('onboarding...');
    return (
      <Navigate to="/onboarding" />
    );
  }

  return children;
};

export default ProtectedRoute;