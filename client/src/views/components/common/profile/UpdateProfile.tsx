import { useState, useEffect } from "react";
import {
  useGetCurrentUserProfileQuery,
  useUpdateProfileMutation,
  useIsUsernameAvailableMutation,
} from "@/features/profiles/profilesSlice";
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  CardFooter,
} from "@/views/components/ui/card";
import { Button } from "@/views/components/ui/button";
import { Input } from "@/views/components/ui/input";
import { Label } from "@/views/components/ui/label";
import { Textarea } from "@/views/components/ui/textarea";
import { Loader2, Check, X } from "lucide-react";
import toast from "react-hot-toast";
import { UpdateProfileRequest } from "@/features/profiles/types";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

// Zod schema for form validation
const profileSchema = z.object({
  firstName: z
    .string()
    .min(1, "First name is required")
    .max(256, "First name must be less than 256 characters"),
  lastName: z
    .string()
    .min(1, "Last name is required")
    .max(256, "Last name must be less than 256 characters"),
  jobTitle: z
    .string()
    .min(1, "Job title is required")
    .max(256, "Job title must be less than 256 characters"),
  bio: z
    .string()
    .min(1, "Bio is required")
    .max(5000, "Bio must be less than 5000 characters"),
  username: z
    .string()
    .min(3, "Username must be at least 3 characters")
    .max(256, "Username must be less than 256 characters")
    .regex(
      /^[a-zA-Z0-9_.-]+$/,
      "Username can only contain letters, numbers, and underscores",
    ),
  skills: z
    .array(
      z
        .string()
        .min(1, "Skill is required")
        .max(256, "Skill must be less than 256 characters"),
    )
    .min(0),
});

type ProfileFormData = z.infer<typeof profileSchema>;

interface UpdateProfileProps {
  onClose: () => void;
}

const UpdateProfile = ({ onClose }: UpdateProfileProps) => {
  const { data: profile, isLoading } = useGetCurrentUserProfileQuery();
  const [
    updateProfile,
    { isSuccess, isError: updateProfileError, isLoading: isUpdating },
  ] = useUpdateProfileMutation();
  const [requestData, setRequestData] = useState<UpdateProfileRequest>();
  const [username, setUsername] = useState(profile?.username || "");
  const [skillInput, setSkillInput] = useState("");
  const [skills, setSkills] = useState<string[]>([]);
  const [
    checkAvailability,
    {
      data: isUsernameAvailable,
      isSuccess: isUsernameAvailableSuccess,
      isError: isUsernameAvailableError,
      isLoading: isCheckingUsername,
    },
  ] = useIsUsernameAvailableMutation();

  // Initialize form with react-hook-form and zod validation
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    setError,
    clearErrors,
  } = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      firstName: profile?.firstName || "",
      lastName: profile?.lastName || "",
      jobTitle: profile?.jobTitle || "",
      bio: profile?.bio || "",
      username: profile?.username || "",
      skills: profile?.skills || [],
    },
    mode: "onChange",
  });

  // Update form values when profile data is loaded
  useEffect(() => {
    if (profile) {
      setValue("firstName", profile.firstName || "");
      setValue("lastName", profile.lastName || "");
      setValue("jobTitle", profile.jobTitle || "");
      setValue("bio", profile.bio || "");
      setValue("username", profile.username || "");
      setUsername(profile.username || "");
      setSkills(profile.skills || []);
      setValue("skills", profile.skills || []);
    }
  }, [profile, setValue]);

  // Handle username availability check results
  useEffect(() => {
    if (!requestData) return; // Only proceed if we have actual request data from form submission

    if (isUsernameAvailable === true || username === profile?.username) {
      setValue("username", username, { shouldValidate: true });
      clearErrors("username");
      updateProfile(requestData);
    }
    if (isUsernameAvailable === false) {
      setError("username", {
        type: "custom",
        message: "Username is already taken for someone else",
      });
    }
    if (isUsernameAvailableError) {
      setError("username", {
        type: "custom",
        message: "Error checking username availability",
      });
    }
  }, [
    isUsernameAvailable,
    isUsernameAvailableError,
    username,
    profile?.username,
    requestData,
    setValue,
    clearErrors,
    setError,
    updateProfile,
  ]);

  useEffect(() => {
    if (isSuccess) {
      toast.success("Profile updated successfully");
      onClose();
    }
    if (updateProfileError) {
      toast.error("Failed to update profile. Please try again.");
    }
  }, [isSuccess, updateProfileError, onClose]);

  const onSubmit = async (data: ProfileFormData) => {
    try {
      const updateData: UpdateProfileRequest = {
        firstName: data.firstName,
        lastName: data.lastName,
        jobTitle: data.jobTitle,
        bio: data.bio || "",
        username: data.username,
        skills: skills || [], // Ensure skills is never undefined
      };
      setRequestData(updateData);
      checkAvailability(data.username);
    } catch (error) {
      console.error("username is not available:", error);
      toast.error("Username is not available. Please try again.");
    }
  };

  const handleAddSkill = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && skillInput.trim()) {
      e.preventDefault();
      const trimmedSkill = skillInput.trim();
      if (!skills.includes(trimmedSkill)) {
        const newSkills = [...skills, trimmedSkill];
        setSkills(newSkills);
        setValue("skills", newSkills);
        toast.success(`Added skill: ${trimmedSkill}`);
      } else {
        toast.error("This skill is already in your list");
      }
      setSkillInput("");
    }
  };

  const handleRemoveSkill = (skillToRemove: string) => {
    const newSkills = skills.filter((skill) => skill !== skillToRemove);
    setSkills(newSkills);
    setValue("skills", newSkills);
  };

  // Determine username input style based on availability
  const getUsernameInputClass = () => {
    if (username === profile?.username) return "";
    if (isUsernameAvailable === true)
      return "border-green-500 focus-visible:ring-green-500";
    if (isUsernameAvailable === false)
      return "border-red-500 focus-visible:ring-red-500";
    return "";
  };

  if (isLoading) {
    return (
      <Card className="w-full max-w-2xl mx-auto">
        <CardContent className="pt-6 flex justify-center items-center h-64">
          <Loader2 className="h-8 w-8 animate-spin text-gray-500" />
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="w-full max-w-2xl mx-auto dark:bg-gray-900 dark:text-white dark:border-gray-700">
      <CardHeader>
        <CardTitle className="text-xl text-center">
          Update Your Profile
        </CardTitle>
      </CardHeader>
      <form onSubmit={handleSubmit(onSubmit)}>
        <CardContent className="space-y-4">
          {/* Personal Information */}
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label
                htmlFor="firstName"
                className={errors.firstName ? "text-red-500" : ""}
              >
                First Name
              </Label>
              <Input
                id="firstName"
                {...register("firstName")}
                className={
                  errors.firstName
                    ? "border-red-500 focus-visible:ring-red-500"
                    : ""
                }
                dir="auto"
              />
              {errors.firstName && (
                <p className="text-red-500 text-xs mt-1">
                  {errors.firstName.message}
                </p>
              )}
            </div>
            <div className="space-y-2">
              <Label
                htmlFor="lastName"
                className={errors.lastName ? "text-red-500" : ""}
              >
                Last Name
              </Label>
              <Input
                id="lastName"
                {...register("lastName")}
                className={
                  errors.lastName
                    ? "border-red-500 focus-visible:ring-red-500"
                    : ""
                }
                dir="auto"
              />
              {errors.lastName && (
                <p className="text-red-500 text-xs mt-1">
                  {errors.lastName.message}
                </p>
              )}
            </div>
          </div>

          {/* Username */}
          <div className="space-y-2">
            <Label
              htmlFor="username"
              className={errors.username ? "text-red-500" : ""}
            >
              Username
            </Label>
            <div className="flex gap-2">
              <div className="relative flex-1">
                <Input
                  id="username"
                  {...register("username")}
                  dir="auto"
                  className={`pr-10 ${getUsernameInputClass()} ${errors.username ? "border-red-500 focus-visible:ring-red-500" : ""}`}
                />
                {isCheckingUsername ? (
                  <Loader2 className="absolute right-3 top-2.5 h-5 w-5 animate-spin text-gray-400" />
                ) : username === profile?.username ? (
                  <Check className="absolute right-3 top-2.5 h-5 w-5 text-green-500" />
                ) : isUsernameAvailable === true &&
                  username !== profile?.username ? (
                  <Check className="absolute right-3 top-2.5 h-5 w-5 text-green-500" />
                ) : isUsernameAvailable === false ? (
                  <X className="absolute right-3 top-2.5 h-5 w-5 text-red-500" />
                ) : null}
              </div>
            </div>
            {errors.username && (
              <p className="text-red-500 text-xs mt-1">
                {errors.username.message}
              </p>
            )}
            {username === profile?.username && (
              <Check className="absolute right-3 top-2.5 h-5 w-5 text-green-500" />
            )}
            {isUsernameAvailable === false &&
              username !== profile?.username &&
              !errors.username && (
                <p className="text-red-500 text-xs mt-1">
                  Username is already taken
                </p>
              )}
          </div>

          {/* Job Title */}
          <div className="space-y-2">
            <Label
              htmlFor="jobTitle"
              className={errors.jobTitle ? "text-red-500" : ""}
            >
              Job Title
            </Label>
            <Input
              id="jobTitle"
              {...register("jobTitle")}
              className={
                errors.jobTitle
                  ? "border-red-500 focus-visible:ring-red-500"
                  : ""
              }
              dir="auto"
            />
            {errors.jobTitle && (
              <p className="text-red-500 text-xs mt-1">
                {errors.jobTitle.message}
              </p>
            )}
          </div>

          {/* Bio */}
          <div className="space-y-2">
            <Label htmlFor="bio" className={errors.bio ? "text-red-500" : ""}>
              Bio
            </Label>
            <Textarea
              id="bio"
              {...register("bio")}
              placeholder="Tell us about yourself"
              rows={4}
              className={
                errors.bio ? "border-red-500 focus-visible:ring-red-500" : ""
              }
              dir="auto"
            />
            {errors.bio && (
              <p className="text-red-500 text-xs mt-1">{errors.bio.message}</p>
            )}
          </div>

          {/* Skills */}
          <div className="space-y-2">
            <Label htmlFor="skills">Skills</Label>
            <div className="flex flex-col gap-2">
              <div className="flex gap-2">
                <Input
                  id="skillInput"
                  value={skillInput}
                  onChange={(e) => setSkillInput(e.target.value)}
                  onKeyDown={handleAddSkill}
                  placeholder="Type a skill and press Enter"
                  dir="auto"
                />
                <Button
                  type="button"
                  size="sm"
                  onClick={() => {
                    if (skillInput.trim()) {
                      handleAddSkill({
                        key: "Enter",
                        preventDefault: () => {},
                      } as React.KeyboardEvent<HTMLInputElement>);
                    }
                  }}
                >
                  Add
                </Button>
              </div>
              <div className="flex flex-wrap gap-2 mt-2">
                {skills.length === 0 && (
                  <p className="text-gray-500 text-sm">No skills added yet</p>
                )}
                {skills.map((skill, index) => (
                  <span
                    key={index}
                    className="bg-primary/10 text-primary px-3 py-1 rounded-full text-sm flex items-center gap-1 cursor-pointer hover:bg-primary/20 transition-colors"
                    onClick={() => handleRemoveSkill(skill)}
                    title="Click to remove"
                  >
                    {skill}
                    <X className="h-3 w-3" />
                  </span>
                ))}
              </div>
            </div>
          </div>
        </CardContent>

        <CardFooter className="flex justify-evenly space-x-2">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isUpdating}
          >
            Cancel
          </Button>
          <Button type="submit" disabled={isUpdating}>
            {isUpdating ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Saving...
              </>
            ) : (
              "Save Changes"
            )}
          </Button>
        </CardFooter>
      </form>
    </Card>
  );
};

export default UpdateProfile;
