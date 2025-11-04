import { useNavigate } from "react-router-dom";
import { Button } from "@/views/components/ui/button";

interface AuthButtonsProps {
	onLoginClick?: () => void;
	onRegisterClick?: () => void;
	className?: string;
	variant?: "default" | "compact";
}

export const AuthButtons = ({
	onLoginClick,
	onRegisterClick,
	className = "",
	variant: _variant = "default",
}: AuthButtonsProps) => {
	const navigate = useNavigate();

	const handleLogin = () => {
		if (onLoginClick) {
			onLoginClick();
		} else {
			navigate("/login");
		}
	};

	const handleRegister = () => {
		if (onRegisterClick) {
			onRegisterClick();
		} else {
			navigate("/signup");
		}
	};

	return (
		<div className={`flex items-center gap-2 ${className}`}>
			<Button
				className="bg-transparent text-primary-foreground border-2 border-primary-foreground hover:bg-primary-foreground hover:text-primary rounded-full px-6"
				onClick={handleLogin}
			>
				Login
			</Button>
			<Button
				className="bg-primary-foreground text-primary hover:bg-primary-foreground/90 rounded-full px-6"
				onClick={handleRegister}
			>
				Register
			</Button>
		</div>
	);
};

// Individual button exports for more flexibility
export const LoginButton = ({
	onClick,
	className = "",
	variant: _variant = "outline",
}: {
	onClick?: () => void;
	className?: string;
	variant?: "outline" | "ghost";
}) => {
	const navigate = useNavigate();

	const handleClick = () => {
		if (onClick) {
			onClick();
		} else {
			navigate("/login");
		}
	};

	return (
		<Button
			className={`bg-transparent text-primary-foreground border-2 border-primary-foreground hover:bg-primary-foreground hover:text-primary rounded-full px-6 ${className}`}
			onClick={handleClick}
		>
			Login
		</Button>
	);
};

export const RegisterButton = ({
	onClick,
	className = "",
}: {
	onClick?: () => void;
	className?: string;
}) => {
	const navigate = useNavigate();

	const handleClick = () => {
		if (onClick) {
			onClick();
		} else {
			navigate("/signup");
		}
	};

	return (
		<Button
			className={`bg-primary-foreground text-primary hover:bg-primary-foreground/90 rounded-full px-6 ${className}`}
			onClick={handleClick}
		>
			Register
		</Button>
	);
};

// Learn More button with same styling as Login button
export const LearnMoreButton = ({
	onClick,
	className = "",
	to = "/AboutUs",
}: {
	onClick?: () => void;
	className?: string;
	to?: string;
}) => {
	const navigate = useNavigate();

	const handleClick = () => {
		if (onClick) {
			onClick();
		} else {
			navigate(to);
		}
	};

	return (
		<Button
			className={`bg-transparent text-primary-foreground border-2 border-primary-foreground hover:bg-primary-foreground hover:text-primary rounded-full px-6 ${className}`}
			onClick={handleClick}
		>
			Learn More
		</Button>
	);
};
