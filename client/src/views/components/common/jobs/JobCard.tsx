import { JobResponse } from "@/features/jobs/types";
import { Button } from "@/views/components/ui/button";
import {
	Calendar,
	CheckCircle,
	DollarSign,
	MapPin,
	MessageCircle,
	User,
} from "lucide-react";
import { Badge } from "@/views/components/ui/badge";

export const JobCard: React.FC<{
	job: JobResponse;
	appUser: any;
	onViewDetails: () => void;
}> = ({ job, appUser, onViewDetails }) => {
	const isWorker = appUser.id === job.workerId;
	const otherParty = isWorker ? job.author : job.worker;

	const formatDate = (dateString: string | null) => {
		if (!dateString) return "Not started";
		return new Date(dateString).toLocaleDateString("en-US", {
			year: "numeric",
			month: "short",
			day: "numeric",
		});
	};

	return (
		<div className="bg-card rounded-lg shadow-sm border border-border hover:shadow-md transition-shadow">
			<div className="p-6">
				{/* Header */}
				<div className="flex items-start justify-between mb-4">
					<div className="flex-1">
						<h3 className="text-lg font-semibold text-card-foreground mb-1 line-clamp-2">
							{job.title}
						</h3>
						<div className="flex items-center text-sm text-muted-foreground">
							<User size={14} className="mr-1" />
							{isWorker ? "Hired by" : "Working with"}{" "}
							{otherParty.firstName}
						</div>
					</div>
					<div className="flex items-center space-x-1">
						{job.isCompleted && (
							<Badge className="bg-green-100 text-green-800">
								<CheckCircle size={12} className="mr-1" />
								Completed
							</Badge>
						)}
						{job.isPaid && (
							<Badge className="bg-blue-100 text-blue-800">
								<DollarSign size={12} className="mr-1" />
								Paid
							</Badge>
						)}
					</div>
				</div>

				{/* Details */}
				<div className="space-y-3">
					<div className="flex items-center text-sm text-card-foreground">
						<DollarSign size={16} className="mr-2 text-green-600" />
						<span className="font-medium">${job.actualCost}</span>
					</div>

					<div className="flex items-center text-sm text-card-foreground">
						<MapPin size={16} className="mr-2 text-blue-600" />
						<span>{job.area}</span>
					</div>

					<div className="flex items-center text-sm text-card-foreground">
						<Calendar size={16} className="mr-2 text-purple-600" />
						<span>Started: {formatDate(job.startedAt)}</span>
					</div>
				</div>

				{/* Description */}
				<p className="text-sm text-card-foreground mt-3 line-clamp-2">
					{job.description}
				</p>

				{/* Actions */}
				<div className="mt-6">
					<Button
						onClick={onViewDetails}
						className="w-full bg-indigo-600 hover:bg-indigo-700 text-white"
					>
						<MessageCircle size={16} className="mr-2" />
						View Details
					</Button>
				</div>
			</div>
		</div>
	);
};
