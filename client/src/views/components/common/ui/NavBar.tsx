import { Link, useNavigate } from "react-router-dom";
import { auth } from "@/lib/firebase";
import useSignOut from "@/lib/firebase/useSignOut";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import RegisterBtn from "../../custom/RegisterBtn";
import {
  Avatar
} from "@/views/components/custom/avatar"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu"
import { Button } from "@/views/components/custom/button"
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
  CommandShortcut,
} from "@/views/components/custom/command"
import { DialogTitle, DialogDescription } from "@radix-ui/react-dialog"
import { Search } from 'lucide-react';
import { useState } from "react";
import { SvgIcon } from "@mui/material";


const NavBar = () => {
  const navigate = useNavigate();
  const [user] = useAuthSubscribtion(auth);
  const [signOut, loading, error] = useSignOut(auth);

  const handleSignOut = async () => {
    try {
      await signOut();
      console.log("Signed Out");
      // navigate("/login"); // Redirect to login page after signing out
    } catch (error) {
      console.error("Error signing out:", error);
    }
  };

  const [open, setOpen] = useState(false);
  const [searchInput, setSearchInput] = useState("");

  const handelSearch = () => {
    setOpen((open) => !open)
    console.log(open)
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
  };


  return (
    <div className="flex items-center w-full bg-main-blue h-16 drop-shadow-md">
      <SvgIcon>
        <path d="M9 21c0 .55.45 1 1 1h4c.55 0 1-.45 1-1v-1H9v1zm3-19C8.14 2 5 5.14 5 9c0 2.38 1.19 4.47 3 5.74V17c0 .55.45 1 1 1h6c.55 0 1-.45 1-1v-2.26c1.81-1.27 3-3.36 3-5.74 0-3.86-3.14-7-7-7zm2.85 11.1l-.85.6V16h-4v-2.3l-.85-.6C7.8 12.16 7 10.63 7 9c0-2.76 2.24-5 5-5s5 2.24 5 5c0 1.63-.8 3.16-2.15 4.1z" />
      </SvgIcon>
      <div className="flex items-center mx-9">
        <h1 className="text-white text-3xl max-sm:text-lg">
          <b>Expert</b>Bridge
        </h1>
        {user ? (
          <>
            <Link to="/home" className="text-white font-light mx-5 max-sm:hidden">Home</Link>
            <Link to="/home" className="text-white font-light mx-5 max-sm:hidden">Jobs</Link>
          </>
        ) : (
          <>
            <a href="#" className="text-white font-light mx-5 max-sm:hidden">
              About Us
            </a>
          </>
        )}
      </div>

      <div className="flex ml-auto mr-9">
        {user ? (
          <>
            <div className="flex justify-center items-center gap-5">
              {/* Search bar */}
              <Button className="bg-gray-100 text-gray-500 px-9 hover:bg-gray-200 hover:text-main-blue max-md:p-2 
              max-md:bg-main-blue max-md:text-white max-md:hover:bg-main-blue max-md:hover:text-white"
                onClick={handelSearch}>
                <Search /> <div className="max-md:hidden">Search in the questions</div>
              </Button>

              {/* Search popup */}
              <CommandDialog open={open} onOpenChange={setOpen}>
                <DialogTitle className="sr-only">Search</DialogTitle>
                <DialogDescription className="sr-only">Search about questions</DialogDescription>

                <CommandInput placeholder="Type a question to search..." onChangeCapture={handleChange} />
                <CommandList>
                  <CommandEmpty>No results found.</CommandEmpty>
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
                  <Avatar className="bg-white flex justify-center items-center">
                    {/* using the name's first letter as a profile */}
                    <h1 className="text-main-blue font-bold text-lg ">{user.displayName?.charAt(0).toUpperCase()}</h1>
                  </Avatar>
                </DropdownMenuTrigger>
                <DropdownMenuContent>
                  <DropdownMenuLabel>My Account</DropdownMenuLabel>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem><a href="#">Profile</a></DropdownMenuItem>
                  <DropdownMenuItem className="text-red-600 hover:cursor-pointer" onClick={handleSignOut}>Sign Out</DropdownMenuItem>
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
}

export default NavBar;
