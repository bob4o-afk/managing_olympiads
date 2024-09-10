import React, { useRef } from 'react';
import { Button, Typography, Divider, Row, Col, Progress } from 'antd';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';
import { MailOutlined, PhoneOutlined, HomeOutlined, InstagramOutlined, GithubOutlined } from '@ant-design/icons';

import './ui/CVTemplate.css';

const { Title, Text } = Typography;

const CVTemplate: React.FC = () => {
  const exportButtonRef = useRef<HTMLButtonElement>(null);

  const exportToPDF = async () => {
    const element = document.getElementById('cv');
    if (!element) return;

    if (exportButtonRef.current) {
      exportButtonRef.current.classList.add('export-btn-hidden'); // Hide the button
    }

    const computedStyle = window.getComputedStyle(document.body);
    const backgroundColor = computedStyle.getPropertyValue('--header-background-color').trim();

    const canvas = await html2canvas(element, {
      scale: 2,
      useCORS: true,
      allowTaint: true,
      backgroundColor: backgroundColor,
      logging: true,
      scrollX: 0,
      scrollY: 0,
      windowWidth: document.documentElement.scrollWidth,
      windowHeight: document.documentElement.scrollHeight
    });

    const dataURL = canvas.toDataURL('image/png');
    const pdf = new jsPDF({
      orientation: 'portrait',
      unit: 'mm',
      format: [210, 297]
    });

    pdf.addImage(dataURL, 'PNG', 0, 0, 210, 297);
    pdf.save('CV.pdf');

    if (exportButtonRef.current) {
      exportButtonRef.current.classList.remove('export-btn-hidden');
    }
  };

  return (
    <div style={{ width: '1075px', margin: '0 auto', background: 'var(--header-background-color)' }}>
      <div id="cv" className="cv">
        {/* Header */}
        <div className="cv-header">
          <div className="cv-header-text">
            <Title level={1} className="cv-name">Borislav Milanov</Title>
            <Text className="cv-job-title">Senior Web Developer</Text>
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
              <Title level={3} style={{ color: "var(--text-color)" }}>Experience</Title>
              <Divider className="section-divider" />
              <Text strong style={{ color: "var(--text-color)" }}>Appolica | Intership | 01.07.2024 - 30.07.2024</Text>
              <p>Responsible for leading front-end development, collaborating with UX/UI designers, and optimizing application performance.</p> {/* TODO: write real stuff */}
              
              <Text strong style={{ color: "var(--text-color)" }}>Codbex | Senior Developer | 01.06.2024 - Present</Text>
              <p>Worked on building scalable web applications, implementing modern JavaScript frameworks, and improving site performance.</p>
            </div>

            {/* Education Section */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>Education</Title>
              <Divider className="section-divider" />
              <Text strong style={{ color: "var(--text-color)" }}>Technological School Electronic Systems (TUES)</Text>
              <p>Sofia, Bulgaria | 2020 - Present</p>
              <p>Specialization: Computer Science & Software Development</p>
            </div>

            {/* Hobbies Section */}
            <div className="cv-section">
              <Title level={3} style={{ color: "var(--text-color)" }}>Hobbies</Title>
              <Divider className="section-divider" />
              <p>Enjoy writing and listening to music, reading, and participating in local hackathons.</p>
            </div>
          </Col>

          {/* Right Column */}
          <Col span={12} className="cv-right">
            {/* Profile Section */}
            <div className="cv-section">
              <Title level={3}>Profile</Title>
              <Divider className="section-divider" />
              <p style={{ color: "black" }}>Passionate web developer with over 10 years of experience in creating responsive, user-friendly websites and web applications. Strong expertise in JavaScript, React, and Node.js.</p>
            </div>

            {/* Skills Section */}
            <div className="cv-section">
              <Title level={3}>Skills</Title>
              <Divider className="section-divider" />
              <Text>JavaScript</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={95} showInfo={false} />
              <Text>React</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={90} showInfo={false} />
              <Text>Node.js</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={85} showInfo={false} />
              <Text>HTML/CSS</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={90} showInfo={false} />
            </div>

            {/* Personality Section */}
            <div className="cv-section">
              <Title level={3}>Personality</Title>
              <Divider className="section-divider" />
              <Text>Problem Solver</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={95} showInfo={false} />
              <Text>Creative Thinker</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={90} showInfo={false} />
              <Text>Team Player</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={85} showInfo={false} />
              <Text>Ambitious</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={80} showInfo={false} />
            </div>

            {/* Languages Section */}
            <div className="cv-section">
              <Title level={3}>Languages</Title>
              <Divider className="section-divider" />
              <Text>English</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={90} showInfo={false} />
              <Text>German</Text>
              <Progress className='progress' strokeColor="var(--cv-bar)" percent={40} showInfo={false} />
            </div>
          </Col>
        </Row>

        <Button 
          type="primary" 
          onClick={exportToPDF} 
          className="export-btn"
          ref={exportButtonRef}
        >
          Export as PDF
        </Button>
      </div>
    </div>
  );
};

export default CVTemplate;
