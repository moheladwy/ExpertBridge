const AboutUsPage = () => {
	return (
		<div className="min-h-screen bg-gradient-to-b from-gray-50 to-gray-100 transition-colors duration-200">
			{/* Hero Section */}
			<div className="relative overflow-hidden">
				<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 sm:py-24">
					<div className="text-center">
						<h1 className="text-4xl sm:text-5xl lg:text-6xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent transition-colors duration-200">
							About ExpertBridge
						</h1>
						<p className="mt-6 text-xl sm:text-2xl text-muted-foreground max-w-3xl mx-auto transition-colors duration-200">
							Connecting expertise with opportunity through
							innovative technology
						</p>
						<div className="mt-8 w-24 h-1 bg-gradient-to-r from-blue-600 to-indigo-600 mx-auto rounded-full"></div>
					</div>
				</div>
			</div>

			{/* Mission Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="bg-card rounded-2xl shadow-lg p-8 sm:p-12 transition-all duration-200">
					<div className="text-center mb-12">
						<h2 className="text-3xl sm:text-4xl font-bold text-card-foreground transition-colors duration-200">
							Our Mission
						</h2>
						<div className="mt-4 w-16 h-1 bg-gradient-to-r from-emerald-500 to-teal-500 mx-auto rounded-full"></div>
					</div>
					<div className="max-w-4xl mx-auto">
						<p className="text-lg sm:text-xl text-muted-foreground leading-relaxed text-center transition-colors duration-200">
							At ExpertBridge, we believe that every problem
							deserves the right expert, and every expert deserves
							the right opportunity. We're revolutionizing how
							professionals connect by creating a platform where
							expertise meets demand, fostering meaningful
							relationships that drive success for both service
							providers and clients.
						</p>
					</div>
				</div>
			</div>

			{/* What We Do Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="text-center mb-16">
					<h2 className="text-3xl sm:text-4xl font-bold text-foreground transition-colors duration-200">
						What We Do
					</h2>
					<div className="mt-4 w-16 h-1 bg-gradient-to-r from-blue-600 to-indigo-600 mx-auto rounded-full"></div>
				</div>

				<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
					{/* Feature 1 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-blue-500 to-indigo-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							Professional Networking
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Connect with skilled professionals and technicians
							across various industries. Build meaningful
							relationships that drive your projects forward.
						</p>
					</div>

					{/* Feature 2 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							Smart Job Matching
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Our AI-powered system matches job requirements with
							expert skills, ensuring the right professional for
							every task.
						</p>
					</div>

					{/* Feature 3 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-purple-500 to-pink-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							Seamless Communication
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Built-in chat system with AI-powered content
							moderation ensures safe, professional communication
							between all users.
						</p>
					</div>

					{/* Feature 4 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-orange-500 to-red-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							Reputation System
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Our dual-role reputation system helps build trust by
							allowing both clients and professionals to rate each
							other.
						</p>
					</div>

					{/* Feature 5 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-teal-500 to-cyan-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							Personalized Feed
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Stay updated with relevant opportunities and
							industry insights through our AI-curated
							personalized content feed.
						</p>
					</div>

					{/* Feature 6 */}
					<div className="bg-card rounded-xl shadow-lg p-8 hover:shadow-xl transition-all duration-200">
						<div className="w-12 h-12 bg-gradient-to-r from-indigo-500 to-purple-500 rounded-lg flex items-center justify-center mb-6">
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
						<h3 className="text-xl font-semibold text-card-foreground mb-4 transition-colors duration-200">
							AI-Powered Innovation
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Leverage cutting-edge AI for content moderation,
							smart recommendations, and automated tagging to
							enhance your experience.
						</p>
					</div>
				</div>
			</div>

			{/* Our Values Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="bg-card rounded-2xl shadow-lg p-8 sm:p-12 transition-all duration-200">
					<div className="text-center mb-12">
						<h2 className="text-3xl sm:text-4xl font-bold text-card-foreground transition-colors duration-200">
							Our Values
						</h2>
						<div className="mt-4 w-16 h-1 bg-gradient-to-r from-purple-500 to-pink-500 mx-auto rounded-full"></div>
					</div>

					<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
						<div className="text-center">
							<div className="w-16 h-16 bg-gradient-to-r from-blue-500 to-indigo-500 rounded-full flex items-center justify-center mx-auto mb-4">
								<svg
									className="w-8 h-8 text-white"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
									/>
								</svg>
							</div>
							<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
								Trust
							</h3>
							<p className="text-muted-foreground transition-colors duration-200">
								Building lasting relationships through
								transparency, reliability, and authentic
								connections.
							</p>
						</div>

						<div className="text-center">
							<div className="w-16 h-16 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-full flex items-center justify-center mx-auto mb-4">
								<svg
									className="w-8 h-8 text-white"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M13 10V3L4 14h7v7l9-11h-7z"
									/>
								</svg>
							</div>
							<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
								Innovation
							</h3>
							<p className="text-muted-foreground transition-colors duration-200">
								Continuously pushing boundaries with
								cutting-edge technology and creative solutions.
							</p>
						</div>

						<div className="text-center">
							<div className="w-16 h-16 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center mx-auto mb-4">
								<svg
									className="w-8 h-8 text-white"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"
									/>
								</svg>
							</div>
							<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
								Excellence
							</h3>
							<p className="text-muted-foreground transition-colors duration-200">
								Empowering every user to achieve their highest
								potential through quality connections.
							</p>
						</div>
					</div>
				</div>
			</div>

			{/* Statistics Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl shadow-lg p-8 sm:p-12 transition-all duration-200">
					<div className="text-center mb-12">
						<h2 className="text-3xl sm:text-4xl font-bold text-white">
							ExpertBridge by the Numbers
						</h2>
						<p className="mt-4 text-xl text-blue-100">
							Growing stronger every day
						</p>
					</div>

					<div className="grid grid-cols-1 md:grid-cols-3 gap-8">
						<div className="text-center">
							<div className="text-4xl sm:text-5xl font-bold text-white mb-2">
								10K+
							</div>
							<div className="text-blue-100 text-lg">
								Active Professionals
							</div>
						</div>
						<div className="text-center">
							<div className="text-4xl sm:text-5xl font-bold text-white mb-2">
								25K+
							</div>
							<div className="text-blue-100 text-lg">
								Successful Projects
							</div>
						</div>
						<div className="text-center">
							<div className="text-4xl sm:text-5xl font-bold text-white mb-2">
								98%
							</div>
							<div className="text-blue-100 text-lg">
								Client Satisfaction
							</div>
						</div>
					</div>
				</div>
			</div>

			{/* Team Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="text-center mb-16">
					<h2 className="text-3xl sm:text-4xl font-bold text-foreground transition-colors duration-200">
						Meet Our Team
					</h2>
					<div className="mt-4 w-16 h-1 bg-gradient-to-r from-orange-500 to-red-500 mx-auto rounded-full"></div>
					<p className="mt-6 text-lg text-muted-foreground max-w-3xl mx-auto transition-colors duration-200">
						We're a passionate team of innovators, engineers, and
						problem-solvers dedicated to revolutionizing
						professional networking.
					</p>
				</div>

				<div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
					{/* Team Member 1 */}
					<div className="bg-card rounded-xl shadow-lg p-8 text-center hover:shadow-xl transition-all duration-200">
						<div className="w-24 h-24 bg-gradient-to-r from-blue-500 to-indigo-500 rounded-full mx-auto mb-4 flex items-center justify-center">
							<span className="text-2xl font-bold text-white">
								EB
							</span>
						</div>
						<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
							Engineering Team
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Building the future of professional connections with
							cutting-edge technology and innovative solutions.
						</p>
					</div>

					{/* Team Member 2 */}
					<div className="bg-card rounded-xl shadow-lg p-8 text-center hover:shadow-xl transition-all duration-200">
						<div className="w-24 h-24 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-full mx-auto mb-4 flex items-center justify-center">
							<span className="text-2xl font-bold text-white">
								AI
							</span>
						</div>
						<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
							AI & ML Team
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Developing intelligent systems that power smart
							matching, content moderation, and personalized
							experiences.
						</p>
					</div>

					{/* Team Member 3 */}
					<div className="bg-card rounded-xl shadow-lg p-8 text-center hover:shadow-xl transition-all duration-200">
						<div className="w-24 h-24 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full mx-auto mb-4 flex items-center justify-center">
							<span className="text-2xl font-bold text-white">
								UX
							</span>
						</div>
						<h3 className="text-xl font-semibold text-card-foreground mb-2 transition-colors duration-200">
							Design Team
						</h3>
						<p className="text-muted-foreground transition-colors duration-200">
							Creating intuitive, beautiful experiences that make
							professional networking effortless and enjoyable.
						</p>
					</div>
				</div>
			</div>

			{/* Call to Action Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="bg-card rounded-2xl shadow-lg p-8 sm:p-12 text-center transition-all duration-200">
					<h2 className="text-3xl sm:text-4xl font-bold text-card-foreground mb-6 transition-colors duration-200">
						Ready to Bridge the Gap?
					</h2>
					<p className="text-lg sm:text-xl text-muted-foreground max-w-3xl mx-auto mb-8 transition-colors duration-200">
						Join thousands of professionals who are already
						transforming their careers and projects through
						ExpertBridge. Whether you're seeking expertise or
						offering your skills, we're here to connect you with the
						right opportunities.
					</p>
					<div className="flex flex-col sm:flex-row gap-4 justify-center">
						<button className="px-8 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white font-semibold rounded-lg shadow-lg hover:shadow-xl transition-all duration-200">
							Get Started Today
						</button>
						<button className="px-8 py-3 bg-transparent border-2 border-border text-foreground hover:border-accent font-semibold rounded-lg transition-all duration-200">
							Learn More
						</button>
					</div>
				</div>
			</div>

			{/* Contact Section */}
			<div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16">
				<div className="text-center">
					<h2 className="text-3xl sm:text-4xl font-bold text-foreground mb-6 transition-colors duration-200">
						Get in Touch
					</h2>
					<p className="text-lg text-muted-foreground mb-8 transition-colors duration-200">
						Have questions or want to learn more about ExpertBridge?
						We'd love to hear from you.
					</p>
					<div className="flex flex-col sm:flex-row gap-8 justify-center items-center">
						<div className="flex items-center gap-3">
							<div className="w-10 h-10 bg-gradient-to-r from-blue-500 to-indigo-500 rounded-full flex items-center justify-center">
								<svg
									className="w-5 h-5 text-white"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
									/>
								</svg>
							</div>
							<div>
								<p className="text-foreground font-semibold transition-colors duration-200">
									Email
								</p>
								<p className="text-muted-foreground transition-colors duration-200">
									support@expertbridge.com
								</p>
							</div>
						</div>
						<div className="flex items-center gap-3">
							<div className="w-10 h-10 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-full flex items-center justify-center">
								<svg
									className="w-5 h-5 text-white"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
									/>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
									/>
								</svg>
							</div>
							<div>
								<p className="text-foreground font-semibold transition-colors duration-200">
									Location
								</p>
								<p className="text-muted-foreground transition-colors duration-200">
									Cairo, Egypt
								</p>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
};

export default AboutUsPage;
