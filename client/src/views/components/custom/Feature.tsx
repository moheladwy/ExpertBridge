interface FeatureProps {
	icon: string;
	title: string;
	body: string;
}

const Feature: React.FC<FeatureProps> = ({ icon, title, body }) => {
	return (
		<div className="flex flex-col justify-start max-md:w-full group">
			<div className="w-12 h-12 mb-3 rounded-lg bg-primary/10 flex items-center justify-center group-hover:bg-primary/20 transition-colors">
				<img src={icon} alt="Icon" className="w-6 h-6" />
			</div>
			<div>
				<h3 className="text-lg font-bold text-card-foreground max-md:text-base mb-2">
					{title}
				</h3>
				<p className="text-muted-foreground max-md:text-sm leading-relaxed">
					{body}
				</p>
			</div>
		</div>
	);
};

export default Feature;
