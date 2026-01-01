import { initializeApp } from "firebase/app";
import { getAuth } from "firebase/auth";
import { getMessaging } from "firebase/messaging";

const firebaseConfig = {
	apiKey: process.env.VITE_API_KEY,
	authDomain: process.env.VITE_AUTH_DOMAIN,
	projectId: process.env.VITE_PROJECT_ID,
	storageBucket: process.env.VITE_STORAGE_BUCKET,
	messagingSenderId: process.env.VITE_MESSAGING_SENDER_ID,
	appId: process.env.VITE_APP_ID,
	measurementId: process.env.VITE_MEASUREMENT_ID,
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const messaging = getMessaging(app);

export { app, auth, messaging };
