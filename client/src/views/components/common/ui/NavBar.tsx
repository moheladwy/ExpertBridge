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
import { Bell, FileQuestion, Search, User, Menu, X, Home, Briefcase, Info, Shield } from "lucide-react";
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
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

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

  const closeMobileMenu = () => {
    setMobileMenuOpen(false);
  };

  const handleMobileNavigation = (path: string) => {
    navigate(path);
    closeMobileMenu();
  };

  return (
    <>
      <div className="flex items-center justify-between w-full bg-main-blue h-16 drop-shadow-md px-4 lg:px-9">
        {/* Left side - Logo and Desktop Navigation */}
        <div className="flex items-center">
          <h1
            className="text-white text-2xl lg:text-3xl cursor-pointer font-bold"
            onClick={handleLogoClick}
          >
            <span className="hidden sm:inline">Expert</span>
            <span className="sm:hidden">EB</span>
            <span className="hidden sm:inline">Bridge</span>
          </h1>

          {/* Desktop Navigation Links */}
          <nav className="hidden lg:flex ml-5">
            <Link
              to="/home"
              className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
                ${location.pathname === "/home" ? "bg-blue-950" : ""}`}
            >
              <div className="text-white font-light">Home</div>
            </Link>

            {isLoggedIn ? (
              <Link
                to="/jobs"
                className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
                  ${location.pathname === "/jobs" ? "bg-blue-950" : ""}`}
              >
                <div className="text-white font-light">Jobs</div>
              </Link>
            ) : (
              <Link
                to="/AboutUs"
                className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
                  ${location.pathname === "/AboutUs" ? "bg-blue-950" : ""}`}
              >
                <div className="text-white font-light">About Us</div>
              </Link>
            )}
          </nav>
        </div>

        {/* Right side - Actions */}
        <div className="flex items-center gap-2 sm:gap-3 lg:gap-5">
          {/* Search Button */}
          <Button
            className="bg-gray-100 dark:bg-gray-700 text-gray-500 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600 hover:text-main-blue transition-colors
                       hidden sm:flex px-4 lg:px-9"
            onClick={handelSearch}
          >
            <Search className="w-4 h-4" />
            <span className="ml-2 hidden md:inline">Search about questions or users</span>
            <span className="ml-2 md:hidden">Search</span>
          </Button>

          {/* Mobile Search Button */}
          <Button
            className="sm:hidden bg-transparent hover:bg-blue-950 text-white p-2"
            onClick={handelSearch}
          >
            <Search className="w-5 h-5" />
          </Button>

          {/* Mode Toggle */}
          <div className="hidden sm:block">
            <ModeToggle />
          </div>

          {isLoggedIn ? (
            <>
              {/* Notifications - Hidden on mobile */}
              <Link
                to="/notifications"
                className="text-white hover:text-blue-300 relative hidden sm:block"
              >
                <Bell className="w-6 h-6" />
                {hasNewNotifications && (
                  <span className="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full" />
                )}
              </Link>

              {/* Profile Dropdown */}
              <DropdownMenu>
                <DropdownMenuTrigger className="flex items-center">
                  <div className="w-8 h-8 lg:w-11 lg:h-11">
                    {userProfile?.profilePictureUrl ? (
                      <img
                        src={userProfile.profilePictureUrl}
                        className="w-full h-full rounded-full object-cover"
                        alt="Profile"
                      />
                    ) : (
                      <img
                        src={defaultProfile}
                        alt="Profile Picture"
                        className="w-full h-full rounded-full object-cover"
                      />
                    )}
                  </div>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="dark:bg-gray-800 w-48" align="end">
                  <DropdownMenuLabel>My Account</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  
                  {/* Mobile-only menu items */}
                  <div className="sm:hidden">
                    <DropdownMenuItem>
                      <Link to="/home" className="flex items-center w-full">
                        <Home className="w-4 h-4 mr-2" />
                        Home
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Link to="/jobs" className="flex items-center w-full">
                        <Briefcase className="w-4 h-4 mr-2" />
                        Jobs
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Link to="/notifications" className="flex items-center w-full">
                        <Bell className="w-4 h-4 mr-2" />
                        Notifications
                        {hasNewNotifications && (
                          <span className="ml-auto w-2 h-2 bg-red-500 rounded-full" />
                        )}
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                  </div>

                  <DropdownMenuItem>
                    <Link to="/profile" className="flex items-center w-full">
                      <User className="w-4 h-4 mr-2" />
                      Profile
                    </Link>
                  </DropdownMenuItem>
                  
                  {/* Mobile Mode Toggle */}
                  <div className="sm:hidden">
                    <DropdownMenuSeparator />
                    <DropdownMenuItem className="flex items-center justify-between">
                      <span>Theme</span>
                      <ModeToggle />
                    </DropdownMenuItem>
                  </div>
                  
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    className="text-red-600 hover:cursor-pointer focus:text-red-600"
                    onClick={handleSignOut}
                  >
                    Sign Out
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </>
          ) : (
            <>
              {/* Mobile hamburger menu for non-logged in users */}
              <div className="lg:hidden">
                <Button
                  variant="ghost"
                  size="sm"
                  className="text-white hover:bg-blue-950 p-2"
                  onClick={() => setMobileMenuOpen(true)}
                >
                  <Menu className="w-5 h-5" />
                </Button>
              </div>

              {/* Desktop login/register buttons */}
              <div className="hidden lg:flex items-center gap-2">
                <LoginBtn />
                <RegisterBtn />
              </div>
            </>
          )}
        </div>
      </div>

      {/* Mobile Menu Overlay for non-logged in users */}
      {!isLoggedIn && mobileMenuOpen && (
        <div className="lg:hidden fixed inset-0 z-50 bg-black bg-opacity-50" onClick={closeMobileMenu}>
          <div className="fixed right-0 top-0 h-full w-64 bg-white dark:bg-gray-800 shadow-lg transform transition-transform duration-300 ease-in-out">
            <div className="flex items-center justify-between p-4 border-b dark:border-gray-700">
              <h2 className="text-lg font-semibold text-gray-800 dark:text-white">Menu</h2>
              <Button
                variant="ghost"
                size="sm"
                onClick={closeMobileMenu}
                className="p-2"
              >
                <X className="w-5 h-5" />
              </Button>
            </div>
            
            <nav className="p-4 space-y-4">
              <Link
                to="/home"
                className="flex items-center text-gray-800 dark:text-white hover:text-main-blue dark:hover:text-blue-400 transition-colors"
                onClick={closeMobileMenu}
              >
                <Home className="w-5 h-5 mr-3" />
                Home
              </Link>
              <Link
                to="/AboutUs"
                className="flex items-center text-gray-800 dark:text-white hover:text-main-blue dark:hover:text-blue-400 transition-colors"
                onClick={closeMobileMenu}
              >
                <Info className="w-5 h-5 mr-3" />
                About Us
              </Link>
              <Link
                to="/privacy-policy"
                className="flex items-center text-gray-800 dark:text-white hover:text-main-blue dark:hover:text-blue-400 transition-colors"
                onClick={closeMobileMenu}
              >
                <Shield className="w-5 h-5 mr-3" />
                Privacy Policy
              </Link>
              
              <div className="pt-4 border-t dark:border-gray-700 space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-gray-800 dark:text-white">Theme</span>
                  <ModeToggle />
                </div>
                <div className="space-y-2">
                  <LoginBtn />
                  <RegisterBtn />
                </div>
              </div>
            </nav>
          </div>
        </div>
      )}

      {/* Search Dialog */}
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
    </>
  );
};

export default NavBar;
