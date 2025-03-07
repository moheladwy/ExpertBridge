import { Auth, onAuthStateChanged, User } from 'firebase/auth';
import { useEffect } from 'react';
import { LoadingHook, useLoadingValue } from '@/lib/util';
import { useUpdateUserMutation } from '@/features/users/usersSlice';

export type AuthStateHook = LoadingHook<User | null, Error>;

type AuthStateOptions = {
  onUserChanged?: (user: User | null) => Promise<void>;
};

export default (auth: Auth, options?: AuthStateOptions): AuthStateHook => {
  const { error, loading, setError, setValue, value } = useLoadingValue<User | null, Error>(
    () => auth.currentUser);

  // @ts-expect-error - We don't need the mutation result here
  const [updateUser, result] = useUpdateUserMutation();
  // @ts-expect-error - We don't need the mutation result here
  const {
      isLoading: updateUserLoading,
      isError: updateUserIsError, 
      error: updateUserError,
    } = result;

  useEffect(() => {
    const listener = onAuthStateChanged(
      auth,
      async (user) => {
        // TODO - Call RTK here to update the user in through the api.
        // Most likely a PUT request to the user endpoint. (Make sure the PUT behaviour is creational)
        if (user) { /* empty */ }

        
        if (options?.onUserChanged) {
          // onUserChanged function to process custom claims on any other trigger function
          try {
            await options.onUserChanged(user);
          }
          catch (e) {
            setError(e as Error);
          }
        }
        
        if (!user?.emailVerified) {
          setError(new Error('Email Unverified'));
        }
        else {
          setValue(user);
        }
      },
      setError
    );

    return () => {
      listener();
    };
  }, [auth]);

  return [value, loading, error];
};