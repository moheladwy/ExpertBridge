import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useSignOut from "@/lib/firebase/useSignOut";
import { useEffect } from "react";
import { Navigate, useNavigate } from "react-router-dom";

// ✅ Protected Route Component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const [signOut, loading] = useSignOut(auth);
  const navigate = useNavigate();

  const [
    isLoggedIn,
    loginLoading,
    loginError,
    authUser,
    appUser
  ] = useIsUserLoggedIn();

  // Handle login errors (e.g., token expired)
  useEffect(() => {
    if (loginError && !loginLoading) {
      console.log('ProtectedRoute: Error during auth', loginError);
      signOut();
      navigate('/login');
    }
  }, [loginError, loginLoading, signOut, navigate]);

  // Handle valid auth but unverified email or missing onboarding
  useEffect(() => {
    if (authUser && appUser) {
      if (!authUser.emailVerified) {
        signOut();
        console.log('ProtectedRoute: email not verified');
        navigate('/login');
      }

      const isOnboarded = true; // <- change if needed

      if (!isOnboarded) {
        console.log('ProtectedRoute: user needs onboarding');
        navigate('/onboarding');
      }
    }
  }, [authUser, appUser, signOut, navigate]);

  // ⛔️ Not logged in, redirect to login
  if (!loginLoading && !isLoggedIn) {
    return <Navigate to="/login" replace />;
  }

  // ⏳ Still verifying auth state
  if (loginLoading) {
    return <div>Loading...</div>;
  }

  // ✅ All good, show protected children
  return <>{children}</>;
};

export default ProtectedRoute;