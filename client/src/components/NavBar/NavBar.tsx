import { useNavigate } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import RegisterBtn from "./RegisterBtn";

const NavBar = () => {
  const navigate = useNavigate();
  const [user] = useAuthSubscribtion(auth);
  const [signOut, loading, error] = useSignOut(auth);

  const handleSignOut = async () => {
    try {
      await signOut();
      console.log("Signed Out");
      // navigate("/login"); // Redirect to login page after signing out
    } catch (error) {
      console.error("Error signing out:", error);
    }
  };

  return (
    <div className="flex items-center w-full bg-main-blue h-16 drop-shadow-md">
      <div className="flex items-center mx-9">
        <h1 className="text-white text-3xl max-sm:text-lg">
          <b>Expert</b>Bridge
        </h1>
        <a href="#" className="text-white font-light mx-5 max-sm:hidden">
          About Us
        </a>
      </div>

      <div className="flex ml-auto mr-9">
        {user ? (
          <>
            <button
              onClick={handleSignOut}
              disabled={loading}
              className="text-white border-2 rounded-full px-6 py-2 hover:text-main-blue hover:bg-white hover:font-bold max-sm:text-xs max-sm:px-3"
            >
              {loading ? "Signing Out..." : "Sign Out"}
            </button>
            {error && <p className="text-red-500 ml-4">{error.message}</p>}
          </>
        ) : (
          <>
            <button
              onClick={() => navigate("/login")}
              className="text-white border-2 rounded-full px-6 py-2 mr-4 hover:text-main-blue hover:bg-white hover:font-bold max-sm:text-xs max-sm:px-3"
            >
              Login
            </button>
            <RegisterBtn />
          </>
        )}
      </div>
    </div>
  );
}

export default NavBar;
