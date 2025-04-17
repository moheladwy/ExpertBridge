import { useEffect } from "react";
import useIsUserLoggedIn from "./useIsUserLoggedIn";

export default (refetch: any) => {
  const [_, __, ___, authUser] = useIsUserLoggedIn();

  useEffect(() => {
    if (authUser) {
      refetch();
      console.log('refetching...');
    }
  }, [authUser, refetch]);
};
