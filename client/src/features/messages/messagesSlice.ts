import { apiSlice } from "../api/apiSlice";
import { MessageResponse, CreateMessageRequest } from "./types";
import config from "@/lib/util/config";
import * as signalR from "@microsoft/signalr";
import { createAction } from "@reduxjs/toolkit";

export const newMessageReceived = createAction<MessageResponse>(
  "messages/newMessageReceived"
);

export const messagingApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getMessagesByChatId: builder.query<MessageResponse[], string>({
      query: (chatId) => `/messages?chatId=${chatId}`,

      onCacheEntryAdded: async (chatId, lifecycleApi) => {
        const connection = new signalR.HubConnectionBuilder()
          .withUrl(`${config.VITE_SERVER_URL}/notificationsHub`)
          .withAutomaticReconnect()
          .build();

        try {
          await connection.start();
          console.log("Connected to Notifications Hub for messaging.");

          connection.on("ReceiveMessage", (message: MessageResponse & { receiverId: string }) => {
            // Only cache if the message belongs to the chat
            if (message.chatId !== chatId) return;

            lifecycleApi.updateCachedData((draft) => {
              draft.push(message);
              draft.sort((a, b) =>
                new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
              );
            });

            lifecycleApi.dispatch(newMessageReceived(message));
          });
        } catch (error) {
          console.error("Connection to SignalR hub failed (messages):", error);
        }

        await lifecycleApi.cacheEntryRemoved;
        try {
          await connection.stop();
        } catch (error) {
          console.error("Connection to SignalR hub failed (messages):", error);
        }
      },
    }),

    sendMessage: builder.mutation<MessageResponse, CreateMessageRequest>({
      query: (message) => ({
        url: "/messages",
        method: "POST",
        body: message,
      }),
    }),
  }),
});

export const {
  useGetMessagesByChatIdQuery,
  useSendMessageMutation,
} = messagingApiSlice;
