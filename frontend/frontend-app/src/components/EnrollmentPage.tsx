import React, { useState, FormEvent } from 'react';
import { Button, Form, Select, Typography, notification } from 'antd';

import './ui/EnrollmentPage.css'; // Ensure you have this CSS file for custom styles

const { Title } = Typography;
const { Option } = Select;

const EnrollmentPage: React.FC = () => {
  const [selectedOlympiad, setSelectedOlympiad] = useState<string>('');
  const [, setEmailSent] = useState<boolean>(false);

  const olympiads = [
    { id: 1, subject: 'Mathematics' },
    { id: 2, subject: 'Mathematics - International' },
    { id: 3, subject: 'Mathematics - National' }
  ];

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
        const response = await fetch(`${process.env.REACT_APP_API_URL!}/api/email/send`, {
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
      <Form onSubmitCapture={handleSubmit} className="enrollment-form">
        <Form.Item label="Select an Olympiad">
          <Select value={selectedOlympiad} onChange={handleSelectChange} placeholder="Select an Olympiad">
            {olympiads.map(olympiad => (
              <Option key={olympiad.id} value={olympiad.subject}>
                {olympiad.subject}
              </Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit">
            Submit Enrollment
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default EnrollmentPage;
