import { useEffect, useMemo } from "react";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

export default (refetch: any) => {
  const [_, __, ___, authUser] = useIsUserLoggedIn();
  const uid = useMemo(() => authUser?.uid, [authUser?.uid]);

  useEffect(() => {
    if (authUser) {
      if (uid === authUser.uid) return;
      refetch();
      console.log('refetching...');
    }
  }, [authUser, refetch, uid]);
};
