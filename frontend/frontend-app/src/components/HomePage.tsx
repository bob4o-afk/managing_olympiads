import React from 'react';
import { Button, Card, Typography } from 'antd';
import './ui/HomePage.css';
const { Title, Paragraph } = Typography;

interface HomePageProps {
  onNavigate: (key: string) => void;
}

const HomePage: React.FC<HomePageProps> = ({ onNavigate }) => {
  return (
    <div className="home-page">
      <Title style={{color: "var(--text-color)",wordBreak: "keep-all"}}>
        Welcome to the Olympiad Management Service
      </Title>

      <Paragraph className='paragraph'>
        This service is designed to help students and teachers streamline the process of participating in Olympiads. 
        Browse through available Olympiads, fill out the necessary documents, and send them to your teacher with just a few clicks.
      </Paragraph>

      <div className="cards-container">
        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">Browse Olympiads</Title>
            <Paragraph className="card-text">
              Explore a list of all available Olympiads. Click on any Olympiad to get more details and start the application process.
            </Paragraph>
          </div>
          <div className="card-actions">
            <Button className="button" onClick={() => onNavigate('all-olympiads')}>
              View All Olympiads
            </Button>
          </div>
        </Card>

        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">Fill Out Documents</Title>
            <Paragraph className="card-text">
              Complete the required forms for your selected Olympiad. Once completed, the forms will be automatically sent to your teacher and a copy will be sent to you.
            </Paragraph>
          </div>
          <div className="card-actions">
            <Button className="button" onClick={() => onNavigate('documents')}>
              Go to Documents
            </Button>
          </div>
        </Card>

        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">Manage Your Profile</Title>
            <Paragraph className="card-text">
              Update your personal information, view your participation history, and manage your account settings.
            </Paragraph>
          </div>
          <div className="card-actions">
            <Button className="button" onClick={() => onNavigate('my-profile')}>
              Go to Profile
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
};

export default HomePage;
