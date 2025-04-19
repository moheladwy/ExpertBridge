import mobile from '@/assets/LandingPageAssets/Mobile.svg'
import icon from '@/assets/LandingPageAssets/Icons/PlaceHolderIcon.svg'
import Feature from '@/views/components/custom/Feature'
import RegisterBtn from '@/views/components/custom/RegisterBtn'
import Footer from '@/views/components/common/ui/Footer'
import { auth } from '@/lib/firebase'
import { useNavigate } from 'react-router'
import { useEffect } from 'react'
import useIsUserLoggedIn from '@/hooks/useIsUserLoggedIn'

function LandingPage() {
  const [isLoggedIn, loading, error, authUser, appUser] = useIsUserLoggedIn();
  const navigate = useNavigate();

  useEffect(() => {
    if (!loading && authUser) {
      navigate("/home", { replace: true });
    }
  }, [authUser, loading, navigate]);

  return (
    <>
      {/* First Section */}
      <div className="max-lg:flex max-lg:justify-center max-lg:items-center h-screen bg-main-blue w-full ">
        <div className="flex justify-center items-center mx-28 gap-20 ">
          <div className="flex flex-col items-start gap-3 text-white max-w-xl">
            <div className="text-6xl max-md:text-5xl"><b>Expert</b>Bridge</div>
            <div className="text-2xl max-md:text-xl">Find Answers and Get Hired</div>
            <p className="font-light">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod  tempor incididunt ut labore et dolore magna aliqua. </p>
          </div>

          <img src={mobile} alt="mobile" className='w-1/5 max-lg:hidden my-16' />
        </div>
      </div>

      {/* Second Section */}
      <div className='h-4/5 p-20'>
        <div className="flex max-lg:flex-col justify-center items-center mx-28 gap-20 max-md:gap-10 max-sm:gap-2 max-md:mx-2">
          <div className="flex flex-col items-start gap-3 text-main-blue max-w-xl">
            <div className="text-6xl font-semibold max-md:text-5xl">Main Features</div>
            <p className="font-light max-md:text-sm">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod  tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim  veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea  commodo consequat.</p>
          </div>

          <div className="grid max-md:grid-cols-1 grid-cols-2 gap-10 max-md:gap-2 px-10 max-md:px-0 py-10">
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
        <div className="flex justify-center items-center mx-28 gap-20 max-md:mx-5">
          <div className="flex flex-col items-center gap-3 text-white max-w-xl">
            <div className="text-4xl font-semibold max-md:text-3xl max-sm:text-2xl max-sm:w-28">Join Now!</div>
            <RegisterBtn />
          </div>
        </div>
      </div>

      <Footer />
    </>
  )
}

export default LandingPage