import { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { userLoggedIn } from "@/features/auth/authSlice";
// import { setDarkTheme, setLightTheme } from "../features/theme/themeSlice";

const useAuthCheck = () => {
  const [authCheck, setAuthCheck] = useState(false);
  const dispatch = useDispatch();

  useEffect(() => {
    const userJson = localStorage.getItem('user');
    // const token = localStorage.getItem("token");
    // const theme = localStorage.getItem("theme") || 'light';
    if (userJson) {
      const user = JSON.parse(userJson);
      if (user) {
        dispatch(
          userLoggedIn({
            currentUser: user,
          })
        );
      }
    }

    // set theme
    // if (theme === "light") {
    //   dispatch(setLightTheme());
    // } else {
    //   dispatch(setDarkTheme());
    // }

    setAuthCheck(true);
  }, [dispatch]);

  return authCheck;
};

export default useAuthCheck;
