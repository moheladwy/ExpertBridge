import { Header } from "@/components/header/header";
import { Footer } from "@/components/shared/footer";

/**
 * Layout for main pages that include header and footer.
 * Used by: landing, about, privacy, profile, etc.
 */
export default function MainLayout({
	children,
}: {
	children: React.ReactNode;
}) {
	return (
		<>
			<Header />
			<main>{children}</main>
			<Footer />
		</>
	);
}
