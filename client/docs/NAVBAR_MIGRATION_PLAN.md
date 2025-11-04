# NavBar Migration Plan: Aceternity Resizable NavBar

## Overview

This document outlines the migration strategy for transitioning the current NavBar component to use the Aceternity Resizable NavBar component while preserving all existing business logic.

The Aceternity resizable-navbar provides an animated navigation bar that changes width on scroll while maintaining responsive design. The migration will preserve all existing business logic while adopting the new animated UI components.

---

## Key Features of Resizable NavBar

1. **Scroll-based animation**: Navbar shrinks to 40% width on desktop and 90% on mobile after scrolling 100px
2. **Backdrop blur effect**: Adds glassmorphic blur when scrolled
3. **Hover animations**: Nav items have animated hover states
4. **Responsive**: Separate desktop (`NavBody`) and mobile (`MobileNav`) implementations
5. **Spring animations**: Smooth transitions using Framer Motion

---

## Migration Strategy

### Phase 1: Installation & Dependencies ✅ COMPLETED

#### 1.1 Install the Component ✅

```bash
npx shadcn@latest add @aceternity/resizable-navbar
```

**Status**: Component successfully created at `src/components/ui/resizable-navbar.tsx`

#### 1.2 Install Additional Dependencies ✅

Ensure the following are installed:

- `motion/react` (Framer Motion) - v12.23.24 ✅
- `@tabler/icons-react` (for menu icons) - v3.35.0 ✅

```bash
npm install motion @tabler/icons-react
```

**Status**: All dependencies installed successfully with no vulnerabilities

---

### Phase 2: Component Structure Mapping ✅ COMPLETED

#### Current Structure - Detailed Analysis

```
Current NavBar (div with bg-nav-brand, h-16)
├── Left Container (div flex items-center)
│   ├── Logo (h1)
│   │   ├── Text: "Expert" (hidden on sm)
│   │   ├── Text: "EB" (visible on mobile)
│   │   ├── Text: "Bridge" (hidden on sm)
│   │   └── onClick: handleLogoClick -> navigate("/")
│   │
│   └── Desktop Navigation (nav, hidden lg:flex ml-5)
│       ├── Link to="/home" (conditional bg on active)
│       ├── Link to="/jobs" (conditional bg on active)
│       └── Conditional based on isLoggedIn:
│           ├── If logged in:
│           │   ├── Link to="/offers"
│           │   └── Link to="/my-jobs"
│           └── If NOT logged in:
│               └── Link to="/AboutUs"
│
└── Right Container (div flex items-center gap-2)
    ├── Search Button (hidden on mobile, visible sm:flex)
    │   └── Opens SearchDialog
    ├── Mobile Search Button (sm:hidden)
    ├── Mode Toggle (hidden sm:block)
    │
    └── Conditional based on isLoggedIn:
        ├── If logged in:
        │   ├── Notifications Link (hidden sm:block)
        │   │   └── Badge if hasNewNotifications
        │   └── Profile DropdownMenu
        │       ├── Trigger: Profile Picture
        │       └── Content:
        │           ├── "My Account" Label
        │           ├── Mobile-only nav items (sm:hidden):
        │           │   ├── Home, Jobs, Offers, My Jobs, Notifications
        │           │   └── Theme toggle
        │           ├── Profile link
        │           └── Sign Out button
        │
        └── If NOT logged in:
            ├── Mobile hamburger (lg:hidden)
            │   └── onClick: setMobileMenuOpen(true)
            └── Desktop buttons (hidden lg:flex)
                ├── LoginBtn
                └── RegisterBtn

Mobile Menu Overlay (for non-logged in users)
└── Conditional: !isLoggedIn && mobileMenuOpen
    ├── Backdrop (fixed inset-0 z-50)
    └── Slide-out Panel (fixed right-0, w-64)
        ├── Header with Menu title and X button
        └── Nav links:
            ├── Home, Jobs, About Us, Privacy Policy
            └── Footer section:
                ├── Theme toggle
                ├── LoginBtn
                └── RegisterBtn
```

#### New Structure - Target Architecture

```
New Resizable NavBar
├── <Navbar> (container with scroll detection)
│   │
│   ├── <NavBody> (desktop navigation, hidden on lg:)
│   │   ├── Custom Logo Component
│   │   │   └── Same responsive text logic
│   │   │
│   │   ├── Custom NavItems Component (with active state)
│   │   │   └── Dynamic items based on isLoggedIn
│   │   │
│   │   └── Actions Container
│   │       ├── Search Button
│   │       ├── ModeToggle
│   │       └── Conditional:
│   │           ├── If logged in:
│   │           │   ├── Notifications Link
│   │           │   └── Profile DropdownMenu
│   │           └── If NOT logged in:
│   │               ├── LoginBtn
│   │               └── RegisterBtn
│   │
│   └── <MobileNav> (visible on mobile, lg:hidden)
│       ├── <MobileNavHeader>
│       │   ├── Logo
│       │   └── <MobileNavToggle>
│       │
│       └── <MobileNavMenu>
│           ├── Navigation Links (based on isLoggedIn)
│           └── Actions Container:
│               ├── If logged in:
│               │   ├── Profile button
│               │   ├── Notifications button
│               │   ├── Theme toggle
│               │   └── Sign Out button
│               └── If NOT logged in:
│                   ├── Theme toggle
│                   ├── LoginBtn
│                   └── RegisterBtn

SearchDialog (unchanged, separate component)
```

#### Key Differences Identified

1. **Mobile Menu for Logged-in Users**: Currently uses profile dropdown, will migrate to MobileNavMenu
2. **Mobile Menu for Non-logged-in Users**: Currently uses custom overlay, will migrate to MobileNavMenu
3. **Active Route Highlighting**: Need custom implementation in NavItems
4. **Search Button**: Different visibility on mobile vs desktop
5. **Notification Badge**: Custom implementation needs to be preserved
6. **Profile Picture**: Must remain in DropdownMenu on desktop, become button in mobile menu

#### Phase 2 Summary

**Status**: ✅ Complete

**Completed Tasks**:

- ✅ Analyzed entire current NavBar.tsx structure (411 lines)
- ✅ Documented all desktop navigation elements and their behavior
- ✅ Documented all mobile navigation patterns (both logged in and logged out)
- ✅ Identified authentication-based conditional rendering logic
- ✅ Mapped all action buttons and their responsive behavior
- ✅ Created detailed component tree diagrams for current and new structures
- ✅ Identified 6 key architectural differences requiring attention

**Key Findings**:

- Current implementation has dual mobile menu strategies (dropdown for logged-in, overlay for logged-out)
- Search button has 3 different states (desktop full, desktop icon, mobile icon)
- Profile dropdown contains mobile navigation items when viewport is small
- Active route highlighting uses conditional className with location.pathname comparison
- Notification badge uses custom absolute positioning
- All routing uses react-router-dom's Link and useNavigate

**Ready for**: Phase 3 (Component Mapping Details) and Phase 5 (Custom Components Creation)

---

### Phase 3: Component Mapping Details ✅ COMPLETED

#### 3.1 Desktop Navigation (`NavBody`) - Detailed Implementation

**Current Implementation**:

- Logo: `<h1>` with `onClick={handleLogoClick}`
- Navigation: Individual `<Link>` components with conditional `bg-nav-brand-hover`
- Actions: Search, ModeToggle, Notifications (with badge), Profile Dropdown

**New Implementation - Complete Code Structure**:

```tsx
<NavBody
	className={cn(
		"bg-nav-brand dark:bg-nav-brand",
		visible && "bg-nav-brand/80 dark:bg-nav-brand/80"
	)}
>
	{/* SECTION 1: Logo Component */}
	<CustomNavbarLogo onClick={handleLogoClick} />

	{/* SECTION 2: Navigation Links with Active State */}
	<CustomNavItems
		items={navItems}
		className="absolute inset-0 hidden flex-1 flex-row items-center justify-center space-x-2 text-sm font-medium transition duration-200 lg:flex lg:space-x-2"
	/>

	{/* SECTION 3: Action Buttons Container */}
	<div className="flex items-center gap-2 sm:gap-3 lg:gap-5">
		{/* Search Button - Desktop Full */}
		<Button
			className="bg-secondary text-muted-foreground hover:bg-accent hover:text-primary transition-colors hidden sm:flex px-4 lg:px-9 shadow-sm border border-border"
			onClick={handleSearch}
		>
			<Search className="w-4 h-4" />
			<span className="ml-2 hidden md:inline">
				Search jobs, questions, or users...
			</span>
			<span className="ml-2 md:hidden">Search</span>
		</Button>

		{/* Mode Toggle - Desktop Only */}
		<div className="hidden sm:block">
			<ModeToggle />
		</div>

		{/* Conditional: Logged In Actions */}
		{isLoggedIn && (
			<>
				{/* Notifications Link with Badge */}
				<Link
					to="/notifications"
					className="text-white hover:text-primary-foreground transition-colors relative hidden sm:block"
				>
					<Bell className="w-6 h-6" />
					{hasNewNotifications && (
						<span className="absolute -top-1 -right-1 w-3 h-3 bg-destructive rounded-full" />
					)}
				</Link>

				{/* Profile Dropdown - Keep Existing */}
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
									className="w-full h-full rounded-full object-cover"
									alt="Profile"
								/>
							)}
						</div>
					</DropdownMenuTrigger>
					<DropdownMenuContent className="bg-card w-48" align="end">
						<DropdownMenuLabel>My Account</DropdownMenuLabel>
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

		{/* Conditional: NOT Logged In Actions */}
		{!isLoggedIn && (
			<div className="hidden lg:flex items-center gap-2">
				<LoginBtn />
				<RegisterBtn />
			</div>
		)}
	</div>
</NavBody>
```

**Key Changes**:

1. ✅ Wrap entire desktop nav in `<NavBody>`
2. ✅ Replace `<h1>` with `<CustomNavbarLogo>`
3. ✅ Replace individual Links with `<CustomNavItems>` component
4. ✅ Keep all action buttons in their current structure
5. ✅ Profile dropdown simplified (remove mobile nav items - moved to MobileNav)
6. ✅ Apply brand colors with conditional opacity based on scroll state

#### 3.2 Mobile Navigation (`MobileNav`) - Detailed Implementation

**Current Implementation**:

- **Logged In**: Hamburger hidden, navigation items in profile dropdown
- **Logged Out**: Hamburger button opens custom overlay panel
- Different UX patterns for different auth states

**New Implementation - Unified Approach**:

```tsx
<MobileNav
	className={cn(
		"bg-nav-brand dark:bg-nav-brand",
		visible && "bg-nav-brand/80 dark:bg-nav-brand/80"
	)}
>
	{/* SECTION 1: Mobile Header (Always Visible) */}
	<MobileNavHeader>
		{/* Logo Component - Same as Desktop */}
		<CustomNavbarLogo onClick={handleLogoClick} />

		{/* Right Side Container */}
		<div className="flex items-center gap-2">
			{/* Mobile Search Button */}
			<Button
				className="bg-transparent hover:bg-nav-brand-hover text-white p-2"
				onClick={handleSearch}
			>
				<Search className="w-5 h-5" />
			</Button>

			{/* Mobile Menu Toggle */}
			<MobileNavToggle
				isOpen={mobileMenuOpen}
				onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
			/>
		</div>
	</MobileNavHeader>

	{/* SECTION 2: Mobile Menu (Conditional) */}
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
					"flex items-center text-neutral-600 dark:text-neutral-300 px-2 py-3",
					location.pathname === item.link &&
						"text-primary font-semibold"
				)}
			>
				<span className="block">{item.name}</span>
			</Link>
		))}

		{/* Conditional: Logged In User Actions */}
		{isLoggedIn && (
			<div className="flex w-full flex-col gap-4 pt-4 border-t border-border">
				{/* Profile Link */}
				<Link
					to="/profile"
					onClick={() => setMobileMenuOpen(false)}
					className="flex items-center gap-2 text-neutral-600 dark:text-neutral-300 px-2 py-2"
				>
					{userProfile?.profilePictureUrl ? (
						<img
							src={userProfile.profilePictureUrl}
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
					className="flex items-center gap-2 text-neutral-600 dark:text-neutral-300 px-2 py-2"
				>
					<Bell className="w-5 h-5" />
					<span>Notifications</span>
					{hasNewNotifications && (
						<span className="ml-auto w-2 h-2 bg-red-500 rounded-full" />
					)}
				</Link>

				{/* Theme Toggle */}
				<div className="flex items-center justify-between px-2 py-2">
					<span className="text-neutral-600 dark:text-neutral-300">
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

		{/* Conditional: NOT Logged In Actions */}
		{!isLoggedIn && (
			<div className="flex w-full flex-col gap-4 pt-4 border-t border-border">
				{/* Additional Links for Non-Logged Users */}
				<Link
					to="/privacy-policy"
					onClick={() => setMobileMenuOpen(false)}
					className="flex items-center gap-2 text-neutral-600 dark:text-neutral-300 px-2 py-2"
				>
					<Shield className="w-5 h-5" />
					<span>Privacy Policy</span>
				</Link>

				{/* Theme Toggle */}
				<div className="flex items-center justify-between px-2 py-2">
					<span className="text-neutral-600 dark:text-neutral-300">
						Theme
					</span>
					<ModeToggle />
				</div>

				{/* Auth Buttons */}
				<div className="flex flex-col gap-2">
					<LoginBtn />
					<RegisterBtn />
				</div>
			</div>
		)}
	</MobileNavMenu>
</MobileNav>
```

**Key Changes**:

1. ✅ Unified mobile menu for both logged-in and logged-out users
2. ✅ Move all mobile navigation from profile dropdown to MobileNavMenu
3. ✅ Remove custom overlay panel (replaced by MobileNavMenu)
4. ✅ Add mobile search button in header
5. ✅ Preserve notification badge in mobile menu
6. ✅ Profile picture shown in mobile menu for logged-in users
7. ✅ Privacy Policy link added for non-logged-in users
8. ✅ Theme toggle available in both auth states

---

### Phase 4: Business Logic Preservation ✅ COMPLETED

#### 4.1 Authentication State - Complete Preservation

**Current Code (Keep Unchanged)**:

```tsx
// Query hooks - KEEP EXACTLY AS IS
useGetCurrentUserProfileQuery();

// Custom hook - KEEP EXACTLY AS IS
const [isLoggedIn, loginLoading, loginError, authUser, userProfile] =
	useIsUserLoggedIn();

// Notifications query - KEEP EXACTLY AS IS
const { data: notifications } = useGetNotificationsQuery(userProfile?.id ?? "");

// Notification badge logic - KEEP EXACTLY AS IS
const hasNewNotifications =
	notifications?.filter((n) => n.isRead === false).length ?? 0 > 0;
```

**Status**: ✅ No changes required - all authentication logic remains identical

**Usage in New Structure**:

- `isLoggedIn` → Used for conditional rendering in both NavBody and MobileNav
- `userProfile` → Used for profile picture in dropdown and mobile menu
- `hasNewNotifications` → Used for notification badge in both desktop and mobile

---

#### 4.2 Navigation Logic - Complete Preservation

**Current Code (Keep Unchanged)**:

```tsx
// Router hooks - KEEP EXACTLY AS IS
const location = useLocation();
const navigate = useNavigate();

// Logo click handler - KEEP EXACTLY AS IS
const handleLogoClick = useCallback(() => {
	navigate("/");
}, [navigate]);

// Onboarding redirect - KEEP EXACTLY AS IS
useEffect(() => {
	if (userProfile) {
		if (!userProfile.isOnboarded) {
			navigate("/onboarding");
		}
	}
}, [userProfile, navigate]);
```

**Status**: ✅ No changes required - all navigation logic remains identical

**Usage in New Structure**:

- `location.pathname` → Used in CustomNavItems for active route highlighting
- `navigate()` → Used in CustomNavItems onClick handlers
- `handleLogoClick` → Passed to CustomNavbarLogo component

---

#### 4.3 State Management - Complete Preservation

**Current Code (Keep Unchanged)**:

```tsx
// Search dialog state - KEEP EXACTLY AS IS
const [open, setOpen] = useState(false);

// Mobile menu state - KEEP EXACTLY AS IS
const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

// Search handler - KEEP EXACTLY AS IS
const handleSearch = () => {
	setOpen(true);
};
```

**Status**: ✅ No changes required - all state management remains identical

**Usage in New Structure**:

- `open` & `setOpen` → Passed to SearchDialog (unchanged)
- `mobileMenuOpen` & `setMobileMenuOpen` → Used in MobileNavToggle and MobileNavMenu
- `handleSearch` → Used in search buttons (desktop and mobile)

---

#### 4.4 Sign Out Logic - Complete Preservation

**Current Code (Keep Unchanged)**:

```tsx
// Firebase auth - KEEP EXACTLY AS IS
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";

// Sign out hook - KEEP EXACTLY AS IS
const [signOut, loading, error] = useSignOut(auth);

// Sign out handler - KEEP EXACTLY AS IS
const handleSignOut = useCallback(async () => {
	await signOut();
	console.log("Signed Out");
	navigate("/login", { replace: true });
}, [signOut, navigate]);
```

**Status**: ✅ No changes required - all sign out logic remains identical

**Usage in New Structure**:

- Desktop: Profile dropdown Sign Out button
- Mobile: MobileNavMenu Sign Out button

---

#### 4.5 Dynamic Navigation Items - NEW (But Simple)

**New Code to Add**:

```tsx
// Create dynamic navigation items based on auth state
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
```

**Status**: ✅ New code - transforms existing conditional Link components into data structure

**Why This Works**:

- Extracts the same conditional logic currently in JSX
- Makes navigation items reusable for both desktop and mobile
- Maintains exact same routes and behavior

---

#### 4.6 Import Statements - Additions Only

**Current Imports (Keep All)**:

```tsx
import { Link, useNavigate, useLocation } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import RegisterBtn from "@/views/components/custom/RegisterBtn";
import { DropdownMenu, ... } from "@/views/components/ui/dropdown-menu";
import { Button } from "@/views/components/ui/button";
import { Bell, Search, User, Menu, X, Home, Briefcase, Info, Shield } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useGetNotificationsQuery } from "@/features/notifications/notificationsSlice";
import { ModeToggle } from "@/components/mode-toggle";
import LoginBtn from "@/views/components/custom/LoginBtn";
import SearchDialog from "@/views/components/common/search/SearchDialog";
```

**New Imports to Add**:

```tsx
import { useMemo } from "react"; // Add to existing react import
import { motion } from "motion/react";
import { cn } from "@/lib/util/utils";
import {
	Navbar,
	NavBody,
	MobileNav,
	MobileNavHeader,
	MobileNavToggle,
	MobileNavMenu,
} from "@/components/ui/resizable-navbar";
```

**Status**: ✅ Only additions - no removals or changes to existing imports

---

#### 4.7 Business Logic Summary

**What Stays Exactly the Same**:

1. ✅ All authentication hooks and queries
2. ✅ All navigation hooks and callbacks
3. ✅ All state management (open, mobileMenuOpen)
4. ✅ All event handlers (handleSearch, handleSignOut, handleLogoClick)
5. ✅ All conditional rendering logic (isLoggedIn checks)
6. ✅ All notification badge logic
7. ✅ All profile picture logic
8. ✅ All onboarding redirect logic
9. ✅ SearchDialog integration

**What Changes**:

1. 🔄 JSX structure (wrapped in new components)
2. 🔄 Individual Links → CustomNavItems component
3. 🔄 Custom mobile overlay → MobileNavMenu
4. 🔄 Mobile nav in dropdown → MobileNavMenu

**What's New**:

1. ➕ `navItems` useMemo hook (extracts existing logic)
2. ➕ CustomNavbarLogo component
3. ➕ CustomNavItems component
4. ➕ Scroll detection (handled by Navbar component)

**Critical Insight**: 95% of business logic code remains unchanged. Only the JSX structure changes to use new components.

#### Phase 3 & 4 Summary

**Status**: ✅ Both Phases Complete

**Phase 3 Deliverables**:

- ✅ Complete NavBody implementation with all sections detailed
- ✅ Complete MobileNav implementation with unified approach
- ✅ Detailed code structure for desktop navigation
- ✅ Detailed code structure for mobile navigation
- ✅ Conditional rendering logic for both auth states
- ✅ Action buttons placement and styling
- ✅ Profile dropdown simplification plan
- ✅ Mobile menu unification strategy

**Phase 4 Deliverables**:

- ✅ Authentication state preservation (100% unchanged)
- ✅ Navigation logic preservation (100% unchanged)
- ✅ State management preservation (100% unchanged)
- ✅ Sign out logic preservation (100% unchanged)
- ✅ New navItems hook design
- ✅ Import statements update plan
- ✅ Business logic impact analysis

**Key Architectural Decisions**:

1. **Unified Mobile Menu**: Both logged-in and logged-out users will use MobileNavMenu (eliminating dual pattern)
2. **Profile Dropdown Simplified**: Remove mobile navigation items (moved to MobileNavMenu)
3. **Brand Colors Preserved**: Custom styling applied to maintain navy blue theme
4. **Active Route Highlighting**: Custom implementation in NavItems component
5. **Search Button**: Three responsive states maintained
6. **Notification Badge**: Custom positioning preserved

**Migration Risk Assessment**: ✅ LOW

- No changes to business logic or data fetching
- No changes to routing or authentication
- Only structural/presentational changes
- All existing functionality preserved

**Ready for**: Phase 5 (Custom Components Creation) - we can now start building the actual components

---

### Phase 5: Custom Components to Create

#### 5.1 Custom Logo Component

Replace the current `<h1>` logo with a custom component compatible with the new navbar:

```tsx
const CustomNavbarLogo = ({ onClick }: { onClick: () => void }) => {
	return (
		<div
			onClick={onClick}
			className="relative z-20 flex items-center space-x-2 px-2 py-1 cursor-pointer"
		>
			<span className="text-white text-2xl lg:text-3xl font-bold">
				<span className="hidden sm:inline">Expert</span>
				<span className="sm:hidden">EB</span>
				<span className="hidden sm:inline">Bridge</span>
			</span>
		</div>
	);
};
```

#### 5.2 Dynamic Navigation Items

Create navigation items array based on auth state:

```tsx
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
```

#### 5.3 Active Route Highlighting

Since `NavItems` doesn't have built-in active state, we'll need to customize:

**Option A**: Fork the `NavItems` component and add active state comparison

**Option B**: Create custom navigation links outside of `NavItems`

```tsx
// Custom implementation with active state
const CustomNavItems = ({ items, className }: NavItemsProps) => {
	const location = useLocation();
	const navigate = useNavigate();
	const [hovered, setHovered] = useState<number | null>(null);

	return (
		<div
			onMouseLeave={() => setHovered(null)}
			className={cn("flex items-center gap-2", className)}
		>
			{items.map((item, idx) => (
				<button
					key={item.link}
					onMouseEnter={() => setHovered(idx)}
					onClick={() => navigate(item.link)}
					className={cn(
						"relative px-4 py-2 text-neutral-600 dark:text-neutral-300",
						location.pathname === item.link &&
							"text-primary font-semibold"
					)}
				>
					{hovered === idx && (
						<motion.div
							layoutId="hovered"
							className="absolute inset-0 h-full w-full rounded-full bg-gray-100 dark:bg-neutral-800"
						/>
					)}
					<span className="relative z-20">{item.name}</span>
				</button>
			))}
		</div>
	);
};
```

---

### Phase 6: Styling Considerations

#### 6.1 Color Scheme Adaptation

**Current**: Navy blue brand color (`bg-nav-brand`)  
**New**: Default white/neutral with blur effect

**Strategy**: Override the default styles:

```tsx
<NavBody className="bg-nav-brand/90 dark:bg-nav-brand/90">
	{/* When visible=false (not scrolled), add bg-nav-brand */}
	{/* When visible=true (scrolled), keep glassmorphic effect */}
</NavBody>
```

**Custom styling based on scroll state**:

```tsx
<NavBody
  className={cn(
    !visible && "bg-nav-brand dark:bg-nav-brand",
    visible && "bg-nav-brand/80 dark:bg-nav-brand/80"
  )}
>
```

#### 6.2 Responsive Breakpoints

- Current uses `lg:` breakpoint for desktop/mobile switch
- Resizable navbar uses same `lg:` breakpoint (compatible ✅)

#### 6.3 Z-Index Management

- Resizable navbar uses `z-40` for container, `z-50` for mobile
- Ensure no conflicts with existing components (SearchDialog, etc.)
- SearchDialog should use `z-[60]` or higher

---

### Phase 7: Migration Steps

1. ✅ **Install component**: Run the shadcn command - COMPLETED
2. ✅ **Component structure mapping**: Analyze and document current structure - COMPLETED
3. ⏳ **Create custom logo component**: Extract logo into reusable component
4. ⏳ **Build navItems array**: Create dynamic navigation items based on auth
5. ⏳ **Create custom NavItems**: Build NavItems with active state highlighting
6. ⏳ **Wrap desktop navigation**: Replace current desktop nav with `<NavBody>`
7. ⏳ **Migrate action buttons**: Move search, mode toggle, profile to right side of NavBody
8. ⏳ **Wrap mobile navigation**: Replace mobile menu with `<MobileNav>` structure
9. ⏳ **Test scroll behavior**: Verify animation triggers at 100px scroll
10. ⏳ **Test authentication flows**: Verify conditional rendering works
11. ⏳ **Test responsive behavior**: Check mobile/desktop transitions
12. ⏳ **Style overrides**: Apply brand colors and custom styling
13. ⏳ **Test all interactions**: Search, notifications, profile menu, sign out

---

### Phase 8: Potential Challenges & Solutions

| Challenge                           | Solution                                                                  |
| ----------------------------------- | ------------------------------------------------------------------------- |
| **Active route highlighting**       | Create custom NavItems component with location comparison                 |
| **Brand color preservation**        | Override default white background with brand colors                       |
| **Profile dropdown in NavBody**     | Keep existing DropdownMenu, style to match navbar                         |
| **Notification badge**              | Keep existing Bell icon with absolute positioned badge                    |
| **Mobile menu for logged-in users** | Use MobileNavMenu for all mobile navigation instead of dropdown           |
| **Search dialog integration**       | Keep existing SearchDialog, ensure z-index is higher than navbar          |
| **Minimum width constraint**        | NavBody has `minWidth: 800px` - may need adjustment for smaller viewports |

---

### Phase 9: Testing Checklist

#### Desktop Navigation

- [ ] Desktop navigation displays correctly when not scrolled
- [ ] Desktop navigation animates to 40% width after 100px scroll
- [ ] Nav items have hover animations
- [ ] Active route is highlighted
- [ ] Search button works
- [ ] Mode toggle works
- [ ] Notifications link works (logged in)
- [ ] Profile dropdown works (logged in)
- [ ] Login/Register buttons work (logged out)

#### Mobile Navigation

- [ ] Mobile navigation displays correctly when not scrolled
- [ ] Mobile navigation animates to 90% width after 100px scroll
- [ ] Mobile menu opens/closes smoothly
- [ ] Mobile menu items navigate correctly
- [ ] Mobile menu closes on navigation
- [ ] Mobile actions work (Profile, Notifications, Sign Out)

#### Business Logic

- [ ] Logo click navigates to home/root
- [ ] Sign out works correctly
- [ ] Onboarding redirect works
- [ ] Search dialog opens correctly
- [ ] Notification badge displays correctly
- [ ] Profile picture displays correctly

#### Styling & Responsiveness

- [ ] All responsive breakpoints work correctly
- [ ] Brand colors are preserved
- [ ] Dark mode works correctly
- [ ] Text is readable in all states
- [ ] Animations are smooth
- [ ] No layout shifts or glitches

---

## Summary

This migration will modernize your navbar with smooth scroll-based animations while preserving all existing business logic including:

- ✅ Authentication state management
- ✅ Profile and notification fetching
- ✅ Conditional rendering based on login status
- ✅ Search functionality
- ✅ Active route highlighting
- ✅ Sign out functionality
- ✅ Onboarding redirect
- ✅ Dark mode support

The main structural change is adopting the component-based architecture of the resizable navbar (`NavBody`, `NavItems`, `MobileNav`, etc.) while keeping all your existing hooks, state management, and business logic intact.

---

## Resources

- **Aceternity Resizable NavBar**: https://ui.aceternity.com/components/resizable-navbar
- **Demo Code**: https://ui.aceternity.com/registry/resizable-navbar-demo.json
- **Framer Motion Docs**: https://www.framer.com/motion/
- **Tabler Icons**: https://tabler.io/icons

---

## Next Steps

1. Review this plan with the team
2. Set up a feature branch for the migration
3. Begin with Phase 1 (Installation & Dependencies)
4. Implement Phase 5 (Custom Components) first
5. Gradually migrate desktop and mobile navigation
6. Thoroughly test before merging

---

**Last Updated**: November 4, 2025  
**Author**: GitHub Copilot  
**Status**: Planning Phase Complete - Ready for Implementation

**Completed Phases**:

- ✅ Phase 1: Installation & Dependencies
- ✅ Phase 2: Component Structure Mapping
- ✅ Phase 3: Component Mapping Details
- ✅ Phase 4: Business Logic Preservation

**Next Phase**: Phase 5 - Custom Components Creation
