import React, { useState, useRef, useEffect, useContext } from "react";
import {
  Button,
  Card,
  Form,
  Input,
  Select,
  Typography,
  notification,
} from "antd";
import "./ui/Documents.css";
import LoadingPage from "../components/LoadingPage";
import { LanguageContext } from "../contexts/LanguageContext";
import { Particle } from "./particles/Particle";
import { spawnParticles } from "./particles/particleUtils";
import { motion } from "framer-motion";

const { Title, Text } = Typography;
const { Option } = Select;

const Documents: React.FC = () => {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [sessionName, setSessionName] = useState<string>("");
  const [isLoading, setIsLoading] = useState(false);
  const [form] = Form.useForm();
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const predefinedSchool =
    'Технологично училище "Електронни системи" към ТУ-София';
  const [schoolSuggestion, setSchoolSuggestion] = useState("");

  useEffect(() => {
    const session = localStorage.getItem("userSession");
    if (session) {
      const parsed = JSON.parse(session);
      setEmail(parsed.email);
      setSessionName(parsed.full_name);
    }

    const canvas = canvasRef.current;
    const ctx = canvas?.getContext("2d");
    if (!canvas || !ctx) return;

    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;

    const particles: Particle[] = [];
    const colors = ["#FF0080", "#FF8C00", "#40E0D0", "#00BFFF", "#FFB6C1"];

    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      particles.forEach((p, i) => {
        p.update();
        p.draw(ctx);
        if (p.life <= 0) particles.splice(i, 1);
      });
      spawnParticles(canvas, colors, particles);
      requestAnimationFrame(animate);
    };
    animate();
    return () => {
      particles.length = 0;
    };
  }, []);

  const handleSubmit = async () => {
    setIsLoading(true);
    const values = form.getFieldsValue();
    const fullPhone = `+359${values.telephone}`;

    const nameParts = (val: string) =>
      val
        .trim()
        .split(/[\s-]+/)
        .filter(Boolean);

    if (!email) {
      notification.error({
        message: "Login",
        description: isBG ? "Моля, влезте." : "Please log in.",
      });
      setIsLoading(false);
      return;
    }

    if (nameParts(sessionName).length !== 3) {
      notification.error({
        message: isBG ? "Невалидно име" : "Invalid Name",
        description: isBG
          ? "Трябва да въведете трите имена на ученика."
          : "Please enter the student's full name.",
      });
      setIsLoading(false);
      return;
    }

    const data = {
      ...values,
      telephone: fullPhone,
      email,
      studentName: sessionName,
    };

    try {
      const response = await fetch(
        `${process.env.REACT_APP_PYTHON_API_URL}/fill_pdf`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(data),
        }
      );
      if (!response.ok) throw new Error("Failed");
      const result = await response.json();
      notification.success({
        message: isBG ? "Изпратено" : "Submitted",
        description:
          result.message ||
          (isBG ? "Формулярът е изпратен!" : "Form submitted!"),
      });
      form.resetFields();
    } catch (e) {
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description: isBG ? "Нещо се обърка." : "Something went wrong.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      {isLoading && <LoadingPage />}
      <div className="document-container">
        <div className="form-container">
          <Title level={3}>
            {isBG ? "Формуляр за документи" : "Document Form"}
          </Title>

          {email ? (
            <Form
              form={form}
              layout="vertical"
              onFinish={handleSubmit}
              initialValues={{ studentName: sessionName }}
              style={{ width: "100%" }}
            >
              <Form.Item
                label={isBG ? "Име на ученик" : "Student Name"}
                name="studentName"
              >
                <Input readOnly />
              </Form.Item>

              <Form.Item
                label={isBG ? "Име на родител" : "Parent Name"}
                name="parentName"
                rules={[
                  {
                    required: true,
                    message: isBG ? "Въведете име" : "Enter name",
                  },
                  {
                    validator: (_, val) => {
                      return val && val.trim().split(/[\s-]+/).length === 3
                        ? Promise.resolve()
                        : Promise.reject(
                            isBG
                              ? "Трябва да въведете три имена"
                              : "Enter full (3-part) name"
                          );
                    },
                  },
                ]}
              >
                <Input />
              </Form.Item>

              <Form.Item
                label={isBG ? "Адрес" : "Address"}
                name="address"
                rules={[
                  {
                    required: true,
                    message: isBG
                      ? "Моля, въведете адрес"
                      : "Please enter address",
                  },
                ]}
              >
                <Input />
              </Form.Item>

              <Form.Item
                label={isBG ? "Телефон" : "Telephone"}
                name="telephone"
                rules={[
                  {
                    required: true,
                    message: isBG ? "Въведете телефон" : "Enter phone",
                  },
                  {
                    pattern: /^\d{9}$/,
                    message: isBG
                      ? "Въведете 9 цифри без кода (напр. 888123456)"
                      : "Enter 9 digits without prefix (e.g. 888123456)",
                  },
                ]}
              >
                <Input
                  addonBefore={
                    <span style={{ color: "var(--text-color)" }}>+359</span>
                  }
                  placeholder="888123456"
                  style={{ color: "var(--text-color)" }}
                />
              </Form.Item>

              <Form.Item
                label={isBG ? "Клас" : "Grade"}
                name="grade"
                rules={[
                  {
                    required: true,
                    message: isBG ? "Изберете клас" : "Select grade",
                  },
                ]}
              >
                <Select className="grade-select">
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
                  ].map((g) => (
                    <Option key={g} value={g}>
                      {g}
                    </Option>
                  ))}
                </Select>
              </Form.Item>

              <Form.Item
                label={isBG ? "Училище" : "School"}
                name="school"
                rules={[
                  {
                    required: true,
                    message: isBG ? "Въведете училище" : "Enter school",
                  },
                ]}
              >
                <Input
                  onChange={(e) => {
                    const val = e.target.value;
                    if (
                      predefinedSchool.startsWith(val) &&
                      val !== predefinedSchool
                    )
                      setSchoolSuggestion(predefinedSchool);
                    else setSchoolSuggestion("");
                  }}
                  onKeyDown={(e) => {
                    if (e.key === "Tab" && schoolSuggestion) {
                      e.preventDefault();
                      form.setFieldValue("school", predefinedSchool);
                      setSchoolSuggestion("");
                    }
                  }}
                  onBlur={() => setSchoolSuggestion("")}
                />
              </Form.Item>

              {schoolSuggestion && (
                <div
                  className="school-autocomplete-suggestion"
                  onMouseDown={() => {
                    form.setFieldValue("school", predefinedSchool);
                    setSchoolSuggestion("");
                  }}
                >
                  {schoolSuggestion}
                </div>
              )}

              <motion.div whileHover={{ scale: 1.05 }}>
                <Button className="button" htmlType="submit">
                  {isBG ? "Изпрати" : "Submit"}
                </Button>
              </motion.div>
            </Form>
          ) : (
            <Card
              style={{ backgroundColor: "white", padding: 20, borderRadius: 8 }}
            >
              <Text style={{ fontSize: 16, fontWeight: 600, color: "#888" }}>
                {isBG
                  ? "Трябва да влезете, за да попълните формуляра."
                  : "You need to log in to fill out the form."}
              </Text>
            </Card>
          )}
        </div>

        <div className="cool-container">
          <canvas ref={canvasRef} className="animated-canvas"></canvas>
        </div>
      </div>
    </>
  );
};

export default Documents;
