import React from "react";
import { Spin } from "antd";
import "./ui/LoadingPage.css";

const LoadingPage: React.FC = () => (
  <div className="loading-overlay">
    <Spin size="large"></Spin>
  </div>
);

export default LoadingPage;
