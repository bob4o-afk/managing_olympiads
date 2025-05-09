import React, { useState, useEffect, FormEvent } from "react";
import { Button, Form, Select, Typography, notification, Card } from "antd";
import "./ui/EnrollmentPage.css";

import { Olympiad } from "../types/OlympiadTypes";
import { AcademicYear } from "../types/AcademicYearTypes";
import LoadingPage from "../components/LoadingPage";

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

  useEffect(() => {
    setIsLoading(true);
    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      const parsedSession = JSON.parse(storedSession);
      setEmail(parsedSession.email);
      setUserId(parsedSession.userId);
    }

    fetchOlympiads();
    fetchAcademicYears();
    setIsLoading(false);
  }, []);

  const fetchOlympiads = async () => {
    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/olympiad`
      );

      if (response.ok) {
        const data = await response.json();
        setOlympiads(data);
      } else {
        notification.error({
          message: "Error",
          description: "Failed to load Olympiad data.",
        });
      }
    } catch (error) {
      console.error("Error fetching Olympiads:", error);
      notification.error({
        message: "Network Error",
        description: "There was an issue fetching the Olympiad data.",
      });
    }
  };

  const fetchAcademicYears = async () => {
    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/academicyear`
      );

      if (response.ok) {
        const data = await response.json();
        setAcademicYears(data);
      } else {
        notification.error({
          message: "Error",
          description: "Failed to load Academic Year data.",
        });
      }
    } catch (error) {
      console.error("Error fetching Academic Years:", error);
      notification.error({
        message: "Network Error",
        description: "There was an issue fetching the Academic Year data.",
      });
    }
  };

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

  const formatDateToLocal = (utcDate: string) => {
    const date = new Date(utcDate);
    return date.toLocaleDateString("en-US", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const formatTimeToLocal = (utcTime: string | null) => {
    if (!utcTime) return "No time specified";
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

  const sendEnrollmentEmail = async (emailData: any) => {
    const response = await fetch(`${process.env.REACT_APP_API_URL}/api/email/send`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(emailData),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Error sending email: ${errorText}`);
    }

    notification.success({ message: "Success", description: "Enrollment email successfully sent!" });
  };

  const enrollStudent = async (enrollmentData: any) => {
    const token = localStorage.getItem("authToken");
    const response = await fetch(`${process.env.REACT_APP_API_URL}/api/studentolympiadenrollment`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(enrollmentData),
    });

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
          message: "Login Required",
          description: "You must log in to enroll.",
        });
        setIsLoading(false);
        return;
      }
      if (!selectedOlympiadId) {
        notification.error({
          message: "Selection Error",
          description: "Please select an Olympiad.",
        });
        setIsLoading(false);
        return;
      }

      const olympiadDetails = olympiads.find(
        (olympiad) => olympiad.olympiadId === selectedOlympiadId
      );
      if (!olympiadDetails) {
        notification.error({
          message: "Error",
          description: "Selected Olympiad data is missing.",
        });
        setIsLoading(false);
        return;
      }

      const academicYearId = getCurrentAcademicYearId();
      if (!academicYearId) {
        notification.error({
          message: "Academic Year Error",
          description: "Could not determine the current academic year.",
        });
        setIsLoading(false);
        return;
      }

      const enrollmentData = {
        userId: userId,
        olympiadId: olympiadDetails.olympiadId,
        academicYearId: academicYearId,
        enrollmentStatus: "pending",
        createdAt: new Date().toISOString(),
      };

      await enrollStudent(enrollmentData);

      notification.success({
        message: "Enrollment Successful",
        description: `You have successfully enrolled in the ${olympiadDetails.subject} Olympiad.`,
      });

      const startTime = new Date(
        olympiadDetails.startTime
      ).toLocaleTimeString();
    
      const emailData = {
        toEmail: email,
        subject: olympiadDetails.subject,
        body: `
        Dear Student, \n\n
        You have successfully enrolled in the ${
          olympiadDetails.subject
        } Olympiad.\n\n
        ${`Location: ${olympiadDetails.location}\n`}
        ${`Date: ${new Date(
          olympiadDetails.dateOfOlympiad
        ).toLocaleDateString()}\n`}
        ${`Start Time: ${startTime}\n`}
        `,
        ccEmail: process.env.REACT_APP_EMAIL_CC,
      };

      await sendEnrollmentEmail(emailData);
    } catch (error) {
      const rawMessage = error instanceof Error ? error.message : "An unknown error occurred";
      const errorMessage = rawMessage.split('\n')[0].replace("Enrollment failed: ", "");
      notification.error({ message: "Error", description: errorMessage });
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
        <Title level={2} style={{ color: "var(--text-color)" }}>
          Olympiad Enrollment
        </Title>

        {userId ? (
          <>
            <div style={{ display: "flex", justifyContent: "center" }}>
              <Text
                style={{
                  fontSize: "18px",
                  fontWeight: "600",
                  marginBottom: "8px",
                  textAlign: "center",
                  color: "var(--text-color)",
                }}
              >
                Please select an Olympiad from the list below:
              </Text>
            </div>
            <Form onSubmitCapture={handleSubmit} className="enrollment-form">
              <Form.Item
                label={<span style={{ color: "black" }}>Filter by Class</span>}
                colon={false}
              >
                <Select
                  onChange={handleClassChange}
                  placeholder="Select a class"
                  allowClear
                >
                  {sortedClasses.map((classNumber) => (
                    <Option key={classNumber} value={classNumber}>
                      Class {classNumber}
                    </Option>
                  ))}
                </Select>
              </Form.Item>

              <Form.Item
                label={<span style={{ color: "black" }}>Filter by Round</span>}
                colon={false}
              >
                <Select
                  onChange={handleRoundChange}
                  placeholder="Select a round"
                  allowClear
                  style={{ width: "100%" }}
                >
                  {Array.from(
                    new Set(olympiads.map((olympiad) => olympiad.round))
                  ).map((round) => (
                    <Option key={round} value={round}>
                      {round}
                    </Option>
                  ))}
                </Select>
              </Form.Item>
              <Form.Item
                label={
                  <span style={{ color: "black" }}>Select an Olympiad</span>
                }
                colon={false}
              >
                <Select
                  value={selectedOlympiadId}
                  onChange={handleSelectChange}
                  placeholder="Select an Olympiad"
                  style={{ width: "100%", height: "100%" }}
                >
                  {filteredOlympiads.map((olympiad) => (
                    <Option
                      key={olympiad.olympiadId}
                      value={olympiad.olympiadId}
                    >
                      <div>
                        <strong>{`${olympiad.subject} - Class ${olympiad.classNumber} (${olympiad.round})`}</strong>
                        <p>{`Location: ${olympiad.location}`}</p>
                        <p>{`Date: ${formatDateToLocal(
                          olympiad.dateOfOlympiad
                        )}`}</p>
                        <p>{`Start Time: ${formatTimeToLocal(
                          olympiad.startTime
                        )}`}</p>
                      </div>
                    </Option>
                  ))}
                </Select>
              </Form.Item>
              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  disabled={!selectedOlympiadId}
                >
                  Submit Enrollment
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
            <Text
              style={{ fontSize: "16px", fontWeight: "600", color: "#888" }}
            >
              You need to log in to enroll in an Olympiad.
            </Text>
          </Card>
        )}
      </div>
    </>
  );
};

export default EnrollmentPage;
