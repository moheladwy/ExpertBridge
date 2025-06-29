import { useNavigate } from "react-router-dom";

function LoginBtn() {
  const navigate = useNavigate();

  return (
    <button
      onClick={() => navigate("/login")}
      className="text-white border-2 rounded-full px-6 py-2 mr-4 hover:text-main-blue hover:bg-white dark:hover:text-white dark:hover:bg-main-blue hover:font-bold max-sm:text-xs max-sm:px-3 dark:bg-gray-800"
    >
      Login
    </button>
  );
}

export default LoginBtn;
