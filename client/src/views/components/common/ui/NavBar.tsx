import { Link, useNavigate, useLocation } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import RegisterBtn from "../../custom/RegisterBtn";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
import { Button } from "@/views/components/custom/button";
import { CommandDialog, CommandInput } from "@/views/components/custom/command";
import { DialogTitle, DialogDescription } from "@radix-ui/react-dialog";
import { Bell, FileQuestion, Search, User } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetNotificationsQuery } from "@/features/notifications/notificationsSlice";
import { ModeToggle } from "../theme/ToggleMode";
import LoginBtn from "../../custom/LoginBtn";

const NavBar = () => {
  useGetCurrentUserProfileQuery();

  //to get the active location
  const location = useLocation();
  const navigate = useNavigate();

  const [isLoggedIn, loginLoading, loginError, authUser, userProfile] =
    useIsUserLoggedIn();

  const { data: notifications } = useGetNotificationsQuery(
    userProfile?.id ?? "",
  );

  // TODO: Refactor to the same model as the one used by RTK Query standard.
  const hasNewNotifications =
    notifications?.filter((n) => n.isRead === false).length ?? 0 > 0;

  useEffect(() => {
    if (userProfile) {
      if (!userProfile.isOnboarded) {
        navigate("/onboarding");
      }
    }
  }, [userProfile, navigate]);

  const [signOut, loading, error] = useSignOut(auth);

  const handleSignOut = useCallback(async () => {
    await signOut();
    console.log("Signed Out");
    navigate("/login", { replace: true }); // Redirect to login page after signing out
  }, [signOut, navigate]);

  const [open, setOpen] = useState(false);
  const [searchInput, setSearchInput] = useState("");
  const [searchType, setSearchType] = useState<"posts" | "users">("posts");

  const handelSearch = () => {
    setOpen((open) => !open);
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
  };

  const handleSearch = () => {
    if (searchInput.trim()) {
      setOpen(false);
      const searchPath = searchType === "posts" ? "/search/p" : "/search/u";
      navigate(`${searchPath}?query=${encodeURIComponent(searchInput.trim())}`);
    }
  };

  const handleLogoClick = useCallback(() => {
    if (isLoggedIn) navigate("/home");
    else navigate("/");
  }, [isLoggedIn, navigate]);

  return (
    <div className="flex items-center w-full bg-main-blue h-16 drop-shadow-md">
      <div className="flex items-center mx-9">
        <h1
          className="text-white text-3xl cursor-pointer max-sm:text-lg"
          onClick={handleLogoClick}
        >
          <b>Expert</b>Bridge
        </h1>

        <Link
          to="/home"
          className={`flex justify-center items-center ml-5 py-5 px-3 max-sm:hidden hover:bg-blue-950
							${location.pathname === "/home" ? "bg-blue-950" : ""}`}
        >
          <div className="text-white font-light max-sm:hidden">Home</div>
        </Link>

        {isLoggedIn ? (
          <>
            {/* TO ADD THE REAL LINK LATER */}
            <Link
              to="/jobs"
              className="flex justify-center items-center py-5 px-3 max-sm:hidden hover:bg-blue-950"
            >
              <div className="text-white font-light max-sm:hidden">Jobs</div>
            </Link>
          </>
        ) : (
          <>
            {/* TO ADD THE REAL LINK LATER */}
            <Link
              to="/AboutUs"
              className="flex justify-center items-center py-5 px-3 max-sm:hidden hover:bg-blue-950"
            >
              <div className="text-white font-light max-sm:hidden">
                About Us
              </div>
            </Link>
          </>
        )}
      </div>

      <div className="flex ml-auto mr-9">
        <div className="flex justify-center items-center gap-5">
          {/* Search bar */}
          <Button
            className="bg-gray-100 dark:bg-gray-700 text-gray-500 dark:text-gray-300 px-9 hover:bg-gray-200 dark:hover:bg-gray-600 hover:text-main-blue max-md:p-2
                            max-md:bg-main-blue max-md:text-white max-md:hover:bg-main-blue max-md:hover:text-white"
            onClick={handelSearch}
          >
            <Search />{" "}
            <div className="max-md:hidden ml-2">
              Search about questions or users
            </div>
          </Button>

          {/* Search popup */}
          <CommandDialog open={open} onOpenChange={setOpen}>
            <DialogTitle className="sr-only">Search</DialogTitle>
            <DialogDescription className="sr-only">
              Search about questions / users
            </DialogDescription>

            <div className="flex h-full w-full items-baseline border-b px-1 dark:bg-gray-800">
              <DropdownMenu>
                <DropdownMenuTrigger asChild className="dark:bg-gray-700">
                  <Button
                    variant="outline"
                    size="sm"
                    className="ml-1 dark:bg-gray-700"
                  >
                    {searchType === "posts" ? (
                      <>
                        <FileQuestion className="h-4 w-4 mr-2" /> Questions
                      </>
                    ) : (
                      <>
                        <User className="h-4 w-4 mr-2" /> Users
                      </>
                    )}
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem
                    onClick={() => setSearchType("posts")}
                    className={
                      searchType === "posts"
                        ? "bg-blue-50 dark:bg-blue-900"
                        : ""
                    }
                  >
                    <FileQuestion className="h-4 w-4 mr-2" />
                    Search Questions
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    onClick={() => setSearchType("users")}
                    className={
                      searchType === "users"
                        ? "bg-blue-50 dark:bg-blue-900"
                        : ""
                    }
                  >
                    <User className="h-4 w-4 mr-2" />
                    Search Users
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
              <CommandInput
                placeholder={`Search ${searchType === "posts" ? "questions" : "users"}...`}
                onChangeCapture={handleChange}
                onKeyDown={(e) => {
                  if (e.key === "Enter" && searchInput.trim()) {
                    handleSearch();
                  }
                }}
              />
            </div>
          </CommandDialog>

          <ModeToggle />

          {isLoggedIn ? (
            <>
              <Link
                to="/notifications"
                className="text-white hover:text-blue-300 relative"
              >
                <Bell className="w-6 h-6" />
                {/* Optional: Red dot for unread indicator */}
                {hasNewNotifications ? (
                  <span className="absolute top-0 right-0 w-2 h-2 bg-red-500 rounded-full" />
                ) : null}
              </Link>

              {/* Profile Pic */}
              <DropdownMenu>
                <DropdownMenuTrigger>
                  {/* Profile Pic */}
                  <div className="flex justify-center items-center">
                    {userProfile?.profilePictureUrl ? (
                      <img
                        src={userProfile.profilePictureUrl}
                        width={45}
                        height={45}
                        className="rounded-full"
                      />
                    ) : (
                      <img
                        src={defaultProfile}
                        alt="Profile Picture"
                        width={45}
                        height={45}
                        className="rounded-full"
                      />
                    )}
                  </div>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="dark:bg-gray-800">
                  <DropdownMenuLabel>My Account</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem>
                    <Link to="/profile">Profile</Link>
                  </DropdownMenuItem>
                  <DropdownMenuItem
                    className="text-red-600 hover:cursor-pointer"
                    onClick={handleSignOut}
                  >
                    Sign Out
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </>
          ) : (
            <div className="flex justify-center items-center gap-0">
              <LoginBtn />
              <RegisterBtn />
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default NavBar;
