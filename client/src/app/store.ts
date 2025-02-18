
import { Action, configureStore, ThunkAction } from '@reduxjs/toolkit';
import { listenerMiddleware } from './listenerMiddleware';
import { appApiSlice } from '@/features/api/apiSlice';

export const store = configureStore({
  reducer: {
    [appApiSlice.reducerPath]: appApiSlice.reducer,
  },
  middleware: getDefaultMiddleware =>
    getDefaultMiddleware()
      // Here goes the listener middleware in the front 
      .prepend(listenerMiddleware.middleware)
      // Here goes the api slice's middleware in the back
      .concat(appApiSlice.middleware)
  ,
});

export type AppStore = typeof store;
export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk = ThunkAction<void, RootState, unknown, Action<string>>;

