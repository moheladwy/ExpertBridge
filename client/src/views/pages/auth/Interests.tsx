import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
  CommandShortcut,
} from "../../components/custom/command"

import { Button } from "../../components/custom/button"
  
import { DialogTitle, DialogDescription } from "@radix-ui/react-dialog"


function Interests(){

  return (
    <>
      <div className="flex justify-center items-center h-screen">
        <Command className="flex flex-col rounded-lg border shadow-md w-2/3 h-2/3 md:min-w-[450px]">
          <h1 className="text-center font-bold m-5 text-2xl text-main-blue">
            What topics you have experience in?
          </h1>
          <CommandSeparator />
          <CommandInput placeholder="Search about key word..." />
          <CommandList className="flex-grow">
            {/* the results of the search should put in here */}
            <CommandEmpty>No results found.</CommandEmpty>
            <CommandGroup heading="Suggestions">
              <CommandItem>
                <span>Web Development</span>
              </CommandItem>
              <CommandItem>
                <span>Plumber</span>
              </CommandItem>
            </CommandGroup>
            <CommandSeparator />

            {/* choosen key words */}
            <CommandGroup heading="Choosen Key Words">
            
            </CommandGroup>
          </CommandList>
          <Button className="mt-auto mx-3 mb-3 bg-main-blue hover:bg-blue-950" >
            Continue
          </Button>
        </Command>
        
      </div>
    </>
  )
}

export default Interests;