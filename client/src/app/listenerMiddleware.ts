import { addListener, createListenerMiddleware } from "@reduxjs/toolkit";
import { AppDispatch, RootState } from "./store";



export const listenerMiddleware = createListenerMiddleware();

export const startAppListening = listenerMiddleware.startListening.withTypes<
  RootState,
  AppDispatch
  >();

export type AppStartListening = typeof startAppListening;

export const addAppListener = addListener.withTypes<RootState, AppDispatch>();
export type AppAddListener = typeof addAppListener;

// For each slice of the app, you can call addSliceListeners function that
// you created in the slice, and pass in the startAppListening in.

// Add here 
