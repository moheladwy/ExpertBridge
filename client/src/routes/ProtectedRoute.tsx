import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useSignOut from "@/lib/firebase/useSignOut";
import { Navigate } from "react-router";

// ✅ Protected Route Component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const [authUser, authLoading, authError] = useAuthSubscribtion(auth);
  const {
    data: appUser,
    isLoading: appUserLoading,
    error: appUserError,
    isError: appUserIsError
  } = useGetCurrentUserProfileQuery();
  const [signOut, loading] = useSignOut(auth);

  if (authLoading || loading || appUserLoading) return <div>Loading...</div>;

  if (!authUser || !appUser || authError || appUserIsError) {
    // TODO: Handle the error here.
    if (authError || appUserError) {
      console.error('An error occurred while authenticating the user', authError || appUserError);
    }

    console.log('challenging the user');
    return <Navigate to="/login" replace />;
  }

  if (!authUser.emailVerified) {
    signOut();
    console.log('challenging the user, email unverified');
    return <Navigate to="/login" />;
  }

  // TODO: useGetUserQuery here to check if the user has finished his onboarding.
  const isOnboarded = true;

  if (!isOnboarded) {
    console.log('onboarding...');
    return (
      <Navigate to="/onboarding" replace />
    );
  }

  return children;
};

export default ProtectedRoute;