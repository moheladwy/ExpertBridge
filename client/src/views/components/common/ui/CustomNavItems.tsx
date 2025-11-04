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
				"nav-items-container",
				isScrolled
					? "nav-items-container-scrolled"
					: "nav-items-container-top",
				className
			)}
		>
			{items.map((item, idx) => (
				<button
					key={item.link}
					onMouseEnter={() => setHovered(idx)}
					onClick={() => navigate(item.link)}
					className={cn(
						"nav-item-btn",
						location.pathname === item.link && "nav-item-btn-active"
					)}
				>
					{hovered === idx && (
						<motion.div
							layoutId="hovered"
							className="nav-item-hover-bg"
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							exit={{ opacity: 0 }}
							transition={{ duration: 0.15 }}
						/>
					)}
					{location.pathname === item.link && (
						<motion.div
							layoutId="active"
							className="nav-item-active-indicator"
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
