import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import RegisterBtn from "@/views/components/custom/RegisterBtn";
import { Footer } from "@/views/components/common/ui/Footer";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import HeroSectionExpertBridge from "@/components/hero-section-expertbridge";
import { RegisterButton, LearnMoreButton } from "@/views/components/custom/AuthButtons";

function LandingPage() {
	const [_isLoggedIn, loading, _error, authUser, _appUser] =
		useIsUserLoggedIn();
	const navigate = useNavigate();

	useEffect(() => {
		if (!loading && authUser) {
			// navigate("/home", { replace: true });
		}
	}, [authUser, loading, navigate]);

	return (
		<div className="overflow-x-hidden">
			{/* Hero Section - Enhanced with Aceternity UI */}
			<HeroSectionExpertBridge />

			{/* What We Do Section */}
			<div className="py-20 bg-muted/30">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="text-center mb-16 space-y-4">
						<div className="inline-block rounded-full bg-primary/10 px-4 py-1.5 text-sm font-medium text-primary">
							Features
						</div>
						<h2 className="text-3xl sm:text-5xl font-bold tracking-tight text-foreground">
							What We Do
						</h2>
						<p className="mt-6 text-lg text-muted-foreground max-w-3xl mx-auto leading-relaxed">
							ExpertBridge is revolutionizing how professionals
							connect by creating a platform where expertise meets
							demand, fostering meaningful relationships.
						</p>
					</div>

					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
						{/* Feature 1 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-primary"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									Professional Networking
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Connect with skilled professionals and
									technicians across various industries. Build
									meaningful relationships that drive your
									projects forward.
								</p>
							</div>
						</div>

						{/* Feature 2 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-secondary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-secondary/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-secondary"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									Smart Job Matching
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Our AI-powered system matches job
									requirements with expert skills, ensuring
									the right professional for every task.
								</p>
							</div>
						</div>

						{/* Feature 3 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-accent/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-accent/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-accent"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									Seamless Communication
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Built-in chat system with AI-powered content
									moderation ensures safe, professional
									communication between all users.
								</p>
							</div>
						</div>

						{/* Feature 4 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-destructive/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-destructive/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-destructive"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									Reputation System
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Our dual-role reputation system helps build
									trust by allowing both clients and
									professionals to rate each other.
								</p>
							</div>
						</div>

						{/* Feature 5 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-secondary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-secondary/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-secondary"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v4a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									Personalized Feed
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Stay updated with relevant opportunities and
									industry insights through our AI-curated
									personalized content feed.
								</p>
							</div>
						</div>

						{/* Feature 6 */}
						<div className="group relative bg-card border rounded-xl p-8 hover:shadow-lg hover:border-primary/50 transition-all duration-300">
							<div className="absolute inset-0 bg-linear-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity rounded-xl" />
							<div className="relative">
								<div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-6 group-hover:scale-110 transition-transform">
									<svg
										className="w-6 h-6 text-primary"
										fill="none"
										stroke="currentColor"
										viewBox="0 0 24 24"
									>
										<path
											strokeLinecap="round"
											strokeLinejoin="round"
											strokeWidth={2}
											d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"
										/>
									</svg>
								</div>
								<h3 className="text-xl font-semibold text-card-foreground mb-3">
									AI-Powered Innovation
								</h3>
								<p className="text-muted-foreground leading-relaxed">
									Leverage cutting-edge AI for content
									moderation, smart recommendations, and
									automated tagging to enhance your
									experience.
								</p>
							</div>
						</div>
					</div>
				</div>
			</div>

			{/* Mission Section */}
			<div className="py-20 bg-background">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="relative bg-card border rounded-2xl p-8 sm:p-16 overflow-hidden">
						<div className="absolute top-0 right-0 w-64 h-64 bg-primary/5 rounded-full -translate-y-1/2 translate-x-1/2 blur-3xl" />
						<div className="absolute bottom-0 left-0 w-64 h-64 bg-secondary/5 rounded-full translate-y-1/2 -translate-x-1/2 blur-3xl" />
						<div className="relative text-center mb-8 space-y-4">
							<div className="inline-block rounded-full bg-primary/10 px-4 py-1.5 text-sm font-medium text-primary">
								Our Mission
							</div>
							<h2 className="text-3xl sm:text-5xl font-bold tracking-tight text-card-foreground">
								Connecting Expertise with Opportunity
							</h2>
						</div>
						<div className="relative max-w-4xl mx-auto">
							<p className="text-lg sm:text-xl text-muted-foreground leading-relaxed text-center">
								At ExpertBridge, we believe that every problem
								deserves the right expert, and every expert
								deserves the right opportunity. We're
								revolutionizing how professionals connect by
								creating a platform where expertise meets
								demand, fostering meaningful relationships that
								drive success for both service providers and
								clients.
							</p>
						</div>
					</div>
				</div>
			</div>

			{/* Statistics Section */}
			<div className="py-20 bg-muted/30">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="relative bg-linear-to-br from-primary to-primary/90 rounded-2xl p-8 sm:p-16 overflow-hidden">
						<div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxnIGZpbGw9IiNmZmZmZmYiIGZpbGwtb3BhY2l0eT0iMC4wNSI+PHBhdGggZD0iTTM2IDE2YzAgMS4xMDUtLjg5NSAyLTIgMnMtMi0uODk1LTItMiAuODk1LTIgMi0yIDIgLjg5NSAyIDJ6bTAgMjBjMCAxLjEwNS0uODk1IDItMiAycy0yLS44OTUtMi0yIC44OTUtMiAyLTIgMiAuODk1IDIgMnptLTIwIDRjMCAxLjEwNS0uODk1IDItMiAycy0yLS44OTUtMi0yIC44OTUtMiAyLTIgMiAuODk1IDIgMnoiLz48L2c+PC9nPjwvc3ZnPg==')] opacity-30" />
						<div className="relative text-center mb-12 space-y-4">
							<h2 className="text-3xl sm:text-5xl font-bold text-primary-foreground">
								ExpertBridge by the Numbers
							</h2>
							<p className="text-xl text-primary-foreground/90">
								Growing stronger every day
							</p>
						</div>

						<div className="relative grid grid-cols-1 md:grid-cols-3 gap-8 md:gap-12">
							<div className="text-center space-y-2 p-6 rounded-xl bg-primary-foreground/5 backdrop-blur-sm border border-primary-foreground/10">
								<div className="text-5xl sm:text-6xl font-bold text-primary-foreground">
									10K+
								</div>
								<div className="text-primary-foreground/90 text-lg font-medium">
									Active Professionals
								</div>
							</div>
							<div className="text-center space-y-2 p-6 rounded-xl bg-primary-foreground/5 backdrop-blur-sm border border-primary-foreground/10">
								<div className="text-5xl sm:text-6xl font-bold text-primary-foreground">
									25K+
								</div>
								<div className="text-primary-foreground/90 text-lg font-medium">
									Successful Projects
								</div>
							</div>
							<div className="text-center space-y-2 p-6 rounded-xl bg-primary-foreground/5 backdrop-blur-sm border border-primary-foreground/10">
								<div className="text-5xl sm:text-6xl font-bold text-primary-foreground">
									98%
								</div>
								<div className="text-primary-foreground/90 text-lg font-medium">
									Client Satisfaction
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>

			{/* Call to Action Section */}
			<div className="py-20 bg-background">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="relative bg-card border rounded-2xl p-8 sm:p-16 overflow-hidden">
						<div className="absolute top-0 left-1/2 -translate-x-1/2 w-full h-full bg-linear-to-b from-primary/10 via-transparent to-transparent" />
						<div className="relative max-w-4xl mx-auto text-center space-y-8">
							<div className="space-y-4">
								<h2 className="text-3xl sm:text-5xl font-bold text-foreground tracking-tight">
									Ready to Bridge the Gap?
								</h2>
								<p className="text-lg sm:text-xl text-muted-foreground max-w-2xl mx-auto">
									Join thousands of professionals who are
									already transforming their careers and
									projects through ExpertBridge.
								</p>
							</div>
							<div className="flex flex-col sm:flex-row gap-4 justify-center pt-4">
								<RegisterButton />
								<LearnMoreButton />
							</div>
						</div>
					</div>
				</div>
			</div>

			<Footer />
		</div>
	);
}

export default LandingPage;
