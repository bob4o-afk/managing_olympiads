import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./ui/UpdateInfo.css";
import { notification } from "antd";
import LoadingPage from "./LoadingPage";

const UpdateInfo: React.FC = () => {
  const [fullName, setFullName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("userSession");
    setFullName("");
    setEmail("");
    navigate("/login");
  };

  const handleUpdateInfo = async (e: React.FormEvent): Promise<void> => {
    setIsLoading(true);
    e.preventDefault();

    const token = localStorage.getItem("authToken");
    const userSession = JSON.parse(localStorage.getItem("userSession") || "{}");
    const userId = userSession?.userId;

    if (!token || !userId) {
      setError("User not authenticated.");
      return;
    }

    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/user/${userId}`,
        {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            name: fullName,
            email: email,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Failed to update user information.");
      }

      notification.success({
        message: "Success",
        description: "You will have to log in again.",
      });

      setTimeout(() => {
        handleLogout();
      }, 2000);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {isLoading && <LoadingPage />}

      <div className="update-info-container">
        <h2>Update Profile Information</h2>
        <form onSubmit={handleUpdateInfo}>
          <div className="form-group">
            <label>Full name</label>
            <input
              type="text"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
              required
            />
          </div>
          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <button type="submit">Update Information</button>
        </form>
        {error && <p className="error-message">{error}</p>}
      </div>
    </>
  );
};

export default UpdateInfo;
