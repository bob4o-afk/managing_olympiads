import React from 'react';
import { Button, Card, Typography } from 'antd';

const { Title, Paragraph } = Typography;

interface HomePageProps {
  onNavigate: (key: string) => void;
}

const HomePage: React.FC<HomePageProps> = ({ onNavigate }) => {
  return (
    <div className="home-page" style={{ padding: '24px', color: 'var(--text-color)' }}>
      <Title style={{ color: 'var(--text-color)' }}>Welcome to the Olympiad Management Service</Title>
      <Paragraph style={{ color: 'var(--text-color)' }}>
        This service is designed to help students and teachers streamline the process of participating in Olympiads. Browse through available Olympiads, fill out the necessary documents, and send them to your teacher with just a few clicks.
      </Paragraph>

      <Card className="card" style={{ marginBottom: '16px' }}>
        <Title level={4} style={{ color: 'var(--text-color)' }}>Browse Olympiads</Title>
        <Paragraph style={{ color: 'var(--text-color)' }}>
          Explore a list of all available Olympiads. Click on any Olympiad to get more details and start the application process.
        </Paragraph>
        <Button className="button" type="primary" onClick={() => onNavigate('all-olympiads')}>
          View All Olympiads
        </Button>
      </Card>

      <Card className="card" style={{ marginBottom: '16px' }}>
        <Title level={4} style={{ color: 'var(--text-color)' }}>Fill Out Documents</Title>
        <Paragraph style={{ color: 'var(--text-color)' }}>
          Complete the required forms for your selected Olympiad. Once completed, the forms will be automatically sent to your teacher and a copy will be sent to you.
        </Paragraph>
        <Button className="button" type="primary" onClick={() => onNavigate('documents')}>
          Go to Documents
        </Button>
      </Card>

      <Card className="card" style={{ marginBottom: '16px' }}>
        <Title level={4} style={{ color: 'var(--text-color)' }}>Manage Your Profile</Title>
        <Paragraph style={{ color: 'var(--text-color)' }}>
          Update your personal information, view your participation history, and manage your account settings.
        </Paragraph>
        <Button className="button" type="primary" onClick={() => onNavigate('my-profile')}>
          Go to Profile
        </Button>
      </Card>
    </div>
  );
};

export default HomePage;
