import type { Metadata } from "next";
import { Badge } from "@/components/ui/badge";

export const metadata: Metadata = {
	title: "About Us - ExpertBridge",
	description:
		"Learn about ExpertBridge's mission to connect talent with opportunity",
};

export default function AboutPage() {
	return (
		<div className="container mx-auto max-w-4xl px-4 py-16 md:py-24">
			{/* Header */}
			<div className="mb-12 text-center">
				<Badge variant="secondary" className="mb-4 px-4 py-1.5 text-sm">
					About Us
				</Badge>
				<h1 className="mb-4 text-4xl font-bold md:text-5xl">
					Connecting Talent with Opportunity
				</h1>
				<p className="text-lg text-muted-foreground">
					Building the future of professional collaboration
				</p>
			</div>

			{/* Story Section */}
			<div className="space-y-12">
				<section className="space-y-4">
					<h2 className="text-2xl font-bold md:text-3xl">
						Our Story
					</h2>
					<div className="space-y-4 text-lg text-muted-foreground">
						<p>
							ExpertBridge was founded with a simple belief: that
							talent and opportunity should connect seamlessly,
							regardless of location or background. We saw a gap
							in the market—a need for a platform that truly
							understands both the challenges businesses face in
							finding the right expertise and the aspirations of
							professionals seeking meaningful work.
						</p>
						<p>
							What started as an idea to make hiring easier has
							evolved into a thriving community where knowledge is
							shared, skills are celebrated, and professional
							relationships flourish. Today, we&apos;re proud to
							serve thousands of experts and businesses across
							industries.
						</p>
					</div>
				</section>

				{/* Values Section */}
				<section className="space-y-6">
					<h2 className="text-2xl font-bold md:text-3xl">
						Our Values
					</h2>
					<div className="grid gap-6 md:grid-cols-2">
						<div className="rounded-lg border bg-card p-6">
							<h3 className="mb-2 text-xl font-semibold">
								Trust & Transparency
							</h3>
							<p className="text-muted-foreground">
								We believe in building relationships on verified
								profiles, honest reviews, and transparent
								processes. Trust is the foundation of every
								successful collaboration.
							</p>
						</div>
						<div className="rounded-lg border bg-card p-6">
							<h3 className="mb-2 text-xl font-semibold">
								Quality Over Quantity
							</h3>
							<p className="text-muted-foreground">
								We focus on meaningful connections between top
								talent and opportunities that match their
								expertise. Quality interactions lead to better
								outcomes for everyone.
							</p>
						</div>
						<div className="rounded-lg border bg-card p-6">
							<h3 className="mb-2 text-xl font-semibold">
								Community First
							</h3>
							<p className="text-muted-foreground">
								Our platform is more than a
								marketplace—it&apos;s a community where
								professionals support each other, share
								knowledge, and grow together.
							</p>
						</div>
						<div className="rounded-lg border bg-card p-6">
							<h3 className="mb-2 text-xl font-semibold">
								Innovation & Growth
							</h3>
							<p className="text-muted-foreground">
								We continuously evolve our platform with new
								features and tools that empower professionals
								and businesses to succeed in an ever-changing
								world.
							</p>
						</div>
					</div>
				</section>

				{/* Mission Section */}
				<section className="space-y-4">
					<h2 className="text-2xl font-bold md:text-3xl">
						Our Mission
					</h2>
					<p className="text-lg text-muted-foreground">
						To empower professionals and businesses by creating a
						trusted platform where expertise meets opportunity.
						We&apos;re committed to making it easier for talented
						individuals to showcase their skills and for
						organizations to find the perfect match for their
						projects.
					</p>
				</section>

				{/* Stats Section */}
				<section className="rounded-lg border bg-muted/30 p-8">
					<h2 className="mb-6 text-center text-2xl font-bold md:text-3xl">
						Our Impact
					</h2>
					<div className="grid gap-8 text-center md:grid-cols-3">
						<div>
							<div className="mb-2 text-4xl font-bold text-primary">
								10,000+
							</div>
							<div className="text-muted-foreground">
								Verified Experts
							</div>
						</div>
						<div>
							<div className="mb-2 text-4xl font-bold text-primary">
								5,000+
							</div>
							<div className="text-muted-foreground">
								Projects Completed
							</div>
						</div>
						<div>
							<div className="mb-2 text-4xl font-bold text-primary">
								4.9/5
							</div>
							<div className="text-muted-foreground">
								Average Rating
							</div>
						</div>
					</div>
				</section>

				{/* Team Section */}
				<section className="space-y-4">
					<h2 className="text-2xl font-bold md:text-3xl">
						Join Our Journey
					</h2>
					<p className="text-lg text-muted-foreground">
						Whether you&apos;re an expert looking to showcase your
						skills or a business seeking talented professionals,
						ExpertBridge is here to help you succeed. Join our
						growing community and be part of the future of work.
					</p>
				</section>
			</div>
		</div>
	);
}
