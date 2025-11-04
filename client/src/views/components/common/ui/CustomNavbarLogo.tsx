interface CustomNavbarLogoProps {
	onClick: () => void;
}

const CustomNavbarLogo = ({ onClick }: CustomNavbarLogoProps) => {
	return (
		<div
			onClick={onClick}
			className="relative z-20 flex items-center gap-2 px-2 py-1 cursor-pointer text-primary-foreground text-2xl lg:text-3xl font-bold"
		>
			<span className="hidden sm:inline">Expert</span>
			<span className="sm:hidden">EB</span>
			<span className="hidden sm:inline">Bridge</span>
		</div>
	);
};

export default CustomNavbarLogo;
