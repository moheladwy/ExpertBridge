import mobile from '../../assets/LandingPageAssets/Mobile.svg'
import icon from '../../assets/LandingPageAssets/Icons/PlaceHolderIcon.svg'
import Feature from './Feature'
import RegisterBtn from '@/components/NavBar/RegisterBtn'
import Footer from '@/components/Footer/Footer'

function LandingPage () {
    return(
        <>
            {/* First Section */}
            <div className="h-screen bg-main-blue w-full">
                <div className="flex justify-center items-center mx-28 gap-20">
                    <div className="flex flex-col items-start gap-3 text-white max-w-xl">
                        <div className="text-6xl"><b>Expert</b>Bridge</div>
                        <div className="text-2xl">Find Answers and Get Hired</div>
                        <p className="font-light">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod  tempor incididunt ut labore et dolore magna aliqua. </p>
                    </div>

                    <img src={mobile} alt="mobile" className='w-1/5 my-16'/>
                </div>
            </div>

            {/* Second Section */}
            <div className='h-4/5 p-20'>
                <div className="flex justify-center items-center mx-28 gap-20">
                    <div className="flex flex-col items-start gap-3 text-main-blue max-w-xl">
                        <div className="text-6xl font-semibold">Main Features</div>
                        <p className="font-light">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod  tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim  veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea  commodo consequat.</p>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-10 px-10 py-10">
                        {/* Feature 1 */}
                         <Feature icon={icon} title='Lorem' body='Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.'></Feature>
                        {/* Feature 2 */}
                        <Feature icon={icon} title='Lorem' body='Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.'></Feature>
                        {/* Feature 3 */}
                        <Feature icon={icon} title='Lorem' body='Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.'></Feature>
                        {/* Feature 4 */}
                        <Feature icon={icon} title='Lorem' body='Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.'></Feature>
                    </div>
                    
                </div>
            </div>

            {/* Third Section */}
            <div className='p-20 bg-main-blue'>
                <div className="flex justify-center items-center mx-28 gap-20">
                    <div className="flex flex-col items-center gap-3 text-white max-w-xl">
                        <div className="text-4xl font-semibold">Join Now!</div>
                        <RegisterBtn/>
                    </div>
                </div>
            </div>

            <Footer/>
        </>
    )
}

export default LandingPage