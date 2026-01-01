import {
	BarChart,
	CodeIcon,
	FileText,
	GlobeIcon,
	Handshake,
	HelpCircle,
	LayersIcon,
	Leaf,
	PlugIcon,
	RotateCcw,
	Shield,
	Star,
	UserPlusIcon,
	Users,
} from "lucide-react";
import type { LinkItemType } from "@/components/sheard";

export const productLinks: LinkItemType[] = [
	{
		label: "Website Builder",
		href: "#",
		description: "Create responsive websites with ease",
		icon: GlobeIcon,
	},
	{
		label: "Cloud Platform",
		href: "#",
		description: "Deploy and scale apps in the cloud",
		icon: LayersIcon,
	},
	{
		label: "Team Collaboration",
		href: "#",
		description: "Tools to help your teams work better together",
		icon: UserPlusIcon,
	},
	{
		label: "Analytics",
		href: "#",
		description: "Track and analyze your website traffic",
		icon: BarChart,
	},
	{
		label: "Integrations",
		href: "#",
		description: "Connect your apps and services",
		icon: PlugIcon,
	},
	{
		label: "API",
		href: "#",
		description: "Build custom integrations with our API",
		icon: CodeIcon,
	},
];

export const companyLinks: LinkItemType[] = [
	{
		label: "About Us",
		href: "#",
		description: "Learn more about our story and team",
		icon: Users,
	},
	{
		label: "Customer Stories",
		href: "#",
		description: "See how we’ve helped our clients succeed",
		icon: Star,
	},
	{
		label: "Partnerships",
		href: "#",
		icon: Handshake,
		description: "Collaborate with us for mutual growth",
	},
];

export const companyLinks2: LinkItemType[] = [
	{
		label: "Terms of Service",
		href: "#",
		icon: FileText,
	},
	{
		label: "Privacy Policy",
		href: "#",
		icon: Shield,
	},
	{
		label: "Refund Policy",
		href: "#",
		icon: RotateCcw,
	},
	{
		label: "Blog",
		href: "#",
		icon: Leaf,
	},
	{
		label: "Help Center",
		href: "#",
		icon: HelpCircle,
	},
];
