import { Badge } from "@/components/ui/badge";

/**
 * Mission section explaining the platform's purpose and values.
 */
export function Mission() {
	return (
		<section className="border-b bg-background py-16 md:py-24">
			<div className="container mx-auto max-w-4xl px-4">
				<div className="space-y-8 text-center">
					{/* Badge */}
					<Badge variant="secondary" className="px-4 py-1.5 text-sm">
						Our Mission
					</Badge>

					{/* Headline */}
					<h2 className="text-3xl font-bold md:text-4xl lg:text-5xl">
						Empowering the Future of Work
					</h2>

					{/* Mission Statement */}
					<div className="mx-auto max-w-3xl space-y-6 text-lg text-muted-foreground">
						<p>
							At ExpertBridge, we believe that talent and
							opportunity should connect seamlessly. Our mission
							is to create a platform where professionals can
							showcase their expertise and businesses can find the
							perfect match for their projects.
						</p>
						<p>
							We&apos;re building more than just a
							marketplace—we&apos;re creating a community where
							knowledge is shared, skills are valued, and
							meaningful collaborations thrive. Whether
							you&apos;re a seasoned expert or just starting your
							journey, ExpertBridge provides the tools and support
							you need to succeed.
						</p>
					</div>

					{/* Values */}
					<div className="mx-auto grid max-w-3xl gap-8 pt-8 md:grid-cols-3">
						<div className="space-y-2">
							<h3 className="text-xl font-semibold">Trust</h3>
							<p className="text-sm text-muted-foreground">
								Built on verified profiles, secure payments, and
								transparent reviews.
							</p>
						</div>
						<div className="space-y-2">
							<h3 className="text-xl font-semibold">Quality</h3>
							<p className="text-sm text-muted-foreground">
								Connecting top talent with opportunities that
								match their expertise.
							</p>
						</div>
						<div className="space-y-2">
							<h3 className="text-xl font-semibold">Community</h3>
							<p className="text-sm text-muted-foreground">
								Fostering a supportive network where everyone
								can grow and succeed.
							</p>
						</div>
					</div>
				</div>
			</div>
		</section>
	);
}
