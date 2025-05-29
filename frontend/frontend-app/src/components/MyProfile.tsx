import React, { useState, useEffect, useCallback, useContext } from "react";
import { Card, Button, Typography, Progress } from "antd";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import { UserSession } from "../types/Session";
import "./ui/MyProfile.css";
import LoadingPage from "./LoadingPage";
import { LanguageContext } from "../contexts/LanguageContext";
import { decryptSession } from "../utils/encryption";
import {
  getAuthGetOptions,
  getAuthPostOptionsNoBody,
} from "../config/apiConfig";
import { API_ROUTES } from "../config/api";

const { Title, Text } = Typography;

const MyProfile: React.FC = () => {
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const [session, setSession] = useState<UserSession | null>(null);
  const [role, setRole] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(true);
  const [enrolledOlympiads, setEnrolledOlympiads] = useState<
    {
      name: string;
      status: string;
      academicYear: string;
      dateOfOlympiad: string;
      round: string;
      location: string;
      startTime: string;
    }[]
  >([]);
  const navigate = useNavigate();

  const ranking = 5;
  const totalStudents = 100;
  const progressPercentage = 75;

  const handleLogout = useCallback(() => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("userSession");
    setSession(null);
    setRole("");
    navigate("/login");
  }, [navigate]);

  const validateSession = useCallback(async () => {
    const storedSession = localStorage.getItem("userSession");
    const token = localStorage.getItem("authToken");

    if (storedSession && token) {
      try {
        const response = await fetch(
          API_ROUTES.validateToken,
          getAuthPostOptionsNoBody(token)
        );

        if (response.ok) {
          return true;
        } else {
          handleLogout();
        }
      } catch (error) {
        console.error("Session validation error:", error);
        handleLogout();
      }
    } else {
      navigate("/login");
    }
    return false;
  }, [handleLogout, navigate]);

  const fetchEnrolledOlympiads = useCallback(
    async (userId: number) => {
      setLoading(true);
      const token = localStorage.getItem("authToken");

      try {
        const response = await fetch(
          API_ROUTES.userEnrollments(String(userId)),
          getAuthGetOptions(token ?? "")
        );

        if (response.ok) {
          const data = await response.json();

          const filteredOlympiads = data
            .filter(
              (enrollment: any) =>
                enrollment.enrollmentStatus === "pending" &&
                enrollment.academicYearId === 2
            )
            .sort(
              (a: any, b: any) =>
                new Date(b.createdAt).getTime() -
                new Date(a.createdAt).getTime()
            )
            .slice(0, 3)
            .map((enrollment: any) => {
              const dateObj = new Date(enrollment.olympiad.dateOfOlympiad);
              const rawTime = enrollment.olympiad.startTime;
              let formattedTime = "N/A";

              if (rawTime) {
                const timeObj = new Date(rawTime);
                if (!isNaN(timeObj.getTime())) {
                  formattedTime = timeObj.toLocaleTimeString("en-US", {
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: false,
                  });
                }
              }

              return {
                name: enrollment.olympiad.subject,
                status: enrollment.enrollmentStatus,
                academicYear: enrollment.academicYearId,
                dateOfOlympiad: dateObj.toLocaleDateString(
                  isBG ? "bg-BG" : "en-US",
                  {
                    weekday: "long",
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                  }
                ),
                round: enrollment.olympiad.round,
                location: enrollment.olympiad.location,
                startTime: formattedTime,
              };
            });

          setEnrolledOlympiads(filteredOlympiads);
        } else {
          console.error("Failed to fetch enrolled Olympiads.");
        }
      } catch (error) {
        console.error("Error fetching enrolled Olympiads:", error);
      } finally {
        setLoading(false);
      }
    },
    [isBG]
  );

  useEffect(() => {
    const initializeSession = async () => {
      const isValid = await validateSession();
      if (isValid) {
        const storedSession = localStorage.getItem("userSession");
        if (storedSession) {
          try {
            const parsedSession = decryptSession(storedSession);
            setSession(parsedSession);
            setRole(parsedSession.role);
            fetchEnrolledOlympiads(parsedSession.userId);
          } catch (error) {
            console.error("Failed to decrypt session:", error);
          }
        }
      }
      setLoading(false);
    };
    initializeSession();
  }, [validateSession, fetchEnrolledOlympiads]);

  return (
    <>
      {loading && <LoadingPage />}

      <div className="container">
        <div className="left-section">
          <Card className="profile-card">
            <Title level={2} style={{ color: "var(--text-color)" }}>
              {isBG ? "Моят профил" : "My Profile"}
            </Title>
            <Text className="profile-card-text">
              <strong>{isBG ? "Име:" : "Name:"}</strong>{" "}
              {session?.full_name || "N/A"}
            </Text>
            <br />
            <Text className="profile-card-text">
              <strong>{isBG ? "Имейл:" : "Email:"}</strong> {session?.email}
            </Text>
            <br />
            <Text className="profile-card-text">
              <strong>{isBG ? "Роля:" : "Role:"}</strong>{" "}
              {role || (isBG ? "Зареждане..." : "Loading role...")}
            </Text>
          </Card>
          <Card className="account-card">
            <Title level={2} style={{ color: "var(--text-color)" }}>
              {isBG ? "Управление на акаунт" : "Account Management"}
            </Title>
            <motion.div whileHover={{ scale: 1.05 }}>
              <Button
                className="button"
                onClick={() => navigate("/request-password-change")}
              >
                {isBG ? "Смяна на парола" : "Change Password"}
              </Button>
            </motion.div>
            <motion.div whileHover={{ scale: 1.05 }}>
              <Button
                className="button"
                onClick={() => navigate("/update-info")}
              >
                {isBG ? "Редактирай профила" : "Update Profile"}
              </Button>
            </motion.div>
            <motion.div whileHover={{ scale: 1.05 }}>
              <Button className="logout-button" danger onClick={handleLogout}>
                {isBG ? "Изход" : "Logout"}
              </Button>
            </motion.div>
          </Card>
        </div>

        <div className="right-section">
          <Title level={2} style={{ color: "var(--text-color)" }}>
            {isBG ? "Прогрес на олимпиади" : "Olympiad Progress"}
          </Title>
          <Text className="ranking-text">
            {isBG
              ? `Класиране: #${ranking} от ${totalStudents}`
              : `Ranking: #${ranking} out of ${totalStudents}`}
          </Text>
          <Progress
            percent={progressPercentage}
            status="active"
            className="progress-bar"
          />
          <Title
            level={5}
            style={{ marginTop: "16px", color: "var(--text-color)" }}
          >
            {isBG ? "Записани олимпиади:" : "Enrolled Olympiads:"}
          </Title>
          {enrolledOlympiads.length > 0 ? (
            <ul className="olympiads-list">
              {enrolledOlympiads.map((olympiad, index) => (
                <li key={index} className="olympiad-item">
                  <strong>{olympiad.name}</strong> - {olympiad.status} <br />
                  <Text style={{ color: "var(--text-color)" }}>
                    <strong>
                      {isBG ? "Учебна година:" : "Academic Year:"}
                    </strong>{" "}
                    {olympiad.academicYear}
                  </Text>
                  <br />
                  <Text style={{ color: "var(--text-color)" }}>
                    <strong>{isBG ? "Дата:" : "Date:"}</strong>{" "}
                    {olympiad.dateOfOlympiad}
                  </Text>
                  <br />
                  <Text style={{ color: "var(--text-color)" }}>
                    <strong>{isBG ? "Кръг:" : "Round:"}</strong>{" "}
                    {olympiad.round}
                  </Text>
                  <br />
                  <Text style={{ color: "var(--text-color)" }}>
                    <strong>{isBG ? "Локация:" : "Location:"}</strong>{" "}
                    {olympiad.location}
                  </Text>
                  <br />
                  <Text style={{ color: "var(--text-color)" }}>
                    <strong>{isBG ? "Начален час:" : "Start Time:"}</strong>{" "}
                    {olympiad.startTime}
                  </Text>
                </li>
              ))}
            </ul>
          ) : (
            <Text style={{ color: "var(--text-color)" }}>
              {isBG
                ? "Няма записани олимпиади към момента."
                : "No recent pending enrollments."}
            </Text>
          )}
        </div>
      </div>
    </>
  );
};

export default MyProfile;
