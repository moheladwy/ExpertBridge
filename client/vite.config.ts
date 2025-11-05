import { defineConfig, type Plugin } from "vite";
import path from "path";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "@tailwindcss/vite";

// Plugin to add preload links for critical chunks
function preloadCriticalChunks(): Plugin {
	return {
		name: "preload-critical-chunks",
		transformIndexHtml(html, ctx) {
			// Only apply in production builds
			if (!ctx.bundle) return html;

			const preloadChunks = [
				"firebase",
				"redux",
				"fonts",
				"ui",
				"vendor",
			];
			const preloadLinks: string[] = [];

			// Find the chunk files for our critical chunks
			for (const [fileName, chunk] of Object.entries(ctx.bundle)) {
				if (chunk.type === "chunk") {
					const chunkName = chunk.name;
					if (preloadChunks.includes(chunkName)) {
						preloadLinks.push(
							`<link rel="modulepreload" href="/${fileName}" />`
						);
					}
				}
			}

			// Insert preload links before closing </head> tag
			if (preloadLinks.length > 0) {
				html = html.replace(
					"</head>",
					`  ${preloadLinks.join("\n  ")}\n  </head>`
				);
			}

			return html;
		},
	};
}

export default defineConfig({
	plugins: [react(), tailwindcss(), preloadCriticalChunks()],
	base: "/",
	resolve: {
		alias: {
			"@": path.resolve(__dirname, "./src"),
			"@assets": path.resolve(__dirname, "./src/assets"),
			"@views": path.resolve(__dirname, "./src/views"),
		},
		dedupe: ["react", "react-dom", "lucide-react"],
	},
	build: {
		// Increase chunk size warning limit
		chunkSizeWarningLimit: 500,

		// Enable source maps for production debugging
		sourcemap: process.env.NODE_ENV === "production" ? "hidden" : true,

		// Minification settings
		minify: "terser",
		terserOptions: {
			compress: {
				drop_console: process.env.NODE_ENV === "production",
				drop_debugger: true,
			},
			format: {
				comments: false,
			},
			mangle: {
				safari10: true,
			},
		},

		// Ensure proper module format
		modulePreload: {
			polyfill: true,
		},

		// CommonJS options
		commonjsOptions: {
			include: [/node_modules/],
			transformMixedEsModules: true,
		},

		// Rollup specific options
		rollupOptions: {
			output: {
				// Simplified manual chunk strategy - 5 main chunks + pages
				manualChunks(id) {
					if (id.includes("node_modules")) {
						// Firebase chunk: Firebase SDK
						if (id.includes("firebase")) {
							return "firebase";
						}

						// Redux chunk: Redux and related
						if (
							id.includes("@reduxjs") ||
							id.includes("react-redux") ||
							id.includes("redux-persist")
						) {
							return "redux";
						}

						if (id.includes("@fontsource/roboto")) {
							return "fonts";
						}

						if (
							id.includes("@radix-ui") ||
							id.includes("cmdk") ||
							id.includes("vaul") ||
							id.includes("lucide-react") ||
							id.includes("@tabler/icons-react") ||
							id.includes("@heroicons/react") ||
							id.includes("next-themes") ||
							id.includes("framer-motion") ||
							id.includes("motion") ||
							id.includes("class-variance-authority") ||
							id.includes("clsx") ||
							id.includes("sonner") ||
							id.includes("tailwindcss") ||
							id.includes("tw-animate-css") ||
							id.includes("tailwindcss-animate") ||
							id.includes("tailwind-merge")
						) {
							return "ui";
						}

						// Vendor chunk: Everything else from node_modules
						return "vendor";
					}
				},

				// Configure chunk file naming
				chunkFileNames: (chunkInfo) => {
					const facadeModuleId = chunkInfo.facadeModuleId
						? chunkInfo.facadeModuleId.split("/").pop()
						: "chunk";
					return `assets/js/[name]-${facadeModuleId}-[hash].js`;
				},

				// Entry file naming
				entryFileNames: "assets/js/[name]-[hash].js",

				// Asset file naming
				assetFileNames: (assetInfo) => {
					if (!assetInfo.name) {
						return `assets/[name]-[hash][extname]`;
					}

					const extType = assetInfo.name.split(".").at(1);

					if (
						extType &&
						/png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)
					) {
						return `assets/images/[name]-[hash][extname]`;
					}

					if (extType && /woff2?|ttf|eot|otf/i.test(extType)) {
						return `assets/fonts/[name]-[hash][extname]`;
					}

					if (extType === "css") {
						return `assets/css/[name]-[hash][extname]`;
					}

					return `assets/[name]-[hash][extname]`;
				},
			},
		},

		// CSS code splitting
		cssCodeSplit: true,

		// Asset inlining threshold (4kb)
		assetsInlineLimit: 4096,

		// Generate manifest for caching
		manifest: true,

		// Report compressed size
		reportCompressedSize: false,
	},

	// Optimize dependencies
	optimizeDeps: {
		include: [
			"react",
			"react-dom",
			"react-router-dom",
			"@reduxjs/toolkit",
			"lucide-react",
			"@tabler/icons-react",
		],
		exclude: ["@vite/client", "@vite/env"],
	},

	// Server configuration for development
	server: {
		port: 5173,
		strictPort: false,
		host: true,
		open: false,
		cors: true,
		// Warm up frequently used files
		warmup: {
			clientFiles: [
				"./src/App.tsx",
				"./src/main.tsx",
				"./src/routes.tsx",
			],
		},
	},

	// Preview configuration
	preview: {
		port: 4173,
		strictPort: false,
		host: true,
		open: false,
		cors: true,
	},
});
