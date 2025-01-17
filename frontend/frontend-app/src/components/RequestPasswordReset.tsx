import { useState, FormEvent } from "react";
import "./ui/RequestPasswordReset.css";

const RequestPasswordReset = () => {
  const [usernameOrEmail, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleResetPassword = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    setError("");

    try {
        localStorage.setItem('resetInfo', usernameOrEmail);
        const response = await fetch('http://localhost:5138/api/auth/request-password-change', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ usernameOrEmail: usernameOrEmail }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "An error occurred. Please try again.");
        }

        const data = await response.json();
        setMessage(data.message);
    } catch (error: any) {
        setError(error.message);
    } finally {
        setLoading(false);
    }
  };

  return (
    <div className="reset-password-container">
      <h2>Reset Password</h2>
      <form onSubmit={handleResetPassword} className="reset-password-form">
        <input
          type="email"
          placeholder="Enter your email"
          value={usernameOrEmail}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <button type="submit" disabled={loading}>
          {loading ? "Sending..." : "Send Password Reset Email"}
        </button>
      </form>
      {message && <p className="success-message">{message}</p>}
      {error && <p className="error-message">{error}</p>}
    </div>
  );
};

export default RequestPasswordReset;
