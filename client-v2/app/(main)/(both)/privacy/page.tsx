import type { Metadata } from "next";
import { Badge } from "@/components/ui/badge";

export const metadata: Metadata = {
	title: "Privacy Policy - ExpertBridge",
	description:
		"ExpertBridge Privacy Policy - How we collect, use, and protect your data",
};

export default function PrivacyPolicyPage() {
	return (
		<div className="container mx-auto max-w-4xl px-4 py-16 md:py-24">
			{/* Header */}
			<div className="mb-12">
				<Badge variant="secondary" className="mb-4 px-4 py-1.5 text-sm">
					Legal
				</Badge>
				<h1 className="mb-4 text-4xl font-bold md:text-5xl">
					Privacy Policy
				</h1>
				<p className="text-muted-foreground">
					Last updated: January 1, 2026
				</p>
			</div>

			{/* Content */}
			<div className="prose prose-gray max-w-none dark:prose-invert">
				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">1. Introduction</h2>
					<p className="text-muted-foreground">
						Welcome to ExpertBridge. We respect your privacy and are
						committed to protecting your personal data. This privacy
						policy will inform you about how we look after your
						personal data when you visit our platform and tell you
						about your privacy rights and how the law protects you.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						2. Information We Collect
					</h2>
					<p className="text-muted-foreground">
						We collect several types of information from and about
						users of our platform:
					</p>
					<ul className="list-disc space-y-2 pl-6 text-muted-foreground">
						<li>
							<strong>Personal Information:</strong> Name, email
							address, phone number, profile picture, and other
							information you provide when creating an account.
						</li>
						<li>
							<strong>Professional Information:</strong> Skills,
							experience, portfolio, certifications, and other
							career-related information.
						</li>
						<li>
							<strong>Payment Information:</strong> Billing
							details and payment card information (processed
							securely through our payment providers).
						</li>
						<li>
							<strong>Usage Data:</strong> Information about how
							you use our platform, including your interactions,
							preferences, and activity logs.
						</li>
						<li>
							<strong>Technical Data:</strong> IP address, browser
							type, device information, and cookies.
						</li>
					</ul>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						3. How We Use Your Information
					</h2>
					<p className="text-muted-foreground">
						We use the information we collect for the following
						purposes:
					</p>
					<ul className="list-disc space-y-2 pl-6 text-muted-foreground">
						<li>To provide, maintain, and improve our services</li>
						<li>
							To process transactions and send you related
							information
						</li>
						<li>
							To communicate with you about updates, security
							alerts, and support
						</li>
						<li>
							To personalize your experience and show relevant
							content
						</li>
						<li>
							To match experts with appropriate job opportunities
						</li>
						<li>To prevent fraud and ensure platform security</li>
						<li>To comply with legal obligations</li>
					</ul>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						4. Information Sharing
					</h2>
					<p className="text-muted-foreground">
						We do not sell your personal information. We may share
						your information in the following circumstances:
					</p>
					<ul className="list-disc space-y-2 pl-6 text-muted-foreground">
						<li>
							<strong>With Other Users:</strong> Your profile
							information is visible to other users to facilitate
							connections and collaborations.
						</li>
						<li>
							<strong>With Service Providers:</strong> We share
							information with third-party service providers who
							perform services on our behalf.
						</li>
						<li>
							<strong>For Legal Reasons:</strong> We may disclose
							information if required by law or in response to
							legal requests.
						</li>
						<li>
							<strong>Business Transfers:</strong> In the event of
							a merger, acquisition, or sale of assets, your
							information may be transferred.
						</li>
					</ul>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">5. Data Security</h2>
					<p className="text-muted-foreground">
						We implement appropriate technical and organizational
						measures to protect your personal data against
						unauthorized access, alteration, disclosure, or
						destruction. However, no method of transmission over the
						Internet is 100% secure, and we cannot guarantee
						absolute security.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">6. Your Rights</h2>
					<p className="text-muted-foreground">
						Depending on your location, you may have the following
						rights:
					</p>
					<ul className="list-disc space-y-2 pl-6 text-muted-foreground">
						<li>Access to your personal data</li>
						<li>Correction of inaccurate data</li>
						<li>Deletion of your data (right to be forgotten)</li>
						<li>Restriction of processing</li>
						<li>Data portability</li>
						<li>Object to processing</li>
						<li>Withdraw consent at any time</li>
					</ul>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						7. Cookies and Tracking
					</h2>
					<p className="text-muted-foreground">
						We use cookies and similar tracking technologies to
						track activity on our platform and hold certain
						information. You can control cookies through your
						browser settings. Note that disabling cookies may affect
						the functionality of our services.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						8. Children&apos;s Privacy
					</h2>
					<p className="text-muted-foreground">
						Our platform is not intended for users under the age of
						18. We do not knowingly collect personal information
						from children. If you become aware that a child has
						provided us with personal data, please contact us.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						9. International Data Transfers
					</h2>
					<p className="text-muted-foreground">
						Your information may be transferred to and processed in
						countries other than your own. We ensure that
						appropriate safeguards are in place to protect your data
						in accordance with this privacy policy.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">
						10. Changes to This Policy
					</h2>
					<p className="text-muted-foreground">
						We may update this privacy policy from time to time. We
						will notify you of any changes by posting the new policy
						on this page and updating the &quot;Last updated&quot;
						date. We encourage you to review this policy
						periodically.
					</p>
				</section>

				<section className="mb-10 space-y-4">
					<h2 className="text-2xl font-bold">11. Contact Us</h2>
					<p className="text-muted-foreground">
						If you have any questions about this privacy policy or
						our data practices, please contact us at:
					</p>
					<div className="rounded-lg border bg-muted/30 p-4">
						<p className="text-muted-foreground">
							<strong>Email:</strong> privacy@expertbridge.com
						</p>
						<p className="text-muted-foreground">
							<strong>Address:</strong> ExpertBridge Inc., 123
							Business St, San Francisco, CA 94102
						</p>
					</div>
				</section>
			</div>
		</div>
	);
}
