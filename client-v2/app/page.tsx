import { Header } from "@/components/header";
import { Footer } from "@/components/footer";
import { Hero } from "@/components/sections/hero";
import { Features } from "@/components/sections/features";
import { Mission } from "@/components/sections/mission";
import { CTA } from "@/components/sections/cta";

/**
 * Landing page for ExpertBridge.
 *
 * Sections:
 * 1. Header - Navigation and branding
 * 2. Hero - Main headline and CTAs
 * 3. Features - Key platform capabilities
 * 4. Mission - Purpose and values
 * 5. CTA - Final call-to-action
 * 6. Footer - Links and social
 */
export default function Page() {
	return (
		<>
			<Header />
			<main>
				<Hero />
				<Features />
				<Mission />
				<CTA />
			</main>
			<Footer />
		</>
	);
}
