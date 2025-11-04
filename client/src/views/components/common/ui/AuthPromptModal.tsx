import React from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/views/components/ui/button";
import {
	Dialog,
	DialogContent,
	DialogDescription,
	DialogFooter,
	DialogHeader,
	DialogTitle,
	DialogClose,
} from "@/views/components/ui/dialog";

interface AuthPromptModalProps {
	open: boolean;
	onOpenChange: (open: boolean) => void;
	title?: string;
	description?: string;
}

const AuthPromptModal: React.FC<AuthPromptModalProps> = ({
	open,
	onOpenChange,
	title = "Create an account to continue",
	description = "Tailor your experience and unlock all features by creating an account or logging in.",
}) => {
	const navigate = useNavigate();

	const handleNavigate = (path: string) => {
		onOpenChange(false);
		navigate(path);
	};

	return (
		<Dialog open={open} onOpenChange={onOpenChange}>
			<DialogContent className="sm:max-w-[425px] bg-card">
				<DialogHeader>
					<DialogTitle>{title}</DialogTitle>
					<DialogDescription>{description}</DialogDescription>
				</DialogHeader>
				<DialogFooter className="flex flex-col sm:flex-row sm:justify-between gap-2">
					<div className="flex gap-2 justify-end">
						<Button
							type="button"
							variant="outline"
							onClick={() => handleNavigate("/login")}
							className="rounded-full"
						>
							Log In
						</Button>
						<Button
							type="button"
							onClick={() => handleNavigate("/signup")}
							className="bg-primary hover:bg-primary/90 rounded-full"
						>
							Sign Up
						</Button>
					</div>
					<DialogClose asChild>
						<Button
							type="button"
							variant="ghost"
							className="rounded-full"
						>
							Skip
						</Button>
					</DialogClose>
				</DialogFooter>
			</DialogContent>
		</Dialog>
	);
};

export default AuthPromptModal;
