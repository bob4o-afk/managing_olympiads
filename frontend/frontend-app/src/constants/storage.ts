export const SESSION_KEY = "docker_session";

export const generateSessionId = (): string => Date.now().toString();
