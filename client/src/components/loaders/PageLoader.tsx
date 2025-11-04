import React from "react";

const PageLoader: React.FC = () => {
	return (
		<div className="min-h-screen flex items-center justify-center bg-background">
			<div className="text-center">
				{/* Loading spinner */}
				<div className="inline-flex items-center justify-center">
					<div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
				</div>

				{/* Loading text */}
				<p className="mt-4 text-muted-foreground text-sm font-medium">
					Loading...
				</p>

				{/* Progress dots animation */}
				<div className="mt-2 flex justify-center space-x-1">
					<div
						className="w-2 h-2 bg-primary rounded-full animate-bounce"
						style={{ animationDelay: "0ms" }}
					></div>
					<div
						className="w-2 h-2 bg-primary rounded-full animate-bounce"
						style={{ animationDelay: "150ms" }}
					></div>
					<div
						className="w-2 h-2 bg-primary rounded-full animate-bounce"
						style={{ animationDelay: "300ms" }}
					></div>
				</div>
			</div>
		</div>
	);
};

export default PageLoader;
