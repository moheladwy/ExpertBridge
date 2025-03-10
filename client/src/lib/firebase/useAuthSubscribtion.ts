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

  const [updateUser, result] = useUpdateUserMutation();
  
  const {
      isLoading: updateUserLoading,
      isError: updateUserIsError, 
      error: updateUserError,
    } = result;

  useEffect(() => {
    const listener = onAuthStateChanged(
      auth,
      async (user) => {
        // BE AWARE, THE AUTH STATE COULD CHANGE A LOT.
        // SO WHAT TO DO? 
        
        // Solution: setTimeout for something like 5 seconds or more, before checking if (user)
        // to make sure that the user is still there. 
        
        // BE AWARE, THE user IS BASSED TO THIS CALLBACK ALREADY!
        // WE MIGHT USE auth.currentUser IN THE IF CHECK, THEN USE user IN THE 
        // CONDITIONAL OPERATION WE ARE WILLING TO DO.

        // TODO: Call RTK here to update the user in through the api.
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

        if (!user) {
          setError(new Error('User Signed Out'));
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