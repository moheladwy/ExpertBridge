// api/jobService.ts
import { JobResponse } from "@/features/jobs/types";
import { auth } from "@/lib/firebase";
import config from "@/lib/util/config";

const API_BASE_URL = config.VITE_SERVER_URL;

// Helper function to get auth token (adjust based on your auth implementation)
const getAuthToken = async () => {
	// Replace with your actual token retrieval logic
	const token = await auth.currentUser?.getIdToken();
	return token;
};

// Helper function to handle API responses
const handleApiResponse = async (response: Response) => {
	if (!response.ok) {
		const errorText = await response.text();
		throw new Error(`API Error: ${response.status} - ${errorText}`);
	}
	return response.json();
};

export const acceptOffer = async (offerId: string): Promise<JobResponse> => {
	const token = await getAuthToken();

	const response = await fetch(
		`${API_BASE_URL}/jobs/offers/${offerId}/accept`,
		{
			method: "PATCH",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${token}`,
			},
		}
	);

	return handleApiResponse(response);
};

export const acceptApplication = async (
	applicationId: string
): Promise<JobResponse> => {
	const token = await getAuthToken();

	const response = await fetch(
		`${API_BASE_URL}/jobs/applications/${applicationId}/accept`,
		{
			method: "PATCH",
			headers: {
				"Content-Type": "application/json",
				Authorization: `Bearer ${token}`,
			},
		}
	);

	return handleApiResponse(response);
};
