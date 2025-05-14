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
          const parsedSession = JSON.parse(storedSession);
          setSession(parsedSession);
        }

        const response = await fetch(
          `${process.env.REACT_APP_API_URL}/api/studentolympiadenrollment`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.ok) {
          const data = await response.json();
          setEnrollments(data);
        } else {
          notification.error({
            message: "Error",
            description: "Failed to fetch enrollments",
          });
        }
      } catch (error) {
        notification.error({
          message: "Network Error",
          description: "Failed to connect to the server.",
        });
      } finally {
        setLoading(false);
      }
    };

    fetchEnrollments();
  }, []);

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
        `${process.env.REACT_APP_API_URL}/api/studentolympiadenrollment/${editingEnrollment.enrollmentId}`,
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
          message: "Success",
          description: "Enrollment updated successfully!",
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
          message: "Error",
          description: `Failed to update: ${errorMessage}`,
        });
      }
    } catch (error) {
      notification.error({
        message: "Network Error",
        description: "Failed to update enrollment",
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
        `${process.env.REACT_APP_API_URL}/api/email/send`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(emailData),
        }
      );

      if (response.ok) {
        notification.success({
          message: "Email Sent",
          description: "Notification email sent successfully!",
        });
      } else {
        notification.error({
          message: "Email Error",
          description: "Failed to send notification email.",
        });
      }
    } catch (error) {
      notification.error({
        message: "Email Error",
        description: "Failed to send email notification.",
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
            type="primary"
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
        `${process.env.REACT_APP_API_URL}/api/studentolympiadenrollment/${enrollmentId}`,
        {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.ok) {
        notification.success({
          message: "Success",
          description: "Enrollment deleted successfully!",
        });
        setEnrollments((prev) =>
          prev.filter((e) => e.enrollmentId !== enrollmentId)
        );
      } else {
        notification.error({
          message: "Error",
          description: "Failed to delete enrollment",
        });
      }
    } catch (error) {
      notification.error({
        message: "Network Error",
        description: "Failed to delete enrollment",
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
          <Button onClick={() => handleEdit(record, "enrollmentStatus")}>
            {isBG ? "Редактирай статус" : "Edit Status"}
          </Button>
          <Button onClick={() => handleEdit(record, "score")}>
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
      <Title level={2} style={{ color: "var(--text-color)" }}>
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
            type="primary"
            onClick={handleSaveEdit}
            style={{ textAlign: "center" }}
          >
            OK
          </Button>,
          <Button
            key="cancel"
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
