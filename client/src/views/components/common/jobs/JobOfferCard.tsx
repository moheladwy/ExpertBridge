import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { JobOfferResponse } from "@/features/jobs/types";
import {
	Card,
	CardContent,
	CardHeader,
	CardTitle,
} from "@/views/components/ui/card";
import { Badge } from "@/views/components/ui/badge";
import {
	Calendar,
	CheckCircle,
	MapPin,
	Trash2,
	User,
	XCircle,
} from "lucide-react";
import { Button } from "@/views/components/ui/button";
import { acceptOffer } from "@/api/jobService";

const JobOfferCard = ({
	offer,
	type,
	onStatusUpdate,
	onDelete,
}: {
	offer: JobOfferResponse;
	type: "sent" | "received";
	onStatusUpdate?: (id: string, status: "accepted" | "declined") => void;
	onDelete?: (id: string) => void;
}) => {
	const navigate = useNavigate();
	const [isAccepting, setIsAccepting] = useState(false);

	const formatCurrency = (amount: number) => {
		return new Intl.NumberFormat("en-US", {
			style: "currency",
			currency: "USD",
		}).format(amount);
	};

	const formatDate = (dateString: string) => {
		return new Date(dateString).toLocaleDateString("en-US", {
			year: "numeric",
			month: "short",
			day: "numeric",
		});
	};

	const handleAcceptOffer = async () => {
		setIsAccepting(true);
		try {
			const jobResponse = await acceptOffer(offer.id);

			// Call the original onStatusUpdate callback if provided
			if (onStatusUpdate) {
				onStatusUpdate(offer.id, "accepted");
			}

			// Redirect to the job page using the returned job ID
			navigate(`/my-jobs/${jobResponse.id}`);
		} catch (error) {
			console.error("Failed to accept offer:", error);
			// Handle error (show toast, etc.)
			// You might want to show an error message to the user
		} finally {
			setIsAccepting(false);
		}
	};

	const handleDeclineOffer = () => {
		if (onStatusUpdate) {
			onStatusUpdate(offer.id, "declined");
		}
	};

	return (
		<Card className="hover:shadow-md transition-shadow border-border">
			<CardHeader className="pb-3">
				<div className="flex justify-between items-start">
					<CardTitle className="text-lg font-semibold text-card-foreground">
						{offer.title}
					</CardTitle>
					<Badge variant="secondary" className="ml-2">
						{formatCurrency(offer.budget)}
					</Badge>
				</div>
				<div className="flex items-center gap-4 text-sm text-muted-foreground">
					<div className="flex items-center gap-1">
						<User className="h-4 w-4" />
						{offer.author.firstName} {offer.author.lastName}
					</div>
					<div className="flex items-center gap-1">
						<MapPin className="h-4 w-4" />
						{offer.area}
					</div>
					<div className="flex items-center gap-1">
						<Calendar className="h-4 w-4" />
						{formatDate(offer.createdAt)}
					</div>
				</div>
			</CardHeader>
			<CardContent>
				<p className="text-card-foreground mb-4 line-clamp-3">
					{offer.description}
				</p>
				<div className="flex gap-2 flex-wrap">
					{type === "received" && onStatusUpdate && (
						<>
							<Button
								size="sm"
								variant="default"
								onClick={handleAcceptOffer}
								disabled={isAccepting}
								className="flex items-center gap-1"
							>
								{isAccepting ? (
									<>
										<div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
										Accepting...
									</>
								) : (
									<>
										<CheckCircle className="h-4 w-4" />
										Accept
									</>
								)}
							</Button>
							<Button
								size="sm"
								variant="outline"
								onClick={handleDeclineOffer}
								disabled={isAccepting}
								className="flex items-center gap-1"
							>
								<XCircle className="h-4 w-4" />
								Decline
							</Button>
						</>
					)}
					{type === "sent" && onDelete && (
						<Button
							size="sm"
							variant="destructive"
							onClick={() => onDelete(offer.id)}
							className="flex items-center gap-1"
						>
							<Trash2 className="h-4 w-4" />
							Delete
						</Button>
					)}
				</div>
			</CardContent>
		</Card>
	);
};

export default JobOfferCard;
