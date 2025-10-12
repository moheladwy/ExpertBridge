import { Outlet } from "react-router";
import NavBar from "@/views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";
import { auth } from "./lib/firebase";
import { UpdateUserRequest } from "./features/users/types";
import { useUpdateUserMutation } from "./features/users/usersSlice";
import { useEffect, useState } from "react";
import { onAuthStateChanged } from "firebase/auth";
import { useLocation } from "react-router-dom";
import AuthPromptModal from "@/views/components/common/ui/AuthPromptModal";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import {
  AuthPromptProvider,
  useAuthPrompt,
} from "@/contexts/AuthPromptContext";
import { ThemeProvider } from "@/views/components/common/theme/ThemeProvider";
import ApiHealthGuard from "@/views/components/common/ui/ApiHealthGuard";

function AppContent() {
  const [updateUser] = useUpdateUserMutation();
  const authUser = useCurrentAuthUser();

  const [showInitialAuthPrompt, setShowInitialAuthPrompt] = useState(false);
  const location = useLocation();
  const isLandingPage = location.pathname === "/";

  const { isAuthPromptOpen, hideAuthPrompt } = useAuthPrompt();

  useEffect(() => {
    let timer: NodeJS.Timeout | undefined;
    if (!authUser && !isLandingPage) {
      console.log("setting timeout for auth prompt");
      timer = setTimeout(() => {
        console.log("timeout finished setting showInitialAuthPrompt true");
        if (!isAuthPromptOpen) {
          setShowInitialAuthPrompt(true);
        }
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
  }, [authUser, isLandingPage, isAuthPromptOpen]);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (user) => {
      if (!user) return;
      console.log(
        "invalidating profile cache!.........................................",
      );
      const token = await user.getIdToken();
      const name = user.displayName?.split(" ") || [];
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
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <div className="fixed top-0 left-0 right-0 z-50">
        <NavBar />
      </div>

      <div className="pt-16 dark:bg-gray-900 bg-gray-100 min-h-screen">
        <Toaster />
        <Outlet /> {/* Renders the current route's element */}
      </div>

      <AuthPromptModal
        open={showInitialAuthPrompt}
        onOpenChange={setShowInitialAuthPrompt}
        title="Tailor Your Experience"
        description="Create an account to unlock all features and get a personalized experience on ExpertBridge."
      />

      <AuthPromptModal
        open={isAuthPromptOpen}
        onOpenChange={(open) => !open && hideAuthPrompt()}
        title="Authentication Required"
        description="Please log in or sign up to continue with this action."
      />
    </ThemeProvider>
  );
}

function App() {
  return (
    <AuthPromptProvider>
      <ApiHealthGuard>
        <AppContent />
      </ApiHealthGuard>
    </AuthPromptProvider>
  );
}
export default App;
