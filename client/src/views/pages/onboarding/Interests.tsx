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

import { useEffect, useState } from "react";

import { useGetTagsQuery } from "@/features/tags/tagsSlice";
import { Tag } from "@/features/tags/types";
import { useUpdateUserMutation } from "@/features/users/usersSlice";
import { useGetCurrentUserProfileQuery, useOnboardUserMutation } from "@/features/profiles/profilesSlice";
import toast from "react-hot-toast";
import { useNavigate } from "react-router";



function Interests() {
  const navigate = useNavigate();
  const { data: tags = [], isLoading } = useGetTagsQuery();
  const [onboardUser, onboardingResult] = useOnboardUserMutation();

  const [interests, setInterests] = useState<{ interest: string; interestStatus: boolean }[]>([]);

  const [searchInput, setSearchInput] = useState("");

  // Filtered & limited suggestions (excluding already selected)
  const filteredSuggestions = tags
    .filter(tag =>
      tag.englishName.toLowerCase().includes(searchInput.toLowerCase()) &&
      !interests.some(i => i.interest === tag.englishName)
    )
    .slice(0, 30); // Limit to 10

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
  };

  const handleSubmit = () => {
    if (searchInput.trim() !== "") {
      setInterests([...interests, { interest: searchInput, interestStatus: true }]);
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
    setInterests(prev => prev.filter((_, i) => i !== index));
  };

  const handleUpdateUser = async () => {
    const selectedTags = interests
      .filter(i => i.interestStatus)
      .map(i => i.interest.toLowerCase());

    const selectedTagIds = tags
      .filter(tag => selectedTags.includes(tag.englishName.toLowerCase()))
      .map(tag => tag.tagId);

    await onboardUser({
      tagIds: selectedTagIds,
    });
  }

  useEffect(() => {
    if (onboardingResult.isSuccess) {
      toast.success("Onboarding successful");
      navigate('/home');
    } else if (onboardingResult.isError) {
      toast.error(`Onboarding failed {onboardingResult.error}`);
      // Show error feedback
    }
  }, [onboardingResult.isSuccess, onboardingResult.isError, navigate]);


  return (
    <>
      <div className="flex justify-center items-center h-screen">
        <Command className="flex flex-col rounded-lg border shadow-md w-2/3 h-2/3 md:min-w-[450px]">
          <h1 className="text-center font-bold m-5 text-2xl text-main-blue">
            What are you good at?
          </h1>
          <h2 className="text-center font-bold m-2 text-1xl text-main-blue">
            Select topics to shape your personalized experience.
          </h2>
          <CommandSeparator />
          <CommandInput
            placeholder="Search about keyword..."
            value={searchInput}
            onChangeCapture={handleChange}
            onKeyDown={handleKeyDown}
          />
          <CommandList className="flex-grow overflow-y-auto">
            {/* Search Suggestions */}
            <div className="pb-36"> {/* Push up space to avoid overlap with fixed */}
              <CommandEmpty>No results found.</CommandEmpty>
              <CommandGroup heading="Suggestions">
                {isLoading ? (
                  <CommandItem disabled>Loading...</CommandItem>
                ) : (
                  filteredSuggestions.map((tag) => (
                    <CommandItem
                      key={tag.tagId}
                      onSelect={() => {
                        setInterests([...interests, { interest: `${tag.englishName} ${tag.arabicName}`, interestStatus: true }]);
                        setSearchInput("");
                      }}
                    >
                      {tag.englishName} {tag.arabicName}
                    </CommandItem>
                  ))
                )}
              </CommandGroup>
              <CommandSeparator />
            </div>

            {/* Fixed Chosen Interests Section */}
            <div className="fixed bottom-0 w-2/3 md:min-w-[450px] bg-white border-t border-gray-200 shadow-inner p-3 z-10">
              <CommandGroup heading="Chosen Key Words">
                <div className="flex flex-wrap gap-2 mb-3">
                  {interests.map((item, index) => (
                    <Toggle
                      key={index}
                      variant="outline"
                      pressed={item.interestStatus}
                      onPressedChange={() => toggleInterest(index)}
                      className="px-3 py-1 text-sm data-[state=on]:bg-main-blue data-[state=on]:text-white data-[state=on]:shadow-[0_0_15px_1px_rgba(15,23,42,0.3)]"
                    >
                      {item.interest}
                    </Toggle>
                  ))}
                </div>
              </CommandGroup>

              <Button
                className="w-full bg-emerald-600 hover:bg-emerald-700 text-white"
                disabled={interests.length < 5 || onboardingResult.isLoading}
                onClick={handleUpdateUser}
              >
                {onboardingResult.isLoading
                  ? "Saving..."
                  : interests.length >= 5
                    ? "Continue"
                    : `Add at least ${5 - interests.length} keywords to continue`}
              </Button>
            </div>
          </CommandList>
        </Command>

      </div>
    </>
  )
}

export default Interests;
