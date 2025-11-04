import { useGetJobByIdQuery } from "@/features/jobs/jobsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { ChatComponent } from "@/views/components/chat/ChatComponent";
import { Button } from "@/views/components/ui/button";
import { Badge } from "@/views/components/ui/badge";
import { Skeleton } from "@/views/components/ui/skeleton";
import {
	Calendar,
	CheckCircle,
	DollarSign,
	MapPin,
	MessageCircle,
	User,
} from "lucide-react";
import toast from "react-hot-toast";
import { useParams } from "react-router";

const JobDetailsPage: React.FC = () => {
	const { jobId } = useParams<{ jobId: string }>();
	const {
		data: job,
		isLoading,
		error,
	} = useGetJobByIdQuery(jobId!, { skip: !jobId });
	const [_, __, ___, authUser, appUser] = useIsUserLoggedIn();

	// Check if user is logged in
	if (!authUser || !appUser) {
		return (
			<div className="text-center p-8">
				<p className="text-muted-foreground">
					Please log in to view job details.
				</p>
			</div>
		);
	}

	if (isLoading) {
		return (
			<div className="w-full flex justify-center">
				<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full bg-card rounded-lg shadow-md border border-border p-6">
					<div className="space-y-6">
						<div className="bg-card rounded-lg shadow-sm border border-border p-6">
							<Skeleton className="h-8 w-3/4 mb-4" />
							<Skeleton className="h-4 w-1/2 mb-4" />
							<Skeleton className="h-20 w-full" />
						</div>
						<div className="bg-card rounded-lg shadow-sm border border-border p-6">
							<Skeleton className="h-6 w-32 mb-4" />
							<div className="grid grid-cols-1 md:grid-cols-3 gap-6">
								{[1, 2, 3].map((i) => (
									<div
										key={i}
										className="flex items-center space-x-3"
									>
										<Skeleton className="h-12 w-12 rounded-lg" />
										<div>
											<Skeleton className="h-4 w-20 mb-2" />
											<Skeleton className="h-6 w-24" />
										</div>
									</div>
								))}
							</div>
						</div>
						<div className="bg-card rounded-lg shadow-sm border border-border p-6">
							<Skeleton className="h-6 w-32 mb-4" />
							<Skeleton className="h-96 w-full" />
						</div>
					</div>
				</div>
			</div>
		);
	}

	if (error || !job) {
		toast.error("Error loading job details. Please try again.");
		return (
			<div className="text-center text-destructive p-8">
				<p>Error loading job details. Please try again.</p>
			</div>
		);
	}

	const isWorker = appUser.id === job.workerId;
	const otherParty = isWorker ? job.author : job.worker;

	const formatDate = (dateString: string | null) => {
		if (!dateString) return "Not started";
		return new Date(dateString).toLocaleDateString("en-US", {
			year: "numeric",
			month: "long",
			day: "numeric",
		});
	};

	const handleConfirmAction = () => {
		// TODO: Implement confirmation logic
		toast.success(
			isWorker
				? "Job completion confirmed!"
				: "Payment confirmation sent!"
		);
	};

	return (
		<div className="w-full flex justify-center">
			<div className="mt-5 w-3/5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full space-y-6">
				{/* Header */}
				<div className="bg-card rounded-lg shadow-md border border-border p-6">
					<div className="flex items-start justify-between mb-4">
						<div>
							<h1 className="text-2xl font-bold text-card-foreground mb-2">
								{job.title}
							</h1>
							<div className="flex items-center text-sm text-muted-foreground">
								<User size={16} className="mr-1" />
								{isWorker ? "Hired by" : "Working with"}{" "}
								{otherParty.firstName}
							</div>
						</div>
						<div className="flex items-center space-x-2">
							{job.isCompleted && (
								<Badge className="bg-green-500/10 text-green-600 border-0">
									<CheckCircle size={14} className="mr-1" />
									Completed
								</Badge>
							)}
							{job.isPaid && (
								<Badge className="bg-primary/10 text-primary border-0">
									<DollarSign size={14} className="mr-1" />
									Paid
								</Badge>
							)}
						</div>
					</div>{" "}
					<p className="text-muted-foreground leading-relaxed">
						{job.description}
					</p>
				</div>

				{/* Job Details */}
				<div className="bg-card rounded-lg shadow-md border border-border p-6">
					<h2 className="text-lg font-semibold text-card-foreground mb-4">
						Job Details
					</h2>
					<div className="grid grid-cols-1 md:grid-cols-3 gap-6">
						<div className="flex items-center space-x-3">
							<div className="p-2 bg-green-500/10 rounded-lg">
								<DollarSign
									size={20}
									className="text-green-600"
								/>
							</div>
							<div>
								<p className="text-sm text-muted-foreground">
									Actual Cost
								</p>
								<p className="text-lg font-semibold text-card-foreground">
									${job.actualCost}
								</p>
							</div>
						</div>

						<div className="flex items-center space-x-3">
							<div className="p-2 bg-primary/10 rounded-lg">
								<MapPin size={20} className="text-primary" />
							</div>
							<div>
								<p className="text-sm text-muted-foreground">
									Area
								</p>
								<p className="text-lg font-semibold text-card-foreground">
									{job.area}
								</p>
							</div>
						</div>

						<div className="flex items-center space-x-3">
							<div className="p-2 bg-purple-500/10 rounded-lg">
								<Calendar
									size={20}
									className="text-purple-600"
								/>
							</div>
							<div>
								<p className="text-sm text-muted-foreground">
									Start Date
								</p>
								<p className="text-lg font-semibold text-card-foreground">
									{formatDate(job.startedAt)}
								</p>
							</div>
						</div>
					</div>
				</div>

				{/* Chat Section */}
				<div className="bg-card rounded-lg shadow-md border border-border p-6">
					<div className="flex items-center space-x-2 mb-4">
						<MessageCircle
							size={20}
							className="text-muted-foreground"
						/>
						<h2 className="text-lg font-semibold text-card-foreground">
							Chat with {otherParty.firstName}
						</h2>
					</div>

					<ChatComponent
						chatId={job.chatId}
						currentUserId={appUser.id}
						otherPartyName={otherParty.firstName}
					/>
				</div>

				{/* Action Button */}
				<div className="bg-card rounded-lg shadow-md border border-border p-6">
					<div className="flex items-center justify-between">
						<div>
							<h3 className="text-lg font-semibold text-card-foreground">
								{isWorker
									? "Confirm Job Completion"
									: "Confirm Payment"}
							</h3>
							<p className="text-sm text-muted-foreground mt-1">
								💡 <strong>Tip:</strong> Both parties should
								confirm their respective actions - the worker
								confirms job completion and the hirer confirms
								payment received.
							</p>
						</div>
						<Button
							onClick={handleConfirmAction}
							disabled={isWorker ? job.isCompleted : job.isPaid}
							className={`px-6 py-3 rounded-full ${
								(isWorker && job.isCompleted) ||
								(!isWorker && job.isPaid)
									? "bg-muted text-muted-foreground cursor-not-allowed"
									: "bg-primary hover:bg-primary/90 text-primary-foreground"
							}`}
						>
							{isWorker
								? job.isCompleted
									? "Job Confirmed"
									: "Confirm Job is Done"
								: job.isPaid
									? "Payment Confirmed"
									: "Confirm Receiving Money"}
						</Button>
					</div>
				</div>
			</div>
		</div>
	);
};

export default JobDetailsPage;
