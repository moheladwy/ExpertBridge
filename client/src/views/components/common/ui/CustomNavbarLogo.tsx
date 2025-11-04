interface CustomNavbarLogoProps {
	onClick: () => void;
}

const CustomNavbarLogo = ({ onClick }: CustomNavbarLogoProps) => {
	return (
		<div onClick={onClick} className="navbar-logo">
			<span className="hidden sm:inline">Expert</span>
			<span className="sm:hidden">EB</span>
			<span className="hidden sm:inline">Bridge</span>
		</div>
	);
};

export default CustomNavbarLogo;
