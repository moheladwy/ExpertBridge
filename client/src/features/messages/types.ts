
export interface MessageResponse {
  senderId: string;
  chatId: string;
  content: string;
  isConfirmationMessage: boolean;
  createdAt: string;
}

export interface CreateMessageRequest {
  chatId: string;
  content: string;
}
