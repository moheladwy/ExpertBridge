"use client";

import { Component, type ErrorInfo, type ReactNode } from "react";
import { AlertTriangle, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";

interface ErrorBoundaryProps {
	children: ReactNode;
	fallback?: ReactNode;
}

interface ErrorBoundaryState {
	hasError: boolean;
	error: Error | null;
}

/**
 * Error boundary component to catch and handle runtime errors gracefully.
 * Wraps child components and displays a fallback UI when errors occur.
 *
 * @example
 * ```tsx
 * <ErrorBoundary>
 *   <MyComponent />
 * </ErrorBoundary>
 *
 * // With custom fallback
 * <ErrorBoundary fallback={<CustomErrorUI />}>
 *   <MyComponent />
 * </ErrorBoundary>
 * ```
 */
export class ErrorBoundary extends Component<
	ErrorBoundaryProps,
	ErrorBoundaryState
> {
	constructor(props: ErrorBoundaryProps) {
		super(props);
		this.state = { hasError: false, error: null };
	}

	static getDerivedStateFromError(error: Error): ErrorBoundaryState {
		return { hasError: true, error };
	}

	componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
		// Log error to console in development, could integrate with error reporting service
		console.error("ErrorBoundary caught an error:", error, errorInfo);
	}

	handleReset = (): void => {
		this.setState({ hasError: false, error: null });
	};

	render(): ReactNode {
		if (this.state.hasError) {
			// Return custom fallback if provided
			if (this.props.fallback) {
				return this.props.fallback;
			}

			// Default error UI
			return (
				<div className="flex min-h-100 items-center justify-center p-4">
					<Card className="w-full max-w-md">
						<CardHeader className="text-center">
							<div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-destructive/10">
								<AlertTriangle className="h-6 w-6 text-destructive" />
							</div>
							<CardTitle>Something went wrong</CardTitle>
							<CardDescription>
								An unexpected error occurred while rendering
								this component.
							</CardDescription>
						</CardHeader>
						<CardContent>
							{this.state.error && (
								<div className="rounded-md bg-muted p-3">
									<p className="text-sm font-mono text-muted-foreground break-all">
										{this.state.error.message}
									</p>
								</div>
							)}
						</CardContent>
						<CardFooter className="flex justify-center">
							<Button
								onClick={this.handleReset}
								variant="outline"
							>
								<RefreshCw className="mr-2 h-4 w-4" />
								Try Again
							</Button>
						</CardFooter>
					</Card>
				</div>
			);
		}

		return this.props.children;
	}
}

export default ErrorBoundary;
