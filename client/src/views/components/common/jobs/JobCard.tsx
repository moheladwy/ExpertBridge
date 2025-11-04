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
		<div className="group bg-card rounded-xl shadow-sm border border-border hover:border-primary/50 hover:shadow-lg transition-all duration-200">
			<div className="p-6">
				{/* Header */}
				<div className="flex items-start justify-between mb-4">
					<div className="flex-1">
						<h3 className="text-lg font-semibold text-card-foreground group-hover:text-primary mb-1 line-clamp-2 transition-colors">
							{job.title}
						</h3>
						<div className="flex items-center text-sm text-muted-foreground">
							<User size={14} className="mr-1" />
							{isWorker ? "Hired by" : "Working with"}{" "}
							<span className="font-medium ml-1">
								{otherParty.firstName}
							</span>
						</div>
					</div>
					<div className="flex flex-col items-end gap-1">
						{job.isCompleted && (
							<Badge className="bg-green-100 text-green-800 border-green-200">
								<CheckCircle size={12} className="mr-1" />
								Completed
							</Badge>
						)}
						{job.isPaid && (
							<Badge className="bg-blue-100 text-blue-800 border-blue-200">
								<DollarSign size={12} className="mr-1" />
								Paid
							</Badge>
						)}
					</div>
				</div>

				{/* Details */}
				<div className="space-y-2.5 bg-muted/30 rounded-lg p-3">
					<div className="flex items-center text-sm text-card-foreground">
						<div className="w-8 h-8 rounded-full bg-green-100 flex items-center justify-center mr-3">
							<DollarSign size={16} className="text-green-600" />
						</div>
						<span className="font-semibold text-green-700">
							${job.actualCost}
						</span>
					</div>

					<div className="flex items-center text-sm text-card-foreground">
						<div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center mr-3">
							<MapPin size={16} className="text-blue-600" />
						</div>
						<span>{job.area}</span>
					</div>

					<div className="flex items-center text-sm text-card-foreground">
						<div className="w-8 h-8 rounded-full bg-purple-100 flex items-center justify-center mr-3">
							<Calendar size={16} className="text-purple-600" />
						</div>
						<span>Started: {formatDate(job.startedAt)}</span>
					</div>
				</div>

				{/* Description */}
				<p className="text-sm text-muted-foreground mt-4 line-clamp-2 leading-relaxed">
					{job.description}
				</p>

				{/* Actions */}
				<div className="mt-6">
					<Button
						onClick={onViewDetails}
						className="w-full bg-primary hover:bg-primary/90 text-primary-foreground rounded-full font-medium"
					>
						<MessageCircle size={16} className="mr-2" />
						View Details
					</Button>
				</div>
			</div>
		</div>
	);
};
