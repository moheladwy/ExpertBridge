let lastUserId: string | undefined;

export const getLastUserId = () => lastUserId;
export const setLastUserId = (id: string | undefined) => {
  lastUserId = id;
};
