import { Outlet } from "react-router";
import NavBar from "./views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";
import useAuthSubscribtion from "./lib/firebase/useAuthSubscribtion";
import { auth } from "./lib/firebase";
import { UpdateUserRequest } from "./features/users/types";
import { useUpdateUserMutation } from "./features/users/usersSlice";
import { useCallback, useEffect, useRef, useState } from "react";
import { onAuthStateChanged, User } from "firebase/auth";
import { useAppDispatch, useAppSelector } from "./app/hooks";
import { saveAuthUser, selectAuthUser } from "./features/auth/authSlice";

import { useLocation } from "react-router-dom";
import AuthPromptModal from "./views/components/common/ui/AuthPromptModal";

import { useCurrentAuthUser } from "./hooks/useCurrentAuthUser";

function App() {

  const [updateUser] = useUpdateUserMutation();
  const authUser = useCurrentAuthUser();

  const [showInitialAuthPrompt, setShowInitialAuthPrompt] = useState(false);
  const location = useLocation();
  const isLandingPage = location.pathname === "/";

  useEffect(() => {
    let timer: NodeJS.Timeout | undefined;

    // console.log({
    //   authUser,
    //   loading,
    //   isLandingPage,
    //   showInitialAuthPrompt,
    //   pathname: location.pathname
    // });


    if (!authUser && !isLandingPage) {
      console.log("setting timeout for auth prompt");
      timer = setTimeout(() => {
        console.log("timeout finished setting showInitialAuthPrompt true");
        setShowInitialAuthPrompt(true);
      }, 10000);

    } else {
      setShowInitialAuthPrompt(false);
    }


    return () => {
      if (timer) {
        console.log("clearing timeout for auth prompt");
        clearTimeout(timer);
      }
    };

  }, [authUser, isLandingPage]);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (user) => {

      if (!user) return;

      // if (!user.emailVerified) {
      //   await auth.signOut();
      //   return;
      // }

      console.log('invalidating profile cache!.........................................');
      const token = await user.getIdToken();
      const name = user.displayName?.split(' ') || [];
      const request: UpdateUserRequest = {
        firstName: name[0],
        lastName: name[1],
        email: user.email!,
        phoneNumber: user.phoneNumber,
        providerId: user.uid,
        profilePictureUrl: user.photoURL,
        isEmailVerified: user.emailVerified,
        token,
      };

      await updateUser(request);
    });

    return () => unsubscribe();
  }, []);

  // useAuthSubscribtion(auth, {
  //   onUserChanged: useCallback(async (user: User | null) => {

  //     if (!user) return;

  //     const name = user.displayName?.split(' ') || [];
  //     const request: UpdateUserRequest = {
  //       firstName: name[0],
  //       lastName: name[1],
  //       email: user.email!,
  //       phoneNumber: user.phoneNumber,
  //       providerId: user.uid,
  //       profilePictureUrl: user.photoURL,
  //     };

  //     await updateUser(request);
  //   }, [updateUser])
  // });

  return (
    <>
      <div className="fixed top-0 left-0 right-0 z-50">
        <NavBar />
      </div>

      <div className="pt-16">
        <Toaster />
        <Outlet /> {/* Renders the current route's element */}
      </div>

      <AuthPromptModal
        open={showInitialAuthPrompt}
        onOpenChange={setShowInitialAuthPrompt}
        title="Tailor Your Experience"
        description="Create an account to unlock all features and get a personalized experience on ExpertBridge."
      />

      {/* <AuthPromptModal
        open={true}
        onOpenChange={() => {true}}
        title="Tailor Your Experience"
        description="Create an account to unlock all features and get a personalized experience on ExpertBridge."
      /> */}

    </>
  );
}

export default App;
