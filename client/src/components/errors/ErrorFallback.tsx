import React from "react";
import { AlertCircle, RefreshCw, Home } from "lucide-react";
import { useNavigate } from "react-router-dom";

interface ErrorFallbackProps {
	error?: Error;
	resetErrorBoundary?: () => void;
	componentName?: string;
}

const ErrorFallback: React.FC<ErrorFallbackProps> = ({
	error,
	resetErrorBoundary,
	componentName,
}) => {
	const navigate = useNavigate();

	// Determine if this is a chunk loading error
	const isChunkLoadError =
		error?.message?.includes("Loading chunk") ||
		error?.message?.includes("Failed to fetch dynamically imported module");

	const handleReload = () => {
		if (isChunkLoadError) {
			// For chunk errors, reload the page to get fresh chunks
			window.location.reload();
		} else if (resetErrorBoundary) {
			// For other errors, try to reset the error boundary
			resetErrorBoundary();
		}
	};

	const handleGoHome = () => {
		navigate("/home");
	};

	return (
		<div className="min-h-[400px] flex items-center justify-center p-4">
			<div className="max-w-md w-full bg-card rounded-lg shadow-lg p-6 text-center">
				{/* Error Icon */}
				<div className="flex justify-center mb-4">
					<div className="rounded-full bg-destructive/10 p-3">
						<AlertCircle className="h-8 w-8 text-destructive" />
					</div>
				</div>

				{/* Error Title */}
				<h2 className="text-xl font-semibold text-card-foreground mb-2">
					{isChunkLoadError
						? "Loading Error"
						: "Something went wrong"}
				</h2>

				{/* Error Description */}
				<p className="text-muted-foreground mb-6">
					{isChunkLoadError
						? "There was a problem loading this page. This might be due to a connection issue or an outdated version."
						: `Failed to load ${componentName || "this component"}. Please try again.`}
				</p>

				{/* Error Details (in development) */}
				{process.env.NODE_ENV === "development" && error && (
					<div className="mb-6 p-3 bg-muted rounded text-left">
						<p className="text-xs text-muted-foreground font-mono break-all">
							{error.message}
						</p>
					</div>
				)}

				{/* Action Buttons */}
				<div className="flex flex-col sm:flex-row gap-3 justify-center">
					<button
						onClick={handleReload}
						className="inline-flex items-center justify-center px-4 py-2 bg-primary hover:bg-primary/90 text-primary-foreground rounded-lg font-medium transition-colors duration-200"
					>
						<RefreshCw className="h-4 w-4 mr-2" />
						{isChunkLoadError ? "Reload Page" : "Try Again"}
					</button>

					<button
						onClick={handleGoHome}
						className="inline-flex items-center justify-center px-4 py-2 bg-muted hover:bg-accent text-muted-foreground rounded-lg font-medium transition-colors duration-200"
					>
						<Home className="h-4 w-4 mr-2" />
						Go to Home
					</button>
				</div>

				{/* Additional Help Text */}
				{isChunkLoadError && (
					<p className="mt-6 text-xs text-muted-foreground">
						If this problem persists, try clearing your browser
						cache or using a different browser.
					</p>
				)}
			</div>
		</div>
	);
};

export default ErrorFallback;
