import { useNavigate } from "react-router-dom";

function RegisterBtn() {

  const navigate = useNavigate();

  return (
    <>
      <button className="text-main-blue bg-white border-2 rounded-full px-6 py-2 hover:text-white hover:bg-main-blue hover:font-bold max-sm:text-xs max-sm:px-3"
      onClick={()=>navigate("/signup")}>
        Register
      </button>
    </>
  )
}

export default RegisterBtn