import React, { useContext, useEffect, useState } from "react";
import {
  Table,
  Typography,
  Button,
  Modal,
  Select,
  InputNumber,
  notification,
} from "antd";
import "./ui/StudentOlympiadEnrollments.css";
import { ColumnType } from "antd/es/table";
import { UserSession } from "../types/Session";
import { LanguageContext } from "../contexts/LanguageContext";
import { decryptSession } from "../utils/encryption";
import { getAuthDeleteOptions, getAuthGetOptions, getAuthPostOptions } from "../config/apiConfig";
import { API_ROUTES } from "../config/api";

const { Title } = Typography;
const { Option } = Select;

interface EnrollmentRow {
  enrollmentId: number;
  user: { name: string; email: string };
  olympiad: { subject: string; round: string };
  academicYear: { startYear: number; endYear: number };
  enrollmentStatus: string;
  score: number | null;
}

const StudentOlympiadEnrollments: React.FC = () => {
  const [enrollments, setEnrollments] = useState<EnrollmentRow[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [editingEnrollment, setEditingEnrollment] =
    useState<EnrollmentRow | null>(null);
  const [editType, setEditType] = useState<"enrollmentStatus" | "score" | null>(
    null
  );
  const [newValue, setNewValue] = useState<string | number | null>(null);
  const [session, setSession] = useState<UserSession | null>(null);
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  useEffect(() => {
    const fetchEnrollments = async () => {
      try {
        const token = localStorage.getItem("authToken");
        const storedSession = localStorage.getItem("userSession");
        if (storedSession) {
          try {
            const parsedSession = decryptSession(storedSession);
            setSession(parsedSession);
          } catch (error) {
            console.error("Failed to decrypt session:", error);
          }
        }

        const response = await fetch(
          API_ROUTES.studentOlympiadEnrollment,
          getAuthGetOptions(token ?? "")
        );

        if (response.ok) {
          const data = await response.json();
          setEnrollments(data);
        } else {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Неуспешно зареждане на записванията"
              : "Failed to fetch enrollments",
          });
        }
      } catch (error) {
        notification.error({
          message: isBG ? "Мрежова грешка" : "Network Error",
          description: isBG
            ? "Неуспешна връзка със сървъра."
            : "Failed to connect to the server.",
        });
      } finally {
        setLoading(false);
      }
    };

    fetchEnrollments();
  }, [isBG]);

  useEffect(() => {
    let hasShownNotification = false;
    let timeoutId: number | null = null;

    const checkOrientation = () => {
      const isMobile = window.innerWidth <= 768;
      const aspectRatio = window.innerWidth / window.innerHeight;
      const isSixteenNine = Math.abs(aspectRatio - 16 / 9) < 0.1;

      if (isMobile && !isSixteenNine && !hasShownNotification) {
        hasShownNotification = true;
        notification.info({
          message: isBG ? "Завъртете устройството" : "Rotate Your Device",
          description: isBG
            ? "По-добро изживяване в хоризонтален режим (16:9)."
            : "For a better experience, use landscape mode (16:9).",
          duration: 8,
        });
      }
    };

    const delayedCheck = () => {
      if (timeoutId !== null) {
        clearTimeout(timeoutId);
      }
      timeoutId = window.setTimeout(checkOrientation, 200);
    };

    delayedCheck();

    const handleOrientationChange = () => {
      if (!hasShownNotification) {
        delayedCheck();
      }
    };

    window.addEventListener("orientationchange", handleOrientationChange);

    return () => {
      if (timeoutId !== null) {
        clearTimeout(timeoutId);
      }
      window.removeEventListener("orientationchange", handleOrientationChange);
    };
  }, [isBG]);

  const handleEdit = (
    enrollment: EnrollmentRow,
    type: "enrollmentStatus" | "score"
  ) => {
    setEditingEnrollment(enrollment);
    setEditType(type);
    setNewValue(
      type === "score" ? enrollment.score : enrollment.enrollmentStatus
    );
  };

  const handleSaveEdit = async () => {
    if (!editingEnrollment || !editType) return;

    try {
      const token = localStorage.getItem("authToken");

      const updatedEnrollment = {
        ...editingEnrollment,
        [editType]: newValue,
      };

      const response = await fetch(
        API_ROUTES.studentOlympiadEnrollmentById(
          String(editingEnrollment.enrollmentId)
        ),
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(updatedEnrollment),
        }
      );

      if (response.ok) {
        notification.success({
          message: isBG ? "Успешно" : "Success",
          description: isBG
            ? "Записът е обновен успешно!"
            : "Enrollment updated successfully!",
        });
        setEnrollments((prev) =>
          prev.map((e) =>
            e.enrollmentId === editingEnrollment.enrollmentId
              ? { ...e, [editType]: newValue }
              : e
          )
        );

        await sendEmailNotification(editingEnrollment);

        setEditingEnrollment(null);
      } else {
        const errorMessage = await response.text();
        notification.error({
          message: isBG ? "Грешка" : "Error",
          description: isBG
            ? `Неуспешно обновяване: ${errorMessage}`
            : `Failed to update: ${errorMessage}`,
        });
      }
    } catch (error) {
      notification.error({
        message: isBG ? "Мрежова грешка" : "Network Error",
        description: isBG
          ? "Неуспешно обновяване на записването"
          : "Failed to update enrollment",
      });
    }
  };

  const sendEmailNotification = async (enrollment: any) => {
    try {
      const token = localStorage.getItem("authToken");

      const emailData = {
        toEmail: enrollment.user.email,
        subject: "Olympiad Enrollment Updated",
        body: `
          <p>Dear ${enrollment.user.name},</p>
      
          <p>Your Olympiad enrollment has been updated.</p>
      
          <ul>
            <li><strong>Olympiad:</strong> ${
              enrollment.olympiad.subject
            } (Round ${enrollment.olympiad.round})</li>
            <li><strong>Academic Year:</strong> ${
              enrollment.academicYear.startYear
            }-${enrollment.academicYear.endYear}</li>
            <li><strong>Updated Field:</strong> ${
              editType === "enrollmentStatus" ? "Enrollment Status" : "Score"
            }</li>
            <li><strong>New Value:</strong> ${newValue}</li>
          </ul>
      
          <p>If you have any questions, please contact the administrator.</p>
      
          <p>Best regards,<br>Olympiad System</p>
        `,
        ccEmail: session?.email,
      };

      const response = await fetch(
        API_ROUTES.sendEmail,
        getAuthPostOptions(token ?? "", emailData)
      );

      if (response.ok) {
        notification.success({
          message: isBG ? "Имейл изпратен" : "Email Sent",
          description: isBG
            ? "Известие по имейл беше изпратено успешно!"
            : "Notification email sent successfully!",
        });
      } else {
        notification.error({
          message: isBG ? "Имейл грешка" : "Email Error",
          description: isBG
            ? "Неуспешно изпращане на имейл."
            : "Failed to send notification email.",
        });
      }
    } catch (error) {
      notification.error({
        message: isBG ? "Имейл грешка" : "Email Error",
        description: isBG
          ? "Неуспешно изпращане на имейл."
          : "Failed to send notification email.",
      });
    }
  };

  const handleDelete = async (enrollmentId: number) => {
    Modal.confirm({
      title: isBG
        ? "Сигурни ли сте, че искате да изтриете този запис?"
        : "Are you sure you want to delete?",
      content: isBG
        ? "Това действие не може да бъде отменено."
        : "This action cannot be undone.",
      okText: isBG ? "Да" : "Yes",
      cancelText: isBG ? "Не" : "No",
      centered: true,
      footer: [
        <div style={{ textAlign: "center", width: "100%" }} key="yes-footer">
          <Button
            key="yes"
            danger
            onClick={async () => {
              await deleteEnrollment(enrollmentId);
              Modal.destroyAll();
            }}
          >
            {isBG ? "Да" : "Yes"}
          </Button>
        </div>,
        <div
          style={{ textAlign: "center", width: "100%", marginTop: "10px" }}
          key="no-footer"
        >
          <Button key="no" onClick={() => Modal.destroyAll()}>
            {isBG ? "Не" : "No"}
          </Button>
        </div>,
      ],
    });
  };

  const deleteEnrollment = async (enrollmentId: number) => {
    try {
      const token = localStorage.getItem("authToken");
      const response = await fetch(
        API_ROUTES.studentOlympiadEnrollmentById(String(enrollmentId)),
        getAuthDeleteOptions(token ?? "")
      );

      if (response.ok) {
        notification.success({
          message: isBG ? "Успешно" : "Success",
          description: isBG
            ? "Записването е изтрито успешно!"
            : "Enrollment deleted successfully!",
        });
        setEnrollments((prev) =>
          prev.filter((e) => e.enrollmentId !== enrollmentId)
        );
      } else {
        notification.error({
          message: isBG ? "Грешка" : "Error",
          description: isBG
            ? "Неуспешно изтриване на записването"
            : "Failed to delete enrollment",
        });
      }
    } catch (error) {
      notification.error({
        message: isBG ? "Мрежова грешка" : "Network Error",
        description: isBG
          ? "Неуспешна връзка със сървъра."
          : "Failed to connect to the server.",
      });
    }
  };

  const capitalize = (text: string) =>
    text.charAt(0).toUpperCase() + text.slice(1);

  const columns: ColumnType<EnrollmentRow>[] = [
    {
      title: isBG ? "Ученик" : "User",
      dataIndex: "user",
      key: "user",
      render: (user) => `${user.name} (${user.email})`,
      align: "center",
    },
    {
      title: isBG ? "Олимпиада" : "Olympiad",
      dataIndex: "olympiad",
      key: "olympiad",
      render: (olympiad) => {
        const roundBG =
          olympiad.round === "Regional Ring"
            ? "Регионален"
            : olympiad.round === "District Ring"
            ? "Областен кръг"
            : olympiad.round === "National Ring"
            ? "Национален кръг"
            : olympiad.round;

        return `${olympiad.subject} (${
          isBG ? roundBG : capitalize(olympiad.round)
        })`;
      },
      align: "center",
    },
    {
      title: isBG ? "Учебна година" : "Academic Year",
      dataIndex: "academicYear",
      key: "academicYear",
      render: (year) => `${year.startYear}-${year.endYear}`,
      align: "center",
    },
    {
      title: isBG ? "Статус" : "Enrollment Status",
      dataIndex: "enrollmentStatus",
      key: "enrollmentStatus",
      filters: [
        { text: isBG ? "В изчакване" : "Pending", value: "pending" },
        { text: isBG ? "Одобрен" : "Approved", value: "approved" },
        { text: isBG ? "Отхвърлен" : "Rejected", value: "rejected" },
      ],
      onFilter: (value, record) =>
        record.enrollmentStatus === value ||
        (value === "" && record.enrollmentStatus == null),
      sorter: (a, b) =>
        (a.enrollmentStatus || "").localeCompare(b.enrollmentStatus || ""),
      align: "center",
      render: (status: string) => {
        if (!isBG) return capitalize(status);
        return status === "pending"
          ? "В изчакване"
          : status === "approved"
          ? "Одобрен"
          : status === "rejected"
          ? "Отхвърлен"
          : status;
      },
    },
    {
      title: isBG ? "Резултат" : "Score",
      dataIndex: "score",
      key: "score",
      render: (score) => (score !== null ? score : isBG ? "Няма" : "N/A"),
      align: "center",
    },
    {
      title: isBG ? "Действия" : "Actions",
      key: "actions",
      render: (_, record) => (
        <div style={{ display: "flex", justifyContent: "center", gap: "10px" }}>
          <Button
            type="primary"
            onClick={() => handleEdit(record, "enrollmentStatus")}
          >
            {isBG ? "Редактирай статус" : "Edit Status"}
          </Button>
          <Button type="primary" onClick={() => handleEdit(record, "score")}>
            {isBG ? "Редактирай резултат" : "Edit Score"}
          </Button>
          <Button danger onClick={() => handleDelete(record.enrollmentId)}>
            {isBG ? "Изтрий" : "Delete"}
          </Button>
        </div>
      ),
      align: "center",
    },
  ];

  return (
    <div style={{ padding: "24px" }}>
      <Title
        level={2}
        style={{ color: "var(--text-color)", textAlign: "center" }}
      >
        {isBG ? "Записвания на всички ученици" : "Student Olympiad Enrollments"}
      </Title>
      <Table
        dataSource={enrollments}
        columns={columns}
        rowKey="enrollmentId"
        loading={loading}
        pagination={{ pageSize: 5 }}
        scroll={{ x: 800 }}
      />
      <Modal
        title={
          isBG
            ? editType === "enrollmentStatus"
              ? "Редактирай статуса"
              : "Редактирай резултата"
            : `Edit ${editType === "enrollmentStatus" ? "Status" : "Score"}`
        }
        open={!!editingEnrollment}
        onOk={handleSaveEdit}
        onCancel={() => setEditingEnrollment(null)}
        centered
        footer={[
          <Button
            key="ok"
            className="button"
            onClick={handleSaveEdit}
            style={{ textAlign: "center" }}
          >
            OK
          </Button>,
          <Button
            key="cancel"
            className="cancel-button"
            onClick={() => setEditingEnrollment(null)}
            style={{ display: "block", margin: "10px auto" }}
          >
            Cancel
          </Button>,
        ]}
      >
        {editType === "enrollmentStatus" ? (
          <Select
            value={newValue}
            onChange={(value) => setNewValue(value)}
            style={{ width: "100%" }}
          >
            <Option value="pending">{isBG ? "В изчакване" : "Pending"}</Option>
            <Option value="approved">{isBG ? "Одобрен" : "Approved"}</Option>
            <Option value="rejected">{isBG ? "Отхвърлен" : "Rejected"}</Option>
          </Select>
        ) : (
          <InputNumber
            value={newValue}
            onChange={(value) => setNewValue(value)}
            style={{ width: "100%" }}
          />
        )}
      </Modal>
    </div>
  );
};

export default StudentOlympiadEnrollments;
