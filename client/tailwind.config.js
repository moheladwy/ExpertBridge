/** @type {import('tailwindcss').Config} */
export default {
	darkMode: ["class"],
	content: ["./index.html", "./src/**/*.{ts,tsx,js,jsx}"],
	theme: {
		extend: {
			colors: {
				border: "oklch(var(--border))",
				input: "oklch(var(--input))",
				ring: "oklch(var(--ring))",
				background: "oklch(var(--background))",
				foreground: "oklch(var(--foreground))",
				primary: {
					DEFAULT: "oklch(var(--primary))",
					foreground: "oklch(var(--primary-foreground))",
				},
				secondary: {
					DEFAULT: "oklch(var(--secondary))",
					foreground: "oklch(var(--secondary-foreground))",
				},
				destructive: {
					DEFAULT: "oklch(var(--destructive))",
					foreground: "oklch(var(--destructive-foreground))",
				},
				muted: {
					DEFAULT: "oklch(var(--muted))",
					foreground: "oklch(var(--muted-foreground))",
				},
				accent: {
					DEFAULT: "oklch(var(--accent))",
					foreground: "oklch(var(--accent-foreground))",
				},
				card: {
					DEFAULT: "oklch(var(--card))",
					foreground: "oklch(var(--card-foreground))",
				},
				popover: {
					DEFAULT: "oklch(var(--popover))",
					foreground: "oklch(var(--popover-foreground))",
				},
				// State colors
				success: {
					DEFAULT: "oklch(var(--success))",
					foreground: "oklch(var(--success-foreground))",
					muted: "oklch(var(--success-muted))",
				},
				warning: {
					DEFAULT: "oklch(var(--warning))",
					foreground: "oklch(var(--warning-foreground))",
					muted: "oklch(var(--warning-muted))",
				},
				info: {
					DEFAULT: "oklch(var(--info))",
					foreground: "oklch(var(--info-foreground))",
					muted: "oklch(var(--info-muted))",
				},
				// Vote colors
				upvote: {
					DEFAULT: "oklch(var(--upvote))",
					active: "oklch(var(--upvote-active))",
				},
				downvote: {
					DEFAULT: "oklch(var(--downvote))",
					active: "oklch(var(--downvote-active))",
				},
				"vote-neutral": "oklch(var(--vote-neutral))",
				// Badge/Tag colors
				badge: {
					DEFAULT: "oklch(var(--badge-default))",
					foreground: "oklch(var(--badge-foreground))",
					skill: "oklch(var(--badge-skill))",
					"skill-foreground": "oklch(var(--badge-skill-foreground))",
					language: "oklch(var(--badge-language))",
					"language-foreground":
						"oklch(var(--badge-language-foreground))",
				},
				// Navigation brand
				"nav-brand": {
					DEFAULT: "oklch(var(--nav-brand))",
					hover: "oklch(var(--nav-brand-hover))",
				},
				// Chart colors
				chart: {
					1: "oklch(var(--chart-1))",
					2: "oklch(var(--chart-2))",
					3: "oklch(var(--chart-3))",
					4: "oklch(var(--chart-4))",
					5: "oklch(var(--chart-5))",
				},
				// Sidebar colors
				sidebar: {
					DEFAULT: "oklch(var(--sidebar))",
					foreground: "oklch(var(--sidebar-foreground))",
					primary: "oklch(var(--sidebar-primary))",
					"primary-foreground":
						"oklch(var(--sidebar-primary-foreground))",
					accent: "oklch(var(--sidebar-accent))",
					"accent-foreground":
						"oklch(var(--sidebar-accent-foreground))",
					border: "oklch(var(--sidebar-border))",
					ring: "oklch(var(--sidebar-ring))",
				},
			},
			borderRadius: {
				lg: "var(--radius)",
				md: "calc(var(--radius) - 2px)",
				sm: "calc(var(--radius) - 4px)",
			},
			boxShadow: {
				sm: "var(--shadow-sm)",
				DEFAULT: "var(--shadow)",
				md: "var(--shadow-md)",
				lg: "var(--shadow-lg)",
				xl: "var(--shadow-xl)",
			},
			transitionDuration: {
				fast: "var(--transition-fast)",
				base: "var(--transition-base)",
				slow: "var(--transition-slow)",
			},
		},
	},
	plugins: [require("tailwindcss-animate")],
};
