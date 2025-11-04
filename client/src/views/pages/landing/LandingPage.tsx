import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import RegisterBtn from "@/views/components/custom/RegisterBtn";
import { Footer } from "@/views/components/common/ui/Footer";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import HeroSectionExpertBridge from "@/components/hero-section-expertbridge";

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
			<div className="py-20 bg-background transition-colors duration-200">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="text-center mb-16">
						<h2 className="text-3xl sm:text-4xl font-bold text-foreground">
							What We Do
						</h2>
						<div className="mt-4 w-16 h-1 bg-primary mx-auto rounded-full"></div>
						<p className="mt-6 text-lg text-muted-foreground max-w-3xl mx-auto">
							ExpertBridge is revolutionizing how professionals
							connect by creating a platform where expertise meets
							demand, fostering meaningful relationships.
						</p>
					</div>

					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
						{/* Feature 1 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-primary rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								Professional Networking
							</h3>
							<p className="text-muted-foreground">
								Connect with skilled professionals and
								technicians across various industries. Build
								meaningful relationships that drive your
								projects forward.
							</p>
						</div>

						{/* Feature 2 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-secondary rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								Smart Job Matching
							</h3>
							<p className="text-muted-foreground">
								Our AI-powered system matches job requirements
								with expert skills, ensuring the right
								professional for every task.
							</p>
						</div>

						{/* Feature 3 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-accent rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								Seamless Communication
							</h3>
							<p className="text-muted-foreground">
								Built-in chat system with AI-powered content
								moderation ensures safe, professional
								communication between all users.
							</p>
						</div>

						{/* Feature 4 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-destructive rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								Reputation System
							</h3>
							<p className="text-muted-foreground">
								Our dual-role reputation system helps build
								trust by allowing both clients and professionals
								to rate each other.
							</p>
						</div>

						{/* Feature 5 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-secondary rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								Personalized Feed
							</h3>
							<p className="text-muted-foreground">
								Stay updated with relevant opportunities and
								industry insights through our AI-curated
								personalized content feed.
							</p>
						</div>

						{/* Feature 6 */}
						<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
							<div className="w-12 h-12 bg-primary rounded-lg flex items-center justify-center mb-6">
								<svg
									className="w-6 h-6 text-white"
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
							<h3 className="text-xl font-semibold text-card-foreground mb-4">
								AI-Powered Innovation
							</h3>
							<p className="text-muted-foreground">
								Leverage cutting-edge AI for content moderation,
								smart recommendations, and automated tagging to
								enhance your experience.
							</p>
						</div>
					</div>
				</div>
			</div>

			{/* Mission Section */}
			<div className="py-16 bg-secondary transition-colors duration-200">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="bg-card rounded-2xl shadow-lg p-8 sm:p-12 transition-all duration-200">
						<div className="text-center mb-12">
							<h2 className="text-3xl sm:text-4xl font-bold text-card-foreground transition-colors duration-200">
								Our Mission
							</h2>
							<div className="mt-4 w-16 h-1 bg-secondary mx-auto rounded-full"></div>
						</div>
						<div className="max-w-4xl mx-auto">
							<p className="text-lg sm:text-xl text-muted-foreground leading-relaxed text-center transition-colors duration-200">
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
			<div className="py-16 bg-background transition-colors duration-200">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="bg-primary rounded-2xl shadow-lg p-8 sm:p-12 transition-all duration-200">
						<div className="text-center mb-12">
							<h2 className="text-3xl sm:text-4xl font-bold text-primary-foreground">
								ExpertBridge by the Numbers
							</h2>
							<p className="mt-4 text-xl text-primary-foreground/90">
								Growing stronger every day
							</p>
						</div>

						<div className="grid grid-cols-1 md:grid-cols-3 gap-8">
							<div className="text-center">
								<div className="text-4xl sm:text-5xl font-bold text-primary-foreground mb-2">
									10K+
								</div>
								<div className="text-primary-foreground/90 text-lg">
									Active Professionals
								</div>
							</div>
							<div className="text-center">
								<div className="text-4xl sm:text-5xl font-bold text-primary-foreground mb-2">
									25K+
								</div>
								<div className="text-primary-foreground/90 text-lg">
									Successful Projects
								</div>
							</div>
							<div className="text-center">
								<div className="text-4xl sm:text-5xl font-bold text-primary-foreground mb-2">
									98%
								</div>
								<div className="text-primary-foreground/90 text-lg">
									Client Satisfaction
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>

			{/* Call to Action Section */}
			<div className="py-16 bg-primary">
				<div className="container mx-auto px-6 lg:px-8">
					<div className="max-w-4xl mx-auto text-center">
						<h2 className="text-3xl sm:text-4xl font-bold text-primary-foreground mb-6">
							Ready to Bridge the Gap?
						</h2>
						<p className="text-lg sm:text-xl text-primary-foreground/90 mb-8">
							Join thousands of professionals who are already
							transforming their careers and projects through
							ExpertBridge.
						</p>
						<div className="flex flex-col sm:flex-row gap-4 justify-center">
							<RegisterBtn />
							<button className="px-8 py-4 bg-transparent border-2 border-primary-foreground text-primary-foreground hover:bg-primary-foreground/10 font-semibold rounded-lg transition-all duration-200">
								Learn More
							</button>
						</div>
					</div>
				</div>
			</div>

			<Footer />
		</div>
	);
}

export default LandingPage;
