interface FeatureProps {
    icon: string;
    title: string;
    body: string;
}

const Feature: React.FC<FeatureProps> = ({ icon, title, body }) => {
    return (
      <div className="flex flex-col justify-start max-md:w-full">
        <img src={icon} alt="Icon" className="w-10 h-10 text-main-blue" />
        <div>
          <h3 className="text-lg font-bold max-md:text-base">{title}</h3>
          <p className="text-gray-600 max-md:text-sm">{body}</p>
        </div>
      </div>
    );
  };
  

export default Feature