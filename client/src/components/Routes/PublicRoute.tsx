import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { Navigate } from "react-router";

// ✅ Public Route: Redirects authenticated users to /home
 const PublicRoute = ({ children }: { children: React.ReactNode }) => {
  const [user, loading] = useAuthSubscribtion(auth);

  if (loading) return <div>Loading...</div>;
  
  if (user) {
    console.log("User already logged in, redirecting...");
    return <Navigate to="/home" replace />;
  }
  return children;
};

export default PublicRoute;
