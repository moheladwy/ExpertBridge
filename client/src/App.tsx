import { Outlet } from "react-router";
import NavBar from "./views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";
import useAuthSubscribtion from "./lib/firebase/useAuthSubscribtion";
import { auth } from "./lib/firebase";

function App() {
  const _ = useAuthSubscribtion(auth);

  return (
    <>
      <NavBar />
      <Toaster />
      <Outlet /> {/* Renders the current route's element */}
    </>
  );
}

export default App;
