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
import {
	createTheme,
	StyledEngineProvider,
	ThemeProvider,
} from "@mui/material/styles";

const rootElement = document.getElementById("root");
const root = createRoot(rootElement!);

const theme = createTheme({
	cssVariables: true,
	components: {
		MuiPopover: {
			defaultProps: {
				container: rootElement,
			},
		},
		MuiPopper: {
			defaultProps: {
				container: rootElement,
			},
		},
	},
});

root.render(
	<StrictMode>
		<StyledEngineProvider injectFirst>
			<ThemeProvider theme={theme}>
				<ReduxProvider store={store}>
					<RouterProvider router={router} />
				</ReduxProvider>
			</ThemeProvider>
		</StyledEngineProvider>
	</StrictMode>
);
