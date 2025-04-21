import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { sendEmailVerification } from "firebase/auth";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { auth } from "@/lib/firebase";

const EmailVerificationPage = () => {
  const [
    isLoggedIn, 
    loading, 
    error, 
    authUser, 
    appUser] = useIsUserLoggedIn();
  const navigate = useNavigate();
  const [sendingEmail, setSendingEmail] = useState(false);
  const [emailSent, setEmailSent] = useState(false);
  
  useEffect(() => {
    console.log("Auth User from the EmailVerificationPage: ", authUser);
    // If email is verified in both places, redirect to feed
    if (!loading && authUser?.emailVerified) {
      navigate("/home");
    }
    
    // // If not logged in, redirect to login
    // if (!loading && !isLoggedIn) {
    //   navigate("/login");
    //   return;
    // }
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
      // Force refresh the token to get updated email verification status
      if (authUser) {
        await authUser.reload();
        window.location.reload();
      }
    } catch (error) {
      console.error("Error refreshing user data:", error);
    }
  };

  if (loading) {
    return <div className="flex justify-center items-center h-screen">Loading...</div>;
  }

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4">
      <div className="max-w-md w-full bg-white p-8 rounded-lg shadow-md">
        <h1 className="text-2xl font-bold mb-4">Email Verification Required</h1>
        <p className="mb-6">
          Please verify your email address to continue using the application.
          Check your inbox for a verification link.
        </p>
        
        <div className="space-y-4">
          <button
            onClick={handleSendVerificationEmail}
            disabled={sendingEmail}
            className="w-full bg-blue-500 hover:bg-blue-600 text-white py-2 px-4 rounded disabled:bg-gray-400"
          >
            {sendingEmail ? "Sending..." : emailSent ? "Email Sent! Check Your Inbox" : "Resend Verification Email"}
          </button>
          
          <button
            onClick={handleCheckVerification}
            className="w-full bg-gray-200 hover:bg-gray-300 text-gray-800 py-2 px-4 rounded"
          >
            I've Verified My Email
          </button>
        </div>
      </div>
    </div>
  );
};

export default EmailVerificationPage;