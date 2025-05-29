export const defaultFetchOptions = {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
  },
};

export const apiHeaders = {
  json: {
    "Content-Type": "application/json",
  },
};

export const getAuthHeaders = (token: string) => ({
  ...apiHeaders.json,
  Authorization: `Bearer ${token}`,
});

export const getAuthPostOptions = <T extends object>(
  token: string,
  body: T
) => ({
  method: "POST",
  headers: getAuthHeaders(token),
  body: JSON.stringify(body),
});

export const getAuthPostOptionsNoBody = (token: string) => ({
  method: "POST",
  headers: {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  },
});

export const getAuthGetOptions = (token: string) => ({
  method: "GET",
  headers: getAuthHeaders(token),
});

export const getAuthDeleteOptions = (token: string) => ({
  method: "DELETE",
  headers: {
    Authorization: `Bearer ${token}`,
  },
});

export const getAuthPatchOptions = <T extends object>(token: string, body: T) => ({
  method: "PATCH",
  headers: {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  },
  body: JSON.stringify(body),
});
