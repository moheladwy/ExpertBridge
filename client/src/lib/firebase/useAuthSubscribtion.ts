import { Auth, onAuthStateChanged, User } from 'firebase/auth';
import { useEffect } from 'react';
import { LoadingHook, useLoadingValue } from '@/lib/util';
import { useUpdateUserMutation } from '@/features/users/usersSlice';
import { UpdateUserRequest } from '@/features/users/types';

export type AuthStateHook = LoadingHook<User | null, Error>;

type AuthStateOptions = {
  onUserChanged?: (user: User | null) => Promise<void>;
};

export default (auth: Auth, options?: AuthStateOptions): AuthStateHook => {
  const { error, loading, setError, setValue, value } = useLoadingValue<User | null, Error>(
    () => auth.currentUser
  );

  const [updateUser, result] = useUpdateUserMutation();

  const {
    isLoading: updateUserLoading,
    isError: updateUserIsError,
    error: updateUserError,
    isSuccess: updateUserSuccess,
  } = result;

  useEffect(() => {
    const listener = onAuthStateChanged(
      auth,
      async (user) => {
        // BE AWARE, THE AUTH STATE COULD CHANGE A LOT.
        // SO WHAT TO DO? 

        // Solution: setTimeout for something like 5 seconds or more, before checking if (user)
        // to make sure that the user is still there. 

        // BE AWARE, THE user IS PASSED TO THIS CALLBACK ALREADY!
        // WE MIGHT USE auth.currentUser IN THE IF CHECK, THEN USE user IN THE 
        // CONDITIONAL OPERATION WE ARE WILLING TO DO.

        if (user) {
          setTimeout(async () => {
            if (user && !updateUserLoading && !updateUserSuccess) {
              const name = user.displayName?.split(' ') || [];
              const request: UpdateUserRequest = {
                firstName: name[0],
                lastName: name[1],
                email: user.email!,
                phoneNumber: user.phoneNumber,
                providerId: user.uid,
              };

              await updateUser(request);
            }
          }, 500);
        }


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