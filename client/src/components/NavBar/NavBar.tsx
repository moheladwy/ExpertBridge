import RegisterBtn from "./RegisterBtn"

function NavBar (){


    return(
        <>
            <div className="flex items-center w-full bg-main-blue h-16 drop-shadow-md">
                <div className="flex items-center mx-9">
                    <h1 className="text-white text-3xl"> <b>Expert</b>Bridge</h1>
                    <a href="" className="text-white font-light mx-5">About Us</a>
                </div>
                    <div className="flex ml-auto mr-9">
                        <button className="text-white border-2 rounded-full px-6 py-2 mr-4 hover:text-main-blue hover:bg-white hover:font-bold">
                            Login
                        </button>
                        
                        <RegisterBtn/>
                    </div>
            </div>
        </>
    ) 
} 

export default NavBar