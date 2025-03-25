import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "../../components/custom/command"
import { Button } from "../../components/custom/button"
import { Toggle } from "../../components/custom/toggle"

import { useState } from "react";


function Interests(){

  const [interests, setInterests] = useState<{ interest: string; interestStatus: boolean }[]>([]);

  const [searchInput, setSearchInput] = useState("");

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
  };

  const handleSubmit = () => {
    if (searchInput.trim() !== "") {
      setInterests([...interests, { interest: searchInput, interestStatus: true}]); 
      console.log(interests)
      console.log("Submitted!");
      setSearchInput(""); // Reset input field
    }
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      handleSubmit();
    }
  };
  
  const toggleInterest = (index: number) => {
    setInterests((prevInterests) =>
      prevInterests.map((item, i) =>
        i === index ? { ...item, interestStatus: !item.interestStatus } : item
      )
    );
  };


  return (
    <>
      <div className="flex justify-center items-center h-screen">
        <Command className="flex flex-col rounded-lg border shadow-md w-2/3 h-2/3 md:min-w-[450px]">
          <h1 className="text-center font-bold m-5 text-2xl text-main-blue">
            What topics you have experience in?
          </h1>
          <CommandSeparator />
          <CommandInput 
            placeholder="Search about keyword..."
            value={searchInput}
            onChangeCapture={handleChange}
            onKeyDown={handleKeyDown}
          />
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
              {interests.map((item, index) => (
                <Toggle
                  key={index}
                  variant="outline"
                  pressed={item.interestStatus}
                  onPressedChange={() => toggleInterest(index)}
                  className="m-1 px-3 py-1 text-sm data-[state=on]:bg-main-blue data-[state=on]:text-white data-[state=on]:shadow-[0_0_15px_1px_rgba(15,23,42,0.3)]"
                >
                  {item.interest}
                </Toggle>
              ))}
            </CommandGroup>
          </CommandList>
          <Button 
            className="mt-auto mx-3 mb-3 bg-main-blue hover:bg-blue-950"
            disabled = {interests.length < 5}
          >
              {interests.length >= 5 ? "Continue" : `Add at least ${5 - interests.length} keywords to continue`}
          </Button>
        </Command>
        
      </div>
    </>
  )
}

export default Interests;