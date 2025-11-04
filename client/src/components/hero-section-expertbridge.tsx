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
		<div className="relative mx-auto flex min-h-screen w-full flex-col items-center justify-center bg-linear-to-br from-primary via-primary to-secondary">
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
			<div className="container mx-auto px-4 py-10 md:py-20">
				<div className="flex flex-col lg:flex-row items-center justify-between gap-12">
					{/* Text Content - Left Side */}
					<div className="flex-1 flex flex-col gap-6">
						<h1 className="relative z-10 text-left text-2xl font-bold text-primary-foreground md:text-4xl lg:text-6xl">
							{"Connect with Experts Build Your Career"
								.split(" ")
								.map((word, index) => (
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
						<motion.p
							initial={{
								opacity: 0,
							}}
							animate={{
								opacity: 1,
							}}
							transition={{
								duration: 0.3,
								delay: 0.8,
							}}
							className="relative z-10 max-w-xl py-4 text-left text-lg font-normal text-primary-foreground/80"
						>
							The professional networking platform that connects
							you with the right expertise. Find answers, get
							hired, and create meaningful professional
							relationships that drive success.
						</motion.p>
						<motion.div
							initial={{
								opacity: 0,
							}}
							animate={{
								opacity: 1,
							}}
							transition={{
								duration: 0.3,
								delay: 1,
							}}
							className="relative z-10 mt-4 flex flex-wrap items-center gap-4"
						>
							<button
								onClick={handleGettingStarted}
								className="px-8 py-3 transform rounded-lg bg-primary-foreground font-medium text-primary transition-all duration-300 hover:-translate-y-0.5 hover:bg-primary-foreground/90 shadow-lg"
							>
								Get Started
							</button>
							<button
								onClick={handleLearnMore}
								className="px-8 py-3 transform rounded-lg border-2 border-primary-foreground bg-transparent font-medium text-primary-foreground transition-all duration-300 hover:-translate-y-0.5 hover:bg-primary-foreground/10"
							>
								Learn More
							</button>
						</motion.div>
					</div>

					{/* Image - Right Side */}
					<motion.div
						initial={{
							opacity: 0,
							x: 20,
						}}
						animate={{
							opacity: 1,
							x: 0,
						}}
						transition={{
							duration: 0.5,
							delay: 1.2,
						}}
						className="flex-1 relative z-10 flex items-center justify-center"
					>
						<div className="w-full max-w-md lg:max-w-lg">
							<img
								src={mobile}
								alt="ExpertBridge mobile app preview"
								className="h-auto w-full object-contain drop-shadow-2xl"
							/>
						</div>
					</motion.div>
				</div>
			</div>
		</div>
	);
}
