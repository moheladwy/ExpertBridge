import { useLocation, useNavigate } from "react-router-dom";
import { useState } from "react";
import { motion } from "motion/react";
import { cn } from "@/lib/util/utils";

interface NavItem {
	name: string;
	link: string;
}

interface CustomNavItemsProps {
	items: NavItem[];
	className?: string;
	isScrolled?: boolean;
}

const CustomNavItems = ({
	items,
	className,
	isScrolled = false,
}: CustomNavItemsProps) => {
	const location = useLocation();
	const navigate = useNavigate();
	const [hovered, setHovered] = useState<number | null>(null);

	return (
		<motion.div
			onMouseLeave={() => setHovered(null)}
			className={cn(
				"flex flex-row items-center gap-2 py-1 text-sm font-normal w-auto bg-primary px-6",
				isScrolled ? "rounded-full mt-5" : "rounded-b-full",
				className
			)}
		>
			{items.map((item, idx) => (
				<button
					key={item.link}
					onMouseEnter={() => setHovered(idx)}
					onClick={() => navigate(item.link)}
					className={cn(
						"relative cursor-pointer px-5 py-1.5 text-primary-foreground/90 transition-colors hover:text-primary-foreground",
						location.pathname === item.link &&
							"font-medium text-primary-foreground"
					)}
				>
					{hovered === idx && (
						<motion.div
							layoutId="hovered"
							className="absolute inset-0 h-full w-full rounded-md bg-white/10"
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							exit={{ opacity: 0 }}
							transition={{ duration: 0.15 }}
						/>
					)}
					{location.pathname === item.link && (
						<motion.div
							layoutId="active"
							className="absolute bottom-0 left-1 right-1 h-0.5 bg-primary-foreground"
							transition={{
								type: "spring",
								bounce: 0.2,
								duration: 0.6,
							}}
						/>
					)}
					<span className="relative z-20">{item.name}</span>
				</button>
			))}
		</motion.div>
	);
};

export default CustomNavItems;
