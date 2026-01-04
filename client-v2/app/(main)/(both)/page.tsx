import { Hero } from "@/components/landing/hero";
import { Features } from "@/components/landing/features";
import { Mission } from "@/components/landing/mission";
import { CTA } from "@/components/landing/cta";

/**
 * Landing page for ExpertBridge.
 */
export default function Page() {
	return (
		<>
			<Hero />
			<Features />
			<Mission />
			<CTA />
		</>
	);
}
