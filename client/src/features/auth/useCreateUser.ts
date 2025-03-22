import { Auth, AuthError, CustomParameters, UserCredential } from "firebase/auth";
import { AppUser, CreateUserError, CreateUserRequest, UpdateUserRequest, UserFormData } from "../users/types";
import { useSignInWithGoogle } from "@/lib/firebase/useSignInWithPopup";
import { useUpdateUserMutation } from "../users/usersSlice";
import { useCallback, useEffect, useState } from "react";
import useCreateUserWithEmailAndPassword from "@/lib/firebase/EmailAuth/useCreateUserWithEmailAndPassword";

export type GoogleCallback =
  (scopes?: string[], customOAuthParameters?: CustomParameters)
    => Promise<UserCredential | undefined>

export type EmailCallback = (
  email: string,
  password: string,
  user: UserFormData
)
  => Promise<UserCredential | undefined>

export type CreateUserWithGoogleHook = [
  GoogleCallback,
  EmailCallback,
  UserCredential | undefined,
  AppUser | undefined,
  boolean, // loading
  AuthError | undefined,
  CreateUserError, // createUserError from the rtkq hook
  boolean // createUserSuccess
];

export type AuthType = 'email' | 'google';

export const useCreateUser = (auth: Auth): CreateUserWithGoogleHook => {
  const [
    signInWithGoogle,
    googleUser,
    googleLoading,
    googleError
  ] = useSignInWithGoogle(auth);

  // Email/Password Sign-Up Hook
  const [
    registerWithEmailAndPassword,
    emailUser,
    emailLoading,
    emailError
  ] = useCreateUserWithEmailAndPassword(auth, { sendEmailVerification: true });

  const [createAppUser, result] = useUpdateUserMutation();
  const {
    isLoading: createUserLoading,
    isError: createUserError,
    isSuccess: createUserSuccess,
    data: createdUser,
  } = result;

  const [createUserErrorMessage, setCreateUserErrorMessage] = useState<CreateUserError>();
  const [userInfo, setUserInfo] = useState<UserFormData>();

  const createWithEmail = useCallback(async (email: string, passowrd: string, userInfo: UserFormData) => {
    setUserInfo(userInfo);

    return await registerWithEmailAndPassword(email, passowrd);
  }, [registerWithEmailAndPassword]);

  const create = useCallback(async (user: CreateUserRequest | UpdateUserRequest) => {
    await createAppUser(user);
  }, [createAppUser]);

  useEffect(() => {
    console.log('useCreateUser Hook');
    if (emailUser && userInfo) {
      const user: UpdateUserRequest = {
        firstName: userInfo.firstName,
        lastName: userInfo.lastName,
        email: emailUser.user.email!,
        username: userInfo.email,
        providerId: emailUser.user.uid,
      }

      create(user);
    }
    else if (googleUser) {
      const name = googleUser.user.displayName!.split(' ');
      console.log(googleUser.user.uid);

      const user: UpdateUserRequest = {
        firstName: name[0],
        lastName: name[1],
        email: googleUser.user.email!,
        username: googleUser.user.email!,
        phoneNumber: googleUser.user.phoneNumber,
        providerId: googleUser.user.uid,
      }

      console.log('updating user');

      create(user);
    }
  }, [googleUser, emailUser, userInfo, create]);

  const handleCreateBackendUserError = useCallback(async () => {
    // Delete the user from firebase if api user creation fails.
    await auth.currentUser?.delete();
    await auth.signOut();
  }, [auth]);

  useEffect(() => {
    if (createUserError) {
      handleCreateBackendUserError();

      // TODO: use createUserError to extract useful info and give it to the user
      // from the response. createUserError is only a boolean

      setCreateUserErrorMessage('An error occurred while creating your account. Please try again.');
      console.log(createUserError);
    }
  }, [createUserError, handleCreateBackendUserError]);

  return [
    signInWithGoogle,
    createWithEmail,
    googleUser || emailUser,
    createdUser,
    googleLoading || createUserLoading || emailLoading,
    googleError || emailError,
    createUserErrorMessage,
    createUserSuccess
  ];
}
