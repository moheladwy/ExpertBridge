import { useState, useEffect, useCallback } from "react";
import { Button } from "@/views/components/ui/button";
import {
	RefreshCw,
	AlertTriangle,
	Wifi,
	WifiOff,
	Database,
	Activity,
} from "lucide-react";
import { Card, CardContent } from "@/views/components/ui/card";
import { HealthCheckResponse } from "@/hooks/useApiHealth";

interface ApiHealthErrorPageProps {
	onRetry?: () => void;
	isRetrying?: boolean;
	healthData?: HealthCheckResponse | null;
}

const ApiHealthErrorPage: React.FC<ApiHealthErrorPageProps> = ({
	onRetry,
	isRetrying = false,
	healthData,
}) => {
	const [retryCount, setRetryCount] = useState(0);

	const handleRetry = useCallback(() => {
		setRetryCount((prev) => prev + 1);
		onRetry?.();
	}, [onRetry]);

	useEffect(() => {
		// Auto-retry after 30 seconds for the first few attempts
		if (retryCount < 3) {
			const timer = setTimeout(() => {
				handleRetry();
			}, 30000);

			return () => clearTimeout(timer);
		}
	}, [retryCount, handleRetry]);

	return (
		<div className="min-h-screen bg-gray-100 dark:bg-gray-900 flex items-center justify-center p-4">
			<Card className="w-full max-w-md mx-auto">
				<CardContent className="p-8 text-center">
					<div className="mb-6">
						{isRetrying ? (
							<Wifi className="w-16 h-16 mx-auto text-blue-500 animate-pulse" />
						) : (
							<WifiOff className="w-16 h-16 mx-auto text-red-500" />
						)}
					</div>

					<div className="mb-6">
						<h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-2">
							Service Unavailable
						</h1>
						<p className="text-gray-600 dark:text-gray-400 mb-4">
							We're having trouble connecting to our servers. This
							might be temporary.
						</p>

						<div className="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-4 mb-6">
							<div className="flex items-center">
								<AlertTriangle className="w-5 h-5 text-yellow-600 dark:text-yellow-400 mr-2" />
								<div className="text-left">
									<p className="text-sm font-medium text-yellow-800 dark:text-yellow-200">
										API Health Check Failed
									</p>
									<p className="text-xs text-yellow-700 dark:text-yellow-300 mt-1">
										Our backend services are currently
										experiencing issues.
									</p>
								</div>
							</div>
						</div>
					</div>

					{/* Service Status Details */}
					{healthData && healthData.entries && (
						<div className="mb-6 bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-4">
							<div className="flex items-center mb-3">
								<Database className="w-5 h-5 text-gray-600 dark:text-gray-400 mr-2" />
								<h3 className="text-sm font-semibold text-gray-900 dark:text-white">
									Service Status Details
								</h3>
							</div>
							<div className="space-y-2">
								{Object.entries(healthData.entries).map(
									([service, details]) => (
										<div
											key={service}
											className="flex items-center justify-between py-2 px-3 bg-white dark:bg-gray-900 rounded border border-gray-200 dark:border-gray-700"
										>
											<div className="flex items-center gap-2">
												<Activity className="w-4 h-4 text-gray-500" />
												<span className="text-sm font-medium capitalize text-gray-700 dark:text-gray-300">
													{service}
												</span>
											</div>
											<div className="flex items-center gap-2">
												<span className="text-xs text-gray-500 dark:text-gray-400">
													{details.duration}
												</span>
												<span
													className={`text-xs px-2 py-0.5 rounded ${
														details.status ===
														"Healthy"
															? "bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-300"
															: "bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-300"
													}`}
												>
													{details.status}
												</span>
											</div>
										</div>
									)
								)}
							</div>
							<p className="text-xs text-gray-500 dark:text-gray-400 mt-3">
								Total Duration: {healthData.totalDuration}
							</p>
						</div>
					)}

					<div className="space-y-4">
						<Button
							onClick={handleRetry}
							disabled={isRetrying}
							className="w-full"
							size="lg"
						>
							{isRetrying ? (
								<>
									<RefreshCw className="w-4 h-4 mr-2 animate-spin" />
									Checking Connection...
								</>
							) : (
								<>
									<RefreshCw className="w-4 h-4 mr-2" />
									Try Again
								</>
							)}
						</Button>

						<div className="text-sm text-gray-500 dark:text-gray-400">
							{retryCount > 0 && (
								<p className="mb-2">
									Retry attempt: {retryCount}
									{retryCount < 3 &&
										" (Auto-retrying in 30s)"}
								</p>
							)}
							<p>
								If the problem persists, please contact our
								support team.
							</p>
						</div>
					</div>

					<div className="mt-8 pt-6 border-t border-gray-200 dark:border-gray-700">
						<p className="text-xs text-gray-400 dark:text-gray-500">
							Error Code: API_HEALTH_CHECK_FAILED
						</p>
					</div>
				</CardContent>
			</Card>
		</div>
	);
};

export default ApiHealthErrorPage;
