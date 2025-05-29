export const SESSION_KEY = process.env.REACT_APP_SESSION_KEY as string;
export const generateSessionId = (): string => Date.now().toString();
