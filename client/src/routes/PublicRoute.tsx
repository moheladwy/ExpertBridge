import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { Navigate } from "react-router";

// ✅ Public Route: Redirects authenticated users to /home
const PublicRoute = ({ children }: { children: React.ReactNode }) => {
  const [authUser, authLodagin, authError] = useAuthSubscribtion(auth);
  const {
    data: appUser,
    isLoading: userLoading,
    isError: userError,
  } = useGetCurrentUserProfileQuery();

  // if (authLodagin || userLoading) return <div>Loading...</div>;

  // if (authUser && appUser) {
  //   console.log("User already logged in, redirecting...");
  //   return <Navigate to="/home" replace />;
  // }
  return children;
};

export default PublicRoute;
