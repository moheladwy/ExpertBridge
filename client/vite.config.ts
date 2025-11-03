import { defineConfig } from "vite";
import path from "path";
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
      "@assets": path.resolve(__dirname, "./src/assets"),
      "@views": path.resolve(__dirname, "./src/views"),
    },
  },
  build: {
    // Increase chunk size warning limit
    chunkSizeWarningLimit: 1000,

    // Enable source maps for production debugging
    sourcemap: process.env.NODE_ENV === "production" ? "hidden" : true,

    // Minification settings
    minify: "terser",
    terserOptions: {
      compress: {
        drop_console: process.env.NODE_ENV === "production",
        drop_debugger: true,
      },
    },

    // Rollup specific options
    rollupOptions: {
      output: {
        // Manual chunk strategy
        manualChunks(id) {
          // Core React dependencies
          if (id.includes("node_modules")) {
            // React core libraries
            if (
              id.includes("react-dom") ||
              id.includes("react/jsx") ||
              id.includes("scheduler")
            ) {
              return "react-dom";
            }

            if (id.includes("react-router")) {
              return "react-router";
            }

            if (id.includes("react") && !id.includes("react-dom")) {
              return "react";
            }

            // Redux and state management
            if (id.includes("@reduxjs") || id.includes("redux")) {
              return "redux";
            }

            // Firebase - separate chunk for heavy Firebase SDK
            if (id.includes("firebase")) {
              return "firebase";
            }

            // UI Libraries
            if (id.includes("@mui")) {
              return "mui";
            }

            if (
              id.includes("@radix-ui") ||
              id.includes("@floating-ui") ||
              id.includes("cmdk") ||
              id.includes("vaul")
            ) {
              return "ui-components";
            }

            // Form and validation libraries
            if (
              id.includes("react-hook-form") ||
              id.includes("zod") ||
              id.includes("@hookform")
            ) {
              return "forms";
            }

            // Rich text and markdown
            if (
              id.includes("lexical") ||
              id.includes("markdown") ||
              id.includes("remark") ||
              id.includes("rehype")
            ) {
              return "editor";
            }

            // Date utilities
            if (
              id.includes("date-fns") ||
              id.includes("dayjs") ||
              id.includes("moment")
            ) {
              return "date-utils";
            }

            // Icons
            if (id.includes("lucide") || id.includes("tabler")) {
              return "icons";
            }

            // Animation libraries
            if (
              id.includes("framer-motion") ||
              id.includes("gsap") ||
              id.includes("@lottiefiles")
            ) {
              return "animation";
            }

            // Utility libraries
            if (
              id.includes("lodash") ||
              id.includes("clsx") ||
              id.includes("classnames") ||
              id.includes("tailwind-merge")
            ) {
              return "utils";
            }

            // HTTP and API related
            if (
              id.includes("axios") ||
              id.includes("ky") ||
              id.includes("swr")
            ) {
              return "http";
            }

            // Remaining vendor libraries
            return "vendor";
          }

          // Application code chunking by feature
          if (id.includes("src/features/auth")) {
            return "feature-auth";
          }

          if (id.includes("src/features/posts")) {
            return "feature-posts";
          }

          if (id.includes("src/features/jobs")) {
            return "feature-jobs";
          }

          if (id.includes("src/features/users")) {
            return "feature-users";
          }

          if (id.includes("src/features/comments")) {
            return "feature-comments";
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

          if (extType && /png|jpe?g|svg|gif|tiff|bmp|ico/i.test(extType)) {
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
    include: ["react", "react-dom", "react-router-dom", "@reduxjs/toolkit"],
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
      clientFiles: ["./src/App.tsx", "./src/main.tsx", "./src/routes.tsx"],
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
