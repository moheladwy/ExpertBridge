import React, { useState, useEffect, useRef } from "react";
import {
	useGetMessagesByChatIdQuery,
	useSendMessageMutation,
} from "@/features/messages/messagesSlice";
import { MessageResponse } from "@/features/messages/types";
import { Button } from "@/views/components/custom/button";
import { Send, Clock, CheckCircle } from "lucide-react";
import { format, isToday, isYesterday, isSameDay, parseISO } from "date-fns";

interface ChatComponentProps {
	chatId: string;
	currentUserId: string;
	otherPartyName: string;
}

export const ChatComponent: React.FC<ChatComponentProps> = ({
	chatId,
	currentUserId,
	otherPartyName,
}) => {
	const [message, setMessage] = useState("");
	const messagesEndRef = useRef<HTMLDivElement>(null);
	const messagesContainerRef = useRef<HTMLDivElement>(null);
	const inputRef = useRef<HTMLInputElement>(null);

	const {
		data: messages = [],
		isLoading,
		error,
	} = useGetMessagesByChatIdQuery(chatId);
	const [sendMessage, { isLoading: isSending }] = useSendMessageMutation();

	// Auto-scroll to bottom when new messages arrive
	useEffect(() => {
		const scrollToBottom = () => {
			if (messagesEndRef.current) {
				messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
			}
		};

		// Small delay to ensure DOM is updated
		const timer = setTimeout(scrollToBottom, 100);
		return () => clearTimeout(timer);
	}, [messages]);

	// Focus input on mount
	useEffect(() => {
		if (inputRef.current) {
			inputRef.current.focus();
		}
	}, []);

	const handleSendMessage = async (e: React.FormEvent) => {
		e.preventDefault();
		if (!message.trim() || isSending) return;

		const messageContent = message.trim();
		setMessage("");

		try {
			await sendMessage({
				chatId,
				content: messageContent,
			}).unwrap();
		} catch (error) {
			console.error("Failed to send message:", error);
			// Optionally show error toast
		}
	};

	const handleKeyPress = (e: React.KeyboardEvent) => {
		if (e.key === "Enter" && !e.shiftKey) {
			e.preventDefault();
			handleSendMessage(e);
		}
	};

	const formatMessageTime = (dateString: string) => {
		const date = parseISO(dateString);
		return format(date, "HH:mm");
	};

	const formatDateSeparator = (dateString: string) => {
		const date = parseISO(dateString);

		if (isToday(date)) {
			return "Today";
		} else if (isYesterday(date)) {
			return "Yesterday";
		} else {
			return format(date, "MMMM d, yyyy");
		}
	};

	const shouldShowDateSeparator = (
		currentMessage: MessageResponse,
		previousMessage?: MessageResponse
	) => {
		if (!previousMessage) return true;

		const currentDate = parseISO(currentMessage.createdAt);
		const previousDate = parseISO(previousMessage.createdAt);

		return !isSameDay(currentDate, previousDate);
	};

	const renderMessage = (msg: MessageResponse, index: number) => {
		const isCurrentUser = msg.senderId === currentUserId;
		const previousMessage = index > 0 ? messages[index - 1] : undefined;
		const showDateSeparator = shouldShowDateSeparator(msg, previousMessage);

		return (
			<div key={`${msg.chatId}-${index}`} className="w-full">
				{/* Date Separator */}
				{showDateSeparator && (
					<div className="flex justify-center my-4">
						<div className="bg-gray-200 dark:bg-gray-700 rounded-full px-3 py-1">
							<span className="text-xs text-gray-600 dark:text-gray-400 font-medium">
								{formatDateSeparator(msg.createdAt)}
							</span>
						</div>
					</div>
				)}

				{/* Confirmation Message */}
				{msg.isConfirmationMessage ? (
					<div className="flex justify-center my-4">
						<div className="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-700 rounded-lg px-4 py-2 max-w-md">
							<div className="flex items-center justify-center space-x-2">
								<CheckCircle
									size={16}
									className="text-yellow-600 dark:text-yellow-400"
								/>
								<span className="text-sm text-yellow-800 dark:text-yellow-200 font-medium text-center">
									{msg.content}
								</span>
							</div>
						</div>
					</div>
				) : (
					/* Regular Message */
					<div
						className={`flex mb-4 ${isCurrentUser ? "justify-end" : "justify-start"}`}
					>
						<div
							className={`max-w-xs lg:max-w-md xl:max-w-lg ${isCurrentUser ? "order-2" : "order-1"}`}
						>
							<div
								className={`relative rounded-2xl px-4 py-2 shadow-sm ${
									isCurrentUser
										? "bg-blue-500 text-white rounded-br-sm"
										: "bg-white dark:bg-gray-700 text-gray-900 dark:text-white border border-gray-200 dark:border-gray-600 rounded-bl-sm"
								}`}
							>
								<p className="text-sm leading-relaxed break-words">
									{msg.content}
								</p>
								<div
									className={`flex items-center justify-end mt-1 space-x-1 ${
										isCurrentUser
											? "text-blue-100"
											: "text-gray-500 dark:text-gray-400"
									}`}
								>
									<Clock size={12} />
									<span className="text-xs">
										{formatMessageTime(msg.createdAt)}
									</span>
								</div>
							</div>
						</div>
					</div>
				)}
			</div>
		);
	};

	if (isLoading) {
		return (
			<div className="flex flex-col h-96 bg-gray-50 dark:bg-gray-900 rounded-lg">
				<div className="flex-1 flex items-center justify-center">
					<div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
				</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="flex flex-col h-96 bg-gray-50 dark:bg-gray-900 rounded-lg">
				<div className="flex-1 flex items-center justify-center">
					<p className="text-red-500 dark:text-red-400">
						Failed to load messages
					</p>
				</div>
			</div>
		);
	}

	return (
		<div className="flex flex-col h-96 bg-gray-50 dark:bg-gray-900 rounded-lg overflow-hidden">
			{/* Chat Header */}
			<div className="bg-white dark:bg-gray-800 border-b dark:border-gray-700 px-4 py-3 flex-shrink-0">
				<div className="flex items-center space-x-3">
					<div className="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-500 rounded-full flex items-center justify-center">
						<span className="text-white font-semibold text-sm">
							{otherPartyName.charAt(0).toUpperCase()}
						</span>
					</div>
					<div>
						<h3 className="font-semibold text-gray-900 dark:text-white">
							{otherPartyName}
						</h3>
						<p className="text-xs text-gray-500 dark:text-gray-400">
							{messages.length === 0
								? "No messages yet"
								: `${messages.length} message${messages.length !== 1 ? "s" : ""}`}
						</p>
					</div>
				</div>
			</div>

			{/* Messages Container */}
			<div
				ref={messagesContainerRef}
				className="flex-1 overflow-y-auto p-4 space-y-2 scroll-smooth"
				style={{ scrollBehavior: "smooth" }}
			>
				{messages.length === 0 ? (
					<div className="flex flex-col items-center justify-center h-full text-center">
						<div className="w-16 h-16 bg-gray-200 dark:bg-gray-700 rounded-full flex items-center justify-center mb-4">
							<Send
								size={24}
								className="text-gray-400 dark:text-gray-500"
							/>
						</div>
						<p className="text-gray-500 dark:text-gray-400 font-medium">
							No messages yet
						</p>
						<p className="text-sm text-gray-400 dark:text-gray-500 mt-1">
							Start the conversation with {otherPartyName}
						</p>
					</div>
				) : (
					messages.map((msg, index) => renderMessage(msg, index))
				)}
				<div ref={messagesEndRef} />
			</div>

			{/* Message Input */}
			<div className="bg-white dark:bg-gray-800 border-t dark:border-gray-700 p-4 flex-shrink-0">
				<form onSubmit={handleSendMessage} className="flex space-x-2">
					<input
						ref={inputRef}
						type="text"
						value={message}
						onChange={(e) => setMessage(e.target.value)}
						onKeyPress={handleKeyPress}
						placeholder={`Message ${otherPartyName}...`}
						className="flex-1 px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-full 
                     bg-white dark:bg-gray-700 text-gray-900 dark:text-white 
                     placeholder-gray-500 dark:placeholder-gray-400 
                     focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 
                     focus:border-transparent transition-all duration-200"
						disabled={isSending}
					/>
					<Button
						type="submit"
						disabled={!message.trim() || isSending}
						className="w-10 h-10 rounded-full bg-blue-500 hover:bg-blue-600 
                     disabled:bg-gray-300 dark:disabled:bg-gray-600 
                     disabled:cursor-not-allowed transition-all duration-200 
                     flex items-center justify-center p-0"
					>
						{isSending ? (
							<div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
						) : (
							<Send size={16} className="text-white" />
						)}
					</Button>
				</form>
			</div>
		</div>
	);
};
