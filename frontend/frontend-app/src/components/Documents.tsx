import React, { useState, FormEvent, useRef, useEffect, useMemo } from "react";
import { Button, Card, Typography, notification } from "antd";
import "./ui/Documents.css";

import { Particle } from "./particles/Particle";
import { spawnParticles } from "./particles/particleUtils";
import { motion } from "framer-motion";

const { Title } = Typography;
const { Text } = Typography;

const Documents: React.FC = () => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [sessionName, setSessionName] = useState<string>("");
  const particlesRef = useRef<Particle[]>([]);
  const colors = useMemo(
    () => ["#FF0080", "#FF8C00", "#40E0D0", "#00BFFF", "#FFB6C1"],
    []
  );

  useEffect(() => {
    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      const parsedSession = JSON.parse(storedSession);
      setEmail(parsedSession.email);
      setSessionName(parsedSession.full_name);
    }

    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;

    const particles = particlesRef.current;

    function animate() {
      if (!ctx || !canvas) return;
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      particles.forEach((particle, index) => {
        particle.update();
        particle.draw(ctx);
        if (particle.life <= 0) {
          particles.splice(index, 1);
        }
      });

      spawnParticles(canvas, colors, particles);
      requestAnimationFrame(animate);
    }

    animate();

    return () => {
      particles.length = 0;
    };
  }, [colors, particlesRef]);

  const formFields = [
    { label: "Parent Name", name: "parentName" },
    { label: "Address", name: "address" },
    { label: "Telephone", name: "telephone" },
    { label: "Grade", name: "grade" },
    { label: "School", name: "school" },
  ];

  const [formData, setFormData] = useState(
    formFields.reduce((acc, field) => {
      acc[field.name] = "";
      return acc;
    }, {} as Record<string, string>)
  );

  const [schoolSuggestion, setSchoolSuggestion] = useState<string>("");

  const predefinedSchool =
    'Технологично училище "Електронни системи" към ТУ-София';

  const isValidName = (name: string) => {
    const parts = name.split(/[\s-]+/).filter(Boolean);
    return parts.length === 3;
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleGradeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const { value } = e.target;
    setFormData((prev) => ({ ...prev, grade: value }));
  };

  const handleSchoolChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { value } = e.target;
    setFormData((prev) => ({ ...prev, school: value }));

    if (predefinedSchool.startsWith(value) && value !== predefinedSchool) {
      setSchoolSuggestion(predefinedSchool);
    } else {
      setSchoolSuggestion("");
    }
  };

  const handleSchoolKeyPress = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Tab") {
      if (schoolSuggestion) {
        setFormData((prev) => ({
          ...prev,
          school: predefinedSchool,
        }));
        setSchoolSuggestion("");
      } else {
        e.preventDefault();
      }
    }
  };

  const handleSchoolBlur = () => {
    setSchoolSuggestion("");
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!email) {
      notification.error({
        message: "Login Required",
        description: "You must log in to fill out the form.",
      });
      return;
    }

    const studentValid = isValidName(sessionName);
    const parentValid = isValidName(formData.parentName);

    if (!studentValid) {
      notification.error({
        message: "Invalid Student Name",
        description: "Student name must be full.",
      });
      return;
    }

    if (!parentValid) {
      notification.error({
        message: "Invalid Parent Name",
        description: "Parent name must be full.",
      });
      return;
    }

    const data = { ...formData, email, studentName: sessionName };

    try {
      const response = await fetch(
        `${process.env.REACT_APP_PYTHON_API_URL}/fill_pdf`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(data),
        }
      );

      if (!response.ok) {
        throw new Error("Form submission failed");
      }

      const result = await response.json();
      notification.success({
        message: "Form Submitted",
        description:
          result.message || "Your form has been submitted successfully!",
      });

      setFormData(
        formFields.reduce((acc, field) => {
          acc[field.name] = "";
          return acc;
        }, {} as Record<string, string>)
      );
    } catch (error) {
      if (error instanceof Error) {
        notification.error({
          message: "Submission Error",
          description:
            error.message || "An error occurred while submitting the form.",
        });
      } else {
        notification.error({
          message: "Unknown Error",
          description: "An unexpected error occurred.",
        });
      }
    }
  };

  return (
    <div className="document-container">
      <div className="form-container">
        <Title level={3}>Document Form</Title>
        {email ? (
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="studentName">Name</label>
              <input
                type="text"
                id="studentName"
                name="studentName"
                value={sessionName}
                readOnly
                className="readonly-field"
                autoFocus
              />
            </div>

            {formFields.map((field) => {
              if (field.name === "grade") {
                return (
                  <div key={field.name} className="form-group">
                    <label htmlFor={field.name}>{field.label}</label>
                    <select
                      id={field.name}
                      name={field.name}
                      value={formData[field.name]}
                      onChange={handleGradeChange}
                      required
                      className="custom-dropdown"
                    >
                      {[
                        "8А",
                        "8Б",
                        "8В",
                        "8Г",
                        "9А",
                        "9Б",
                        "9В",
                        "9Г",
                        "10А",
                        "10Б",
                        "10В",
                        "10Г",
                        "11А",
                        "11Б",
                        "11В",
                        "11Г",
                        "12А",
                        "12Б",
                        "12В",
                        "12Г",
                      ].map((grade) => (
                        <option key={grade} value={grade}>
                          {grade}
                        </option>
                      ))}
                    </select>
                  </div>
                );
              } else if (field.name === "school") {
                return (
                  <div key={field.name} className="form-group">
                    <label htmlFor={field.name}>{field.label}</label>
                    <input
                      type="text"
                      id={field.name}
                      name={field.name}
                      value={formData[field.name]}
                      onChange={handleSchoolChange}
                      onKeyDown={handleSchoolKeyPress}
                      onBlur={handleSchoolBlur}
                      required
                      placeholder="Enter school name"
                    />
                    {schoolSuggestion && (
                      <div className="suggestion">{schoolSuggestion}</div>
                    )}
                  </div>
                );
              } else {
                return (
                  <div key={field.name} className="form-group">
                    <label htmlFor={field.name}>{field.label}</label>
                    <input
                      type="text"
                      id={field.name}
                      name={field.name}
                      value={formData[field.name]}
                      onChange={handleInputChange}
                      required
                    />
                  </div>
                );
              }
            })}

            <motion.div whileHover={{ scale: 1.05 }}>
              <Button htmlType="submit" className="submit-button">
                Submit
              </Button>
            </motion.div>
          </form>
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

      <div className="cool-container">
        <canvas ref={canvasRef} className="animated-canvas"></canvas>
      </div>
    </div>
  );
};

export default Documents;
