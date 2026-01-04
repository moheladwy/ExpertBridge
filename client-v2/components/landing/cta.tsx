import Link from "next/link";
import { HugeiconsIcon } from "@hugeicons/react";
import { ArrowRight01Icon } from "@hugeicons/core-free-icons";
import { Button } from "@/components/ui/button";

/**
 * Call-to-action section encouraging users to sign up.
 */
export function CTA() {
	return (
		<section className="border-b bg-primary/5 py-16 md:py-20">
			<div className="container mx-auto max-w-4xl px-4">
				<div className="relative overflow-hidden rounded-2xl border bg-gradient-to-br from-primary/10 to-primary/5 p-8 md:p-12">
					<div className="relative z-10 text-center">
						{/* Headline */}
						<h2 className="mb-4 text-3xl font-bold md:text-4xl lg:text-5xl">
							Ready to Bridge the Gap?
						</h2>

						{/* Description */}
						<p className="mb-8 text-lg text-muted-foreground md:text-xl">
							Join thousands of professionals and businesses
							already using ExpertBridge to connect, collaborate,
							and succeed.
						</p>

						{/* CTAs */}
						<div className="flex flex-col items-center justify-center gap-4 sm:flex-row">
							<Button size="lg" className="w-full sm:w-auto">
								<Link
									href="/auth/signup"
									className="flex items-center"
								>
									Start Your Journey
									<HugeiconsIcon
										icon={ArrowRight01Icon}
										className="ml-2 h-4 w-4"
									/>
								</Link>
							</Button>
							<Button
								variant="outline"
								size="lg"
								className="w-full sm:w-auto"
							>
								<Link href="/about">Learn More</Link>
							</Button>
						</div>

						{/* Trust Signal */}
						<p className="mt-6 text-sm text-muted-foreground">
							Free to join • No credit card required • Get started
							in minutes
						</p>
					</div>

					{/* Background decoration */}
					<div className="absolute inset-0 -z-10 bg-[radial-gradient(circle_at_50%_50%,hsl(var(--primary)/0.1),transparent_50%)]" />
				</div>
			</div>
		</section>
	);
}
