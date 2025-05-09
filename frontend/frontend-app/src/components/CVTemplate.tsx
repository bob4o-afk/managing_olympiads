import React, { useEffect, useRef, useState } from "react";
import { Button, Typography, Divider, Row, Col, Progress } from "antd";
import html2canvas from "html2canvas";
import jsPDF from "jspdf";
import {
  MailOutlined,
  PhoneOutlined,
  HomeOutlined,
  InstagramOutlined,
  GithubOutlined,
} from "@ant-design/icons";
import { PieChart, Pie, Tooltip } from "recharts";

import "./ui/CVTemplate.css";
import { UserSession } from "../types/Session";

const { Title, Text } = Typography;

const CVTemplate: React.FC = () => {
  const [session, setSession] = useState<UserSession | null>(null);

  useEffect(() => {
    const fetchSession = () => {
      const storedSession = localStorage.getItem("userSession");
      if (!storedSession) return;

      try {
        const parsedSession = JSON.parse(storedSession);
        setSession(parsedSession);
      } catch (error) {
        console.error("Failed to parse session data:", error);
      }
    };

    fetchSession();
  }, []);

  const exportButtonRef = useRef<HTMLButtonElement>(null);

  const personalityData = [
    { name: "Problem Solver", value: 95 },
    { name: "Ambitious", value: 80 },
    { name: "Team Player", value: 85 },
    { name: "Creative Thinker", value: 90 },
  ];

  const exportToPDF = async () => {
    if (session?.email !== "bobi06bobi@gmail.com") return;

    const element = document.getElementById("cv");
    if (!element) return;

    if (exportButtonRef.current) {
      exportButtonRef.current.classList.add("export-btn-hidden"); // Hide the button
    }

    const computedStyle = window.getComputedStyle(document.body);
    const backgroundColor = computedStyle
      .getPropertyValue("--header-background-color")
      .trim();

    const canvas = await html2canvas(element, {
      scale: 2,
      useCORS: true,
      allowTaint: true,
      backgroundColor: backgroundColor,
      logging: true,
      scrollX: 0,
      scrollY: 0,
      windowWidth: document.documentElement.scrollWidth,
      windowHeight: document.documentElement.scrollHeight,
    });

    const dataURL = canvas.toDataURL("image/png");
    const pdf = new jsPDF({
      orientation: "portrait",
      unit: "mm",
      format: [210, 450],
    });

    pdf.addImage(dataURL, "PNG", 0, 0, 210, 450);
    pdf.save("CV.pdf");

    if (exportButtonRef.current) {
      exportButtonRef.current.classList.remove("export-btn-hidden");
    }
  };

  return (
    <div
      style={{
        width: "100%",
        maxWidth: "1075px",
        margin: "0 auto",
        background: "var(--header-background-color)",
      }}
    >
      <div id="cv" className="cv">
        {/* Header */}
        <div className="cv-header">
          <div className="cv-header-text">
            <Title level={1} className="cv-name">
              Borislav Milanov
            </Title>
            <Text className="cv-job-title">Software Developer</Text>
          </div>
          <div className="cv-photo">
            <img src="./me.png" alt="Profile" />
          </div>
        </div>

        {/* Contact Information */}
        <div className="cv-contact">
          <Row justify="space-between" className="contact-info">
            <Col>
              <PhoneOutlined /> <Text>+359876046545</Text>
            </Col>
            <Col>
              <MailOutlined /> <Text>bobi06bobi@gmail.com</Text>
            </Col>
            <Col>
              <HomeOutlined /> <Text>Sofia, Bulgaria</Text>
            </Col>
            <Col>
              <InstagramOutlined /> <Text>instagram.com/borislav_milanov_</Text>
            </Col>
            <Col>
              <GithubOutlined /> <Text>github.com/bob4o-afk</Text>
            </Col>
          </Row>
        </div>

        <Divider />

        {/* Content */}
        <Row>
          {/* Left Column */}
          <Col span={12} className="cv-left">
            {/* Experience Section */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>
                Competitions & Hackathons
              </Title>
              <Divider className="section-divider" />

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/bob4o-afk/smart_car"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "InnoAir Challenge: Fair Green Transition and Urban Mobility"
                  hackathon | Fourth Place | 07.11.2022 - 11.11.2022
                </a>
              </Text>
              <p>
                Secured 4th place at the "InnoAir Challenge: Fair Green
                Transition and Urban Mobility" hackathon with a project focused
                on autonomous vehicles and smart roads.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/bob4o-afk/wall-e"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "Robodays '23" - TU-Sofia hackathon | First Place | 28.04.2023
                  - 09.04.2023
                </a>
              </Text>
              <p>
                Won first place in the "Hardware Projects" category at the
                "Robodays '23" hackathon with the project "Wall-E," an
                autonomous robot designed to detect and collect trash using a
                robotic arm.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/bob4o-afk/SparkBot"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "InnoAir & Europeana: Sustainability and Social Justice"
                  hackathon | First Place | 05.05.2023 - 08.06.2023
                </a>
              </Text>
              <p>
                Won first place at the "InnoAir & Europeana: Sustainability and
                Social Justice" hackathon with the project "SparkBot," an
                automated robot designed to locate dangers, collect trash, and
                send alerts when necessary.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/bob4o-afk/Wall-E-2.0"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "Engineers in Action" hackathon - Sofia | Third place |
                  11.05.2024 - 12.05.2024
                </a>
              </Text>
              <p>
                Along with fellow students from TUES, I participated in the
                "Engineers in Action" hackathon in Sofia, where our teams
                collectively won third place with several innovative projects.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/Ne-Se-Chete/hacktues2024"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "Hacktues X" hackathon | First place | 13.05.2024 - 16.05.2024
                </a>
              </Text>
              <p>
                My team, "Ne se chete" and I won the honorary first place at the
                10th anniversary edition of HackTUES, a hackathon organized by
                TUES. Our project, which impressed the jury, was a set of
                sensors that can be mounted in various locations to detect
                underwater and surface trash and send alerts. This victory also
                marked the foundation of our team, "Ne se chete".
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/Ne-Se-Chete/CableUndefined-Application"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "Para Robotics Incubator Vol. 3" | First Place | 26.09.2024
                </a>
              </Text>
              <p>
                As part of team "Ne se chete," we secured first place in the
                third edition of the "Para Robotics Incubator" competition with
                our project "CableUndefined".
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/Ne-Se-Chete/CableUndefined-Application"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  "Future Engineers in Bulgaria" | First Place | 26.09.2024
                </a>
              </Text>
              <p>
                As part of team "Ne se chete," we secured first place in the
                national competition "Future Engineers in Bulgaria" with our
                project "CableUndefined".
              </p>

              <br></br>
              <Title level={3} style={{ color: "var(--text-color)" }}>
                Experience
              </Title>
              <Divider className="section-divider" />
              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://github.com/Ne-Se-Chete/AppolicaInternSmartBBQ"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  Appolica | Internship | 01.07.2024 - 30.07.2024
                </a>
              </Text>
              <p>
                During my internship at Appolica, I developed a Slack bot for
                automated receipt recognition and food ordering. I assisted my
                team as needed, focusing primarily on frontend in the first week
                before transitioning to hardware tasks.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                <a
                  href="https://www.codbex.com/"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "var(--text-color)" }}
                >
                  Codbex | Intern Software Developer | 01.06.2024 - Present
                </a>
              </Text>
              <p>
                Working on diverse real-world projects using the cutting-edge
                technology "Eclipse Dirigible."
              </p>
            </div>

            {/* Certificates Section */}
            {/* cisco + c1 soon */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>
                Certificates
              </Title>
              <Divider className="section-divider" />

              <Text strong style={{ color: "var(--text-color)" }}>
                Robotica Certificate - 10th Class
              </Text>
              <p>
                Certificate awarded for participation in a robotics competition
                during the 10th grade.
              </p>

              <Text strong style={{ color: "var(--text-color)" }}>
                Startup Weekend Participation - 25.06.2023
              </Text>
              <p>
                Participated in Startup Weekend, gaining experience in startup
                development and entrepreneurship.
              </p>
            </div>

            {/* Education Section */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>
                Education
              </Title>
              <Divider className="section-divider" />
              <Text strong style={{ color: "var(--text-color)" }}>
                Technological School Electronic Systems (TUES)
              </Text>
              <p>Sofia, Bulgaria | 2020 - Present</p>
              <p>Specialization: Computer Science & Software Development</p>
            </div>

            {/* Hobbies Section */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>
                Hobbies
              </Title>
              <Divider className="section-divider" />
              <p>
                I enjoy writing and listening to music, reading and
                participating in local hackathons. I also love creating things,
                such as origami and web riddles. Additionally, I play games and
                have experience in game development. You can check out one of my
                games{" "}
                <a
                  href="https://bobinkata.itch.io/escape2-0"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  here{" "}
                </a>
                and you can check my{" "}
                <a
                  href="https://riddle-1.free.bg/"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  first
                </a>{" "}
                riddle.
              </p>
            </div>
          </Col>

          {/* Right Column */}
          <Col span={12} className="cv-right">
            {/* Profile Section */}
            <div className="cv-section">
              <Title level={3}>Profile</Title>
              <Divider className="section-divider" />
              <p style={{ color: "black" }}>
                Passionate web developer and hardware enthusiast with over 5
                years of experience creating responsive, user-friendly websites
                and web applications, as well as innovative hardware projects
                and robust backend solutions. Expertise in Python, React, C,
                Golang, and Arduino.
              </p>
            </div>

            {/* Skills Section */}
            <div className="cv-section">
              <Title level={3}>Skills</Title>
              <Divider className="section-divider" />

              {/* Frontend */}
              <div className="cv-subsection">
                <Title level={4}>Frontend</Title>
                <Text>React</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={83}
                  showInfo={false}
                />
                <Text>HTML/CSS</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={70}
                  showInfo={false}
                />
              </div>

              {/* Backend */}
              <div className="cv-subsection">
                <Title level={4}>Backend</Title>
                <Text>Python</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={100}
                  showInfo={false}
                />
                <Text>GoLang</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={87}
                  showInfo={false}
                />
                <Text>.NET</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={78}
                  showInfo={false}
                />
                <Text>SpringBoot</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={67}
                  showInfo={false}
                />
              </div>

              {/* Hardware */}
              <div className="cv-subsection">
                <Title level={4}>Hardware</Title>
                <Text>Arduino</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={80}
                  showInfo={false}
                />
                <Text>ESP</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={75}
                  showInfo={false}
                />
                <Text>Raspberry Pi</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={70}
                  showInfo={false}
                />
              </div>

              {/* Networks */}
              <div className="cv-subsection">
                <Title level={4}>Networks</Title>
                <Text>Cisco</Text>
                <Progress
                  className="progress"
                  strokeColor="var(--cv-bar)"
                  percent={35}
                  showInfo={false}
                />
              </div>
            </div>

            {/* Personality Section */}
            <div className="cv-section personality-chart">
              <Title level={3}>Personality</Title>
              <Divider className="section-divider" />
              <PieChart width={400} height={300}>
                <Pie
                  data={personalityData}
                  dataKey="value"
                  cx={200}
                  cy={150}
                  outerRadius={100}
                  fill="var(--cv-bar)"
                  label={(entry) => entry.name}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: "#fff",
                    borderRadius: "50px",
                    border: "2px solid var(--cv-bar)",
                    textAlign: "center",
                    padding: "10px",
                  }}
                  cursor={false}
                />
              </PieChart>
            </div>

            {/* Languages Section */}
            <div className="cv-section">
              <Title level={3}>Languages</Title>
              <Divider className="section-divider" />
              <Text>English</Text>
              <Progress
                className="progress"
                strokeColor="var(--cv-bar)"
                percent={90}
                showInfo={false}
              />
              <Text>German</Text>
              <Progress
                className="progress"
                strokeColor="var(--cv-bar)"
                percent={40}
                showInfo={false}
              />
            </div>
          </Col>
        </Row>

        {session?.email === "bobi06bobi@gmail.com" && (
          <Button
            type="primary"
            onClick={exportToPDF}
            className="export-btn"
            ref={exportButtonRef}
          >
            Export as PDF
          </Button>
        )}
      </div>
    </div>
  );
};

export default CVTemplate;
