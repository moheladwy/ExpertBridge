import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { Navigate } from "react-router";

// ✅ Public Route: Redirects authenticated users to /home
const PublicRoute = ({ children }: { children: React.ReactNode }) => {
	return children;
};

export default PublicRoute;
