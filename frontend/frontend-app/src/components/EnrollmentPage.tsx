import React, { useState, useEffect, FormEvent } from 'react';
import { Button, Form, Select, Typography, notification } from 'antd';
import './ui/EnrollmentPage.css';

const { Title } = Typography;
const { Option } = Select;
const { Text } = Typography;


const EnrollmentPage: React.FC = () => {
  const [selectedOlympiad, setSelectedOlympiad] = useState<string>('');
  const [olympiads, setOlympiads] = useState<any[]>([]);
  const [, setEmailSent] = useState<boolean>(false);

  useEffect(() => {
    const fetchOlympiads = async () => {
      try {
        const response = await fetch("http://localhost:5138/api/olympiad");
        
        if (response.ok) {
          const data = await response.json();
          setOlympiads(data);
        } else {
          notification.error({
            message: 'Error',
            description: 'Failed to load Olympiad data.',
          });
        }
      } catch (error) {
        console.error('Error fetching Olympiads:', error);
        notification.error({
          message: 'Network Error',
          description: 'There was an issue fetching the Olympiad data.',
        });
      }
    };

    fetchOlympiads();
  }, []);

  const handleSelectChange = (value: string) => {
    setSelectedOlympiad(value);
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (selectedOlympiad === '') {
      notification.error({
        message: 'Selection Error',
        description: 'Please select an Olympiad.',
      });
      return;
    }

    const emailData = {
      toEmail: 'borislav.b.milanov.2020@elsys-bg.org',
      subject: selectedOlympiad,
      body: `Dear Student, \n\nYou have successfully enrolled in the ${selectedOlympiad} Olympiad.`,
      ccEmail: 'penkapenka64@gmail.com',
    };

    try {
      const response = await fetch("http://localhost:5138/api/email/send", {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(emailData),
      });

      if (response.ok) {
        setEmailSent(true);
        notification.success({
          message: 'Success',
          description: 'Enrollment email successfully sent!',
        });
      } else {
        const errorText = await response.text();
        notification.error({
          message: 'Sending Error',
          description: `There was an error sending the email: ${errorText}`,
        });
      }
    } catch (error) {
      console.error('Error:', error);
      notification.error({
        message: 'Network Error',
        description: 'There was a problem with the network or server.',
      });
    }
  };

  return (
    <div className="enrollment-page">
      <Title level={2}>Olympiad Enrollment</Title>

      {/* Styled Select Olympiad Text */}
      <Text style={{ fontSize: '18px', fontWeight: '600', marginBottom: '8px' }}>
        Please select an Olympiad from the list below:
      </Text>

      <Form onSubmitCapture={handleSubmit} className="enrollment-form">
        <Form.Item label="Select an Olympiad">
          <Select 
            value={selectedOlympiad} 
            onChange={handleSelectChange} 
            placeholder="Select an Olympiad"
            style={{ width: '100%', height: '100%' }}
          >
            {olympiads.map((olympiad) => (
              <Option key={olympiad.olympiadId} value={olympiad.subject}>
                <div>
                  <strong>{olympiad.subject}</strong>
                  <p>{`Location: ${olympiad.location}`}</p>
                  <p>{`Date: ${new Date(olympiad.dateOfOlympiad).toLocaleDateString()}`}</p>
                  <p>{`Start Time: ${new Date(olympiad.startTime).toDateString()}`}</p>
                </div>
              </Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" disabled={!selectedOlympiad}>
            Submit Enrollment
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default EnrollmentPage;
