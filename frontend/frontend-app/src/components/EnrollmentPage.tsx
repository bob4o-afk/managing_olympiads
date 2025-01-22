import React, { useState, useEffect, FormEvent } from "react";
import { Button, Form, Select, Typography, notification, Card, Input } from "antd";
import "./ui/EnrollmentPage.css";

import { Olympiad } from "../types/OlympiadTypes";
import { AcademicYear } from "../types/AcademicYearTypes";

const { Title } = Typography;
const { Option } = Select;
const { Text } = Typography;

const EnrollmentPage: React.FC = () => {
  const [selectedOlympiadId, setSelectedOlympiadId] = useState<string>("");
  const [olympiads, setOlympiads] = useState<Olympiad[]>([]);
  const [academicYears, setAcademicYears] = useState<AcademicYear[]>([]);
  const [email, setEmail] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [, setEmailSent] = useState<boolean>(false);

  useEffect(() => {
    const fetchOlympiads = async () => {
      try {
        const response = await fetch(`${process.env.REACT_APP_API_URL}/api/olympiad`);

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
        const response = await fetch(`${process.env.REACT_APP_API_URL}/api/academicyear`);
        
        if (response.ok) {
          const data = await response.json();
          setAcademicYears(data);
        } else {
          notification.error({
            message: 'Error',
            description: 'Failed to load Academic Year data.',
          });
        }
      } catch (error) {
        console.error('Error fetching Academic Years:', error);
        notification.error({
          message: 'Network Error',
          description: 'There was an issue fetching the Academic Year data.',
        });
      }
    };

    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      const parsedSession = JSON.parse(storedSession);
      setEmail(parsedSession.email);
      setUserId(parsedSession.userId);
    }

    fetchOlympiads();
    fetchAcademicYears();
  }, []);

  const handleSelectChange = (value: string) => {
    setSelectedOlympiadId(value);
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

  const formatTimeToLocal = (utcTime: string) => {
    const time = new Date(utcTime);
    return time.toLocaleTimeString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
      hour12: false,
    });
  };

  const getCurrentAcademicYearId = () => {
    const currentDate = new Date();
    const currentYear = currentDate.getFullYear();
    const currentMonth = currentDate.getMonth() + 1;

    const selectedAcademicYear = academicYears.find((year) => {
      if (currentMonth < 9) { // Before September
        return year.startYear === currentYear - 1; // Previous academic year
      } else {
        return year.startYear === currentYear; // Current academic year
      }
    });

    return selectedAcademicYear ? selectedAcademicYear.academicYearId : null;
  };


  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!email) {
      notification.error({
        message: "Login Required",
        description: "You must log in to enroll.",
      });
      return;
    }

    if (!selectedOlympiadId) {
      notification.error({
        message: "Selection Error",
        description: "Please select an Olympiad.",
      });
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
      return;
    }

    const academicYearId = getCurrentAcademicYearId();
    if (!academicYearId) {
      notification.error({
        message: 'Academic Year Error',
        description: 'Could not determine the current academic year.',
      });
      return;
    }

    
    const enrollmentData = {
      userId: userId,
      olympiadId: olympiadDetails.olympiadId,
      academicYearId: academicYearId,
      enrollmentStatus: "pending",
      createdAt: new Date().toISOString(),
    };

    try {
      var token = localStorage.getItem("authToken");
      setToken(token);
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

      if (response.ok) {
        notification.success({
          message: "Enrollment Successful",
          description: `You have successfully enrolled in the ${olympiadDetails.subject} Olympiad.`,
        });

        const emailData = {
          toEmail: email,
          subject: olympiadDetails.subject,
          body: `
          Dear Student, \n\n
          You have successfully enrolled in the ${olympiadDetails.subject} Olympiad.\n\n
          ${`Location: ${olympiadDetails.location}\n`}
          ${`Date: ${new Date(
            olympiadDetails.dateOfOlympiad
          ).toLocaleDateString()}\n`}
          ${`Start Time: ${new Date(
            olympiadDetails.startTime
          ).toLocaleTimeString()}\n`}
          `,
          ccEmail: process.env.REACT_APP_EMAIL_CC,
        };
  
        try {
          const response = await fetch(`${process.env.REACT_APP_API_URL}/api/email/send`, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(emailData),
          });
  
          if (response.ok) {
            setEmailSent(true);
            notification.success({
              message: "Success",
              description: "Enrollment email successfully sent!",
            });
          } else {
            const errorText = await response.text();
            notification.error({
              message: "Sending Error",
              description: `There was an error sending the email: ${errorText}`,
            });
          }
        } catch (error) {
          console.error("Error:", error);
          notification.error({
            message: "Network Error",
            description: "There was a problem with the network or server.",
          });
        }
      } else {
        const errorText = await response.text();
        notification.error({
          message: "Enrollment Error",
          description: `There was an error enrolling: ${errorText}`,
        });
      }
    } catch (error) {
      console.error("Error:", error);
      notification.error({
        message: "Network Error",
        description: "There was a problem with the network or server.",
      });
    }
  };

  return (
    <div className="enrollment-page">
      <Title level={2}>Olympiad Enrollment</Title>

      {userId ? (
        <>
          <Text
            style={{ fontSize: "18px", fontWeight: "600", marginBottom: "8px" }}
          >
            Please select an Olympiad from the list below:
          </Text>

          <Form onSubmitCapture={handleSubmit} className="enrollment-form">
            <Form.Item label="Select an Olympiad">
              <Select
                value={selectedOlympiadId}
                onChange={handleSelectChange}
                placeholder="Select an Olympiad"
                style={{ width: "100%", height: "100%" }}
              >
                {olympiads.map((olympiad) => (
                  <Option
                    key={olympiad.olympiadId}
                    value={olympiad.olympiadId}
                  >
                    <div>
                      <strong>{olympiad.subject}</strong>
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
          <Text style={{ fontSize: "16px", fontWeight: "600", color: "#888" }}>
            You need to log in to enroll in an Olympiad.
          </Text>
        </Card>
      )}
    </div>
  );
};

export default EnrollmentPage;
