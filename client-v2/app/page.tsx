import { Hero } from "@/components/sections/hero";
import { Features } from "@/components/sections/features";
import { Mission } from "@/components/sections/mission";
import { CTA } from "@/components/sections/cta";

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
