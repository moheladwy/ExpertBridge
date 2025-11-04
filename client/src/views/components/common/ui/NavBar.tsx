import { Link, useNavigate, useLocation } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
} from "@/views/components/ui/dropdown-menu";
import { Button } from "@/views/components/ui/button";
import { Bell, Search, User, Shield } from "lucide-react";
import { useCallback, useEffect, useState, useMemo } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetNotificationsQuery } from "@/features/notifications/notificationsSlice";
import { ModeToggle } from "@/components/mode-toggle";
import SearchDialog from "@/views/components/common/search/SearchDialog";
import { cn } from "@/lib/util/utils";
import {
	Navbar,
	NavBody,
	MobileNav,
	MobileNavHeader,
	MobileNavToggle,
	MobileNavMenu,
} from "@/components/ui/resizable-navbar";
import CustomNavbarLogo from "./CustomNavbarLogo";
import CustomNavItems from "./CustomNavItems";

const NavBar = () => {
	useGetCurrentUserProfileQuery();

	//to get the active location
	const location = useLocation();
	const navigate = useNavigate();

	const [isLoggedIn, _loginLoading, _loginError, _authUser, userProfile] =
		useIsUserLoggedIn();

	const { data: notifications } = useGetNotificationsQuery(
		userProfile?.id ?? ""
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

	const [signOut, _loading, _error] = useSignOut(auth);

	const handleSignOut = useCallback(async () => {
		await signOut();
		console.log("Signed Out");
		navigate("/login", { replace: true }); // Redirect to login page after signing out
	}, [signOut, navigate]);

	const [open, setOpen] = useState(false);
	const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
	const [isScrolled, setIsScrolled] = useState(false);

	useEffect(() => {
		const handleScroll = () => {
			setIsScrolled(window.scrollY > 0);
		};

		window.addEventListener("scroll", handleScroll);
		return () => window.removeEventListener("scroll", handleScroll);
	}, []);

	const handleSearch = () => {
		setOpen(true);
	};

	const handleLogoClick = useCallback(() => {
		// if (isLoggedIn) navigate("/home");
		navigate("/");
	}, [navigate]);

	// Dynamic navigation items based on auth state
	const navItems = useMemo(() => {
		const baseItems = [
			{ name: "Home", link: "/home" },
			{ name: "Jobs", link: "/jobs" },
		];

		if (isLoggedIn) {
			return [
				...baseItems,
				{ name: "Offers", link: "/offers" },
				{ name: "My Jobs", link: "/my-jobs" },
			];
		} else {
			return [...baseItems, { name: "About Us", link: "/AboutUs" }];
		}
	}, [isLoggedIn]);

	return (
		<>
			<div className="sticky w-full flex flex-col gap-6">
				<Navbar className="w-full">
					{/* Desktop Navigation */}
					<NavBody className="navbar-container">
						{/* Logo */}
						<CustomNavbarLogo onClick={handleLogoClick} />

						{/* Search Bar - Center (Desktop Full Size) */}
						<div className="flex-1 max-w-2xl mx-4 hidden md:block">
							<Button
								className="navbar-search"
								onClick={handleSearch}
							>
								<Search className="w-4 h-4 mr-2" />
								<span>Search jobs, questions, or users...</span>
							</Button>
						</div>

						{/* Right Side Actions */}
						<div className="flex items-center gap-2 sm:gap-3 lg:gap-4">
							{/* Search Icon - Mobile Only */}
							<Button
								className="navbar-search-icon md:hidden"
								onClick={handleSearch}
							>
								<Search className="w-5 h-5" />
							</Button>

							{/* Mode Toggle */}
							<span className="hidden sm:block">
								<ModeToggle />
							</span>

							{/* Conditional: Logged In */}
							{isLoggedIn && (
								<>
									{/* Notifications */}
									<Link
										to="/notifications"
										className="navbar-notification hidden sm:block"
									>
										<Bell className="w-6 h-6" />
										{hasNewNotifications && (
											<span className="navbar-notification-badge" />
										)}
									</Link>

									{/* Profile Dropdown */}
									<DropdownMenu>
										<DropdownMenuTrigger className="flex items-center">
											<div className="w-8 h-8 lg:w-10 lg:h-10">
												{userProfile?.profilePictureUrl ? (
													<img
														src={
															userProfile.profilePictureUrl
														}
														className="w-full h-full rounded-full object-cover"
														alt="Profile"
													/>
												) : (
													<img
														src={defaultProfile}
														className="w-full h-full rounded-full object-cover"
														alt="Profile"
													/>
												)}
											</div>
										</DropdownMenuTrigger>
										<DropdownMenuContent
											className="bg-card w-48"
											align="end"
										>
											<DropdownMenuLabel>
												My Account
											</DropdownMenuLabel>
											<DropdownMenuSeparator />
											<DropdownMenuItem>
												<Link
													to="/profile"
													className="flex items-center w-full"
												>
													<User className="w-4 h-4 mr-2" />
													Profile
												</Link>
											</DropdownMenuItem>
											<DropdownMenuSeparator />
											<DropdownMenuItem
												className="text-destructive hover:cursor-pointer focus:text-destructive"
												onClick={handleSignOut}
											>
												Sign Out
											</DropdownMenuItem>
										</DropdownMenuContent>
									</DropdownMenu>
								</>
							)}

							{/* Conditional: NOT Logged In */}
							{!isLoggedIn && (
								<div className="hidden lg:flex items-center gap-2">
									<Button
										variant="outline"
										className="navbar-login-btn"
										onClick={() => navigate("/login")}
									>
										Login
									</Button>
									<Button
										className="navbar-register-btn"
										onClick={() => navigate("/signup")}
									>
										Register
									</Button>
								</div>
							)}
						</div>
					</NavBody>

					{/* Mobile Navigation */}
					<MobileNav className="navbar-container">
						<MobileNavHeader>
							{/* Logo */}
							<CustomNavbarLogo onClick={handleLogoClick} />

							{/* Right Side */}
							<div className="flex items-center gap-2">
								{/* Mobile Menu Toggle */}
								<MobileNavToggle
									isOpen={mobileMenuOpen}
									onClick={() =>
										setMobileMenuOpen(!mobileMenuOpen)
									}
								/>
							</div>
						</MobileNavHeader>

						{/* Mobile Menu */}
						<MobileNavMenu
							isOpen={mobileMenuOpen}
							onClose={() => setMobileMenuOpen(false)}
						>
							{/* Navigation Links */}
							{navItems.map((item, idx) => (
								<Link
									key={`mobile-link-${idx}`}
									to={item.link}
									onClick={() => setMobileMenuOpen(false)}
									className={cn(
										"navbar-mobile-link",
										location.pathname === item.link &&
											"navbar-mobile-link-active"
									)}
								>
									<span className="block">{item.name}</span>
								</Link>
							))}

							{/* Conditional: Logged In */}
							{isLoggedIn && (
								<div className="flex w-full flex-col gap-4 pt-4 border-t border-border">
									{/* Profile Link */}
									<Link
										to="/profile"
										onClick={() => setMobileMenuOpen(false)}
										className="navbar-mobile-item"
									>
										{userProfile?.profilePictureUrl ? (
											<img
												src={
													userProfile.profilePictureUrl
												}
												className="w-8 h-8 rounded-full object-cover"
												alt="Profile"
											/>
										) : (
											<img
												src={defaultProfile}
												className="w-8 h-8 rounded-full object-cover"
												alt="Profile"
											/>
										)}
										<span>My Profile</span>
									</Link>

									{/* Notifications Link */}
									<Link
										to="/notifications"
										onClick={() => setMobileMenuOpen(false)}
										className="navbar-mobile-item"
									>
										<Bell className="w-5 h-5" />
										<span>Notifications</span>
										{hasNewNotifications && (
											<span className="ml-auto w-2 h-2 navbar-notification-badge" />
										)}
									</Link>

									{/* Theme Toggle */}
									<div className="flex items-center justify-between px-2 py-2">
										<span className="navbar-mobile-theme-label">
											Theme
										</span>
										<ModeToggle />
									</div>

									{/* Sign Out Button */}
									<Button
										onClick={() => {
											setMobileMenuOpen(false);
											handleSignOut();
										}}
										variant="destructive"
										className="w-full"
									>
										Sign Out
									</Button>
								</div>
							)}

							{/* Conditional: NOT Logged In */}
							{!isLoggedIn && (
								<div className="flex w-full flex-col gap-4 pt-4 border-t border-border">
									{/* Privacy Policy Link */}
									<Link
										to="/privacy-policy"
										onClick={() => setMobileMenuOpen(false)}
										className="navbar-mobile-item"
									>
										<Shield className="w-5 h-5" />
										<span>Privacy Policy</span>
									</Link>

									{/* Theme Toggle */}
									<div className="flex items-center justify-between px-2 py-2">
										<span className="navbar-mobile-theme-label">
											Theme
										</span>
										<ModeToggle />
									</div>

									{/* Auth Buttons */}
									<div className="flex flex-col gap-2">
										<Button
											variant="outline"
											className="w-full"
											onClick={() => {
												setMobileMenuOpen(false);
												navigate("/login");
											}}
										>
											Login
										</Button>
										<Button
											className="w-full"
											onClick={() => {
												setMobileMenuOpen(false);
												navigate("/signup");
											}}
										>
											Register
										</Button>
									</div>
								</div>
							)}
						</MobileNavMenu>
					</MobileNav>
				</Navbar>

				{/* Navigation Items - Below Navbar */}
				<div className="hidden md:flex justify-center">
					<CustomNavItems items={navItems} isScrolled={isScrolled} />
				</div>
			</div>

			{/* Search Dialog */}
			<SearchDialog open={open} setOpen={setOpen} />
		</>
	);
};

export default NavBar;
