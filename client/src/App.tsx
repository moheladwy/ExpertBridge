import { Outlet } from "react-router";
import NavBar from "./views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";

function App() {

  return (
    <>
      <NavBar />
      <Toaster />
      <Outlet /> {/* Renders the current route's element */}
    </>
  );
}

export default App;
