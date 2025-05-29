import CryptoJS from "crypto-js";

export const encryptSession = (sessionData: object): string => {
  const secretKey = process.env.REACT_APP_SECRET_KEY;
  if (!secretKey) {
    throw new Error("REACT_APP_SECRET_KEY is not defined in .env");
  }

  return CryptoJS.AES.encrypt(JSON.stringify(sessionData), secretKey).toString();
};

export const decryptSession = (encrypted: string): any => {
  const secretKey = process.env.REACT_APP_SECRET_KEY;
  if (!secretKey) {
    throw new Error("REACT_APP_SECRET_KEY is not defined in .env");
  }

  const bytes = CryptoJS.AES.decrypt(encrypted, secretKey);
  const decryptedData = bytes.toString(CryptoJS.enc.Utf8);
  return JSON.parse(decryptedData);
};
