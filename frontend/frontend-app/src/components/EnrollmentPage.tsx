import React, { useState, useEffect, FormEvent, useContext } from "react";
import { Button, Form, Select, Typography, notification, Card } from "antd";
import "./ui/EnrollmentPage.css";

import { Olympiad } from "../types/OlympiadTypes";
import { AcademicYear } from "../types/AcademicYearTypes";
import { StudentOlympiadEnrollmentData } from "../types/EnrollmentTypes";
import LoadingPage from "../components/LoadingPage";
import { LanguageContext } from "../contexts/LanguageContext";

const { Title } = Typography;
const { Option } = Select;
const { Text } = Typography;

const EnrollmentPage: React.FC = () => {
  const [selectedOlympiadId, setSelectedOlympiadId] = useState<string>("");
  const [olympiads, setOlympiads] = useState<Olympiad[]>([]);
  const [academicYears, setAcademicYears] = useState<AcademicYear[]>([]);
  const [email, setEmail] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [selectedClass, setSelectedClass] = useState<number | null>(null);
  const [selectedRound, setSelectedRound] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);

      const storedSession = localStorage.getItem("userSession");
      if (storedSession) {
        const parsedSession = JSON.parse(storedSession);
        setEmail(parsedSession.email);
        setUserId(parsedSession.userId);
      }

      try {
        const olympiadResponse = await fetch(
          `${process.env.REACT_APP_API_URL}/api/olympiad`
        );

        if (olympiadResponse.ok) {
          const data = await olympiadResponse.json();
          setOlympiads(data);
        } else {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Неуспешно зареждане на данни за олимпиадата."
              : "Failed to load Olympiad data.",
          });
        }

        const yearResponse = await fetch(
          `${process.env.REACT_APP_API_URL}/api/academicyear`
        );

        if (yearResponse.ok) {
          const data = await yearResponse.json();
          setAcademicYears(data);
        } else {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Неуспешно зареждане на данни за учебната година."
              : "Failed to load Academic Year data.",
          });
        }
      } catch (error) {
        console.error("Error loading data:", error);
        notification.error({
          message: isBG ? "Грешка в мрежата" : "Network Error",
          description: isBG
            ? "Проблем при зареждане на данните."
            : "There was an issue fetching the data.",
        });
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [isBG]);

  const handleSelectChange = (value: string) => {
    setSelectedOlympiadId(value);
  };

  const handleClassChange = (value: number) => {
    setSelectedClass(value);
    setSelectedOlympiadId("");
  };

  const handleRoundChange = (value: string) => {
    setSelectedRound(value);
    setSelectedOlympiadId("");
  };

  const formatDateToLocal = (utcDate: string, locale: string) => {
    const date = new Date(utcDate);
    return date.toLocaleDateString(locale, {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const formatTimeToLocal = (utcTime: string | null) => {
    if (!utcTime) return isBG ? "Няма конкретен час" : "No specific time";
    const time = new Date(utcTime);
    return time.toLocaleTimeString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  };

  const getCurrentAcademicYearId = () => {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1;

    const selectedAcademicYear = academicYears.find((year) => {
      if (currentMonth < 9) {
        // Before September
        return year.startYear === currentYear - 1; // Previous academic year
      } else {
        return year.startYear === currentYear; // Current academic year
      }
    });

    return selectedAcademicYear ? selectedAcademicYear.academicYearId : null;
  };

  const enrollStudent = async (
    enrollmentData: StudentOlympiadEnrollmentData
  ) => {
    const token = localStorage.getItem("authToken");
    const response = await fetch(
      `${process.env.REACT_APP_API_URL}/api/studentolympiadenrollment`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(enrollmentData),
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Enrollment failed: ${errorText}`);
    }
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      if (!email) {
        notification.error({
          message: isBG ? "Трябва Ви профил" : "Login Required",
          description: isBG
            ? "Трябва да влезете в профила си, за да се запишете."
            : "You must log in to enroll.",
        });
        setIsLoading(false);
        return;
      }
      if (!selectedOlympiadId) {
        notification.error({
          message: isBG ? "Грешка при избора" : "Selection Error",
          description: isBG
            ? "Моля, изберете олимпиада."
            : "Please select an Olympiad.",
        });
        setIsLoading(false);
        return;
      }

      const olympiadDetails = olympiads.find(
        (olympiad) => olympiad.olympiadId === selectedOlympiadId
      );
      if (!olympiadDetails) {
        notification.error({
          message: isBG ? "Грешка" : "Error",
          description: isBG
            ? "Избраните данни за олимпиадата липсват."
            : "Selected Olympiad data is missing.",
        });
        setIsLoading(false);
        return;
      }

      const academicYearId = getCurrentAcademicYearId();
      if (!academicYearId) {
        notification.error({
          message: isBG ? "Грешка" : "Error",
          description: isBG
            ? "Неуспешно зареждане на данни за учебната година."
            : "Failed to load Academic Year data.",
        });
        setIsLoading(false);
        return;
      }

      if (!userId) {
        notification.error({
          message: isBG ? "Трябва Ви профил" : "Login Required",
          description: isBG
            ? "Трябва да влезете в профила си, за да се запишете."
            : "You must log in to enroll.",
        });
        setIsLoading(false);
        return;
      }

      const enrollmentData: StudentOlympiadEnrollmentData = {
        userId: userId,
        olympiadId: olympiadDetails.olympiadId,
        academicYearId: academicYearId,
        enrollmentStatus: "pending",
        createdAt: new Date().toISOString(),
      };

      await enrollStudent(enrollmentData);

      notification.success({
        message: isBG ? "Успешно записване" : "Enrollment Successful",
        description: isBG
          ? `Успешно се записахте за олимпиадата по ${olympiadDetails.subject}.`
          : `You have successfully enrolled in the ${olympiadDetails.subject} Olympiad.`,
      });
    } catch (error) {
      const rawMessage =
        error instanceof Error ? error.message : "An unknown error occurred";
      const errorMessage = rawMessage
        .split("\n")[0]
        .replace("Enrollment failed: ", "");
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description: isBG
          ? `Възникна грешка при записването: ${errorMessage}`
          : `An error occurred during enrollment: ${errorMessage}`,
      });
    } finally {
      setIsLoading(false);
    }
  };

  const sortedClasses = Array.from(
    new Set(olympiads.map((olympiad) => olympiad.classNumber))
  ).sort((a, b) => a - b);

  const filteredOlympiads = olympiads.filter((olympiad) => {
    return (
      (selectedClass === null || olympiad.classNumber === selectedClass) &&
      (selectedRound === null || olympiad.round === selectedRound)
    );
  });

  return (
    <>
      {isLoading && <LoadingPage />}

      <div className="enrollment-page">
        <Title level={2}>
          {isBG ? "Записване за олимпиада" : "Olympiad Enrollment"}
        </Title>

        {userId ? (
          <>
            <div style={{ display: "flex", justifyContent: "center" }}>
              <Text
                className="text"
                style={{
                  fontSize: "18px",
                  fontWeight: "600",
                  marginBottom: "8px",
                  textAlign: "center",
                  color: "var(--text-color)",
                }}
              >
                {isBG
                  ? "Моля, изберете олимпиада от списъка по-долу:"
                  : "Please select an Olympiad from the list below:"}
              </Text>
            </div>
            <Form
              onSubmitCapture={handleSubmit}
              layout="vertical"
              className="enrollment-form"
            >
              <Form.Item
                label={
                  <span style={{ color: "black" }}>
                    {isBG ? "Филтрирай по клас" : "Filter by Class"}
                  </span>
                }
                colon={false}
              >
                <Select
                  onChange={handleClassChange}
                  placeholder={isBG ? "Избери клас" : "Select a class"}
                  allowClear
                >
                  {sortedClasses.map((classNumber) => (
                    <Option key={classNumber} value={classNumber}>
                      {isBG ? `${classNumber}. Клас` : `Class ${classNumber}`}
                    </Option>
                  ))}
                </Select>
              </Form.Item>

              <Form.Item
                label={
                  <span style={{ color: "black" }}>
                    {isBG ? "Филтрирай по кръг" : "Filter by Round"}
                  </span>
                }
                colon={false}
              >
                <Select
                  onChange={handleRoundChange}
                  placeholder={isBG ? "Избери кръг" : "Select a round"}
                  allowClear
                  style={{ width: "100%" }}
                >
                  {Array.from(
                    new Set(olympiads.map((olympiad) => olympiad.round))
                  ).map((round) => (
                    <Option key={round} value={round}>
                      {isBG
                        ? round === "Regional Ring"
                          ? "Регионален"
                          : round === "National Ring"
                          ? "Национален"
                          : round === "District Ring"
                          ? "Областен"
                          : round
                        : round.charAt(0).toUpperCase() + round.slice(1)}
                    </Option>
                  ))}
                </Select>
              </Form.Item>
              <Form.Item
                label={
                  <span style={{ color: "black" }}>
                    {isBG ? "Избери олимпиада" : "Select an Olympiad"}
                  </span>
                }
                colon={false}
              >
                <Select
                  value={selectedOlympiadId}
                  onChange={handleSelectChange}
                  style={{ width: "100%", height: "100%" }}
                >
                  {filteredOlympiads.map((olympiad) => (
                    <Option
                      key={olympiad.olympiadId}
                      value={olympiad.olympiadId}
                    >
                      <div>
                        <strong>
                          {`${olympiad.subject} - ${
                            isBG
                              ? `${olympiad.classNumber}. Клас`
                              : `Class ${olympiad.classNumber}:`
                          } (${olympiad.round})`}
                        </strong>
                        <p>{`${isBG ? "Място" : "Location"}: ${
                          olympiad.location
                        }`}</p>
                        <p>{`${isBG ? "Дата" : "Date"}: ${formatDateToLocal(
                          olympiad.dateOfOlympiad,
                          isBG ? "bg-BG" : "en-US"
                        )}`}</p>
                        <p>{`${
                          isBG ? "Начален час" : "Start Time"
                        }: ${formatTimeToLocal(olympiad.startTime)}`}</p>
                      </div>
                    </Option>
                  ))}
                </Select>
              </Form.Item>
              <Form.Item>
                <Button
                  className="button"
                  htmlType="submit"
                  disabled={!selectedOlympiadId}
                >
                  {isBG ? "Запиши се" : "Submit Enrollment"}
                </Button>
              </Form.Item>
            </Form>
          </>
        ) : (
          <Card
            style={{
              backgroundColor: "white",
              padding: "20px",
              borderRadius: "8px",
            }}
          >
            <Text className="text-card">
              {isBG
                ? "Трябва да влезете в своя профил, за да се запишете за олимпиада."
                : "You need to log in to enroll in an Olympiad."}
            </Text>
          </Card>
        )}
      </div>
    </>
  );
};

export default EnrollmentPage;
