import React, { createContext, useState, useEffect, ReactNode } from "react";

export const LanguageContext = createContext<{
  locale: string;
  setLocale: (lang: string) => void;
}>({
  locale: "en",
  setLocale: () => {},
});

export const LanguageProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [locale, setLocale] = useState("en");

  useEffect(() => {
    const storedLocale = localStorage.getItem("locale");
    if (storedLocale) {
      setLocale(storedLocale);
    } else {
      const browserLocale = navigator.language.startsWith("bg") ? "bg" : "en";
      localStorage.setItem("locale", browserLocale);
      setLocale(browserLocale);
    }
  }, []);

  const changeLocale = (lang: string) => {
    localStorage.setItem("locale", lang);
    setLocale(lang);
  };

  return (
    <LanguageContext.Provider value={{ locale, setLocale: changeLocale }}>
      {children}
    </LanguageContext.Provider>
  );
};
