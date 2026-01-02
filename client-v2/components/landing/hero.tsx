"use client";

import { Button } from "@/components/ui/button";
import { HugeiconsIcon } from "@hugeicons/react";
import { ArrowRight01Icon, SparklesIcon } from "@hugeicons/core-free-icons";
import Orb from "@/components/ui/Orb";

/**
 * Hero section for the landing page.
 * Features main headline, description, and primary CTAs.
 */
export function Hero() {
	return (
		<section className="relative overflow-hidden border-b bg-background py-20 md:py-28 lg:py-32">
			{/* Background decoration with Orb */}
			<div className="absolute inset-0 -z-1- flex items-center justify-center opacity-60">
				<Orb
					hue={10}
					hoverIntensity={0.3}
					rotateOnHover={true}
					forceHoverState={false}
				/>
			</div>
			<div className="container relative z-10 mx-auto max-w-5xl px-4">
				<div className="mx-auto max-w-3xl text-center">
					{/* Badge */}
					<div className="mb-6 inline-flex items-center gap-2 rounded-full border bg-muted px-3 py-1.5 text-sm">
						<HugeiconsIcon
							icon={SparklesIcon}
							className="h-4 w-4 text-primary"
						/>
						<span className="font-medium">
							Connecting Experts with Opportunities
						</span>
					</div>

					{/* Headline */}
					<h1 className="mb-6 text-4xl font-bold tracking-tight md:text-5xl lg:text-6xl">
						Bridge the Gap Between
						<span className="text-primary"> Talent </span>
						and
						<span className="text-primary"> Opportunity</span>
					</h1>

					{/* Description */}
					<p className="mb-8 text-lg text-muted-foreground md:text-xl">
						ExpertBridge connects skilled professionals with
						businesses seeking expertise. Whether you&apos;re hiring
						or looking for work, find your perfect match in our
						thriving community.
					</p>

					{/* CTAs */}
					<div className="flex flex-col items-center justify-center gap-4 sm:flex-row">
						<Button size="lg" className="w-full sm:w-auto">
							Get Started Free
							<HugeiconsIcon
								icon={ArrowRight01Icon}
								className="ml-2 h-4 w-4"
							/>
						</Button>
						<Button
							size="lg"
							variant="outline"
							className="w-full sm:w-auto"
						>
							Browse Experts
						</Button>
					</div>

					{/* Social Proof */}
					<div className="mt-12 flex flex-col items-center justify-center gap-6 text-sm text-muted-foreground sm:flex-row sm:gap-8">
						<div className="flex items-center gap-2">
							<div className="flex -space-x-2">
								{[1, 2, 3, 4].map((i) => (
									<div
										key={i}
										className="h-8 w-8 rounded-full border-2 border-background bg-muted"
									/>
								))}
							</div>
							<span>10,000+ Experts</span>
						</div>
						<div className="hidden h-4 w-px bg-border sm:block" />
						<div>5,000+ Projects Completed</div>
						<div className="hidden h-4 w-px bg-border sm:block" />
						<div>4.9/5 Average Rating</div>
					</div>
				</div>
			</div>
		</section>
	);
}
