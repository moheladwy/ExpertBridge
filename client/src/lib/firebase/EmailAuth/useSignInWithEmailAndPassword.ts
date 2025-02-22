import {
  Auth,
  AuthError,
  signInWithEmailAndPassword as firebaseSignInWithEmailAndPassword,
  UserCredential,
} from 'firebase/auth';
import { useCallback, useState } from 'react';
import { EmailAndPasswordActionHook } from '../types';

export default (auth: Auth): EmailAndPasswordActionHook => {
  const [error, setError] = useState<AuthError>();
  const [loggedInUser, setLoggedInUser] = useState<UserCredential>();
  const [loading, setLoading] = useState<boolean>(false);

  const signInWithEmailAndPassword = useCallback(
    async (email: string, password: string) => {
      setLoading(true);
      setError(undefined);

      try {
        const userCredential = await firebaseSignInWithEmailAndPassword(
          auth,
          email,
          password
        );

        if (userCredential.user.emailVerified === false) { 
          throw new Error('Email is not verified');
        }

        setLoggedInUser(userCredential);

        return userCredential;
      }
      catch (err) {
        setError(err as AuthError);
      }
      finally {
        setLoading(false);
      }
    },
    [auth]
  );

  return [signInWithEmailAndPassword, loggedInUser, loading, error];
};