
import { Action, configureStore, ThunkAction } from '@reduxjs/toolkit';
import { listenerMiddleware } from './listenerMiddleware';
import { firebaseApiSlice } from '@/features/api/firebaseApiSlice';

export const store = configureStore({
  reducer: {
    [firebaseApiSlice.reducerPath]: firebaseApiSlice.reducer,
  },
  middleware: getDefaultMiddleware =>
    getDefaultMiddleware()
      // Here goes the listener middleware in the front 
      .prepend(listenerMiddleware.middleware)
      // Here goes the api slices middleware in the back
      .concat(firebaseApiSlice.middleware)
  ,
});

export type AppStore = typeof store;
export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk = ThunkAction<void, RootState, unknown, Action<string>>;

