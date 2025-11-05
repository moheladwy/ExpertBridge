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
import { Skeleton } from "@/views/components/ui/skeleton";
import { Bell, Search, User, Shield } from "lucide-react";
import { useCallback, useEffect, useState, useMemo } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetNotificationsQuery } from "@/features/notifications/notificationsSlice";
import { useAuthReady } from "@/hooks/useAuthReady"; // ✅ NEW
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
import { AuthButtons } from "@/views/components/custom/AuthButtons";

const NavBar = () => {
	// ✅ NEW: Check auth state before querying
	const { isAuthReady: _isAuthReady, isAuthenticated } = useAuthReady();

	// ✅ UPDATED: Conditional profile query
	const { data: currentProfile } = useGetCurrentUserProfileQuery(undefined, {
		skip: !isAuthenticated,
	});

	//to get the active location
	const location = useLocation();
	const navigate = useNavigate();

	const [isLoggedIn, _loginLoading, _loginError, _authUser, userProfile] =
		useIsUserLoggedIn();

	// ✅ UPDATED: Use currentProfile or userProfile, skip if not authenticated
	const { data: notifications } = useGetNotificationsQuery(
		currentProfile?.id ?? userProfile?.id ?? "",
		{
			skip: !isAuthenticated, // ✅ NEW: Skip if not authenticated
		}
	);

	// TODO: Refactor to the same model as the one used by RTK Query standard.
	const hasNewNotifications =
		notifications?.filter((n) => n.isRead === false).length ?? 0 > 0;

	useEffect(() => {
		// ✅ UPDATED: Use currentProfile for onboarding check
		const profile = currentProfile ?? userProfile;
		if (profile) {
			if (!profile.isOnboarded) {
				navigate("/onboarding");
			}
		}
	}, [currentProfile, userProfile, navigate]);

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

	// ✅ NEW: Show skeleton while loading
	if (!_isAuthReady || (isAuthenticated && !currentProfile && !userProfile)) {
		return (
			<Navbar className="w-full">
				<NavBody className="bg-primary text-primary-foreground h-16">
					<CustomNavbarLogo onClick={handleLogoClick} />
					<div className="flex items-center gap-4 ml-auto">
						<Skeleton className="h-8 w-8 rounded-full" />
						<Skeleton className="h-8 w-24" />
					</div>
				</NavBody>
			</Navbar>
		);
	}

	return (
		<>
			<div className="sticky w-full flex flex-col gap-6">
				<Navbar className="w-full">
					{/* Desktop Navigation */}
					<NavBody className="bg-primary text-primary-foreground h-16">
						{/* Logo */}
						<CustomNavbarLogo onClick={handleLogoClick} />

						{/* Search Bar - Center (Desktop Full Size) */}
						<div className="flex-1 max-w-2xl mx-4 hidden md:block">
							<Button
								className="w-full bg-primary-foreground/10 text-primary-foreground/80 hover:bg-primary-foreground/20 transition-all px-4 shadow-sm border border-primary-foreground/20 rounded-full justify-start"
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
								className="md:hidden bg-transparent hover:bg-primary/80 text-primary-foreground p-2"
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
										className="text-primary-foreground hover:text-primary-foreground/90 transition-colors relative hidden sm:block"
									>
										<Bell className="w-6 h-6" />
										{hasNewNotifications ? (
											<span className="absolute -top-1 -right-1 w-3 h-3 bg-destructive rounded-full" />
										) : null}
									</Link>

									{/* Profile Dropdown */}
									<DropdownMenu>
										<DropdownMenuTrigger className="flex items-center">
											<div className="w-8 h-8 lg:w-10 lg:h-10">
												{/* ✅ UPDATED: Use currentProfile or fallback to userProfile */}
												{(currentProfile?.profilePictureUrl ??
												userProfile?.profilePictureUrl) ? (
													<img
														src={
															currentProfile?.profilePictureUrl ??
															userProfile?.profilePictureUrl
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
								<div className="hidden lg:flex">
									<AuthButtons />
								</div>
							)}
						</div>
					</NavBody>

					{/* Mobile Navigation */}
					<MobileNav className="bg-primary text-primary-foreground">
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
										"flex items-center text-muted-foreground px-2 py-3",
										location.pathname === item.link &&
											"text-primary font-semibold"
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
										className="flex items-center gap-2 text-muted-foreground px-2 py-2"
									>
										{/* ✅ UPDATED: Use currentProfile or fallback to userProfile */}
										{(currentProfile?.profilePictureUrl ??
										userProfile?.profilePictureUrl) ? (
											<img
												src={
													currentProfile?.profilePictureUrl ??
													userProfile?.profilePictureUrl
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
									</Link>{" "}
									{/* Notifications - Hidden on mobile */}
									<Link
										to="/notifications"
										className="relative hidden sm:block text-muted-foreground px-2 py-2 gap-2"
									>
										<Bell className="w-6 h-6" />
										{hasNewNotifications ? (
											<span className="absolute -top-1 -right-1 w-3 h-3 bg-destructive rounded-full" />
										) : null}
									</Link>
									{/* Theme Toggle */}
									<div className="flex items-center justify-between px-2 py-2">
										<span className="text-muted-foreground">
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
										className="flex items-center gap-2 text-muted-foreground px-2 py-2"
									>
										<Shield className="w-5 h-5" />
										<span>Privacy Policy</span>
									</Link>

									{/* Theme Toggle */}
									<div className="flex items-center justify-between px-2 py-2">
										<span className="text-muted-foreground">
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
					{/* Navigation Items - Below Navbar */}
					<div className="hidden md:flex justify-center">
						<CustomNavItems
							items={navItems}
							isScrolled={isScrolled}
						/>
					</div>
				</Navbar>
			</div>

			{/* Search Dialog */}
			<SearchDialog open={open} setOpen={setOpen} />
		</>
	);
};

export default NavBar;
