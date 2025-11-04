/** @type {import('tailwindcss').Config} */
export default {
	darkMode: ["class"],
	content: ["./index.html", "./src/**/*.{ts,tsx,js,jsx}"],
	theme: {
		extend: {
			colors: {
				border: "hsl(var(--border))",
				input: "hsl(var(--input))",
				ring: "hsl(var(--ring))",
				background: "hsl(var(--background))",
				foreground: "hsl(var(--foreground))",
				primary: {
					DEFAULT: "hsl(var(--primary))",
					foreground: "hsl(var(--primary-foreground))",
				},
				secondary: {
					DEFAULT: "hsl(var(--secondary))",
					foreground: "hsl(var(--secondary-foreground))",
				},
				destructive: {
					DEFAULT: "hsl(var(--destructive))",
					foreground: "hsl(var(--destructive-foreground))",
				},
				muted: {
					DEFAULT: "hsl(var(--muted))",
					foreground: "hsl(var(--muted-foreground))",
				},
				accent: {
					DEFAULT: "hsl(var(--accent))",
					foreground: "hsl(var(--accent-foreground))",
				},
				card: {
					DEFAULT: "hsl(var(--card))",
					foreground: "hsl(var(--card-foreground))",
				},
				popover: {
					DEFAULT: "hsl(var(--popover))",
					foreground: "hsl(var(--popover-foreground))",
				},
				// State colors
				success: {
					DEFAULT: "hsl(var(--success))",
					foreground: "hsl(var(--success-foreground))",
					muted: "hsl(var(--success-muted))",
				},
				warning: {
					DEFAULT: "hsl(var(--warning))",
					foreground: "hsl(var(--warning-foreground))",
					muted: "hsl(var(--warning-muted))",
				},
				info: {
					DEFAULT: "hsl(var(--info))",
					foreground: "hsl(var(--info-foreground))",
					muted: "hsl(var(--info-muted))",
				},
				// Vote colors
				upvote: {
					DEFAULT: "hsl(var(--upvote))",
					active: "hsl(var(--upvote-active))",
				},
				downvote: {
					DEFAULT: "hsl(var(--downvote))",
					active: "hsl(var(--downvote-active))",
				},
				"vote-neutral": "hsl(var(--vote-neutral))",
				// Badge/Tag colors
				badge: {
					DEFAULT: "hsl(var(--badge-default))",
					foreground: "hsl(var(--badge-foreground))",
					skill: "hsl(var(--badge-skill))",
					"skill-foreground": "hsl(var(--badge-skill-foreground))",
					language: "hsl(var(--badge-language))",
					"language-foreground":
						"hsl(var(--badge-language-foreground))",
				},
				// Navigation brand
				"nav-brand": {
					DEFAULT: "hsl(var(--nav-brand))",
					hover: "hsl(var(--nav-brand-hover))",
				},
				// Chart colors
				chart: {
					1: "hsl(var(--chart-1))",
					2: "hsl(var(--chart-2))",
					3: "hsl(var(--chart-3))",
					4: "hsl(var(--chart-4))",
					5: "hsl(var(--chart-5))",
				},
				// Sidebar colors
				sidebar: {
					DEFAULT: "hsl(var(--sidebar))",
					foreground: "hsl(var(--sidebar-foreground))",
					primary: "hsl(var(--sidebar-primary))",
					"primary-foreground":
						"hsl(var(--sidebar-primary-foreground))",
					accent: "hsl(var(--sidebar-accent))",
					"accent-foreground":
						"hsl(var(--sidebar-accent-foreground))",
					border: "hsl(var(--sidebar-border))",
					ring: "hsl(var(--sidebar-ring))",
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
