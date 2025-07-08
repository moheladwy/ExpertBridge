import { Link, useNavigate, useLocation } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import RegisterBtn from "@/views/components/custom/RegisterBtn";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
import { Button } from "@/views/components/custom/button";
import {
  Bell,
  Search,
  User,
  Menu,
  X,
  Home,
  Briefcase,
  Info,
  Shield,
} from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetNotificationsQuery } from "@/features/notifications/notificationsSlice";
import { ModeToggle } from "@/views/components/common/theme/ToggleMode";
import LoginBtn from "@/views/components/custom/LoginBtn";
import SearchDialog from "@/views/components/common/search/SearchDialog";

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
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const handleSearch = () => {
    setOpen(true);
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
            <Link
              to="/jobs"
              className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
              ${location.pathname === "/jobs" ? "bg-blue-950" : ""}`}
            >
              <div className="text-white font-light">Jobs</div>
            </Link>

            {isLoggedIn ? (
              <>
                <Link
                  to="/offers"
                  className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
                  ${location.pathname === "/offers" ? "bg-blue-950" : ""}`}
                >
                  <div className="text-white font-light">Offers</div>
                </Link>
                <Link
                  to="/my-jobs"
                  className={`flex justify-center items-center py-5 px-3 hover:bg-blue-950 transition-colors
                  ${location.pathname === "/my-jobs" ? "bg-blue-950" : ""}`}
                >
                  <div className="text-white font-light">My Jobs</div>
                </Link>
              </>
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
        <div className="flex w-2/5 items-center justify-center text-center gap-2 sm:gap-3 lg:gap-5">
          {/* Search Button */}
          <Button
            className="bg-gray-100 dark:bg-gray-700 text-gray-500 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600 hover:text-main-blue transition-colors
                       hidden sm:flex px-4 lg:px-9 shadow-sm border border-gray-200 dark:border-gray-600"
            onClick={handleSearch}
          >
            <Search className="w-4 h-4" />
            <span className="ml-2 hidden md:inline">
              Search jobs, questions, or users...
            </span>
            <span className="ml-2 md:hidden">Search</span>
          </Button>

          {/* Mobile Search Button */}
          <Button
            className="sm:hidden bg-transparent hover:bg-blue-950 text-white p-2"
            onClick={handleSearch}
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
                        className="w-full text-white h-full rounded-full object-cover"
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
                <DropdownMenuContent
                  className="dark:bg-gray-800 w-48"
                  align="end"
                >
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
                      <Link to="/offers" className="flex items-center w-full">
                        <Briefcase className="w-4 h-4 mr-2" />
                        Offers
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Link to="/my-jobs" className="flex items-center w-full">
                        <Briefcase className="w-4 h-4 mr-2" />
                        My Jobs
                      </Link>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <Link
                        to="/notifications"
                        className="flex items-center w-full"
                      >
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
        <div
          className="lg:hidden fixed inset-0 z-50 bg-black bg-opacity-50"
          onClick={closeMobileMenu}
        >
          <div className="fixed right-0 top-0 h-full w-64 bg-white dark:bg-gray-800 shadow-lg transform transition-transform duration-300 ease-in-out">
            <div className="flex items-center justify-between p-4 border-b dark:border-gray-700">
              <h2 className="text-lg font-semibold text-gray-800 dark:text-white">
                Menu
              </h2>
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
                to="/jobs"
                className="flex items-center text-gray-800 dark:text-white hover:text-main-blue dark:hover:text-blue-400 transition-colors"
                onClick={closeMobileMenu}
              >
                <Briefcase className="w-5 h-5 mr-3" />
                Jobs
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
      <SearchDialog open={open} setOpen={setOpen} />
    </>
  );
};

export default NavBar;
