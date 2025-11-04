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
		<Card className="group hover:shadow-lg hover:border-primary/50 transition-all duration-200 border-border">
			<CardHeader className="pb-3">
				<div className="flex justify-between items-start gap-3">
					<CardTitle className="text-lg font-semibold text-card-foreground group-hover:text-primary transition-colors line-clamp-2">
						{offer.title}
					</CardTitle>
					<Badge className="ml-2 bg-primary/10 text-primary font-semibold whitespace-nowrap">
						{formatCurrency(offer.budget)}
					</Badge>
				</div>
				<div className="flex flex-wrap items-center gap-3 text-sm text-muted-foreground">
					<div className="flex items-center gap-1.5">
						<div className="w-6 h-6 rounded-full bg-primary/10 flex items-center justify-center">
							<User className="h-3.5 w-3.5 text-primary" />
						</div>
						<span className="font-medium">
							{offer.author.firstName} {offer.author.lastName}
						</span>
					</div>
					<span className="text-muted-foreground/50">•</span>
					<div className="flex items-center gap-1.5">
						<MapPin className="h-4 w-4" />
						{offer.area}
					</div>
					<span className="text-muted-foreground/50">•</span>
					<div className="flex items-center gap-1.5">
						<Calendar className="h-4 w-4" />
						{formatDate(offer.createdAt)}
					</div>
				</div>
			</CardHeader>
			<CardContent>
				<p className="text-muted-foreground mb-4 line-clamp-3 leading-relaxed">
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
								className="flex items-center gap-1.5 bg-green-600 hover:bg-green-700 text-white rounded-full px-4"
							>
								{isAccepting ? (
									<>
										<div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
										Accepting...
									</>
								) : (
									<>
										<CheckCircle className="h-4 w-4" />
										Accept Offer
									</>
								)}
							</Button>
							<Button
								size="sm"
								variant="outline"
								onClick={handleDeclineOffer}
								disabled={isAccepting}
								className="flex items-center gap-1.5 rounded-full px-4 hover:bg-destructive/10 hover:text-destructive hover:border-destructive"
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
							className="flex items-center gap-1.5 rounded-full px-4"
						>
							<Trash2 className="h-4 w-4" />
							Delete Offer
						</Button>
					)}
				</div>
			</CardContent>
		</Card>
	);
};

export default JobOfferCard;
