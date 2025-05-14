import React, { useContext } from "react";
import { Button, Card, Typography } from "antd";
import { LanguageContext } from "../contexts/LanguageContext";
import "./ui/HomePage.css";

const { Title, Paragraph } = Typography;

interface HomePageProps {
  onNavigate: (key: string) => void;
}

const HomePage: React.FC<HomePageProps> = ({ onNavigate }) => {
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  return (
    <div className="home-page">
      <Title className="home-title">
        {isBG
          ? "Добре дошли в системата за управление на олимпиади"
          : "Welcome to the Olympiad Management Service"}
      </Title>

      <Paragraph className="paragraph">
        {isBG
          ? "Тази страница е създадена, за да помогне на ученици и учители да улеснят процеса на участие в олимпиади. Разгледайте наличните олимпиади, попълнете необходимите документи и ги изпратете на вашия учител само с няколко клика."
          : "This service is designed to help students and teachers streamline the process of participating in Olympiads. Browse through available Olympiads, fill out the necessary documents, and send them to your teacher with just a few clicks."}
      </Paragraph>

      <div className="cards-container">
        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">
              {isBG ? "Разгледай олимпиадите" : "Browse Olympiads"}
            </Title>
            <div className="card-text-wrapper">
              <Paragraph className="card-text">
                {isBG
                  ? "Разгледайте списъка с всички налични олимпиади, вижте подробностите и започнете процеса по кандидатстване."
                  : "Explore the list of all available Olympiads, check the details and start the application process."}
              </Paragraph>
            </div>
          </div>
          <Button
            className="button"
            onClick={() => onNavigate("all-olympiads")}
          >
            {isBG ? "Прегледай всички" : "View All Olympiads"}
          </Button>
        </Card>

        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">
              {isBG ? "Попълни документи" : "Fill Out Documents"}
            </Title>
            <div className="card-text-wrapper">
              <Paragraph className="card-text">
                {isBG
                  ? "Попълнете необходимите формуляри за избраната олимпиада. След това те автоматично ще бъдат изпратени на вашия учител, а вие ще получите копие."
                  : "Complete the required forms for your selected Olympiad. Once completed, the forms will be automatically sent to your teacher and a copy will be sent to you."}
              </Paragraph>
            </div>
          </div>
          <Button className="button" onClick={() => onNavigate("documents")}>
            {isBG ? "Към документи" : "Go to Documents"}
          </Button>
        </Card>

        <Card className="card">
          <div className="card-content">
            <Title level={2} className="card-title">
              {isBG ? "Управлявай профила си" : "Manage Your Profile"}
            </Title>
            <div className="card-text-wrapper">
              <Paragraph className="card-text">
                {isBG
                  ? "Актуализирайте личната си информация, вижте историята на участието си и управлявайте настройките на акаунта си."
                  : "Update your personal information, view your participation history, and manage your account settings."}
              </Paragraph>
            </div>
          </div>
          <Button className="button" onClick={() => onNavigate("my-profile")}>
            {isBG ? "Към профила" : "Go to Profile"}
          </Button>
        </Card>
      </div>
    </div>
  );
};

export default HomePage;
