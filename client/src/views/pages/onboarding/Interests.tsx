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
import { useOnboardUserMutation } from "@/features/profiles/profilesSlice";
import toast from "react-hot-toast";
import { useNavigate } from "react-router";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

function Interests() {
  const navigate = useNavigate();
  const { data: tags = [], isLoading } = useGetTagsQuery();
  const [onboardUser, onboardingResult] = useOnboardUserMutation();

  // Replace current interests state with simple string array
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [searchInput, setSearchInput] = useState("");

  const [,,,, userProfile] = useIsUserLoggedIn();

  // Updated filteredSuggestions logic
  const filteredSuggestions = tags
    .filter(tag => {      
      // Check if tag name is already selected
      const normalizedSearchInput = searchInput.trim().toLowerCase();
      const isAlreadySelected =
        (tag.englishName && selectedTags.includes(tag.englishName.toLowerCase())) ||
        (tag.arabicName && selectedTags.includes(tag.arabicName?.toLowerCase()));
      
      // If already selected, exclude from results
      if (isAlreadySelected) return false;
      
      // Search in all relevant fields
      return (
        (tag.englishName && tag.englishName.toLowerCase().includes(normalizedSearchInput)) ||
        (tag.arabicName&& tag.arabicName.toLowerCase().includes(normalizedSearchInput)) ||
        (tag.description && tag.description?.toLowerCase().includes(normalizedSearchInput))
      );
    })
    .slice(0, 50); // Limit to 50 suggestions

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchInput(event.target.value);
  };
  
  const addTag = (tagName: string) => {
    const normalizedTagName = tagName?.toLowerCase();
    setSelectedTags([...selectedTags, normalizedTagName]);
    setSearchInput(""); // Clear search after adding
  };
  
  const removeTag = (tagToRemove: string) => {
    setSelectedTags(selectedTags.filter(tag => tag !== tagToRemove?.toLowerCase()));
  };

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      if (searchInput.trim() !== "") {
        // Check if tag already exists in tags list
        const matchedTag = tags.find(tag => 
          (tag.englishName && tag.englishName.toLowerCase() === searchInput.trim().toLowerCase()) ||
          (tag.arabicName && tag.arabicName.toLowerCase() === searchInput.trim().toLowerCase())
        );
        // if it exists then add the existing one.
        if (matchedTag) {
          addTag(matchedTag.englishName);
        } else { // else add the search input.
          addTag(searchInput.trim());
        }
      }
    }
  };

  const handleUpdateUser = async () => {
    if (selectedTags.length >= 5) {
      console.log(`Selected Tags to be sumbitted to the user => [${selectedTags}]`)
      await onboardUser({
        tags: selectedTags.map(tag => tag.toLowerCase()),
      });
    } else {
      toast.error("Please select at least 5 tags before continuing");
    }
  };

  useEffect(() => {
    if (userProfile) {
      if (userProfile.isOnboarded) {
        navigate('/home');
      }
    }
  }, [userProfile, navigate]);

  useEffect(() => {
    if (onboardingResult.isSuccess) {
      toast.success("Onboarding successful");
      navigate('/home');
    } else if (onboardingResult.isError) {
      toast.error(`Onboarding failed ${onboardingResult.isError}`);
      // Show error feedback
    }
  }, [onboardingResult.isSuccess, onboardingResult.isError, navigate]);
  
  useEffect(() => {
    setSearchInput(searchInput);
    console.log(`Search Input = ${searchInput}`);
    
    setSelectedTags(selectedTags);
    console.log(`Selected Tags = [${selectedTags}]`);
    
  }, [searchInput, selectedTags]);
  
  return (
    <div className="min-h-screen w-full flex justify-center items-center bg-gradient-to-b from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800 p-4 transition-colors duration-200">
      <div className="w-full max-w-3xl">
        <Command className="flex flex-col rounded-xl border shadow-lg dark:shadow-gray-800/20 backdrop-blur-sm w-full h-[80vh] bg-white dark:bg-gray-900 border-gray-200 dark:border-gray-700 transition-all duration-200 overflow-hidden">
          <div className="space-y-2 px-6 py-8">
            <h1 className="text-center font-bold text-3xl bg-gradient-to-r from-blue-600 to-indigo-600 dark:from-blue-400 dark:to-indigo-400 bg-clip-text text-transparent">
              What are you good at?
            </h1>
            <h2 className="text-center text-lg text-gray-600 dark:text-gray-300">
              Select topics to shape your personalized experience.
            </h2>
          </div>
          
          <CommandSeparator className="dark:bg-gray-800" />
          
          <div className="relative py-3 border-b-0">
            <CommandInput
              placeholder="Search keywords..."
              value={searchInput}
              onChangeCapture={handleChange}
              onKeyDown={handleKeyDown}
              className="w-full px-4 bg-gray-100 border-0 dark:border-0 dark:bg-gray-800 text-gray-900 dark:text-gray-100 placeholder:text-gray-500 dark:placeholder:text-gray-400"
            />
          </div>
          
          <CommandList className="flex-grow overflow-y-auto px-2 scrollbar-thin scrollbar-thumb-gray-300 dark:scrollbar-thumb-gray-700">
            {/* Search Suggestions */}
            <div className="pb-36"> {/* Push up space to avoid overlap with fixed */}
              <CommandEmpty className="py-6 text-center text-gray-500 dark:text-gray-400">
                <div>No matching tags found.</div>
                <div className="mt-2">Press Enter to add "{searchInput.trim()}" as a new tag.</div>
              </CommandEmpty>
              
              <CommandGroup heading="Suggestions" className="text-gray-600 dark:text-gray-300">
                {isLoading ? (
                  <div className="flex justify-center py-4">
                    <div className="animate-pulse text-gray-500 dark:text-gray-400">Loading suggestions...</div>
                  </div>
                ) : filteredSuggestions.length > 0 ? (
                  filteredSuggestions.map((tag) => (
                    <CommandItem
                      key={tag.tagId}
                      onSelect={() => addTag(tag.englishName)}
                      className="rounded-md px-3 py-2.5 my-0.5 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors duration-150"
                    >
                      {tag.englishName} {tag.arabicName}
                    </CommandItem>
                  ))
                ) : searchInput.trim() !== "" ? (
                  <CommandItem 
                    onSelect={() => addTag(searchInput.trim())}
                    className="rounded-md px-3 py-2.5 my-0.5 cursor-pointer hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors duration-150 italic"
                  >
                    Add new tag: "{searchInput.trim()}"
                  </CommandItem>
                ) : null}
              </CommandGroup>
              <CommandSeparator className="my-2 dark:bg-gray-800" />
            </div>

            {/* Fixed Chosen Interests Section */}
            <div className="fixed bottom-0 max-w-3xl w-full bg-white dark:bg-gray-900 border-t border-gray-200 dark:border-gray-700 shadow-lg dark:shadow-gray-800/30 p-5 z-10 rounded-b-xl transition-all duration-200">
              <CommandGroup heading="Selected Keywords" className="text-gray-600 dark:text-gray-300 mb-3">
                <div className="flex flex-wrap gap-2 mb-4">
                  {selectedTags.length === 0 ? (
                    <p className="text-sm text-gray-500 dark:text-gray-400 italic">No keywords selected yet. Select at least 5 to continue.</p>
                  ) : (
                    selectedTags.map((tag, index) => (
                      <Toggle
                        key={index}
                        variant="outline"
                        pressed={true}
                        onPressedChange={() => removeTag(tag)}
                        className="px-4 py-1.5 text-sm bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700 border border-gray-200 dark:border-gray-700 rounded-full transition-all 
                        data-[state=on]:bg-gradient-to-r data-[state=on]:from-blue-600 data-[state=on]:to-indigo-600 dark:data-[state=on]:from-blue-500 dark:data-[state=on]:to-indigo-500
                        data-[state=on]:text-white data-[state=on]:border-transparent data-[state=on]:shadow-md"
                      >
                        {tag}
                      </Toggle>
                    ))
                  )}
                </div>
              </CommandGroup>

              <Button
                className={`w-full py-3 rounded-lg transition-all duration-200 ${
                  selectedTags.length < 5 ? 
                  'bg-gray-400 dark:bg-gray-700 text-white cursor-not-allowed' : 
                  'bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600 text-white shadow-md hover:shadow-lg'
                }`}
                disabled={selectedTags.length < 5 || onboardingResult.isLoading}
                onClick={handleUpdateUser}
              >
                {onboardingResult.isLoading
                  ? <span className="flex items-center justify-center"><svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle><path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>Saving...</span>
                  : selectedTags.length >= 5
                    ? "Continue"
                    : `Add ${5 - selectedTags.length} more keyword${selectedTags.length === 4 ? '' : 's'} to continue`}
              </Button>
            </div>
          </CommandList>
        </Command>
      </div>
    </div>
  )
}

export default Interests;
