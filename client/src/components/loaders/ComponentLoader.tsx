import React from "react";

interface ComponentLoaderProps {
	size?: "sm" | "md" | "lg";
	text?: string;
	className?: string;
}

const ComponentLoader: React.FC<ComponentLoaderProps> = ({
	size = "md",
	text = "Loading...",
	className = "",
}) => {
	const sizeClasses = {
		sm: "h-4 w-4 border",
		md: "h-8 w-8 border-2",
		lg: "h-12 w-12 border-2",
	};

	const textSizeClasses = {
		sm: "text-xs",
		md: "text-sm",
		lg: "text-base",
	};

	return (
		<div
			className={`flex flex-col items-center justify-center p-8 ${className}`}
		>
			<div className="inline-flex items-center justify-center">
				<div
					className={`animate-spin rounded-full ${sizeClasses[size]} border-b-primary border-border`}
				></div>
			</div>
			{text && (
				<p
					className={`mt-3 text-muted-foreground ${textSizeClasses[size]}`}
				>
					{text}
				</p>
			)}
		</div>
	);
};

export default ComponentLoader;
