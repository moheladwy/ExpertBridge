import {
	ActionCodeSettings,
	AuthError,
	CustomParameters,
	UserCredential,
} from "firebase/auth";

/**
 * Generic authentication action hook return type.
 * @template Callback - The callback function type for the auth action
 *
 * Tuple elements:
 * - [0] callback: The function to trigger the auth action
 * - [1] credential: The resulting UserCredential after successful auth
 * - [2] loading: Whether the auth action is in progress
 * - [3] error: Any AuthError that occurred during the action
 */
export type AuthActionHook<Callback> = [
	/** The function to trigger the auth action */
	callback: Callback,
	/** The resulting UserCredential after successful auth */
	credential: UserCredential | undefined,
	/** Whether the auth action is in progress */
	loading: boolean,
	/** Any AuthError that occurred during the action */
	error: AuthError | undefined
];

export type CreateUserOptions = {
	emailVerificationOptions?: ActionCodeSettings;
	sendEmailVerification?: boolean;
};

export type EmailAndPasswordActionHook = AuthActionHook<
	(email: string, password: string) => Promise<UserCredential | undefined>
>;

export type SignInWithEmailLinkHook = AuthActionHook<
	(email: string, emailLink?: string) => Promise<UserCredential | undefined>
>;

export type SignInWithPopupHook = AuthActionHook<
	(
		scopes?: string[],
		customOAuthParameters?: CustomParameters
	) => Promise<UserCredential | undefined>
>;
