import { Outlet } from "react-router";
import NavBar from "./views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";
import useAuthSubscribtion from "./lib/firebase/useAuthSubscribtion";
import { auth } from "./lib/firebase";
import { UpdateUserRequest } from "./features/users/types";
import { useUpdateUserMutation } from "./features/users/usersSlice";
import { useCallback, useEffect } from "react";
import { onAuthStateChanged, User } from "firebase/auth";
import { useAppDispatch, useAppSelector } from "./app/hooks";
import { saveAuthUser, selectAuthUser } from "./features/auth/authSlice";

function App() {
  const [updateUser] = useUpdateUserMutation();
  const [authUser, loading, error] = useAuthSubscribtion(auth);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (user) => {

      if (user?.uid === authUser?.uid)
        return;

      if (!user) return;

      console.log('invalidating profile cache!.........................................');

      const name = user.displayName?.split(' ') || [];
      const request: UpdateUserRequest = {
        firstName: name[0],
        lastName: name[1],
        email: user.email!,
        phoneNumber: user.phoneNumber,
        providerId: user.uid,
        profilePictureUrl: user.photoURL,
        isEmailVerified: user.emailVerified,
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
    </>
  );
}

export default App;
