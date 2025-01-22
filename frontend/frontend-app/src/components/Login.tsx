import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { HiEye, HiEyeOff } from "react-icons/hi";
import "./ui/Login.css";

const Login: React.FC = (): JSX.Element => {
    const [usernameOrEmail, setUsernameOrEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string | null>(null);
    const [token, setToken] = useState<string | null>(null);
    const [showPassword, setShowPassword] = useState<boolean>(false);
    const navigate = useNavigate();

    useEffect(() => {
        const storedSession = localStorage.getItem("userSession");
        if (storedSession) {
            const parsedSession = JSON.parse(storedSession);
            if (parsedSession?.email) {
                setToken(parsedSession.email);
                navigate("/my-profile");
            } else {
                clearStorage();
            }
        } else {
            clearStorage();
        }
    }, [navigate]);

    const clearStorage = (): void => {
        localStorage.clear();
        setToken(null);
        setUsernameOrEmail("");
        setPassword("");
        setError(null);
    };

    const handleLogin = async (e: React.FormEvent): Promise<void> => {
        e.preventDefault();

        try {
            const authResponse = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ usernameOrEmail, password }),
            });

            if (!authResponse.ok) {
                const errorText = await authResponse.text();
                console.error("API Error Response:", errorText);
                throw new Error("Login failed. Please check your credentials and try again.");
            }

            const authData = await authResponse.json();
            const token = authData.token;
            setToken(token);
            localStorage.setItem("authToken", token);

            const userDetails = authData.user;

            const roleResponse = await fetch(`${process.env.REACT_APP_API_URL}/api/UserRoleAssignment/`, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                });

            if (!roleResponse.ok) {
                const errorText = await roleResponse.text();
                console.error("Role Fetch Error:", errorText);
                throw new Error("Failed to retrieve user role.");
            }

            const roleData: Array<{ userId: string; role: { roleName: string } }> = await roleResponse.json();
            const userRoleAssignment = roleData.find(
                (assignment) => assignment.userId === userDetails.userId
            );
            const role = userRoleAssignment ? userRoleAssignment.role.roleName : "Student";

            const userSession = {
                userId: userDetails.userId,
                full_name: userDetails.name,
                email: userDetails.email,
                role: role,
            };

            localStorage.setItem("userSession", JSON.stringify(userSession));
            setError(null);
            console.log("Logged in successfully", userSession);

            navigate("/my-profile");
        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : "An unknown error occurred.";
            setError(errorMessage);
            console.error("Login error:", err);
        }
    };

    const handlePasswordRecovery = (): void => {
        navigate("/request-password-change");
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
                            <div className="password-container">
                                <input
                                    type={showPassword ? "text" : "password"}
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    required
                                />
                                <span
                                    className="password-toggle"
                                    onClick={() => setShowPassword(!showPassword)}
                                >
                                    {showPassword ? <HiEyeOff /> : <HiEye />}
                                </span>
                            </div>
                        </div>
                        <button type="submit">Login</button>
                    </form>
                    {error && <p className="error-message">{error}</p>}
                    <div className="recovery-section">
                        <button onClick={handlePasswordRecovery}>Forgot Password?</button>
                    </div>
                </>
            ) : (
                <div className="session-info">
                    <p>Logged in successfully</p>
                </div>
            )}
        </div>
    );
};

export default Login;
