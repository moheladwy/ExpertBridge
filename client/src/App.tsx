import { Outlet } from "react-router";
import NavBar from "./components/NavBar/NavBar";

function App() {

  return (
    <>
      <NavBar />
      <Outlet /> {/* Renders the current route's element */}
    </>
  );
}

export default App;
