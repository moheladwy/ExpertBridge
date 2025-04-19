import { useEffect, useMemo } from "react";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

export default (refetch: any) => {

  // Here we are checking against the appUser because it gets changed
  // every time the auth user changes.
  // (a real change to auth user, not just a reference change with the same user still logged in)
  // that's when the appUser will change, so we listen on that change in appUser. 
  const [_, __, ___, ____, appUser] = useIsUserLoggedIn();

  useEffect(() => {
    if (appUser) {
      refetch();
      console.log('refetching...');
    }
  }, [appUser, refetch]);
};
