import React from 'react';
import { Typography, Form, Input, Button } from 'antd';
import './ui/UpdateInfo.css'; 

const { Title } = Typography;

const UpdateInfo: React.FC = () => {
    const onFinish = (values: any) => {
        // Handle form submission logic here
        console.log('Form values:', values);
    };

    return (
        <div className="update-info-container">
            <Title className="update-info-title" level={2}>Update Profile Information</Title>
            <Form
                layout="vertical"
                onFinish={onFinish}
            >
                <Form.Item
                    label="Full Name"
                    name="full_name"
                    rules={[{ required: true, message: 'Please input your full name!' }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item
                    label="Email"
                    name="email"
                    rules={[{ required: true, message: 'Please input your email!' }]}
                >
                    <Input type="email" />
                </Form.Item>
                <Form.Item>
                    <Button type="primary" htmlType="submit">
                        Update Information
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};

export default UpdateInfo;
