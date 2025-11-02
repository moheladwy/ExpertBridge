import { useState } from "react";
import { Card, CardContent, CardHeader } from "@/views/components/ui/card";
import {
	Tabs,
	TabsContent,
	TabsList,
	TabsTrigger,
} from "@/views/components/ui/tabs";
import { Skeleton } from "@/views/components/ui/skeleton";
import { AlertTriangle, Briefcase, User } from "lucide-react";
import {
	useGetMyJobOffersQuery,
	useGetReceivedJobOffersQuery,
	useUpdateJobOfferStatusMutation,
	useDeleteJobOfferMutation,
} from "@/features/jobs/jobsSlice";
import { toast } from "sonner";
import JobOfferCard from "@/views/components/common/jobs/JobOfferCard";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";

const JobOffersSkeleton = () => (
	<div className="space-y-4">
		{[1, 2, 3].map((i) => (
			<Card key={i}>
				<CardHeader>
					<Skeleton className="h-6 w-3/4" />
					<div className="flex gap-4">
						<Skeleton className="h-4 w-24" />
						<Skeleton className="h-4 w-20" />
						<Skeleton className="h-4 w-20" />
					</div>
				</CardHeader>
				<CardContent>
					<Skeleton className="h-4 w-full mb-2" />
					<Skeleton className="h-4 w-2/3 mb-4" />
					<div className="flex gap-2">
						<Skeleton className="h-8 w-20" />
						<Skeleton className="h-8 w-20" />
					</div>
				</CardContent>
			</Card>
		))}
	</div>
);

const EmptyState = ({ type }: { type: "sent" | "received" }) => (
	<div className="text-center py-12">
		<Briefcase className="mx-auto h-12 w-12 text-gray-400 dark:text-gray-500 mb-4" />
		<h3 className="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-2">
			No {type} job offers
		</h3>
		<p className="text-gray-600 dark:text-gray-400">
			{type === "sent"
				? "You haven't sent any job offers yet."
				: "You haven't received any job offers yet."}
		</p>
	</div>
);

export const JobOffersDashboardPage = () => {
	const [activeTab, setActiveTab] = useState("sent");

	const {
		data: sentOffers = [],
		isLoading: sentLoading,
		error: sentError,
		refetch: refetchMyOffers,
	} = useGetMyJobOffersQuery();

	const {
		data: receivedOffers = [],
		isLoading: receivedLoading,
		error: receivedError,
		refetch: refetchReceivedOffers,
	} = useGetReceivedJobOffersQuery();

	useRefetchOnLogin(refetchReceivedOffers);
	useRefetchOnLogin(refetchMyOffers);

	const [updateJobOfferStatus] = useUpdateJobOfferStatusMutation();
	const [deleteJobOffer] = useDeleteJobOfferMutation();

	const handleStatusUpdate = async (
		id: string,
		status: "accepted" | "declined"
	) => {
		try {
			await updateJobOfferStatus({ id, status }).unwrap();
			toast.success(`Job offer ${status} successfully!`);
		} catch (error) {
			toast.error(`Failed to ${status} job offer. Please try again.`);
		}
	};

	const handleDelete = async (id: string) => {
		try {
			await deleteJobOffer(id).unwrap();
			toast.success("Job offer deleted successfully!");
		} catch (error) {
			toast.error("Failed to delete job offer. Please try again.");
		}
	};

	return (
		<div className="container mx-auto px-4 py-8">
			<div className="flex flex-col items-center mb-8">
				<h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100 mb-2">
					Job Offers
				</h1>
				<p className="text-gray-600 dark:text-gray-400">
					Manage your sent and received job offers
				</p>
			</div>

			<Tabs
				value={activeTab}
				onValueChange={setActiveTab}
				className="w-full"
			>
				<TabsList className="grid w-full grid-cols-2 dark:bg-gray-800">
					<TabsTrigger
						value="sent"
						className="flex items-center gap-2 dark:data-[state=active]:bg-gray-700 dark:text-gray-300"
					>
						<Briefcase className="h-4 w-4" />
						Sent ({sentOffers.length})
					</TabsTrigger>
					<TabsTrigger
						value="received"
						className="flex items-center gap-2 dark:data-[state=active]:bg-gray-700 dark:text-gray-300"
					>
						<User className="h-4 w-4" />
						Received ({receivedOffers.length})
					</TabsTrigger>
				</TabsList>

				<TabsContent value="sent" className="mt-6">
					{sentLoading && <JobOffersSkeleton />}

					{sentError && (
						<div className="flex items-center gap-2 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
							<AlertTriangle className="h-5 w-5 text-red-600 dark:text-red-400" />
							<p className="text-red-800 dark:text-red-300">
								Failed to load sent job offers.
							</p>
						</div>
					)}

					{!sentLoading && !sentError && sentOffers.length === 0 && (
						<EmptyState type="sent" />
					)}

					{!sentLoading && !sentError && sentOffers.length > 0 && (
						<div className="space-y-4">
							{sentOffers.map((offer) => (
								<JobOfferCard
									key={offer.id}
									offer={offer}
									type="sent"
									onDelete={handleDelete}
								/>
							))}
						</div>
					)}
				</TabsContent>

				<TabsContent value="received" className="mt-6">
					{receivedLoading && <JobOffersSkeleton />}

					{receivedError && (
						<div className="flex items-center gap-2 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
							<AlertTriangle className="h-5 w-5 text-red-600 dark:text-red-400" />
							<p className="text-red-800 dark:text-red-300">
								Failed to load received job offers.
							</p>
						</div>
					)}

					{!receivedLoading &&
						!receivedError &&
						receivedOffers.length === 0 && (
							<EmptyState type="received" />
						)}

					{!receivedLoading &&
						!receivedError &&
						receivedOffers.length > 0 && (
							<div className="space-y-4">
								{receivedOffers.map((offer) => (
									<JobOfferCard
										key={offer.id}
										offer={offer}
										type="received"
										onStatusUpdate={handleStatusUpdate}
									/>
								))}
							</div>
						)}
				</TabsContent>
			</Tabs>
		</div>
	);
};
