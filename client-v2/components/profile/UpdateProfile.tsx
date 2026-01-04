"use client";

import { useState, useEffect } from "react";
import {
	useGetCurrentUserProfileQuery,
	useUpdateProfileMutation,
	useCheckUsernameAvailableMutation,
} from "@/features/profiles/profilesSlice";
import type { UpdateProfileRequest } from "@/features/profiles/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Loader2, Check, X } from "lucide-react";
import { cn } from "@/lib/utils";
import toast from "react-hot-toast";

interface UpdateProfileProps {
	onClose: () => void;
}

/**
 * Dialog content for updating user profile.
 */
export function UpdateProfile({ onClose }: UpdateProfileProps) {
	const { data: profile, isLoading: isProfileLoading } =
		useGetCurrentUserProfileQuery(undefined);
	const [updateProfile, { isLoading: isUpdating }] =
		useUpdateProfileMutation();
	const [
		checkUsername,
		{ data: isUsernameAvailable, isLoading: isCheckingUsername },
	] = useCheckUsernameAvailableMutation();

	// Form state
	const [firstName, setFirstName] = useState("");
	const [lastName, setLastName] = useState("");
	const [username, setUsername] = useState("");
	const [jobTitle, setJobTitle] = useState("");
	const [bio, setBio] = useState("");
	const [skills, setSkills] = useState<string[]>([]);
	const [skillInput, setSkillInput] = useState("");

	// Validation state
	const [errors, setErrors] = useState<Record<string, string>>({});
	const [usernameChecked, setUsernameChecked] = useState(false);
	const [initialized, setInitialized] = useState(false);

	// Initialize form with profile data
	useEffect(() => {
		if (profile && !initialized) {
			setFirstName(profile.firstName || "");
			setLastName(profile.lastName || "");
			setUsername(profile.username || "");
			setJobTitle(profile.jobTitle || "");
			setBio(profile.bio || "");
			setSkills(profile.skills || []);
			setInitialized(true);
		}
	}, [profile, initialized]);

	// Check username availability when it changes
	useEffect(() => {
		const timeoutId = setTimeout(() => {
			if (
				username &&
				username !== profile?.username &&
				username.length >= 3
			) {
				checkUsername(username);
				setUsernameChecked(true);
			} else {
				setUsernameChecked(false);
			}
		}, 500);

		return () => {
			clearTimeout(timeoutId);
		};
	}, [username, profile?.username, checkUsername]);

	const validateForm = (): boolean => {
		const newErrors: Record<string, string> = {};

		if (!firstName.trim()) {
			newErrors.firstName = "First name is required";
		}
		if (!lastName.trim()) {
			newErrors.lastName = "Last name is required";
		}
		if (!username.trim()) {
			newErrors.username = "Username is required";
		} else if (username.length < 3) {
			newErrors.username = "Username must be at least 3 characters";
		} else if (!/^[a-zA-Z0-9_.-]+$/.test(username)) {
			newErrors.username =
				"Username can only contain letters, numbers, underscores, dots, and hyphens";
		}
		if (usernameChecked && isUsernameAvailable === false) {
			newErrors.username = "Username is already taken";
		}

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!validateForm()) return;

		try {
			const updateData: UpdateProfileRequest = {
				firstName: firstName.trim(),
				lastName: lastName.trim(),
				username: username.trim(),
				jobTitle: jobTitle.trim() || undefined,
				bio: bio.trim() || undefined,
				skills,
			};

			await updateProfile(updateData).unwrap();
			toast.success("Profile updated successfully");
			onClose();
		} catch (error) {
			console.error("Failed to update profile:", error);
			toast.error("Failed to update profile. Please try again.");
		}
	};

	/**
	 * Adds a skill to the skills list if valid and not duplicate.
	 */
	const addSkill = () => {
		const trimmedSkill = skillInput.trim();
		if (trimmedSkill && !skills.includes(trimmedSkill)) {
			setSkills([...skills, trimmedSkill]);
			setSkillInput("");
		} else if (skills.includes(trimmedSkill)) {
			toast.error("This skill is already in your list");
		}
	};

	const handleAddSkill = (e: React.KeyboardEvent<HTMLInputElement>) => {
		if (e.key === "Enter") {
			e.preventDefault();
			addSkill();
		}
	};

	const handleRemoveSkill = (skillToRemove: string) => {
		setSkills(skills.filter((skill) => skill !== skillToRemove));
	};

	const getUsernameIcon = () => {
		if (isCheckingUsername) {
			return (
				<Loader2 className="h-5 w-5 animate-spin text-muted-foreground" />
			);
		}
		if (username === profile?.username) {
			return <Check className="h-5 w-5 text-green-600" />;
		}
		if (usernameChecked && isUsernameAvailable === true) {
			return <Check className="h-5 w-5 text-green-600" />;
		}
		if (usernameChecked && isUsernameAvailable === false) {
			return <X className="h-5 w-5 text-destructive" />;
		}
		return null;
	};

	if (isProfileLoading) {
		return (
			<div className="flex justify-center items-center h-64">
				<Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
			</div>
		);
	}

	const onFormSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		void handleSubmit(e);
	};

	return (
		<form onSubmit={onFormSubmit} className="space-y-6">
			<div className="text-center mb-4">
				<h2 className="text-xl font-semibold">Update Your Profile</h2>
				<p className="text-sm text-muted-foreground">
					Update your profile information below.
				</p>
			</div>

			{/* Personal Information */}
			<div className="grid grid-cols-2 gap-4">
				<div className="space-y-2">
					<Label
						htmlFor="firstName"
						className={errors.firstName ? "text-destructive" : ""}
					>
						First Name
					</Label>
					<Input
						id="firstName"
						value={firstName}
						onChange={(e) => { setFirstName(e.target.value); }}
						className={errors.firstName ? "border-destructive" : ""}
						dir="auto"
					/>
					{errors.firstName && (
						<p className="text-destructive text-xs">
							{errors.firstName}
						</p>
					)}
				</div>
				<div className="space-y-2">
					<Label
						htmlFor="lastName"
						className={errors.lastName ? "text-destructive" : ""}
					>
						Last Name
					</Label>
					<Input
						id="lastName"
						value={lastName}
						onChange={(e) => { setLastName(e.target.value); }}
						className={errors.lastName ? "border-destructive" : ""}
						dir="auto"
					/>
					{errors.lastName && (
						<p className="text-destructive text-xs">
							{errors.lastName}
						</p>
					)}
				</div>
			</div>

			{/* Username */}
			<div className="space-y-2">
				<Label
					htmlFor="username"
					className={errors.username ? "text-destructive" : ""}
				>
					Username
				</Label>
				<div className="relative">
					<Input
						id="username"
						value={username}
						onChange={(e) => { setUsername(e.target.value); }}
						className={cn(
							"pr-10",
							errors.username ? "border-destructive" : "",
							usernameChecked && isUsernameAvailable === true
								? "border-green-600"
								: ""
						)}
						dir="auto"
					/>
					<div className="absolute right-3 top-1/2 -translate-y-1/2">
						{getUsernameIcon()}
					</div>
				</div>
				{errors.username && (
					<p className="text-destructive text-xs">
						{errors.username}
					</p>
				)}
			</div>

			{/* Job Title */}
			<div className="space-y-2">
				<Label htmlFor="jobTitle">Job Title</Label>
				<Input
					id="jobTitle"
					value={jobTitle}
					onChange={(e) => { setJobTitle(e.target.value); }}
					placeholder="e.g., Software Engineer"
					dir="auto"
				/>
			</div>

			{/* Bio */}
			<div className="space-y-2">
				<Label htmlFor="bio">Bio</Label>
				<Textarea
					id="bio"
					value={bio}
					onChange={(e) => { setBio(e.target.value); }}
					placeholder="Tell us about yourself"
					rows={4}
					dir="auto"
				/>
			</div>

			{/* Skills */}
			<div className="space-y-2">
				<Label htmlFor="skills">Skills</Label>
				<div className="flex gap-2">
					<Input
						id="skillInput"
						value={skillInput}
						onChange={(e) => { setSkillInput(e.target.value); }}
						onKeyDown={handleAddSkill}
						placeholder="Type a skill and press Enter"
						dir="auto"
					/>
					<Button
						type="button"
						size="sm"
						onClick={addSkill}
					>
						Add
					</Button>
				</div>
				<div className="flex flex-wrap gap-2 mt-2">
					{skills.length === 0 && (
						<p className="text-muted-foreground text-sm">
							No skills added yet
						</p>
					)}
					{skills.map((skill, index) => (
						<button
							key={index}
							type="button"
							className="bg-primary/10 text-primary px-3 py-1 rounded-full text-sm flex items-center gap-1 cursor-pointer hover:bg-primary/20 transition-colors focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2"
							onClick={() => { handleRemoveSkill(skill); }}
							onKeyDown={(e) => {
								if (e.key === "Enter" || e.key === " ") {
									e.preventDefault();
									handleRemoveSkill(skill);
								}
							}}
							aria-label={`Remove ${skill} skill`}
						>
							{skill}
							<X className="h-3 w-3" aria-hidden="true" />
						</button>
					))}
				</div>
			</div>

			{/* Actions */}
			<div className="flex justify-end gap-3 pt-4">
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
			</div>
		</form>
	);
}
