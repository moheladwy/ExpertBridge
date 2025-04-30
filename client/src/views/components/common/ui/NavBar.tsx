import { Link, useNavigate, useLocation } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import RegisterBtn from "../../custom/RegisterBtn";
import { Avatar } from "@/views/components/custom/avatar";
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
	CommandDialog,
	CommandEmpty,
	CommandGroup,
	CommandInput,
	CommandItem,
	CommandList,
} from "@/views/components/custom/command";
import { DialogTitle, DialogDescription } from "@radix-ui/react-dialog";
import { Search } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg"
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";

const NavBar = () => {
	useGetCurrentUserProfileQuery();

	const navigate = useNavigate();
	const [isLoggedIn, loginLoading, loginError, authUser, userProfile] =
		useIsUserLoggedIn();
	const [signOut, loading, error] = useSignOut(auth);

	const handleSignOut = useCallback(async () => {
		await signOut();
		console.log("Signed Out");
		navigate("/login", { replace: true }); // Redirect to login page after signing out

	}, [signOut, navigate]);

	const [open, setOpen] = useState(false);
	const [searchInput, setSearchInput] = useState("");

	//to get the active location
	const location = useLocation();

	const handelSearch = () => {
		setOpen((open) => !open);
		// console.log(open);
	};

	const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		setSearchInput(event.target.value);
	};

	const handleLogoClick = useCallback(() => {
		if (isLoggedIn) navigate("/home");
		else navigate("/");
	}, [isLoggedIn, navigate]);

	// console.log(userProfile);

	return (
		<div className="flex items-center w-full bg-main-blue h-16 drop-shadow-md">
			<div className="flex items-center mx-9">
				<h1
					className="text-white text-3xl cursor-pointer max-sm:text-lg"
					onClick={handleLogoClick}
				>
					<b>Expert</b>Bridge
				</h1>

				<Link to="/home"
					className={
						`flex justify-center items-center ml-5 py-5 px-3 max-sm:hidden hover:bg-blue-950
							${location.pathname === '/home' ? 'bg-blue-950' : ''}`
					}
				>
					<div className="text-white font-light max-sm:hidden">
						Home
					</div>
				</Link>

				{isLoggedIn ? (
					<>
						{/* TO ADD THE REAL LINK LATER */}
						<Link to="/jobs" className="flex justify-center items-center py-5 px-3 max-sm:hidden hover:bg-blue-950">
							<div className="text-white font-light max-sm:hidden">
								Jobs
							</div>
						</Link>
					</>
				) : (
					<>
						{/* TO ADD THE REAL LINK LATER */}
						<Link to="/AboutUs" className="flex justify-center items-center py-5 px-3 max-sm:hidden hover:bg-blue-950">
							<div className="text-white font-light max-sm:hidden">
								About Us
							</div>
						</Link>
					</>
				)}
			</div>

			<div className="flex ml-auto mr-9">
				{isLoggedIn ? (
					<>
						<div className="flex justify-center items-center gap-5">
							{/* Search bar */}
							<Button
								className="bg-gray-100 text-gray-500 px-9 hover:bg-gray-200 hover:text-main-blue max-md:p-2 
              max-md:bg-main-blue max-md:text-white max-md:hover:bg-main-blue max-md:hover:text-white"
								onClick={handelSearch}
							>
								<Search />{" "}
								<div className="max-md:hidden">
									Search in the questions
								</div>
							</Button>

							{/* Search popup */}
							<CommandDialog open={open} onOpenChange={setOpen}>
								<DialogTitle className="sr-only">
									Search
								</DialogTitle>
								<DialogDescription className="sr-only">
									Search about questions
								</DialogDescription>

								<CommandInput
									placeholder="Type a question to search..."
									onChangeCapture={handleChange}
								/>
								<CommandList>
									<CommandEmpty>
										No results found.
									</CommandEmpty>
									<CommandGroup heading="Suggestions">
										<CommandItem>
											<span>how to fix my car?</span>
										</CommandItem>
									</CommandGroup>
								</CommandList>
							</CommandDialog>

							{/* Profile Pic */}
							<DropdownMenu>
								<DropdownMenuTrigger>
									{/* Profile Pic */}
									<div className="flex justify-center items-center">
										{userProfile?.profilePictureUrl ?
											<img
												src={
													userProfile.profilePictureUrl
												}
												width={45}
												height={45}
												className="rounded-full"
											/>
											:
											<img
												src={defaultProfile}
												alt="Profile Picture"
												width={45}
												height={45}
												className="rounded-full"
											/>
										}
									</div>
								</DropdownMenuTrigger>
								<DropdownMenuContent>
									<DropdownMenuLabel>
										My Account
									</DropdownMenuLabel>
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
						</div>
					</>
				) : (
					<>
						<button
							onClick={() => navigate("/login")}
							className="text-white border-2 rounded-full px-6 py-2 mr-4 hover:text-main-blue hover:bg-white hover:font-bold max-sm:text-xs max-sm:px-3"
						>
							Login
						</button>
						<RegisterBtn />
					</>
				)}
			</div>
		</div>
	);
};

export default NavBar;
