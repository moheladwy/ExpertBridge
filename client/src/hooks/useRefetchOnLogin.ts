import { useEffect, useMemo, useState } from "react";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

const useRefetchOnLogin = (refetch: (...args: any) => any) => {
  // Here we are checking against the appUser because it gets changed
  // every time the auth user changes.
  // (a real change to auth user, not just a reference change with the same user still logged in)
  // that's when the appUser will change, so we listen on that change in appUser. 
  const [isLoggedIn, __, ___, authUser, appUser] = useIsUserLoggedIn();

  const [lastId, setLastId] = useState<string | undefined | null>(appUser?.id);

  useEffect(() => {
    console.log('refetch on login being used...');
  }, []);

  useEffect(() => {
    const currentId = appUser?.id;
    if (lastId === currentId) return;

    setLastId(currentId);

    refetch();

    console.log("refetching...");
  }, [appUser, isLoggedIn, authUser, lastId, refetch]);
};

export default useRefetchOnLogin;
