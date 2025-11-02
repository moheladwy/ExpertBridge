export interface NotificationResponse {
	id: string;
	createdAt: string;
	recipientId: string;
	message: string;
	isRead: boolean;
	actionUrl?: string | null;
	iconUrl?: string | null;
	iconActionUrl?: string | null;
}
