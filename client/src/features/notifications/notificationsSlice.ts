import config from "@/lib/util/config";
import { apiSlice } from "../api/apiSlice";
import { NotificationResponse } from "./types";
import * as signalR from "@microsoft/signalr";
import { createAction } from "@reduxjs/toolkit";

const newNotificationsReceived = createAction<NotificationResponse>(
	"notifications/newNotificationsReceived"
);

export const notificationsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getNotifications: builder.query<NotificationResponse[], string>({
			query: () => "/notifications",
			onCacheEntryAdded: async (currentUserId, lifecycleApi) => {
				const connection = new signalR.HubConnectionBuilder()
					.withUrl(`${config.VITE_SERVER_URL}/notificationsHub`, {})
					.withAutomaticReconnect()
					.build();

				try {
					await connection.start();
					console.log("Connected to Notifications Hub.");

					connection.on(
						"ReceiveNotification",
						(notification: NotificationResponse) => {
							// console.log('notification received: ', notification);

							if (notification.recipientId !== currentUserId)
								return;

							lifecycleApi.updateCachedData((draft) => {
								draft.push(notification);
								draft.sort((a, b) =>
									b.createdAt.localeCompare(a.createdAt)
								);
							});

							lifecycleApi.dispatch(
								newNotificationsReceived(notification)
							);
						}
					);
				} catch (error) {
					console.error(
						"Connection to Notifications Hub failed: ",
						error
					);
				}
			},
		}),

		readNotifications: builder.mutation<void, string>({
			query: (userId) => ({
				url: "/notifications",
				method: "PATCH",
			}),
			onQueryStarted: async (userId, lifecycleApi) => {
				const patchResult = lifecycleApi.dispatch(
					notificationsApiSlice.util.updateQueryData(
						"getNotifications",
						userId,
						(draft) => {
							draft.forEach(
								(notification) => (notification.isRead = true)
							);
						}
					)
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch (error) {
					console.error(
						"An error occurred while reading notifications",
						error
					);
				}
			},
		}),
	}),
});

export const { useGetNotificationsQuery, useReadNotificationsMutation } =
	notificationsApiSlice;
