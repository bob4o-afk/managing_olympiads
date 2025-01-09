import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import './ui/Login.css';

function Login(): JSX.Element {
    const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string | null>(null);
    const [token, setToken] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        // Check localStorage for token
        const storedToken = localStorage.getItem("authToken");
        if (storedToken) {
            setToken(storedToken);
        }
    }, []);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
    
        try {
            const response = await fetch("http://localhost:5138/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ usernameOrEmail, password }),
            });
    
            if (!response.ok) {
                const errorText = await response.text();
                console.error("API Error Response:", errorText);
                throw new Error("Login failed. Please check your credentials and try again.");
            }
    
            const data = await response.json();
            setToken(data.token);
            localStorage.setItem("authToken", data.token);
            setError(null);
            console.log("Logged in successfully", data);
        } catch (err: any) {
            setError(err.message);
            console.error("Login error:", err);
        }
    };
    

    const handleLogout = () => {
        setToken(null);
        localStorage.removeItem("authToken"); // Clear token from localStorage
        console.log("Logged out successfully");
    };

    const handlePasswordRecovery = () => {
        navigate('/request-password-change');
    };

    return (
        <div className="login-container">
            {!token ? (
                <>
                    <h2>Login</h2>
                    <form onSubmit={handleLogin}>
                        <div className="form-group">
                            <label>Username or Email</label>
                            <input
                                type="text"
                                value={usernameOrEmail}
                                onChange={(e) => setUsernameOrEmail(e.target.value)}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label>Password</label>
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>
                        <button type="submit">Login</button>
                    </form>
                    {error && <p className="error-message">{error}</p>}
                    <div className="recovery-section">
                        <button onClick={handlePasswordRecovery}>
                            Forgot Password?
                        </button>
                    </div>
                </>
            ) : (
                <div className="session-info">
                    <p>Logged in successfully</p>
                    <button onClick={handleLogout}>Logout</button>
                </div>
            )}
        </div>
    );
}

export default Login;
