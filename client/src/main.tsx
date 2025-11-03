import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import "./index.css";
import { store } from "./app/store.ts";
import { router } from "./routes.tsx";
import { Provider as ReduxProvider } from "react-redux";
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";

const rootElement = document.getElementById("root");
const root = createRoot(rootElement!);

root.render(
	<StrictMode>
		<ReduxProvider store={store}>
			<RouterProvider router={router} />
		</ReduxProvider>
	</StrictMode>
);
