import React, { useState, FormEvent, useRef, useEffect } from "react";
import { Card, Typography, notification } from "antd";
import "./ui/Documents.css";

const { Title } = Typography;
const { Text } = Typography;

const Documents: React.FC = () => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [email, setEmail] = useState<string | null>(null);

  useEffect(() => {
    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      const parsedSession = JSON.parse(storedSession);
      setEmail(parsedSession.email);
    }

    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;

    const particles: any[] = [];
    const colors = ["#FF0080", "#FF8C00", "#40E0D0", "#00BFFF", "#FFB6C1"];

    class Particle {
      x: number;
      y: number;
      speed: number;
      angle: number;
      accel: number;
      life: number;
      decay: number;
      color: string;

      constructor(x: number, y: number, color: string) {
        this.x = x;
        this.y = y;
        this.speed = Math.random() * 2 + 0.5;
        this.angle = Math.random() * Math.PI * 2;
        this.accel = 0.01;
        this.life = Math.random() * 50 + 50;
        this.decay = 0.98;
        this.color = color;
      }

      update() {
        this.speed *= this.decay;
        this.x += Math.cos(this.angle) * this.speed;
        this.y += Math.sin(this.angle) * this.speed;
        this.life -= 1;
      }

      draw(ctx: CanvasRenderingContext2D) {
        ctx.beginPath();
        ctx.arc(this.x, this.y, 2, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
      }
    }

    function spawnParticles() {
      if (!canvas) return;
      const centerX = canvas.width / 2;
      const centerY = canvas.height / 2;

      for (let i = 0; i < 5; i++) {
        const color = colors[Math.floor(Math.random() * colors.length)];
        particles.push(new Particle(centerX, centerY, color));
      }
    }

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

      spawnParticles();
      requestAnimationFrame(animate);
    }

    animate();

    return () => {
      particles.length = 0;
    };
  }, []);

  const formFields = [
    { label: "Parent Name", name: "parentName" },
    { label: "Address", name: "address" },
    { label: "Telephone", name: "telephone" },
    { label: "Student Name", name: "studentName" },
    { label: "Grade", name: "grade" },
    { label: "School", name: "school" },
    { label: "Gender", name: "gender" },
    { label: "Test", name: "test" },
  ];

  const [formData, setFormData] = useState(
    formFields.reduce((acc, field) => {
      acc[field.name] = "";
      return acc;
    }, {} as Record<string, string>)
  );

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
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

    const data = { ...formData, email };

    try {
      const response = await fetch(`${process.env.PYTHON_APP_API_URL}/fill_pdf`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error("Form submission failed");
      }

      const result = await response.json();
      notification.success({
        message: "Form Submitted",
        description: result.message || "Your form has been submitted successfully!",
      });

      setFormData(
        formFields.reduce((acc, field) => {
          acc[field.name] = "";
          return acc;
        }, {} as Record<string, string>)
      );
    } catch (error: any) {
      notification.error({
        message: "Submission Error",
        description:
          error.message || "An error occurred while submitting the form.",
      });
    }
  };

  return (
    <div className="document-container">
      <div className="form-container">
        <Title level={3}>Document Form</Title>
        {email ? (
          <form onSubmit={handleSubmit}>
            {formFields.map((field) => (
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
            ))}
            <button type="submit" className="submit-button">
              Submit
            </button>
          </form>
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

      <div className="cool-container">
        <canvas ref={canvasRef} className="animated-canvas"></canvas>
      </div>
    </div>
  );
};

export default Documents;
