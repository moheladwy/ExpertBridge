"use client";

import { motion } from "motion/react";
import { useNavigate } from "react-router-dom";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import mobile from "@/assets/LandingPageAssets/Mobile.svg";

export default function HeroSectionExpertBridge() {
	const navigate = useNavigate();
	const [isLoggedIn] = useIsUserLoggedIn();

	const handleGettingStarted = () => {
		if (!isLoggedIn) {
			navigate("/signup");
		} else {
			navigate("/home");
		}
	};

	const handleLearnMore = () => {
		navigate("/AboutUs");
	};

	return (
		<div className="relative mx-auto flex min-h-screen w-full flex-col items-center justify-center bg-primary">
			{/* Decorative borders */}
			<div className="absolute inset-y-0 left-0 h-full w-px bg-primary-foreground/20">
				<div className="absolute top-0 h-40 w-px bg-linear-to-b from-transparent via-primary-foreground/50 to-transparent" />
			</div>
			<div className="absolute inset-y-0 right-0 h-full w-px bg-primary-foreground/20">
				<div className="absolute h-40 w-px bg-linear-to-b from-transparent via-primary-foreground/50 to-transparent" />
			</div>
			<div className="absolute inset-x-0 bottom-0 h-px w-full bg-primary-foreground/20">
				<div className="absolute mx-auto h-px w-40 bg-linear-to-r from-transparent via-primary-foreground/50 to-transparent" />
			</div>

			<div className="container mx-auto px-6 lg:px-8 py-12 md:py-20">
				<div className="flex flex-col lg:flex-row items-center justify-between gap-12">
					{/* Text Content */}
					<div className="flex flex-col items-start gap-6 lg:text-center max-w-2xl">
						<h1 className="relative z-10 text-5xl md:text-6xl lg:text-7xl font-bold text-primary-foreground">
							{"Expert Bridge".split(" ").map((word, index) => (
								<motion.span
									key={index}
									initial={{
										opacity: 0,
										filter: "blur(4px)",
										y: 10,
									}}
									animate={{
										opacity: 1,
										filter: "blur(0px)",
										y: 0,
									}}
									transition={{
										duration: 0.3,
										delay: index * 0.1,
										ease: "easeInOut",
									}}
									className="mr-2 inline-block"
								>
									{word}
								</motion.span>
							))}
						</h1>

						<motion.h2
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							transition={{ duration: 0.3, delay: 0.3 }}
							className="text-2xl md:text-3xl font-medium text-primary-foreground/90"
						>
							Find Answers. Connect with Experts. Get Hired.
						</motion.h2>

						<motion.p
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							transition={{ duration: 0.3, delay: 0.5 }}
							className="text-lg text-primary-foreground/90 leading-relaxed"
						>
							The professional networking platform that connects
							you with the right expertise. Build your career,
							find solutions, and create meaningful professional
							relationships that drive success.
						</motion.p>

						<motion.div
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							transition={{ duration: 0.3, delay: 0.7 }}
							className="flex flex-wrap gap-4 mt-4"
						>
							<button
								onClick={handleGettingStarted}
								className="px-8 py-4 bg-primary-foreground text-primary hover:bg-primary-foreground/90 font-semibold rounded-lg shadow-lg hover:shadow-xl transition-all duration-200 transform hover:-translate-y-0.5"
							>
								Get Started
							</button>
							<button
								onClick={handleLearnMore}
								className="px-8 py-4 bg-transparent border-2 border-primary-foreground text-primary-foreground hover:bg-primary-foreground/10 font-semibold rounded-lg transition-all duration-200 transform hover:-translate-y-0.5"
							>
								Learn More
							</button>
						</motion.div>
					</div>

					{/* Mobile Image */}
					<motion.img
						src={mobile}
						alt="ExpertBridge mobile app preview"
						className="w-1/5 max-lg:hidden"
						initial={{ opacity: 0, x: 20 }}
						animate={{ opacity: 1, x: 0 }}
						transition={{ duration: 0.5, delay: 0.9 }}
					/>
				</div>
			</div>
		</div>
	);
}
