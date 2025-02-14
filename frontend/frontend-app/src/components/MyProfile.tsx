import React, { useState, useEffect, useCallback } from "react";
import { Card, Button, Typography, Progress } from "antd";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import { UserSession } from "../types/Session";
import "./ui/MyProfile.css";

const { Title, Text } = Typography;

const MyProfile: React.FC = () => {
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

  // Hardcoded Data
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
          `${process.env.REACT_APP_API_URL}/api/auth/validate-token`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
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

  const fetchEnrolledOlympiads = useCallback(async (userId: number) => {
    setLoading(true);
    const token = localStorage.getItem("authToken");

    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/StudentOlympiadEnrollment/user/${userId}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.ok) {
        const data = await response.json();

        // Filter by academic year (hardcoded to 2 for now) and status "pending"
        const filteredOlympiads = data
          .filter(
            (enrollment: any) =>
              enrollment.enrollmentStatus === "pending" &&
              enrollment.academicYearId === 2
          )
          .sort(
            (a: any, b: any) =>
              new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
          )
          .slice(0, 3)
          .map((enrollment: any) => ({
            name: enrollment.olympiad.subject,
            status: enrollment.enrollmentStatus,
            academicYear: enrollment.academicYearId,
            dateOfOlympiad: enrollment.olympiad.dateOfOlympiad,
            round: enrollment.olympiad.round,
            location: enrollment.olympiad.location,
            startTime: enrollment.olympiad.startTime || "N/A",
          }));
        console.log(filteredOlympiads);

        setEnrolledOlympiads(filteredOlympiads);
      } else {
        console.error("Failed to fetch enrolled Olympiads.");
      }
    } catch (error) {
      console.error("Error fetching enrolled Olympiads:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    const initializeSession = async () => {
      const isValid = await validateSession();
      if (isValid) {
        const storedSession = localStorage.getItem("userSession");
        if (storedSession) {
          const parsedSession = JSON.parse(storedSession);
          setSession(parsedSession);
          setRole(parsedSession.role);
          fetchEnrolledOlympiads(parsedSession.userId);
        }
      }
      setLoading(false);
    };
    initializeSession();
  }, [validateSession]);

  if (loading) {
    return <p>Loading profile...</p>;
  }

  return (
    <div className="container">
      <div className="left-section">
        <Card className="profile-card">
          <Title level={2} style={{ color: "var(--text-color)" }}>
            My Profile
          </Title>
          <Text className="profile-card-text">
            <strong>Name:</strong> {session?.full_name || "N/A"}
          </Text>
          <br />
          <Text className="profile-card-text">
            <strong>Email:</strong> {session?.email}
          </Text>
          <br />
          <Text className="profile-card-text">
            <strong>Role:</strong> {role || "Loading role..."}
          </Text>
        </Card>
        <Card className="account-card">
          <Title level={2} style={{ color: "var(--text-color)" }}>
            Account Management
          </Title>
          <motion.div whileHover={{ scale: 1.05 }}>
            <Button
              className="button"
              onClick={() => navigate("/request-password-change")}
            >
              Change Password
            </Button>
          </motion.div>
          <motion.div whileHover={{ scale: 1.05 }}>
            <Button className="button" onClick={() => navigate("/update-info")}>
              Update Profile
            </Button>
          </motion.div>
          <motion.div whileHover={{ scale: 1.05 }}>
            <Button className="logout-button" danger onClick={handleLogout}>
              Logout
            </Button>
          </motion.div>
        </Card>
      </div>

      <div className="right-section">
        <Title level={2} style={{ color: "var(--text-color)" }}>
          Olympiad Progress
        </Title>
        <Text className="ranking-text">
          Ranking: #{ranking} out of {totalStudents}
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
          Enrolled Olympiads:
        </Title>
        {enrolledOlympiads.length > 0 ? (
          <ul className="olympiads-list">
            {enrolledOlympiads.map((olympiad, index) => (
              <li key={index} className="olympiad-item">
                <strong>{olympiad.name}</strong> - {olympiad.status} <br />
                <Text style={{ color: "var(--text-color)" }}>
                  <strong>Academic Year:</strong> {olympiad.academicYear}
                </Text>
                <br />
                <Text style={{ color: "var(--text-color)" }}>
                  <strong>Date:</strong> {olympiad.dateOfOlympiad}
                </Text>
                <br />
                <Text style={{ color: "var(--text-color)" }}>
                  <strong>Round:</strong> {olympiad.round}
                </Text>
                <br />
                <Text style={{ color: "var(--text-color)" }}>
                  <strong>Location:</strong> {olympiad.location}
                </Text>
                <br />
                <Text style={{ color: "var(--text-color)" }}>
                  <strong>Start Time:</strong> {olympiad.startTime}
                </Text>
              </li>
            ))}
          </ul>
        ) : (
          <Text style={{ color: "var(--text-color)" }}>
            No recent pending enrollments.
          </Text>
        )}
      </div>
    </div>
  );
};

export default MyProfile;
