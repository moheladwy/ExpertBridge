
import { Action, configureStore, ThunkAction } from '@reduxjs/toolkit';
import { listenerMiddleware } from './listenerMiddleware';
import { apiSlice } from '@/features/api/apiSlice';

export const store = configureStore({
  reducer: {
    [apiSlice.reducerPath]: apiSlice.reducer,
  },
  middleware: getDefaultMiddleware =>
    getDefaultMiddleware()
      // Here goes the listener middleware in the front 
      .prepend(listenerMiddleware.middleware)
      // Here goes the api slice's middleware in the back
      .concat(apiSlice.middleware)
  ,
});

export type AppStore = typeof store;
export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk = ThunkAction<void, RootState, unknown, Action<string>>;

