import {
	Card,
	CardContent,
	CardDescription,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import { HugeiconsIcon } from "@hugeicons/react";
import {
	UserIcon,
	SearchIcon,
	ShieldCheck,
	Briefcase01Icon,
	MessageIcon,
	StarIcon,
} from "@hugeicons/core-free-icons";

const features = [
	{
		icon: UserIcon,
		title: "Expert Profiles",
		description:
			"Showcase your skills, experience, and portfolio to attract the right clients.",
	},
	{
		icon: SearchIcon,
		title: "Smart Matching",
		description:
			"AI-powered matching connects you with opportunities that fit your expertise.",
	},
	{
		icon: Briefcase01Icon,
		title: "Job Marketplace",
		description:
			"Browse thousands of projects and find work that matches your skills.",
	},
	{
		icon: MessageIcon,
		title: "Direct Communication",
		description:
			"Connect directly with clients or experts through our messaging system.",
	},
	{
		icon: ShieldCheck,
		title: "Secure Payments",
		description:
			"Safe and secure payment processing with escrow protection for both parties.",
	},
	{
		icon: StarIcon,
		title: "Ratings & Reviews",
		description:
			"Build your reputation with verified reviews from completed projects.",
	},
];

/**
 * Features section showcasing key platform capabilities.
 */
export function Features() {
	return (
		<section className="border-b bg-muted/30 py-16 md:py-24">
			<div className="container mx-auto max-w-5xl px-4">
				{/* Section Header */}
				<div className="mb-12 text-center">
					<h2 className="mb-4 text-3xl font-bold md:text-4xl">
						Everything You Need to Succeed
					</h2>
					<p className="mx-auto max-w-2xl text-lg text-muted-foreground">
						Powerful features designed to make hiring and finding
						work effortless.
					</p>
				</div>

				{/* Features Grid */}
				<div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
					{features.map((feature) => (
						<Card
							key={feature.title}
							className="border-border/50 transition-shadow hover:shadow-md"
						>
							<CardHeader>
								<div className="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
									<HugeiconsIcon
										icon={feature.icon}
										className="h-6 w-6 text-primary"
									/>
								</div>
								<CardTitle className="text-xl">
									{feature.title}
								</CardTitle>
							</CardHeader>
							<CardContent>
								<CardDescription className="text-base">
									{feature.description}
								</CardDescription>
							</CardContent>
						</Card>
					))}
				</div>
			</div>
		</section>
	);
}
