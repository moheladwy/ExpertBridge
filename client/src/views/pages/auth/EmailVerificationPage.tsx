import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { sendEmailVerification } from "firebase/auth";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

const EmailVerificationPage = () => {
	const [isLoggedIn, loading, error, authUser, appUser] = useIsUserLoggedIn();
	const navigate = useNavigate();
	const [sendingEmail, setSendingEmail] = useState(false);
	const [emailSent, setEmailSent] = useState(false);

	useEffect(() => {
		console.log("Auth User from the EmailVerificationPage: ", authUser);
		// If email is verified in both places, redirect to feed
		if (!loading && authUser?.emailVerified) {
			navigate("/home");
		}
	}, [isLoggedIn, loading, authUser, appUser, navigate]);

	const handleSendVerificationEmail = async () => {
		if (!authUser) return;
		try {
			setSendingEmail(true);
			await sendEmailVerification(authUser);
			setEmailSent(true);
		} catch (error) {
			console.error("Error sending verification email:", error);
		} finally {
			setSendingEmail(false);
		}
	};

	const handleCheckVerification = async () => {
		try {
			navigate("/login");
		} catch (error) {
			console.error("Error refreshing user data:", error);
		}
	};
	return (
		<div className="flex flex-col items-center justify-center min-h-screen p-4">
			<div className="max-w-md w-full bg-card p-8 rounded-lg shadow-md border border-border">
				<h1 className="text-2xl font-bold mb-4 text-card-foreground">
					Email Verification Required
				</h1>
				<p className="mb-6 text-muted-foreground">
					Please verify your email address to continue using the
					application. Check your inbox for a verification link.
				</p>

				<div className="space-y-4">
					<button
						onClick={handleSendVerificationEmail}
						disabled={sendingEmail}
						className="w-full bg-primary hover:bg-primary/90 text-primary-foreground py-2 px-4 rounded-full disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
					>
						{sendingEmail
							? "Sending..."
							: emailSent
								? "Email Sent! Check Your Inbox"
								: "Resend Verification Email"}
					</button>

					<button
						onClick={handleCheckVerification}
						className="w-full bg-secondary hover:bg-secondary/80 text-secondary-foreground py-2 px-4 rounded-full transition-colors"
					>
						I've Verified My Email
					</button>
				</div>
			</div>
		</div>
	);
};

export default EmailVerificationPage;
